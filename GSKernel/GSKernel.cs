using Core;
using Core.Misc;
using Core.Net;
using GateServer.Net;
using Shared;
using Shared.Net;

namespace GateServer
{
	public sealed class GSKernel
	{
		private static GSKernel _instance;
		public static GSKernel instance => _instance ?? ( _instance = new GSKernel() );

		public GSConfig gsConfig { get; }
		public GSNetSessionMgr netSessionMrg { get; private set; }
		public UserTokenMgr userTokenMgr { get; private set; }
		public CSMsgManager csMsgManager { get; private set; }
		public SSMsgManager ssMsgManager { get; private set; }

		public long csTimeError;
		public uint ssBaseIdx;
		public int ssConnectNum;

		private readonly UpdateContext _context;
		private long _timestamp;

		private GSKernel()
		{
			this.csMsgManager = new CSMsgManager();
			this.ssMsgManager = new SSMsgManager();
			this.userTokenMgr = new UserTokenMgr();
			this.netSessionMrg = new GSNetSessionMgr();
			this._context = new UpdateContext();
			this.gsConfig = new GSConfig();
		}

		public EResult Initialize()
		{

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
			NetSessionPool.instance.Dispose();
		}

		public EResult Start()
		{
			this.netSessionMrg.CreateListener( this.gsConfig.n32GCListenPort, 10240, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 0 );
			this.netSessionMrg.CreateConnector( SessionType.ClientG2C, this.gsConfig.sCSIP, this.gsConfig.n32CSPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240000, 0 );
			this.netSessionMrg.CreateConnector( SessionType.ClientG2B, this.gsConfig.sBSListenIP, this.gsConfig.n32BSListenPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 1024000, 0 );
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
			while ( this._timestamp >= Consts.HEART_PACK )
			{
				++this._context.ticks;
				this._context.utcTime = TimeUtils.utcTime;
				this._context.time = elapsed;
				this._context.deltaTime = Consts.HEART_PACK;
				this._timestamp -= Consts.HEART_PACK;
				EResult eResult = this.OnHeartBeat( this._context );
				if ( EResult.Normal != eResult )
				{
					Logger.Error( $"fail with error code {eResult}!, please amend the error and try again!" );
					return;
				}
			}
			this.netSessionMrg.Update();
		}

		private EResult OnHeartBeat( UpdateContext context )
		{
			this.netSessionMrg.OnHeartBeat( context );
			this.ssMsgManager.PingSS( context.utcTime );
			this.userTokenMgr.ChechUserToken( context.utcTime );
			this.userTokenMgr.ReportGsInfo( context.utcTime );
			return EResult.Normal;
		}
	}
}