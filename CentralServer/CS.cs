using CentralServer.Net;
using Core.Misc;
using Core.Net;
using Shared;
using Shared.Net;
using System;
using System.Collections;
using System.IO;

namespace CentralServer
{

	public class CS
	{
		private static CS _instance;
		public static CS instance => _instance ?? ( _instance = new CS() );

		public SCSKernelCfg m_sCSKernelCfg;
		public CSSSInfo[] m_pcSSInfoList;
		public CSGSInfo[] m_pcGSInfoList;
		public string m_szRemoteConsolekey;

		public CSNetSessionMgr netSessionMgr { get; }

		public SSNetInfo[] m_psSSNetInfoList { get; private set; }//场景列表信息
		public GSNetInfo[] m_psGSNetInfoList { get; private set; }//网关列表信息
		public RCNetInfo[] m_psRCNetInfoList { get; private set; }//场景列表信息

		public CS()
		{
			this.m_sCSKernelCfg = new SCSKernelCfg();
			this.netSessionMgr = new CSNetSessionMgr();
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

			this.m_psSSNetInfoList = new SSNetInfo[this.m_sCSKernelCfg.un32MaxSSNum];
			this.m_psGSNetInfoList = new GSNetInfo[this.m_sCSKernelCfg.un32MaxGSNum];
			this.m_psRCNetInfoList = new RCNetInfo[10];

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
			this.m_sCSKernelCfg.unCSId = json.GetUInt( "GameWorldID" );
			this.m_sCSKernelCfg.ipaddress = json.GetString( "CSIP" );
			this.m_sCSKernelCfg.n32SSNetListenerPort = json.GetInt( "SSPort" );
			this.m_sCSKernelCfg.n32GSNetListenerPort = json.GetInt( "GSPort" );
			this.m_sCSKernelCfg.un32MaxSSNum = json.GetUInt( "MaxSSNum" );
			this.m_sCSKernelCfg.un32SSBaseIdx = json.GetUInt( "SSBaseIndex" );
			this.m_sCSKernelCfg.un32MaxGSNum = json.GetUInt( "MaxGSNum" );
			this.m_sCSKernelCfg.un32GSBaseIdx = json.GetUInt( "GSBaseIndex" );
			this.m_sCSKernelCfg.maxWaitingDBNum = json.GetInt( "WaitingDBNum" );
			this.m_sCSKernelCfg.redisAddress = json.GetString( "redisAddress" );
			this.m_sCSKernelCfg.redisPort = json.GetInt( "redisPort" );
			this.m_sCSKernelCfg.redisLogicAddress = json.GetString( "redisLogicAddress" );
			this.m_sCSKernelCfg.redisLogicPort = json.GetInt( "redisLogicPort" );
			this.m_sCSKernelCfg.LogAddress = json.GetString( "LogAddress" );
			this.m_sCSKernelCfg.LogPort = json.GetInt( "LogPort" );

			string ssIndexStr = json.GetString( "AllSSIndex" );
			string[] ssIndexVec = ssIndexStr.Split( ';' );

			if ( ssIndexVec.Length > 100000 )
			{
				Logger.Warn( $"too many ss!" );
				return ErrorCode.CfgFailed;
			}

			this.m_pcSSInfoList = new CSSSInfo[ssIndexVec.Length];
			for ( int i = 0; i != ssIndexVec.Length; ++i )
			{
				string[] ssInfoVec = ssIndexVec[i].Split( ',' );
				if ( ssInfoVec.Length != 3 )
				{
					Logger.Error( "load CSCfg.xml failed." );
					continue;
				}

				this.m_pcSSInfoList[i].m_n32SSID = int.Parse( ssInfoVec[0] );
				this.m_pcSSInfoList[i].m_szName = ssInfoVec[1];
				this.m_pcSSInfoList[i].m_szUserPwd = ssInfoVec[2];
			}

			string gsIndexStr = json.GetString( "AllGSIndex" );
			string[] gsIndexVec = gsIndexStr.Split( ';' );
			this.m_pcGSInfoList = new CSGSInfo[gsIndexVec.Length];

			for ( int i = 0; i != gsIndexVec.Length; ++i )
			{
				string[] gsInfoVec = gsIndexVec[i].Split( ',' );
				if ( gsInfoVec.Length != 3 )
				{
					Logger.Error( "load CSCfg.xml failed." );
					continue;
				}

				this.m_pcGSInfoList[i].m_n32GSID = int.Parse( gsInfoVec[0] );
				this.m_pcGSInfoList[i].m_szName = gsInfoVec[1];
				this.m_pcGSInfoList[i].m_szUserPwd = gsInfoVec[2];
			}

			this.m_sCSKernelCfg.n32RCNetListenerPort = json.GetInt( "RSPort" );
			this.m_szRemoteConsolekey = json.GetString( "RSKey" );

			return ErrorCode.Success;
		}
		#endregion

		public ErrorCode Start()
		{
			this.netSessionMgr.CreateListener( this.m_sCSKernelCfg.n32GSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateGateSession );
			this.netSessionMgr.CreateListener( this.m_sCSKernelCfg.n32SSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateSceneSession );
			this.netSessionMgr.CreateListener( this.m_sCSKernelCfg.n32RCNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateRemoteConsoleSession );
			this.netSessionMgr.CreateConnector( SessionType.ClientG2B, this.m_sCSKernelCfg.LogAddress,
												this.m_sCSKernelCfg.LogPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 102400,
												0 );

			//连接redis 6379
			if ( this.m_sCSKernelCfg.redisAddress != "0" )
			{
				this.netSessionMgr.CreateConnector( SessionType.ClientC2R, this.m_sCSKernelCfg.redisAddress,
													this.m_sCSKernelCfg.redisPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 102400,
													0 );
			}

			//连接redis 6380，也是redis？
			if ( this.m_sCSKernelCfg.redisLogicAddress != "0" )
			{
				this.netSessionMgr.CreateConnector( SessionType.ClientC2LogicRedis, this.m_sCSKernelCfg.redisLogicAddress,
													this.m_sCSKernelCfg.redisLogicPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE,
													102400, 0 );
			}

			return ErrorCode.Success;
		}

		public void Update( long elapsed, long dt )
		{
			//todo
			this.netSessionMgr.Update();
		}

		public CSGSInfo GetCGSInfoByNSID( uint nsID )
		{
			for ( int i = 0; i < this.m_sCSKernelCfg.un32MaxGSNum; ++i )
			{
				CSGSInfo pcInfo = this.m_psGSNetInfoList[i].pcGSInfo;
				if ( null == pcInfo ) continue;
				if ( pcInfo.m_n32NSID == nsID )
					return pcInfo;
			}
			return null;
		}
	}
}