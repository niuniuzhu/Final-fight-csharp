using BalanceServer.Net;
using Core.Misc;
using Core.Net;
using Shared;
using Shared.Net;

namespace BalanceServer
{
	public class BS
	{
		private static BS _instance;
		public static BS instance => _instance ?? ( _instance = new BS() );

		public BSConfig bsConfig { get; }

		private readonly BSNetSessionMgr _netSessionMgr;
		private readonly UpdateContext _context;
		private long _timestamp;

		private BS()
		{
			this._netSessionMgr = new BSNetSessionMgr();
			this._context = new UpdateContext();
			this.bsConfig = new BSConfig();
		}

		public void Dispose()
		{
			this._netSessionMgr.Dispose();
			NetSessionPool.instance.Dispose();
		}

		public EResult Initialize()
		{
			EResult eResult = this.bsConfig.Load();
			if ( EResult.Normal == eResult )
				Logger.Info( "BS Initialize success" );
			return eResult;
		}

		public EResult Start()
		{
			this._netSessionMgr.CreateListener( this.bsConfig.gs_listen_port, 102400, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE,
												0, this._netSessionMgr.CreateGateSession );
			this._netSessionMgr.CreateListener( this.bsConfig.client_listen_port, 102400, Consts.SOCKET_TYPE,
												Consts.PROTOCOL_TYPE, 1, this._netSessionMgr.CreateClientSession );
			this._netSessionMgr.CreateConnector( SessionType.ClientB2L, this.bsConfig.ls_ip, this.bsConfig.ls_port,
												 Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 102400, 0 );
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
			this._netSessionMgr.Update();
		}

		private EResult OnHeartBeat( UpdateContext context )
		{
			this._netSessionMgr.OnHeartBeat( context );
			return EResult.Normal;
		}
	}
}