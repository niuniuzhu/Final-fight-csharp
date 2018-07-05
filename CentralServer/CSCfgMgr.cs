using CentralServer.Tools;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.Misc;

namespace CentralServer
{
	public class CSCfgMgr
	{
		public Dictionary<string, bool> aiRobotNameMapForCheck { get; } = new Dictionary<string, bool>();
		public Dictionary<uint, HeroBuyCfg> heroBuyCfgMap { get; } = new Dictionary<uint, HeroBuyCfg>();
		public Dictionary<uint, HeroBuyCfg> heroClientMatchMap { get; } = new Dictionary<uint, HeroBuyCfg>();
		public Dictionary<uint, RunesCfg> runesCfgMap { get; } = new Dictionary<uint, RunesCfg>();
		public Dictionary<uint, DiscountCfg> discountCfgMap { get; } = new Dictionary<uint, DiscountCfg>();
		public List<uint> hotGoodsCfgVec { get; } = new List<uint>();
		public List<uint> newGoodsCfgVec { get; } = new List<uint>();
		public List<string> invalidWorlds { get; } = new List<string>();
		public Dictionary<DBType, DBCfg> dbCfgMap { get; } = new Dictionary<DBType, DBCfg>();

		public UserDbSaveConfig userDbSaveCfg { get; } = new UserDbSaveConfig();

		public bool CheckAIRobotName( string nickname ) => this.aiRobotNameMapForCheck.ContainsKey( nickname );

		public ErrorCode Initalize()
		{
			ErrorCode errorCode = ErrorCode.Success;
			if ( ErrorCode.Success == errorCode ) errorCode = this.LoadFilterWordsCfg();
			if ( ErrorCode.Success == errorCode ) errorCode = this.LoadUserCfg();
			return errorCode;
		}

		public void Reset()
		{
			this.runesCfgMap.Clear();
			//todo
			//_loginRewardCfgMap.Clear();
			//_runesSlotCfgMap.Clear();
			//_invalidWorld.Clear();
			this.dbCfgMap.Clear();
			//_guideEndAward.Clear();
		}

		private ErrorCode LoadFilterWordsCfg()
		{
			ErrorCode errorCode = ErrorCode.Success;
			//todo
			return errorCode;
		}

		private ErrorCode LoadUserCfg()
		{
			ErrorCode errorCode = ErrorCode.Success;
			if ( ErrorCode.Success == errorCode ) errorCode = this.LoadDBCfg();
			return errorCode;
		}

		private ErrorCode LoadDBCfg()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\CSDBCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load CSDBCfg failed:{e}" );
				return ErrorCode.CfgFailed;
			}
			Hashtable[] dbs = json.GetMapArray( "DB" );
			foreach ( Hashtable db in dbs )
			{
				DBCfg cfg = new DBCfg();
				DBType dtype = ( DBType )db.GetInt( "type" );
				cfg.aszDBHostIP = db.GetString( "ip" );
				cfg.un32DBHostPort = db.GetInt( "port" );
				cfg.aszDBUserName = db.GetString( "user" );
				cfg.aszDBUserPwd = db.GetString( "pwd" );
				cfg.aszDBName = db.GetString( "dbname" );
				this.dbCfgMap[dtype] = cfg;
			}
			return ErrorCode.Success;
		}

		public DBCfg GetDBCfg( DBType dt )
		{
			this.dbCfgMap.TryGetValue( dt, out DBCfg cfg );
			return cfg;
		}

		public HeroBuyCfg GetHeroClientMatchCfg( uint HeroID )
		{
			this.heroClientMatchMap.TryGetValue( HeroID, out HeroBuyCfg heroBuyCfg );
			return heroBuyCfg;
		}

		public bool CheckInvalidWorld( string word )
		{
			int total = this.invalidWorlds.Count;
			for ( int i = 0; i < total; i++ )
			{
				int npos = word.IndexOf( this.invalidWorlds[i], StringComparison.Ordinal );
				if ( npos != -1 )
					return true;
			}
			return false;
		}
	}
}