using CentralServer.Tools;
using Core.Misc;
using ProtoBuf;
using Shared;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;

namespace CentralServer.User
{
	public partial class CSUser
	{
		private const string LOG_SIGN = "#";

		public UserDBData userDbData { get; private set; }
		public UserNetInfo userNetInfo { get; } = new UserNetInfo();
		public ulong guid => this.userDbData.usrDBData.un64ObjIdx;
		public string username => this.userDbData.szUserName;
		public string nickname => this.userDbData.szNickName;
		public ushort headID => this.userDbData.usrDBData.un16HeaderID;
		public long userLv => this.userDbData.usrDBData.un8UserLv;
		public long diamond => this.userDbData.usrDBData.n64Diamond;
		public long gold => this.userDbData.usrDBData.n64Gold;
		public long score => this.userDbData.usrDBData.n64Score;

		public long timerID { get; set; }
		//todo
		//public CCSUserBattleInfo userBattleInfoEx { get; private set; }
		public UserPlayingStatus userPlayingStatus { get; private set; }

		private readonly Dictionary<ulong, UserRelationshipInfo> m_cAddFVec = new Dictionary<ulong, UserRelationshipInfo>();
		private long _gcLastPing;
		private long _offlineTime;

		public void SetUserNetInfo( UserNetInfo crsUNI )
		{
			this.userNetInfo.Copy( crsUNI );
			this.userPlayingStatus = UserPlayingStatus.UserPlayingStatusPlaying;
		}

		public void ClearNetInfo()
		{
			this.userNetInfo.Clear();
			this.userPlayingStatus = UserPlayingStatus.UserPlayingStatusOffLine;
		}

		public void OnOnline( UserNetInfo netInfo, GCToCS.Login login, bool isFirstInDB, bool isFirstInMem, bool isReLogin = false )
		{
			this.KickOutOldUser();
			this.ResetPingTimer();
			this.LoginRewardInit();
			if ( !isReLogin )
				this.userDbData.ChangeUserDbData( UserDBDataType.Channel, login.Platform );
			CS.instance.csUserMgr.OnUserOnline( this, netInfo );
			this.NotifyUserPlayState();
			this.SynUserIsOnSS();
			this.SynUser_UserBaseInfo();
			this.SynCommidityCfgInfo();
			this.SynUserCLDays();
			this.SynUserAllHeroList();
			//todo
			//SynUser_AllRunesList();
			//CalculateItemAddition();
			//SynOtherItemInfo( 0 );
			//GetCSUserMgrInstance().PostUserCurtMailList( this );
			this.SynUserSNSList( 0, RelationShip.RsTypeFriends );
			this.SynUserSNSList( 0, RelationShip.RsTypeDetestation );
			//PosUserCSCurtGuideSteps();
			//GetTaskMgr().NotifyAllTask();
			if ( !isFirstInDB )
				this.PostCSNotice();

			//if ( isReLogin )
			//	CSSGameLogMgr::GetInstance().AddGameLog( eLog_UserRecon, this.GetUserDBData().usrDBData.un64ObjIdx, 0 );

			//if ( GetUserBattleInfoEx().GetBattleState() < eBattleState_Async )
			//{//由cs进行管理//
			//	if ( GetUserBattleInfoEx().GetBattleState() != eBattleState_Free )
			//	{
			//		Logger.Error( &"战斗类型(%u)战斗状态(%u)战斗ID(%u)房间(%u)队伍(%u)",
			//			  GetUserBattleInfoEx().GetBattleType(),
			//			  GetUserBattleInfoEx().GetBattleState(),
			//			  GetUserBattleInfoEx().GetBattleID(),
			//			  GetRoomPlayer().m_RoomID, GetMatchPlayer().m_MatchTeamId );
			//		Assert( false );
			//	}
			//}
			//else
			//{//由ss进行管理，只通知ss一下//
			//GetBattleMgrInstance().NotifyBattleSSUserIsOnline( this, true );
			//}
		}

		public void OnOffline()
		{
			//todo
			//CS.instance.csUserMgr.DBPoster_UpdateUser( this );
			//更新下线时间
			this._offlineTime = TimeUtils.utcTime + CS.instance.csCfg.userDbSaveCfg.delayDelFromCacheTime;
			CS.instance.csUserMgr.OnUserOffline( this );
			this.NotifyUserPlayState();

			//CSSGameLogMgr::GetInstance().AddGameLog( eLog_Logout, GetUserDBData().szUserName, 0 );

			//if ( GetUserBattleInfoEx().GetBattleState() < eBattleState_Async )
			//{//由cs进行管理//
			//	int ret = eNormal;
			//	switch ( GetUserBattleInfoEx().GetBattleType() )
			//	{
			//		case eBattleType_Room:
			//			ret = GetBattleMgrInstance().AskLeaveRoom( this );
			//			break;
			//		case eBattleType_Match:
			//			ret = GetBattleMgrInstance().RemoveMatchUser( this );
			//			break;
			//	}
			//	Assert( ret == eNormal );
			//}
			//else
			//{//由ss进行管理，只通知ss一下//
			//	GetBattleMgrInstance().NotifyBattleSSUserIsOnline( this, false );
			//}
		}

		private bool GetHeroVec( List<HeroListStruct> heroVec )
		{
			foreach ( KeyValuePair<uint, UserHeroDBData> kv in this.userDbData.heroListMap )
			{
				HeroListStruct sHeroListStruct = new HeroListStruct( kv.Key, kv.Value.endTime, false );
				if ( sHeroListStruct.expiredTime != -1 )
					sHeroListStruct.expiredTime = kv.Value.endTime + kv.Value.buyTime - TimeUtils.utcTime;
				heroVec.Add( sHeroListStruct );
			}
			CS.instance.csCfg.ForeachHeroBuyCfg( ( kv ) =>
			{
				HeroBuyCfg pCfg = kv.Value;
				if ( !pCfg.bIfShowInShop )
					return;

				foreach ( ConsumeStruct consumeStruct in pCfg.sConsumeList )
				{
					if ( consumeStruct.type == ConsumeType.ConsumeTypeFree && kv.Value.un32HeroID > 0 )
					{
						HeroListStruct sHeroListStruct = new HeroListStruct( kv.Value.un32HeroID, 0, true );
						heroVec.Add( sHeroListStruct );
					}
				}
			} );
			return true;
		}

		private ErrorCode KickOutOldUser()
		{
			if ( this.userNetInfo.IsValid() )
			{
				this.PostMsgToGC_NetClash();
				CSGSInfo piGSInfo = CS.instance.GetGSInfoByGSID( ( uint )this.userNetInfo.gsID );
				if ( null != piGSInfo )
				{
					CS.instance.PostMsgToGS_KickoutGC( piGSInfo, this.userNetInfo.gcNetID );
				}
				OnOffline();
			}
			return ErrorCode.Success;
		}

		private void LoginRewardInit()
		{
			DateTime today = CS.instance.csUserMgr.today;
			int curDays = today.DayOfYear;//当天是第几天(1900年1月1日)
			int baseDays = curDays - today.Day + 1;//月初是第几天(1900年1月1日)
			int lastDays = this.userDbData.usrDBData.un32LastGetLoginRewardDay;//上次是第几天(1900年1月1日)(数据库)
			if ( lastDays < baseDays )//当月未签到
			{
				this.userDbData.ChangeUserDbData( UserDBDataType.LastGetLoginReward, 0 );
				this.userDbData.ChangeUserDbData( UserDBDataType.CLDay, 0 );
			}
		}

		public bool CheckIfInFriendList( ulong guidIdx )
		{
			return this.userDbData.friendListMap.ContainsKey( guidIdx );
		}

		public bool CheckIfInBlacklist( ulong guidIdx )
		{
			return this.userDbData.blackListMap.ContainsKey( guidIdx );
		}

		public bool CheckIfInAddSNSList( ulong guidIdx )
		{
			return this.m_cAddFVec.ContainsKey( guidIdx );
		}

		public bool CheckIfEnoughPay( PayType type, int pay )
		{
			switch ( type )
			{
				case PayType.PayTypeGold:
					return pay <= this.gold;

				case PayType.PayTypeDiamond:
					return pay <= this.userDbData.usrDBData.n64Diamond;

				default:
					return false;
			}
		}

		public void CheckHeroValidTimer( long curTime )
		{
			//通知客户端 英雄过期
			GSToGC.NotifyGoodsExpired sMsg = new GSToGC.NotifyGoodsExpired();
			List<uint> expired = new List<uint>();
			foreach ( KeyValuePair<uint, UserHeroDBData> kv in this.userDbData.heroListMap )
			{
				UserHeroDBData pData = kv.Value;
				if ( pData.endTime == Consts.PERSIST_TIME_ALWAYS )
					continue;
				if ( pData.buyTime + pData.endTime < curTime )
				{
					sMsg.Commondityid.Add( ( int )pData.un32HeroID );
					expired.Add( kv.Key );
					//todo
					//CCSUserDbDataMgr::GetDelUserDbHerosSql( GetGUID(), pData.un32HeroID, expireSql );
				}
			}

			int count = expired.Count;
			for ( int i = 0; i < count; i++ )
				this.userDbData.heroListMap.Remove( expired[i] );

			this.PostMsgToGC( sMsg, ( int )GSToGC.MsgID.EMsgToGcfromBsNotifyGoodsExpired );

			//通知从数据库删除
			//todo
			//CS.instance.csUserMgr.PostUserCacheMsgToDBThread( GetGUID(), expireSql );
		}

		public void AskChangeHeaderId( uint headerID )
		{
			this.userDbData.ChangeUserDbData( UserDBDataType.HeaderId, headerID );
			PostMsgToGCNotifyNewHeaderid( this.guid, headerID );
			//notify new header to on-line friend
			foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.friendListMap )
			{
				CSUser user = CS.instance.csUserMgr.GetUser( kv.Value.guididx );
				if ( user != null && user.userPlayingStatus == UserPlayingStatus.UserPlayingStatusPlaying )
					user.SynUserSNSList( this.guid, RelationShip.RsTypeFriends );
			}
			//todo
			//string log = $"{headerID}{LOG_SIGN}{this.userDbData.szNickName}";
			//CSSGameLogMgr::GetInstance().AddGameLog( eLog_HeadUse, this.GetUserDBData().sPODUsrDBData.un64ObjIdx, mystream.str() );
		}

		public ErrorCode LoadDBData( UserDBData userDbData )
		{
			this.userDbData = userDbData.Clone();
			if ( 0 == this.userDbData.usrDBData.un8UserLv )
			{
				this.userDbData.ChangeUserDbData( UserDBDataType.UserLv, 1 );
				this.userDbData.ChangeUserDbData( UserDBDataType.VIPLevel, 1 );
			}

			//auto itemID = RefreshCardBegin + i;
			//auto cfg = CCSCfgMgr::getInstance().GetOtherItemCfg( itemID );
			//if ( !cfg )
			//{
			//	ELOG( LOG_ERROR, "洗练全配置为null" );
			//	continue;
			//}
			//UserItemInfo sSUserItemInfo;
			//sSUserItemInfo.item_id = itemID;
			//sSUserItemInfo.item_num = 5;
			//sSUserItemInfo.end_time = -1;
			//ELOG( LOG_DBBUG, "增加临时洗练" );
			//this._userDBData.item_Map[sSUserItemInfo.item_id] = sSUserItemInfo;

			//GetTaskMgr().UnpackTaskData( this._userDBData.szTaskData );//解析任务数据

			return ErrorCode.Success;
		}

		public void ResetPingTimer()
		{
			this._gcLastPing = TimeUtils.utcTime;
		}

		public void CheckDbSaveTimer( long curtime, long tickspan )
		{
		}

		private void PostMsgToGC_NetClash()
		{
			GSToGC.NetClash sMsg = new GSToGC.NetClash();
			this.PostMsgToGC( sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
		}

		public bool SaveToRedis()
		{
			ConnectionMultiplexer redis = CS.instance.GetUserDBCacheRedisHandler();
			if ( !redis.IsConnected )
				return false;

			//todo
			//GetTaskMgr().PackTaskData( m_sUserDBData.szTaskData, m_sUserDBData.isTaskRush );

			using ( MemoryStream ms = new MemoryStream() )
			{
				Serializer.Serialize( ms, this.userDbData );
				redis.GetDatabase().StringSetAsync( $"usercache:{this.userDbData.usrDBData.un64ObjIdx}", ms.ToArray(), null,
													When.Always, CommandFlags.FireAndForget );
			}

			return true;
		}
	}
}