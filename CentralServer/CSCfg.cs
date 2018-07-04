using Core.Misc;
using Shared;
using System;
using System.Collections.Generic;
using CentralServer.Tools;

namespace CentralServer
{
	public class CSKernelCfg
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
		public string redisPwd;
		public string redisLogicAddress;
		public int redisLogicPort;
		public string redisLogicPwd;
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
				this.m_eSSNetState = ServerNetState.SnsFree;
			else if ( this.m_n32BattleNum >= Consts.MAX_BATTLE_IN_SS / 2 )
				this.m_eSSNetState = ServerNetState.SnsBusy;
			else
				this.m_eSSNetState = ServerNetState.SnsFull;
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

	public class CSCfgMgr
	{
		private readonly Dictionary<string, bool> _aiRobotNameMapForCheck = new Dictionary<string, bool>();
		private readonly Dictionary<uint, HeroBuyCfg> _heroBuyCfgMap = new Dictionary<uint, HeroBuyCfg>();
		private readonly Dictionary<uint, HeroBuyCfg> _heroClientMatchMap = new Dictionary<uint, HeroBuyCfg>();
		private readonly Dictionary<uint, RunesCfg> _runesCfgMap = new Dictionary<uint, RunesCfg>();
		private readonly Dictionary<uint, DiscountCfg> _discountCfgMap = new Dictionary<uint, DiscountCfg>();
		private readonly List<uint> _hotGoodsCfgVec = new List<uint>();
		private readonly List<uint> _newGoodsCfgVec = new List<uint>();
		private readonly List<string> _invalidWorlds = new List<string>();

		public UserDbSaveConfig userDbSaveCfg { get; } = new UserDbSaveConfig();

		public bool CheckAIRobotName( string nickname ) => this._aiRobotNameMapForCheck.ContainsKey( nickname );

		public void ForeachHeroBuyCfg( Action<KeyValuePair<uint, HeroBuyCfg>> handler )
		{
			foreach ( KeyValuePair<uint, HeroBuyCfg> kv in this._heroBuyCfgMap )
				handler.Invoke( kv );
		}

		public HeroBuyCfg GetHeroClientMatchCfg( uint HeroID )
		{
			this._heroClientMatchMap.TryGetValue( HeroID, out HeroBuyCfg heroBuyCfg );
			return heroBuyCfg;
		}

		public void ForeachRunesCfg( Action<KeyValuePair<uint, RunesCfg>> handler )
		{
			foreach ( KeyValuePair<uint, RunesCfg> kv in this._runesCfgMap )
				handler.Invoke( kv );
		}

		public void ForeachDiscountCfg( Action<KeyValuePair<uint, DiscountCfg>> handler )
		{
			foreach ( KeyValuePair<uint, DiscountCfg> kv in this._discountCfgMap )
				handler.Invoke( kv );
		}

		public void ForeachHotGoodsCfg( Action<uint> handler )
		{
			foreach ( uint hotGoodsCfg in this._hotGoodsCfgVec )
				handler.Invoke( hotGoodsCfg );
		}

		public void ForeachNewGoodsCfg( Action<uint> handler )
		{
			foreach ( uint newGoodsCfg in this._newGoodsCfgVec )
				handler.Invoke( newGoodsCfg );
		}

		public bool CheckInvalidWorld( string word )
		{
			int total = this._invalidWorlds.Count;
			for ( int i = 0; i < total; i++ )
			{
				int npos = word.IndexOf( this._invalidWorlds[i], StringComparison.Ordinal );
				if ( npos != -1 )
					return true;
			}
			return false;
		}
	}
}