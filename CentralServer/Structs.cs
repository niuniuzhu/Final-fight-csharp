using Core.Misc;
using Shared;

namespace CentralServer
{
	public class SCSKernelCfg
	{
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
		public string redisLogicAddress;
		public int redisLogicPort;
		public string LogAddress;
		public int LogPort;
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
		public EServerNetState m_eSSNetState;
		public int m_n32NSID;
		public uint m_un32ConnTimes;
		public long m_tLastConnMilsec;
		public long m_tLastPingMilSec;
		public int m_n32BattleNum;

		public EResult AddBattleNum( int n32AddNum )
		{
			this.m_n32BattleNum += n32AddNum;
			if ( 0 > this.m_n32BattleNum )
			{
				this.m_n32BattleNum = 0;
			}
			if ( this.m_n32BattleNum < Consts.MAX_BATTLE_IN_SS / 2 )
			{
				this.m_eSSNetState = EServerNetState.SnsFree;
			}
			else if ( this.m_n32BattleNum >= Consts.MAX_BATTLE_IN_SS / 2 )
			{
				this.m_eSSNetState = EServerNetState.SnsBusy;
			}
			else
			{
				this.m_eSSNetState = EServerNetState.SnsFull;
			}
			return EResult.Normal;
		}

		void ResetPing()
		{
			this.m_tLastPingMilSec = TimeUtils.utcTime;
		}
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
		EServerNetState m_eGSNetState;
		public int m_n32NSID;
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
		EServerNetState m_eNetState;
		public int m_n32NSID;
	}

	public class SSNetInfo
	{
		long tConnMilsec;
		CSSSInfo pcSSInfo;
	}

	public class RCNetInfo
	{
		long tConnMilsec;
		CSRCInfo cRCInfo;
	}

	public class GSNetInfo
	{
		long tConnMilsec;
		CSGSInfo pcGSInfo;
	}
}