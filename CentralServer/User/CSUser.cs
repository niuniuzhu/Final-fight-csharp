using System;
using System.Collections.Generic;
using CentralServer.Tools;
using Core.Misc;
using Shared;

namespace CentralServer.User
{
	public partial class CSUser
	{
		public UserDBData userDbData { get; private set; }
		public UserNetInfo userNetInfo { get; } = new UserNetInfo();
		public ulong guid => this.userDbData.sPODUsrDBData.un64ObjIdx;
		public string username => this.userDbData.szUserName;
		public string nickname => this.userDbData.szNickName;
		public ushort headID => this.userDbData.sPODUsrDBData.un16HeaderID;
		public long timerID { get; set; }
		//todo
		//public CCSUserBattleInfo userBattleInfoEx { get; private set; }

		private UserPlayingStatus _userPlayingStatus;
		private readonly Dictionary<ulong, UserRelationshipInfo> m_cAddFVec = new Dictionary<ulong, UserRelationshipInfo>();
		private long _gcLastPing;
		private long _offlineTime;

		public void SetUserNetInfo( UserNetInfo crsUNI )
		{
			this.userNetInfo.Copy( crsUNI );
			this._userPlayingStatus = UserPlayingStatus.UserPlayingStatusPlaying;
		}

		public void ClearNetInfo()
		{
			this.userNetInfo.Clear();
			this._userPlayingStatus = UserPlayingStatus.UserPlayingStatusOffLine;
		}

		public void OnOnline( UserNetInfo netinfo, GCToCS.Login login, bool isFirstInDB, bool isFirstInMem, bool isReLogin = false )
		{
			this.KickOutOldUser();
			this.ResetPingTimer();
			this.LoginRewardInit();
			if ( !isReLogin )
				this.userDbData.ChangeUserDbData( UserDBDataType.UserDBType_Channel, login.Platform );
			CS.instance.csUserMgr.OnUserOnline( this, netinfo );
			this.NotifyUserPlayState();
			this.SynUser_IsOnSS();
			this.SynUser_UserBaseInfo();
			this.SynCommidityCfgInfo();
			this.SynUserCLDays();
			this.SynUser_AllHeroList();
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
			//	CSSGameLogMgr::GetInstance().AddGameLog( eLog_UserRecon, this.GetUserDBData().sPODUsrDBData.un64ObjIdx, 0 );

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

		private void OnOffline()
		{
			//todo
			//CS.instance.csUserMgr.DBPoster_UpdateUser( this );
			//更新下线时间
			this._offlineTime =TimeUtils.utcTime + CS.instance.csCfg.userDbSaveCfg.delayDelFromCacheTime;
			CS.instance.csUserMgr.OnUserOffline( this );
			this.NotifyUserPlayState();

			//CSSGameLogMgr::GetInstance().AddGameLog( eLog_Logout, GetUserDBData().szUserName, 0 );

			//if ( GetUserBattleInfoEx().GetBattleState() < eBattleState_Async )
			//{//由cs进行管理//
			//	INT32 ret = eNormal;
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
				CSGSInfo piGSInfo = CS.instance.GetGSInfoByGSID( this.userNetInfo.gsID );
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
			int lastDays = this.userDbData.sPODUsrDBData.un32LastGetLoginRewardDay;//上次是第几天(1900年1月1日)(数据库)
			if ( lastDays < baseDays )//当月未签到
			{
				this.userDbData.ChangeUserDbData( UserDBDataType.UserDBType_LastGetLoginReward, 0 );
				this.userDbData.ChangeUserDbData( UserDBDataType.UserDBType_CLDay, 0 );
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

		public ErrorCode LoadDBData( UserDBData userDbData )
		{
			this.userDbData = userDbData.Clone();
			if ( 0 == this.userDbData.sPODUsrDBData.un8UserLv )
			{
				this.userDbData.ChangeUserDbData( UserDBDataType.UserDBType_UserLv, 1 );
				this.userDbData.ChangeUserDbData( UserDBDataType.UserDBType_VIPLevel, 1 );
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

		public void InitRunes( string bagStr, string slotStr )
		{
		}

		public void LoadFromRedisStr( string heroStr, string friendStr, string blackStr, string itemStr, string mailStr )
		{
		}

		private void PostMsgToGC_NetClash()
		{
			GSToGC.NetClash sMsg = new GSToGC.NetClash();
			this.PostMsgToGC( sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
		}
	}
}