using CentralServer.Net;
using CentralServer.Tools;
using CentralServer.UserModule;
using Core.Misc;
using Core.Net;
using Google.Protobuf;
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

		public CSKernelCfg csKernelCfg { get; }
		public CSCfgMgr csCfg { get; }
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
			this.csKernelCfg = new CSKernelCfg();
			this.csCfg = new CSCfgMgr();
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
			Console.Title = "CS";

			ErrorCode eResult = this.LoadCfg();
			if ( ErrorCode.Success == eResult )
				Logger.Info( "CS Initialize success" );

			this.ssNetInfoList = new SSNetInfo[this.csKernelCfg.un32MaxSSNum];
			for ( int i = 0; i < this.csKernelCfg.un32MaxSSNum; i++ )
				this.ssNetInfoList[i] = new SSNetInfo();

			this.gsNetInfoList = new GSNetInfo[this.csKernelCfg.un32MaxGSNum];
			for ( int i = 0; i < this.csKernelCfg.un32MaxGSNum; i++ )
				this.gsNetInfoList[i] = new GSNetInfo();

			this.rcNetInfoList = new RCNetInfo[10];
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
			this.csKernelCfg.unCSId = json.GetUInt( "GameWorldID" );
			this.csKernelCfg.ipaddress = json.GetString( "CSIP" );
			this.csKernelCfg.n32SSNetListenerPort = json.GetInt( "SSPort" );
			this.csKernelCfg.n32GSNetListenerPort = json.GetInt( "GSPort" );
			this.csKernelCfg.un32MaxSSNum = json.GetUInt( "MaxSSNum" );
			this.csKernelCfg.un32SSBaseIdx = json.GetUInt( "SSBaseIndex" );
			this.csKernelCfg.un32MaxGSNum = json.GetUInt( "MaxGSNum" );
			this.csKernelCfg.un32GSBaseIdx = json.GetUInt( "GSBaseIndex" );
			this.csKernelCfg.maxWaitingDBNum = json.GetInt( "WaitingDBNum" );
			this.csKernelCfg.redisAddress = json.GetString( "redisAddress" );
			this.csKernelCfg.redisPort = json.GetInt( "redisPort" );
			this.csKernelCfg.redisPwd = json.GetString( "redisPwd" );
			this.csKernelCfg.redisLogicAddress = json.GetString( "redisLogicAddress" );
			this.csKernelCfg.redisLogicPort = json.GetInt( "redisLogicPort" );
			this.csKernelCfg.redisLogicPwd = json.GetString( "redisLogicPwd" );
			this.csKernelCfg.LogAddress = json.GetString( "LogAddress" );
			this.csKernelCfg.LogPort = json.GetInt( "LogPort" );

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

			this.csKernelCfg.n32RCNetListenerPort = json.GetInt( "RSPort" );
			this.remoteConsolekey = json.GetString( "RSKey" );

			return ErrorCode.Success;
		}
		#endregion

		public ErrorCode Start()
		{
			this.netSessionMgr.CreateListener( this.csKernelCfg.n32GSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateGateSession );
			this.netSessionMgr.CreateListener( this.csKernelCfg.n32SSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateSceneSession );
			this.netSessionMgr.CreateListener( this.csKernelCfg.n32RCNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateRemoteConsoleSession );
			this.netSessionMgr.CreateConnector( SessionType.ClientC2Log, this.csKernelCfg.LogAddress,
												this.csKernelCfg.LogPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 102400,
												0 );

			ConfigurationOptions config = new ConfigurationOptions
			{
				EndPoints =
				{
					{ this.csKernelCfg.redisAddress, this.csKernelCfg.redisPort }
				},
				KeepAlive = 180,
				AbortOnConnectFail = false,
				Password = this.csKernelCfg.redisPwd
			};
			this._userDBredisAsyncContext = ConnectionMultiplexer.Connect( config );
			if ( !this._userDBredisAsyncContext.IsConnected )
				Logger.Error( "connect to redis fail." );

			//if ( this.csKernelCfg.redisLogicAddress != "0" )
			//{
			//	this.netSessionMgr.CreateConnector( SessionType.ClientC2LogicRedis, this.csKernelCfg.redisLogicAddress,
			//										this.csKernelCfg.redisLogicPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE,
			//										102400, 0 );
			//}

			return ErrorCode.Success;
		}

		public void Update( long elapsed, long dt )
		{
			//todo
			this.netSessionMgr.Update();
		}

		/// <summary>
		/// 获取指定位置号的GS信息(配置文件定义的序号)
		/// </summary>
		public CSGSInfo GetGSInfoByGSID( uint gsID )
		{
			gsID -= this.csKernelCfg.un32GSBaseIdx;
			return gsID < this.csKernelCfg.un32MaxGSNum ? this.gsInfoList[gsID] : null;
		}

		/// <summary>
		/// 获取指定逻辑ID的GS信息
		/// </summary>
		public CSGSInfo GetGSInfoByNSID( uint nsID )
		{
			for ( int i = 0; i < this.csKernelCfg.un32MaxGSNum; ++i )
			{
				CSGSInfo csgsInfo = this.gsNetInfoList[i].pcGSInfo;
				if ( null == csgsInfo )
					continue;
				if ( csgsInfo.m_n32NSID == nsID )
					return csgsInfo;
			}
			return null;
		}

		/// <summary>
		/// 获取指定位置号的SS信息(配置文件定义的序号)
		/// </summary>
		public CSSSInfo GetSSInfoBySSID( uint ssID )
		{
			ssID -= this.csKernelCfg.un32SSBaseIdx;
			return ssID < this.csKernelCfg.un32MaxGSNum ? this.ssInfoList[ssID] : null;
		}

		/// <summary>
		/// 获取指定逻辑ID的SS信息
		/// </summary>
		public CSSSInfo GetSSInfoByNSID( uint nsID )
		{
			for ( int i = 0; i < this.csKernelCfg.un32MaxSSNum; ++i )
			{
				CSSSInfo pcInfo = this.ssNetInfoList[i].pcSSInfo;
				if ( null == pcInfo )
					continue;
				if ( pcInfo.m_n32NSID == nsID )
					return pcInfo;
			}
			return null;
		}

		public ErrorCode InvokeGCMsg( CSGSInfo csgsInfo, int msgID, uint gcNetID, byte[] data, int offset, int size ) =>
			this.csUserMgr.Invoke( csgsInfo, msgID, gcNetID, data, offset, size );

		public ConnectionMultiplexer GetUserDBCacheRedisHandler() => this._userDBredisAsyncContext;

		public long AddTimer( HeartbeatCallback pHeartbeatCallback, long interval, bool ifPersist ) =>
			this.battleTimer.AddTimer( pHeartbeatCallback, interval, ifPersist );

		public void RemoveTimer( long timerID ) => this.battleTimer.RemoveTimer( timerID );

		public ErrorCode PostMsgToGS_KickoutGC( CSGSInfo csgsInfo, uint gcNetID )
		{
			CSToGS.OrderKickoutGC sMsg = new CSToGS.OrderKickoutGC();
			sMsg.Gcnid = ( int )gcNetID;
			return this.PostMsgToGS( csgsInfo, sMsg, ( int )CSToGS.MsgID.EMsgToGsfromCsOrderKickoutGc, 0 );
		}

		public ErrorCode PostMsgToGS( CSGSInfo csgsInfo, IMessage msg, int msgID, uint gcNetID )
		{
			if ( csgsInfo == null )
				return ErrorCode.NullMsg;
			this.netSessionMgr.TranMsgToSession( csgsInfo.m_n32NSID, msg, msgID, gcNetID == 0 ? 0 : ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc, gcNetID );
			return ErrorCode.Success;
		}

		public ErrorCode BroadCastMsgToGS( IMessage msg, int msgID )
		{
			this.netSessionMgr.TranMsgToSession( SessionType.ServerCsOnlyGS, msg, msgID, 0, 0, false );
			return ErrorCode.Success;
		}

		public ErrorCode PostMsgToGC( UserNetInfo userNetInfo, IMessage msg, int msgID )
		{
			if ( userNetInfo.gcNetID == 0 )
				return ErrorCode.InvalidUserNetInfo;
			this.PostMsgToGS( this.GetGSInfoByGSID( ( uint ) userNetInfo.gsID ), msg, msgID, userNetInfo.gcNetID );
			return ErrorCode.Success;
		}

		public ErrorCode PostMsgToLogServer( IMessage msg, int msgID )
		{
			this.netSessionMgr.SendMsgToSession( SessionType.ClientC2Log, msg, msgID );
			return ErrorCode.Success;
		}
	}
}