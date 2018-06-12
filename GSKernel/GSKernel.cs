using Core;
using Core.Misc;
using GateServer.Net;
using Shared;
using Shared.Net;
using System.Collections.Generic;

namespace GateServer
{
	public partial class GSKernel
	{
		private static GSKernel _instance;
		public static GSKernel instance => _instance ?? ( _instance = new GSKernel() );

		private const long HEART_PACK = 100;

		public long csTimeError;
		public uint ssBaseIdx;
		public int ssConnectNum;
		public GSConfig gsConfig { get; }
		public GSNetSessionMgr netSessionMrg { get; private set; }
		public CSMsgHandler csMsgHandler { get; private set; }
		public SSMsgHandler ssMsgHandler { get; private set; }

		private readonly UpdateContext _context;
		private long _timestamp;

		private readonly Dictionary<int, GSSSInfo> _GSSSInfoMap = new Dictionary<int, GSSSInfo>();

		private GSKernel()
		{
			this.csMsgHandler = new CSMsgHandler();
			this.ssMsgHandler = new SSMsgHandler();
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
		}

		public EResult Start()
		{
			this.netSessionMrg = new GSNetSessionMgr();
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
			while ( this._timestamp >= HEART_PACK )
			{
				++this._context.ticks;
				this._context.utcTime = TimeUtils.utcTime;
				this._context.time = elapsed;
				this._context.deltaTime = HEART_PACK;
				this._timestamp -= HEART_PACK;
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
			this.CheckSSConnect( context.utcTime );
			this.ChechUserToken( context.utcTime );
			this.ReportGsInfo( context.utcTime );
			return EResult.Normal;
		}

		private EResult CheckSSConnect( long utcTime )
		{
			return EResult.Normal;
		}

		private EResult ChechUserToken( long utcTime )
		{
			return EResult.Normal;
		}

		private void ReportGsInfo( long utcTime )
		{
		}

		public void AddGSSInfo( int ssID, GSSSInfo gsssInfo )
		{
			this._GSSSInfoMap[ssID] = gsssInfo;
		}

		public GSSSInfo GetGSSSInfo( int ssID )
		{
			this._GSSSInfoMap.TryGetValue( ssID, out GSSSInfo ssInfo );
			return ssInfo;
		}

		public bool ContainsSSInfo( int ssID )
		{
			return this._GSSSInfoMap.ContainsKey( ssID );
		}
	}
}