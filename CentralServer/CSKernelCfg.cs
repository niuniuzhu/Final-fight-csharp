using System;
using System.Collections;
using System.IO;
using Core.Misc;
using Shared;

namespace CentralServer
{
	public class CSKernelCfg
	{
		public CSSSInfo[] ssInfoList { get; private set; }
		public CSGSInfo[] gsInfoList { get; private set; }

		public uint unCSId;
		public int n32MaxMsgSize;
		public int n32SSNetListenerPort;
		public int n32GSNetListenerPort;
		public int n32RCNetListenerPort;
		public int n32WorkingThreadNum;
		public uint un32MaxSSNum;
		public uint un32SSBaseIdx;
		public uint un32MaxGSNum;
		public uint un32GSBaseIdx;
		public int maxWaitingDBNum;
		public string ipaddress;
		public string redisAddress;
		public int redisPort;
		public string redisPwd;
		public string redisLogicAddress;
		public int redisLogicPort;
		public string redisLogicPwd;
		public string LogAddress;
		public int LogPort;
		public string remoteConsolekey;

		public ErrorCode Load()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\CSCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load CSCfg failed:{e}" );
				return ErrorCode.CfgFailed;
			}
			this.unCSId = json.GetUInt( "GameWorldID" );
			this.ipaddress = json.GetString( "CSIP" );
			this.n32SSNetListenerPort = json.GetInt( "SSPort" );
			this.n32GSNetListenerPort = json.GetInt( "GSPort" );
			this.un32MaxSSNum = json.GetUInt( "MaxSSNum" );
			this.un32SSBaseIdx = json.GetUInt( "SSBaseIndex" );
			this.un32MaxGSNum = json.GetUInt( "MaxGSNum" );
			this.un32GSBaseIdx = json.GetUInt( "GSBaseIndex" );
			this.maxWaitingDBNum = json.GetInt( "WaitingDBNum" );
			this.redisAddress = json.GetString( "redisAddress" );
			this.redisPort = json.GetInt( "redisPort" );
			this.redisPwd = json.GetString( "redisPwd" );
			this.redisLogicAddress = json.GetString( "redisLogicAddress" );
			this.redisLogicPort = json.GetInt( "redisLogicPort" );
			this.redisLogicPwd = json.GetString( "redisLogicPwd" );
			this.LogAddress = json.GetString( "LogAddress" );
			this.LogPort = json.GetInt( "LogPort" );

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

			this.n32RCNetListenerPort = json.GetInt( "RSPort" );
			this.remoteConsolekey = json.GetString( "RSKey" );

			return ErrorCode.Success;
		}
	}

	public class CSSSInfo
	{
		//property from config file.
		public int m_n32SSID;
		public string m_szName;
		public string m_szUserPwd;
		public string m_sListenIP;
		public int m_n32ListenPort;
		//property from local.
		public ServerNetState m_eSSNetState;
		public int m_n32NSID;
		public uint m_un32ConnTimes;
		public long m_tLastConnMilsec;
		public long m_tLastPingMilSec;
		public int m_n32BattleNum;

		public ErrorCode AddBattleNum( int n32AddNum )
		{
			this.m_n32BattleNum += n32AddNum;
			if ( 0 > this.m_n32BattleNum )
				this.m_n32BattleNum = 0;
			if ( this.m_n32BattleNum < Consts.MAX_BATTLE_IN_SS / 2 )
				this.m_eSSNetState = ServerNetState.Free;
			else if ( this.m_n32BattleNum >= Consts.MAX_BATTLE_IN_SS / 2 )
				this.m_eSSNetState = ServerNetState.Busy;
			else
				this.m_eSSNetState = ServerNetState.Full;
			return ErrorCode.Success;
		}

		void ResetPing() => this.m_tLastPingMilSec = TimeUtils.utcTime;
	}

	public class CSGSInfo
	{
		//property from config file.
		public int m_n32GSID;
		public string m_szName;
		public string m_szUserPwd;
		public string m_sListenIP;
		public int m_n32ListenPort;
		//property from local.
		public ServerNetState m_eGSNetState;
		public uint m_n32NSID;
		public uint m_un32ConnTimes;
		public long m_tLastConnMilsec;
		public long m_tLastPingMilSec;

		public long m_n64MsgReceived;
		public long m_n64MsgSent;
		public long m_n64DataReceived;
		public long m_n64DataSent;
	}

	public class CSRCInfo
	{
		//property from config file.
		public string m_sListenIP;
		public int m_n32ListenPort;
		//property from local.
		ServerNetState _mNetState;
		public int m_n32NSID;
	}

	public class SSNetInfo
	{
		public long tConnMilsec;
		public CSSSInfo pcSSInfo;
	}

	public class RCNetInfo
	{
		public long tConnMilsec;
		public CSRCInfo cRCInfo;
	}

	public class GSNetInfo
	{
		public long tConnMilsec;
		public CSGSInfo pcGSInfo;
	}
}