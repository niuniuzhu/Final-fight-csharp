using Core.Misc;
using Core.Net;
using LoginServer.Net;
using Shared;
using Shared.Net;

namespace LoginServer
{
	public class LS
	{
		private static LS _instance;
		public static LS instance => _instance ?? ( _instance = new LS() );

		public LSConfig lsConfig { get; }
		public SDKAsynHandler sdkAsynHandler { get; }
		public SdkConnector sdkConnector { get; }
		public LSNetSessionMgr netSessionMgr { get; }

		private readonly UpdateContext _context;
		private long _timestamp;

		private LS()
		{
			this._context = new UpdateContext();
			this.lsConfig = new LSConfig();
			this.sdkAsynHandler = new SDKAsynHandler();
			this.sdkConnector = new SdkConnector();
			this.netSessionMgr = new LSNetSessionMgr();
		}

		public void Dispose()
		{
			this.sdkAsynHandler.Dispose();
			this.netSessionMgr.Dispose();
			NetSessionPool.instance.Dispose();
		}

		public ErrorCode Initialize()
		{
			ErrorCode eResult = this.lsConfig.Load();
			if ( ErrorCode.Success == eResult )
				Logger.Info( "LS Initialize success" );
			return eResult;
		}

		public ErrorCode Start()
		{
			this.netSessionMgr.CreateListener( this.lsConfig.bs_listen_port, 102400, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE,
												0, this.netSessionMgr.CreateBlanceSession );//GS端口长连接
			this.netSessionMgr.CreateListener( this.lsConfig.client_listen_port, 102400, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE,
												1, this.netSessionMgr.CreateClientSession );//GC端口短连接
			this.sdkAsynHandler.Init();
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
			this.sdkConnector.Update();
			this.netSessionMgr.Update();
		}

		private ErrorCode OnHeartBeat( UpdateContext context )
		{
			this.netSessionMgr.OnHeartBeat( context );
			return ErrorCode.Success;
		}
	}
}