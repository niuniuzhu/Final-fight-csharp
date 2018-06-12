using Core.Net;
using Google.Protobuf;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Shared.Net
{
	public abstract class NetSessionMgr
	{
		private readonly IListener[] _listeners = new IListener[Consts.MAX_COUNT_LISTENER];
		private readonly Dictionary<uint, NetSession> _idToSession = new Dictionary<uint, NetSession>();
		private readonly Queue<NetEvent> _events = new Queue<NetEvent>();

		public void Dispose()
		{
			this._events.Clear();
		}

		public void AddSession( NetSession session )
		{
			this._idToSession[session.id] = session;
		}

		public bool RemoveSession( NetSession session )
		{
			return this._idToSession.Remove( session.id );
		}

		public bool CreateListener( int port, int recvBufSize, SocketType socketType, ProtocolType protoType, int pos )
		{
			if ( pos >= Consts.MAX_COUNT_LISTENER ) return false;
			if ( this._listeners[pos] != null ) return false;
			IListener listener = new Listener();
			listener.sessionCreateHandler = this.CreateListenerSession;
			listener.packetEncodeHandler = LengthEncoder.Encode;
			listener.packetDecodeHandler = LengthEncoder.Decode;
			listener.recvBufSize = recvBufSize;
			this._listeners[pos] = listener;
			return this._listeners[pos].Start( "0", port, socketType, protoType );
		}

		public bool CreateConnector( SessionType sessionType, string ip, int port, SocketType socketType, ProtocolType protoType, int recvsize, int logicId )
		{
			CliSession session = this.CreateConnectorSession( sessionType );
			this.AddSession( session );
			session.logicID = logicId;
			session.connector.recvBufSize = recvsize;
			session.connector.packetDecodeHandler = LengthEncoder.Decode;
			return session.Connect( ip, port, socketType, protoType );
		}

		public void StopListener( int pos )
		{
			if ( pos < Consts.MAX_COUNT_LISTENER )
				this._listeners[pos].Stop();
		}

		protected abstract SrvCliSession CreateListenerSession();

		protected abstract CliSession CreateConnectorSession( SessionType sessionType );

		public NetSession GetSession( uint sessionID )
		{
			this._idToSession.TryGetValue( sessionID, out NetSession session );
			return session;
		}

		public void SendMsgToSession( SessionType sessionType, uint sessionId, IMessage msg, int msgID )
		{
			byte[] data = msg.ToByteArray();
			int size = data.Length;
			int len = size + 2 * sizeof( int );
			StreamBuffer sBuffer = StreamBufferPool.Pop();
			sBuffer.Write( len );
			sBuffer.Write( msgID );
			sBuffer.Write( data, 0, size );
			byte[] buffer = sBuffer.ToArray();
			StreamBufferPool.Push( sBuffer );
			this.Send( sessionType, sessionId, buffer );
		}

		public void SendMsgToSession( SessionType stype, uint sessionId, byte[] data, int offset, int size, int msgID )
		{
			int len = size + 2 * sizeof( int );
			StreamBuffer sBuffer = StreamBufferPool.Pop();
			sBuffer.Write( len );
			sBuffer.Write( msgID );
			sBuffer.Write( data, offset, size );
			byte[] buffer = sBuffer.ToArray();
			StreamBufferPool.Push( sBuffer );
			this.Send( stype, sessionId, buffer );
		}

		public void TranMsgToSession( SessionType stype, uint sessionId, byte[] data, int offset, int size, int msgID, int transID, int gcNet )
		{
			transID = transID == 0 ? msgID : transID;
			int len = size + 4 * sizeof( int );
			StreamBuffer sBuffer = StreamBufferPool.Pop();
			sBuffer.Write( len );
			sBuffer.Write( transID );
			sBuffer.Write( msgID );
			sBuffer.Write( gcNet );
			sBuffer.Write( data, offset, size );
			byte[] buffer = sBuffer.ToArray();
			StreamBufferPool.Push( sBuffer );
			this.Send( stype, sessionId, buffer );
		}

		private void Send( SessionType sessionType, uint sessionId, byte[] buffer )
		{
			if ( sessionId != uint.MaxValue )
			{
				NetSession session = this.GetSession( sessionId );
				session?.Send( buffer, buffer.Length );
			}
			else
			{
				foreach ( KeyValuePair<uint, NetSession> kv in this._idToSession )
				{
					NetSession session = kv.Value;
					if ( session.type != sessionType )
						continue;
					session.Send( buffer, buffer.Length );
					if ( sessionId == 0 ) break;
				}
			}
		}

		public void OnHeartBeat( UpdateContext context )
		{
			foreach ( KeyValuePair<uint, NetSession> kv in this._idToSession )
				kv.Value.OnHeartBeat( context );
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
						( ( CliSession )netEvent.session ).OnConnError( netEvent.error );
						break;
					case NetEvent.Type.Error:
						netEvent.session.OnError( netEvent.error );
						break;
					case NetEvent.Type.Terminate:
						netEvent.session.OnTerminate();
						break;
					case NetEvent.Type.Recv:
						netEvent.session.OnRecv( netEvent.data, 0, netEvent.data.Length );
						break;
					case NetEvent.Type.Send:
						netEvent.session.OnSend();
						break;
					case NetEvent.Type.BindErr:
						break;
				}
				NetEventMgr.instance.pool.Push( netEvent );
			}
		}
	}
}