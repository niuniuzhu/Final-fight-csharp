using CentralServer.Net;
using CentralServer.Tools;
using CentralServer.UserModule;
using Core.Misc;
using Core.Net;
using Shared;
using Shared.Net;
using StackExchange.Redis;
using System;
using System.Collections;
using System.IO;

namespace CentralServer
{

	public delegate void HeartbeatCallback( long curTime, long tickSpan );

	public class CS
	{
		private static CS _instance;
		public static CS instance => _instance ?? ( _instance = new CS() );

		public CSKernelCfg csCfg { get; }
		public CSCfgMgr csscCfg { get; }
		public CSSSInfo[] ssInfoList { get; private set; }
		public CSGSInfo[] gsInfoList { get; private set; }
		public string remoteConsolekey { get; private set; }

		public CSNetSessionMgr netSessionMgr { get; }
		public CSUserMgr csUserMgr { get; }
		public BattleTimer battleTimer { get; }

		public SSNetInfo[] ssNetInfoList { get; private set; }
		public GSNetInfo[] gsNetInfoList { get; private set; }
		public RCNetInfo[] rcNetInfoList { get; private set; }

		private ConnectionMultiplexer _userDBredisAsyncContext;

		public CS()
		{
			this.csCfg = new CSKernelCfg();
			this.csscCfg = new CSCfgMgr();
			this.netSessionMgr = new CSNetSessionMgr();
			this.csUserMgr = new CSUserMgr();
			this.battleTimer = new BattleTimer();
		}

		public void Dispose()
		{
			this.netSessionMgr.Dispose();
			NetSessionPool.instance.Dispose();
		}

		public ErrorCode Initialize()
		{
			ErrorCode eResult = this.LoadCfg();
			if ( ErrorCode.Success == eResult )
				Logger.Info( "CS Initialize success" );

			this.ssNetInfoList = new SSNetInfo[this.csCfg.un32MaxSSNum];
			for ( int i = 0; i < this.csCfg.un32MaxSSNum; i++ )
				this.ssNetInfoList[i] = new SSNetInfo();

			this.gsNetInfoList = new GSNetInfo[this.csCfg.un32MaxGSNum];
			for ( int i = 0; i < this.csCfg.un32MaxGSNum; i++ )
				this.gsNetInfoList[i] = new GSNetInfo();

			this.rcNetInfoList = new RCNetInfo[10];

			//todo addtimer

			return eResult;
		}

		#region load config
		private ErrorCode LoadCfg()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\CSCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load CSCfg.xml failed for {e}" );
				return ErrorCode.CfgFailed;
			}
			this.csCfg.unCSId = json.GetUInt( "GameWorldID" );
			this.csCfg.ipaddress = json.GetString( "CSIP" );
			this.csCfg.n32SSNetListenerPort = json.GetInt( "SSPort" );
			this.csCfg.n32GSNetListenerPort = json.GetInt( "GSPort" );
			this.csCfg.un32MaxSSNum = json.GetUInt( "MaxSSNum" );
			this.csCfg.un32SSBaseIdx = json.GetUInt( "SSBaseIndex" );
			this.csCfg.un32MaxGSNum = json.GetUInt( "MaxGSNum" );
			this.csCfg.un32GSBaseIdx = json.GetUInt( "GSBaseIndex" );
			this.csCfg.maxWaitingDBNum = json.GetInt( "WaitingDBNum" );
			this.csCfg.redisAddress = json.GetString( "redisAddress" );
			this.csCfg.redisPort = json.GetInt( "redisPort" );
			this.csCfg.redisLogicAddress = json.GetString( "redisLogicAddress" );
			this.csCfg.redisLogicPort = json.GetInt( "redisLogicPort" );
			this.csCfg.LogAddress = json.GetString( "LogAddress" );
			this.csCfg.LogPort = json.GetInt( "LogPort" );

			string ssIndexStr = json.GetString( "AllSSIndex" );
			string[] ssIndexVec = ssIndexStr.Split( ';' );

			if ( ssIndexVec.Length > 100000 )
			{
				Logger.Warn( $"too many ss!" );
				return ErrorCode.CfgFailed;
			}

			this.ssInfoList = new CSSSInfo[ssIndexVec.Length];
			for ( int i = 0; i != ssIndexVec.Length; ++i )
			{
				string[] ssInfoVec = ssIndexVec[i].Split( ',' );
				if ( ssInfoVec.Length != 3 )
				{
					Logger.Error( "load CSCfg.xml failed." );
					continue;
				}
				CSSSInfo csssInfo = new CSSSInfo();
				csssInfo.m_n32SSID = int.Parse( ssInfoVec[0] );
				csssInfo.m_szName = ssInfoVec[1];
				csssInfo.m_szUserPwd = ssInfoVec[2];
				this.ssInfoList[i] = csssInfo;
			}

			string gsIndexStr = json.GetString( "AllGSIndex" );
			string[] gsIndexVec = gsIndexStr.Split( ';' );
			this.gsInfoList = new CSGSInfo[gsIndexVec.Length];
			for ( int i = 0; i != gsIndexVec.Length; ++i )
			{
				string[] gsInfoVec = gsIndexVec[i].Split( ',' );
				if ( gsInfoVec.Length != 3 )
				{
					Logger.Error( "load CSCfg.xml failed." );
					continue;
				}
				CSGSInfo csgsInfo = new CSGSInfo();
				csgsInfo.m_n32GSID = int.Parse( gsInfoVec[0] );
				csgsInfo.m_szName = gsInfoVec[1];
				csgsInfo.m_szUserPwd = gsInfoVec[2];
				this.gsInfoList[i] = csgsInfo;
			}

			this.csCfg.n32RCNetListenerPort = json.GetInt( "RSPort" );
			this.remoteConsolekey = json.GetString( "RSKey" );

			return ErrorCode.Success;
		}
		#endregion

		public ErrorCode Start()
		{
			this.netSessionMgr.CreateListener( this.csCfg.n32GSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateGateSession );
			this.netSessionMgr.CreateListener( this.csCfg.n32SSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateSceneSession );
			this.netSessionMgr.CreateListener( this.csCfg.n32RCNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateRemoteConsoleSession );
			this.netSessionMgr.CreateConnector( SessionType.ClientC2Log, this.csCfg.LogAddress,
												this.csCfg.LogPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 102400,
												0 );

			ConfigurationOptions config = new ConfigurationOptions
			{
				EndPoints =
				{
					{ "juntai.yytou.com", 23680 }
				},
				KeepAlive = 180,
				AbortOnConnectFail = false,
				Password = "159753"
			};
			this._userDBredisAsyncContext = ConnectionMultiplexer.Connect( config );
			if ( !this._userDBredisAsyncContext.IsConnected )
				Logger.Error( "connect to redis fail." );

			//if ( this.csCfg.redisLogicAddress != "0" )
			//{
			//	this.netSessionMgr.CreateConnector( SessionType.ClientC2LogicRedis, this.csCfg.redisLogicAddress,
			//										this.csCfg.redisLogicPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE,
			//										102400, 0 );
			//}

			return ErrorCode.Success;
		}

		public void Update( long elapsed, long dt )
		{
			//todo
			this.netSessionMgr.Update();
		}

		public CSGSInfo GetCGSInfoByNSID( uint nsID )
		{
			for ( int i = 0; i < this.csCfg.un32MaxGSNum; ++i )
			{
				CSGSInfo csgsInfo = this.gsNetInfoList[i].pcGSInfo;
				if ( null == csgsInfo ) continue;
				if ( csgsInfo.m_n32NSID == nsID )
					return csgsInfo;
			}
			return null;
		}

		public CSGSInfo GetGSInfoByGSID( int gsID )
		{
			CSGSInfo pcInfo = null;
			uint index = ( uint )gsID - this.csCfg.un32GSBaseIdx;
			if ( index < this.csCfg.un32MaxGSNum )
				pcInfo = this.gsInfoList[index];
			return pcInfo;
		}

		public ErrorCode InvokeGCMsg( CSGSInfo csgsInfo, int msgID, uint gcNetID, byte[] data, int offset, int size ) =>
			this.csUserMgr.Invoke( csgsInfo, msgID, gcNetID, data, offset, size );

		public ConnectionMultiplexer GetUserDBCacheRedisHandler() => this._userDBredisAsyncContext;

		public long AddTimer( HeartbeatCallback pHeartbeatCallback, long interval, bool ifPersist ) =>
			this.battleTimer.AddTimer( pHeartbeatCallback, interval, ifPersist );

		public void RemoveTimer( long timerID ) => this.battleTimer.RemoveTimer( timerID );
	}
}