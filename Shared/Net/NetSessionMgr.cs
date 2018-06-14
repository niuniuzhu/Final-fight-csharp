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
			foreach ( IListener listener in this._listeners )
				listener?.Dispose();
			foreach ( KeyValuePair<uint, NetSession> kv in this._idToSession )
				kv.Value.Close();
			this._idToSession.Clear();
			this._events.Clear();
		}

		public void AddSession( NetSession session ) => this._idToSession[session.id] = session;

		public bool RemoveSession( NetSession session ) => this._idToSession.Remove( session.id );

		/// <summary>
		/// 创建监听器
		/// </summary>
		/// <param name="port">监听端口</param>
		/// <param name="recvsize">接受缓冲区大小</param>
		/// <param name="socketType">套接字类型</param>
		/// <param name="protoType">协议类型</param>
		/// <param name="pos">在列表中的位置</param>
		/// <returns></returns>
		public bool CreateListener( int port, int recvsize, SocketType socketType, ProtocolType protoType, int pos )
		{
			if ( pos >= Consts.MAX_COUNT_LISTENER ) return false;
			if ( this._listeners[pos] != null ) return false;
			IListener listener = new Listener();
			listener.sessionCreateHandler = this.CreateListenerSession;
			listener.packetEncodeHandler = LengthEncoder.Encode;
			listener.packetDecodeHandler = LengthEncoder.Decode;
			listener.recvBufSize = recvsize;
			this._listeners[pos] = listener;
			return this._listeners[pos].Start( "0", port, socketType, protoType );
		}

		/// <summary>
		/// 创建连接器
		/// </summary>
		/// <param name="sessionType">session类型</param>
		/// <param name="ip">ip地址</param>
		/// <param name="port">远程端口</param>
		/// <param name="socketType">套接字类型</param>
		/// <param name="protoType">协议类型</param>
		/// <param name="recvsize">接受缓冲区大小</param>
		/// <param name="logicId">逻辑id(目前仅用于连接场景服务器时记下连接器和逻辑id的映射)</param>
		/// <returns></returns>
		public bool CreateConnector( SessionType sessionType, string ip, int port, SocketType socketType, ProtocolType protoType, int recvsize, int logicId )
		{
			CliSession session = this.CreateConnectorSession( sessionType );
			this.AddSession( session );
			session.logicID = logicId;
			session.connector.recvBufSize = recvsize;
			session.connector.packetDecodeHandler = LengthEncoder.Decode;
			return session.Connect( ip, port, socketType, protoType );
		}

		/// <summary>
		/// 停止监听器
		/// </summary>
		/// <param name="pos">列表中的位置</param>
		public void StopListener( int pos )
		{
			if ( pos < Consts.MAX_COUNT_LISTENER )
				this._listeners[pos].Stop();
		}

		protected abstract SrvCliSession CreateListenerSession();

		protected abstract CliSession CreateConnectorSession( SessionType sessionType );

		/// <summary>
		/// 获取指定id的session
		/// </summary>
		public NetSession GetSession( uint sessionID )
		{
			this._idToSession.TryGetValue( sessionID, out NetSession session );
			return session;
		}

		/// <summary>
		/// 发送消息到指定的地方session
		/// </summary>
		/// <param name="sessionId">session id</param>
		/// <param name="msg">消息</param>
		/// <param name="msgID">消息id</param>
		public void SendMsgToSession( uint sessionId, IMessage msg, int msgID )
		{
			byte[] data = msg.ToByteArray();
			this.SendMsgToSession( sessionId, data, 0, data.Length, msgID );
		}

		/// <summary>
		/// 发送消息到指定的session
		/// </summary>
		/// <param name="sessionId">session id</param>
		/// <param name="data">需要发送的数据</param>
		/// <param name="offset">data的偏移量</param>
		/// <param name="size">data的有用的数据长度</param>
		/// <param name="msgID">中介端需要处理的消息id</param>
		public void SendMsgToSession( uint sessionId, byte[] data, int offset, int size, int msgID )
		{
			StreamBuffer sBuffer = StreamBufferPool.Pop();
			sBuffer.Write( size + 2 * sizeof( int ) );
			sBuffer.Write( msgID );
			sBuffer.Write( data, offset, size );
			byte[] buffer = sBuffer.ToArray();
			StreamBufferPool.Push( sBuffer );
			this.Send( sessionId, buffer );
		}

		/// <summary>
		/// 发送消息到指定session,通常该消息是一条转发消息
		/// </summary>
		/// <param name="sessionId">session id</param>
		/// <param name="data">需要发送的数据</param>
		/// <param name="offset">data的偏移量</param>
		/// <param name="size">data的有用的数据长度</param>
		/// <param name="msgID">中介端需要处理的消息id</param>
		/// <param name="transID">目标端需要处理的消息id</param>
		/// <param name="gcNet">目标端的网络id</param>
		public void TranMsgToSession( uint sessionId, byte[] data, int offset, int size, int msgID, int transID, uint gcNet )
		{
			transID = transID == 0 ? msgID : transID;
			StreamBuffer sBuffer = StreamBufferPool.Pop();
			sBuffer.Write( size + 4 * sizeof( int ) );
			sBuffer.Write( transID );
			sBuffer.Write( msgID );
			sBuffer.Write( gcNet );
			sBuffer.Write( data, offset, size );
			byte[] buffer = sBuffer.ToArray();
			StreamBufferPool.Push( sBuffer );
			this.Send( sessionId, buffer );
		}

		/// <summary>
		/// 发送消息到指定的session类型
		/// </summary>
		/// <param name="sessionType">session类型</param>
		/// <param name="msg">消息</param>
		/// <param name="msgID">消息id</param>
		/// <param name="once">在查询消息类型时是否只对第一个结果生效</param>
		public void SendMsgToSession( SessionType sessionType, IMessage msg, int msgID, bool once = true )
		{
			byte[] data = msg.ToByteArray();
			this.SendMsgToSession( sessionType, data, 0, data.Length, msgID, once );
		}

		/// <summary>
		/// 发送消息到指定的session类型
		/// </summary>
		/// <param name="sessionType">session类型</param>
		/// <param name="data">需要发送的数据</param>
		/// <param name="offset">data的偏移量</param>
		/// <param name="size">data的有用的数据长度</param>
		/// <param name="msgID">消息id</param>
		/// <param name="once">在查询消息类型时是否只对第一个结果生效</param>
		public void SendMsgToSession( SessionType sessionType, byte[] data, int offset, int size, int msgID, bool once = true )
		{
			StreamBuffer sBuffer = StreamBufferPool.Pop();
			sBuffer.Write( size + 2 * sizeof( int ) );
			sBuffer.Write( msgID );
			sBuffer.Write( data, offset, size );
			byte[] buffer = sBuffer.ToArray();
			StreamBufferPool.Push( sBuffer );
			this.Send( sessionType, buffer, once );
		}

		/// <summary>
		/// 发送消息到指定session,通常该消息是一条转发消息
		/// </summary>
		/// <param name="sessionType">session类型</param>
		/// <param name="data">需要发送的数据</param>
		/// <param name="offset">data的偏移量</param>
		/// <param name="size">data的有用的数据长度</param>
		/// <param name="msgID">中介端需要处理的消息id</param>
		/// <param name="transID">目标端需要处理的消息id</param>
		/// <param name="gcNet">目标端的网络id</param>
		/// <param name="once">在查询消息类型时是否只对第一个结果生效</param>
		public void TranMsgToSession( SessionType sessionType, byte[] data, int offset, int size, int msgID, int transID, uint gcNet, bool once = true )
		{
			transID = transID == 0 ? msgID : transID;
			StreamBuffer sBuffer = StreamBufferPool.Pop();
			sBuffer.Write( size + 4 * sizeof( int ) );
			sBuffer.Write( transID );
			sBuffer.Write( msgID );
			sBuffer.Write( gcNet );
			sBuffer.Write( data, offset, size );
			byte[] buffer = sBuffer.ToArray();
			StreamBufferPool.Push( sBuffer );
			this.Send( sessionType, buffer, once );
		}

		private void Send( uint sessionId, byte[] buffer )
		{
			NetSession session = this.GetSession( sessionId );
			session?.Send( buffer, buffer.Length );
		}

		private void Send( SessionType sessionType, byte[] buffer, bool once )
		{
			foreach ( KeyValuePair<uint, NetSession> kv in this._idToSession )
			{
				NetSession session = kv.Value;
				if ( session.type != sessionType )
					continue;
				session.Send( buffer, buffer.Length );
				if ( once ) break;
			}
		}

		public void DisconnectOne( uint sessionId ) => this.GetSession( sessionId )?.Close();

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
					case NetEvent.Type.Recv:
						netEvent.session.OnRecv( netEvent.data, 0, netEvent.data.Length );
						break;
					case NetEvent.Type.Send:
						netEvent.session.OnSend();
						break;
				}
				NetEventMgr.instance.pool.Push( netEvent );
			}
		}
	}
}