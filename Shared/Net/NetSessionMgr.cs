using Core.Net;
using Google.Protobuf;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Shared.Net
{
	public abstract class NetSessionMgr
	{
		private readonly List<IListener> _listeners = new List<IListener>();
		private readonly Dictionary<int, NetSession> _idToSession = new Dictionary<int, NetSession>();
		private readonly Queue<NetEvent> _events = new Queue<NetEvent>();
		private bool _unSafeSend;

		public void Dispose()
		{
			this._events.Clear();
		}

		public bool CreateListener( int port, int recvBufSize, SocketType socketType, ProtocolType protoType, out int pos )
		{
			pos = this._listeners.Count;
			IListener listener = new Listener();
			listener.sessionCreateHandler = this.CreateListenerSession;
			listener.packetEncodeHandler = LengthEncoder.Decode;
			listener.recvBufSize = recvBufSize;
			this._listeners.Add( listener );
			return this._listeners[pos].Start( "0", port, socketType, protoType );
		}

		public virtual bool CreateConnector( SessionType sessionType, string ip, int port, SocketType socketType, ProtocolType protoType, int recvsize, int logicId )
		{
			IConnector connector = this.CreateConnector();
			CliSession session = this.CreateConnectorSession( sessionType );
			this._idToSession.Add( session.id, session );
			session.logicID = logicId;
			session.connector = connector;
			connector.session = session;
			connector.recvBufSize = recvsize;
			connector.packetEncodeHandler = LengthEncoder.Decode;
			return session.Connect( ip, port, socketType, protoType );
		}

		protected abstract SrvCliSession CreateListenerSession();

		protected abstract IConnector CreateConnector();

		protected abstract CliSession CreateConnectorSession( SessionType sessionType );

		public NetSession GetSession( int sessionID )
		{
			this._idToSession.TryGetValue( sessionID, out NetSession session );
			return session;
		}

		public void SendMsgToSession( SessionType sessionType, int sessionId, IMessage sMsg, int n32MsgID )
		{
			if ( !this._unSafeSend )
			{
				int n32MsgSize = sMsg.CalculateSize();
				int n32Length = n32MsgSize + 2 * sizeof( int );
				StreamBuffer sBuffer = StreamBufferPool.Pop();
				sBuffer.Write( n32Length );
				sBuffer.Write( n32MsgID );
				CodedOutputStream outputStream = new CodedOutputStream( sBuffer.ms, true );
				sMsg.WriteTo( outputStream );
				outputStream.CheckNoSpaceLeft();
				byte[] buffer = sBuffer.ToArray();
				StreamBufferPool.Push( sBuffer );
				this.Send( sessionType, sessionId, buffer );
			}
			else
			{
				//int n32MsgSize = sMsg.ByteSize();
				//int n32Length = n32MsgSize + 4 * sizeof( int );
				//char* pBuffer = new char[n32Length];
				//memcpy( pBuffer + 0 * sizeof( int ), ( char* )&stype, sizeof( int ) );
				//memcpy( pBuffer + 1 * sizeof( int ), ( char* )&sessionId, sizeof( int ) );
				//n32Length = n32MsgSize + 8;
				//memcpy( pBuffer + 2 * sizeof( int ), ( char* )&n32Length, sizeof( int ) );
				//memcpy( pBuffer + 3 * sizeof( int ), ( char* )&n32MsgID, sizeof( int ) );
				//bool res = sMsg.SerializeToArray( pBuffer + 4 * sizeof( int ), n32MsgSize );
				//Assert( res );
				//EnterCriticalSection( &mNetworkCs );
				//m_SafeQueue.push_back( pBuffer );
				//LeaveCriticalSection( &mNetworkCs );
			}
		}

		public void TranMsgToSession( SessionType stype, int sessionId, ByteString bs, int n32MsgID, int n32TransId, int n32GcNet )
		{
			if ( n32TransId == 0 ) n32TransId = n32MsgID;//无法伪装

			if ( !this._unSafeSend )
			{
				int n32Length = bs.Length + 4 * sizeof( int );
				StreamBuffer sBuffer = StreamBufferPool.Pop();
				sBuffer.Write( n32Length );
				sBuffer.Write( n32TransId );
				sBuffer.Write( n32MsgID );
				sBuffer.Write( n32GcNet );
				bs.WriteTo( sBuffer.ms );
				byte[] buffer = sBuffer.ToArray();
				StreamBufferPool.Push( sBuffer );
				this.Send( stype, sessionId, buffer );
			}
			else
			{
				//int n32Length = n32MsgLen + 6 * sizeof( int );
				//char* pBuffer = new char[n32Length];
				//memcpy( pBuffer + 0 * sizeof( int ), ( char* )&stype, sizeof( int ) );
				//memcpy( pBuffer + 1 * sizeof( int ), ( char* )&sessionId, sizeof( int ) );
				//n32Length = n32MsgLen + 16;
				//memcpy( pBuffer + 2 * sizeof( int ), ( char* )&n32Length, sizeof( int ) );
				//memcpy( pBuffer + 3 * sizeof( int ), ( char* )&n32TransId, sizeof( int ) );//伪装消息ID
				//memcpy( pBuffer + 4 * sizeof( int ), ( char* )&n32MsgID, sizeof( int ) );//真实消息ID
				//memcpy( pBuffer + 5 * sizeof( int ), ( char* )&n32GcNet, sizeof( int ) );//插入
				//memcpy( pBuffer + 6 * sizeof( int ), msgBuffer, n32MsgLen );
				//EnterCriticalSection( &mNetworkCs );
				//m_SafeQueue.push_back( pBuffer );
				//LeaveCriticalSection( &mNetworkCs );
			}
		}

		private void Send( SessionType sessionType, int sessionId, byte[] buffer )
		{
			if ( sessionId > 0 )
			{
				NetSession session = this.GetSession( sessionId );
				session?.Send( buffer, buffer.Length );
			}
			else
			{
				foreach ( KeyValuePair<int, NetSession> kv in this._idToSession )
				{
					NetSession session = kv.Value;
					if ( session.type != sessionType )
						continue;
					session.Send( buffer, buffer.Length );
					if ( sessionId == 0 ) break;
				}
			}
		}

		public void Update()
		{
			NetEventMgr.instance.PopEvents( this._events );
			while ( this._events.Count > 0 )
			{
				NetEvent netEvent = this._events.Dequeue();
				switch ( netEvent.type )
				{
					case NetEvent.Type.Invalid:
						break;
					case NetEvent.Type.Establish:
						netEvent.session.OnEstablish();
						break;
					case NetEvent.Type.ConnErr:
						break;
					case NetEvent.Type.Error:
						break;
					case NetEvent.Type.Terminate:
						break;
					case NetEvent.Type.Recv:
						netEvent.session.OnRecv( netEvent.data, netEvent.size );
						break;
					case NetEvent.Type.Send:
						break;
					case NetEvent.Type.BindErr:
						break;
				}
				NetEventMgr.instance.pool.Push( netEvent );
			}
		}
	}
}