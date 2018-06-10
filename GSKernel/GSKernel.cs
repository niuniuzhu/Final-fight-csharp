using Core;
using Core.Misc;
using GateServer.Net;
using Net;
using System;
using System.Net.Sockets;

namespace GateServer
{
	public class GSKernel
	{
		private static GSKernel _instance;
		public static GSKernel instance => _instance ?? ( _instance = new GSKernel() );

		private const long HEART_PACK = 100;

		public delegate int CSMsgHandler( string msg );
		public delegate int SSMsgHandler( CGSSSInfo piSSInfo, string msg );
		public delegate int PFGCMsgHandler( int n32NSID, string msg );

		public GSConfig gsConfig { get; private set; }
		public GSNetSessionMgr netSessionMrg { private set; get; }

		private readonly CSMsgHandler[] _csMsgHandler;//中心服务器消息
		private readonly SSMsgHandler[] _ssMsgHandler;//场景服务器消息
		private readonly UpdateContext _context;
		private long _timestamp;

		private GSKernel()
		{
			this._csMsgHandler = new CSMsgHandler[100];
			this._ssMsgHandler = new SSMsgHandler[100];
			this._context = new UpdateContext();
			this.gsConfig = new GSConfig();
		}

		public EResult Initialize()
		{
			this._csMsgHandler[CSToGS.MsgID.EMsgToGsfromCsAskPingRet - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_AskPingRet;
			this._csMsgHandler[CSToGS.MsgID.EMsgToGsfromCsOrderOpenListen - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_OrderOpenListen;
			this._csMsgHandler[CSToGS.MsgID.EMsgToGsfromCsOrderCloseListen - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_OrderCloseListen;
			this._csMsgHandler[CSToGS.MsgID.EMsgToGsfromCsOrderKickoutGc - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_OrderKickoutGC;
			this._csMsgHandler[CSToGS.MsgID.EMsgToGsfromCsUserConnectedSs - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_UserConnectedToSS;
			this._csMsgHandler[CSToGS.MsgID.EMsgToGsfromCsUserDisConnectedSs - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_UserDisConnectedToSS;

			this._ssMsgHandler[SSToGS.MsgID.EMsgToGsfromSsAskPingRet - SSToGS.MsgID.EMsgToGsfromSsBegin] = this.OnMsgFromSS_AskPingRet;
			this._ssMsgHandler[SSToGS.MsgID.EMsgToGsfromSsOrderKickoutGc - SSToGS.MsgID.EMsgToGsfromSsBegin] = this.OnMsgFromSS_OrderKickoutGC;

			EResult eResult = this.gsConfig.Load();
			if ( EResult.Normal == eResult )
			{
				Logger.Info( "CGSKernel Initialize success" );
			}
			return eResult;
		}

		public void Dispose()
		{
			if ( this.netSessionMrg != null )
			{
				this.netSessionMrg.Dispose();
				this.netSessionMrg = null;
			}
		}

		public EResult Start()
		{
			this.netSessionMrg = new GSNetSessionMgr();
			this.netSessionMrg.CreateListener( this.gsConfig.n32GCListenPort, 102400, SocketType.Stream, ProtocolType.Tcp, out _ );
			this.netSessionMrg.CreateConnector( SessionType.ClientG2C, this.gsConfig.sCSIP, this.gsConfig.n32CSPort, SocketType.Stream, ProtocolType.Tcp, 10240000, 0 );
			this.netSessionMrg.CreateConnector( SessionType.ClientG2B, this.gsConfig.sBSListenIP, this.gsConfig.n32BSListenPort, SocketType.Stream, ProtocolType.Tcp, 1024000, 0 );
			return EResult.Normal;
		}

		private EResult Stop()
		{
			Logger.Info( "GSKernel Stop success!" );
			return EResult.Normal;
		}

		public void Update( long elapsed, long dt )
		{
			this._timestamp += dt;
			while ( this._timestamp >= HEART_PACK )
			{
				++this._context.ticks;
				this._context.utcTime = TimeUtils.utcTime;
				this._context.timeSinceStartup = elapsed;
				this._context.deltaTime = HEART_PACK;
				this.netSessionMrg.Update();
				this._timestamp -= HEART_PACK;
			}
		}

		private int OnMsgFromCS_AskPingRet( string pmsg )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_OrderOpenListen( string pmsg )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_OrderCloseListen( string pmsg )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_OrderKickoutGC( string pmsg )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_UserConnectedToSS( string pmsg )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_UserDisConnectedToSS( string pmsg )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromSS_AskPingRet( CGSSSInfo pissinfo, string pmsg )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromSS_OrderKickoutGC( CGSSSInfo pissinfo, string pmsg )
		{
			throw new NotImplementedException();
		}
	}
}