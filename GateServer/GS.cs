using Core.Misc;
using Core.Net;
using GateServer.Net;
using Google.Protobuf;
using Shared;
using Shared.Net;
using System;
using System.Net.Sockets;

namespace GateServer
{
	public sealed class GS
	{
		private static GS _instance;
		public static GS instance => _instance ?? ( _instance = new GS() );

		public GSConfig gsConfig { get; }
		public GSStorage gsStorage { get; }

		private readonly GSNetSessionMgr _netSessionMgr;
		private readonly UpdateContext _context;
		private long _timestamp;

		private GS()
		{
			this.gsStorage = new GSStorage();
			this._netSessionMgr = new GSNetSessionMgr();
			this._context = new UpdateContext();
			this.gsConfig = new GSConfig();
		}

		public ErrorCode Initialize()
		{
			ErrorCode eResult = this.gsConfig.Load();
			if ( ErrorCode.Success == eResult )
				Logger.Info( "GS Initialize success" );
			Console.Title = $"GS({this.gsConfig.n32GSID})";
			return eResult;
		}

		public void Dispose()
		{
			this._netSessionMgr.Dispose();
			NetSessionPool.instance.Dispose();
		}

		public ErrorCode Start()
		{
			this.CreateListener( this.gsConfig.n32GCListenPort, 10240, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 0 );
			this.CreateConnector( SessionType.ClientG2C, this.gsConfig.sCSIP, this.gsConfig.n32CSPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240000, 0 );
			this._netSessionMgr.CreateConnector( SessionType.ClientG2B, this.gsConfig.sBSListenIP, this.gsConfig.n32BSListenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 1024000, 0 );
			return ErrorCode.Success;
		}

		public void Update( long elapsed, long dt )
		{
			this._timestamp += dt;
			while ( this._timestamp >= Consts.HEART_PACK )
			{
				++this._context.ticks;
				this._context.utcTime = TimeUtils.utcTime;
				this._context.time = elapsed;
				this._context.deltaTime = Consts.HEART_PACK;
				this._timestamp -= Consts.HEART_PACK;
				ErrorCode eResult = this.OnHeartBeat( this._context );
				if ( ErrorCode.Success != eResult )
				{
					Logger.Error( $"fail with error code {eResult}!, please amend the error and try again!" );
					return;
				}
			}
			this._netSessionMgr.Update();
		}

		private ErrorCode OnHeartBeat( UpdateContext context )
		{
			this._netSessionMgr.OnHeartBeat( context );
			this.gsStorage.PingSS( context.utcTime );
			this.gsStorage.ChechUserToken( context.utcTime );
			this.gsStorage.ReportGsInfo( context.utcTime );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 创建监听器
		/// </summary>
		/// <param name="port">监听端口</param>
		/// <param name="recvsize">接受缓冲区大小</param>
		/// <param name="socketType">套接字类型</param>
		/// <param name="protoType">协议类型</param>
		/// <param name="pos">在列表中的位置</param>
		/// <param name="sessionCreateHandler">创建session的委托</param>
		/// <returns></returns>
		public bool CreateListener( int port, int recvsize, SocketType socketType, ProtocolType protoType, int pos,
									SessionCreateHandler sessionCreateHandler = null ) =>
			this._netSessionMgr.CreateListener( port, recvsize, socketType, protoType, pos, sessionCreateHandler ?? this._netSessionMgr.CreateListenerSession );

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
		public bool CreateConnector( SessionType sessionType, string ip, int port, SocketType socketType,
									 ProtocolType protoType, int recvsize, int logicId ) =>
			this._netSessionMgr.CreateConnector( sessionType, ip, port, socketType, protoType, recvsize, logicId );

		/// <summary>
		/// 停止监听器
		/// </summary>
		/// <param name="pos">列表中的位置</param>
		public void StopListener( int pos ) => this._netSessionMgr.StopListener( pos );

		/// <summary>
		/// 获取指定id的session
		/// </summary>
		public NetSession GetSession( uint sessionID ) => this._netSessionMgr.GetSession( sessionID );

		/// <summary>
		/// 发送消息到指定的session
		/// </summary>
		/// <param name="sessionId">session id</param>
		/// <param name="msg">消息</param>
		/// <param name="msgID">消息id</param>
		public void SendMsgToSession( uint sessionId, IMessage msg, int msgID ) =>
			this._netSessionMgr.SendMsgToSession( sessionId, msg, msgID );

		/// <summary>
		/// 发送消息到指定的session
		/// </summary>
		/// <param name="sessionId">session id</param>
		/// <param name="data">需要发送的数据</param>
		/// <param name="offset">data的偏移量</param>
		/// <param name="size">data的有用的数据长度</param>
		/// <param name="msgID">中介端需要处理的消息id</param>
		public void SendMsgToSession( uint sessionId, byte[] data, int offset, int size, int msgID ) =>
			this._netSessionMgr.SendMsgToSession( sessionId, data, offset, size, msgID );

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
		public void TranMsgToSession( uint sessionId, byte[] data, int offset, int size, int msgID, int transID, uint gcNet ) =>
			this._netSessionMgr.TranMsgToSession( sessionId, data, offset, size, msgID, transID, gcNet );

		/// <summary>
		/// 发送消息到指定的session类型
		/// </summary>
		/// <param name="sessionType">session类型</param>
		/// <param name="msg">消息</param>
		/// <param name="msgID">消息id</param>
		/// <param name="once">在查询消息类型时是否只对第一个结果生效</param>
		public void SendMsgToSession( SessionType sessionType, IMessage msg, int msgID, bool once = true ) =>
			this._netSessionMgr.SendMsgToSession( sessionType, msg, msgID, once );

		/// <summary>
		/// 发送消息到指定的session类型
		/// </summary>
		/// <param name="sessionType">session类型</param>
		/// <param name="data">需要发送的数据</param>
		/// <param name="offset">data的偏移量</param>
		/// <param name="size">data的有用的数据长度</param>
		/// <param name="msgID">消息id</param>
		/// <param name="once">在查询消息类型时是否只对第一个结果生效</param>
		public void SendMsgToSession( SessionType sessionType, byte[] data, int offset, int size, int msgID,
									  bool once = true ) =>
			this._netSessionMgr.SendMsgToSession( sessionType, data, offset, size, msgID );

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
		public void TranMsgToSession( SessionType sessionType, byte[] data, int offset, int size, int msgID, int transID,
									  uint gcNet, bool once = true ) =>
			this._netSessionMgr.TranMsgToSession( sessionType, data, offset, size, msgID, transID, gcNet, once );

		/// <summary>
		/// 消息广播到所有客户端
		/// </summary>
		public int BroadcastToGameClient( byte[] data, int offset, int size, int msgID )
		{
			this._netSessionMgr.SendMsgToSession( SessionType.ServerGS, data, offset, size, msgID, false );
			return 0;
		}

		/// <summary>
		/// 消息分发到指定id的客户端
		/// </summary>
		public int PostToGameClient( uint sessionID, byte[] data, int offset, int size, int msgID )
		{
			this._netSessionMgr.SendMsgToSession( sessionID, data, offset, size, msgID );
			return 0;
		}

		/// <summary>
		/// 消息分发到指定id的客户端
		/// </summary>
		public int PostToGameClient( uint sessionID, IMessage msg, int msgID )
		{
			this._netSessionMgr.SendMsgToSession( sessionID, msg, msgID );
			return 0;
		}

		/// <summary>
		/// 强制客户端下线
		/// </summary>
		public int PostGameClientDisconnect( uint nsID )
		{
			this._netSessionMgr.DisconnectOne( ( uint )nsID );
			return 0;
		}
	}
}