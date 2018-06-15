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
		public GSNetSessionMgr netSessionMrg { get; }
		public GSStorage gsStorage { get; }
		public CSMsgManager csMsgManager { get; }
		public SSMsgManager ssMsgManager { get; }
		public GCMsgManager gcMsgManager { get; }

		private readonly UpdateContext _context;
		private long _timestamp;

		private GSKernel()
		{
			this.csMsgManager = new CSMsgManager();
			this.ssMsgManager = new SSMsgManager();
			this.gcMsgManager = new GCMsgManager();
			this.gsStorage = new GSStorage();
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
			this.netSessionMrg.Dispose();
			NetSessionPool.instance.Dispose();
		}

		public EResult Start()
		{
			this.netSessionMrg.CreateListener( this.gsConfig.n32GCListenPort, 10240, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 0 );
			this.netSessionMrg.CreateConnector( SessionType.ClientG2C, this.gsConfig.sCSIP, this.gsConfig.n32CSPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240000, 0 );
			this.netSessionMrg.CreateConnector( SessionType.ClientG2B, this.gsConfig.sBSListenIP, this.gsConfig.n32BSListenPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 1024000, 0 );
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
			this.gsStorage.OnHeartBeat( context );
			return EResult.Normal;
		}
	}
}