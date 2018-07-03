using CentralServer.Tools;
using Core.Misc;
using Google.Protobuf;
using Shared;
using System;
using System.Collections.Generic;

namespace CentralServer.User
{
	public partial class CSUser
	{
		public ErrorCode SynUser_UserBaseInfo()
		{
			GSToGC.UserBaseInfo sUserBaseInfo = new GSToGC.UserBaseInfo
			{
				Nickname = this.userDbData.szNickName,
				Name = this.userDbData.szUserName,
				Sex = this.userDbData.sPODUsrDBData.n16Sex,
				Curscore = this.userDbData.sPODUsrDBData.n64Score,
				Curgold = this.userDbData.sPODUsrDBData.n64Gold,
				Curdiamoand = this.userDbData.sPODUsrDBData.n64Diamond,
				Guid = 29001,
				//todo
				//Mapid = GetBattleMgrInstance().GetBattleMapID( m_sUserBattleInfoEx.GetBattleID() ),
				//Battleid = m_sUserBattleInfoEx.GetBattleID(),
				Mapid =  0,
				Battleid =  0,
				Ifreconnect = false,
				Level = this.userDbData.sPODUsrDBData.un8UserLv,
				Headid = this.userDbData.sPODUsrDBData.un16HeaderID,
				VipLevel = this.userDbData.sPODUsrDBData.un16VipLv,
				VipScore = this.userDbData.sPODUsrDBData.vipScore,
				CurExp = ( int )this.userDbData.sPODUsrDBData.un32UserCurLvExp
			};
			return this.PostMsgToGC( sUserBaseInfo, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserBaseInfo );
		}

		private ErrorCode PostMsgToGC( IMessage sMsg, int msgID )
		{
			CSGSInfo csgsInfo = CS.instance.GetGSInfoByGSID( this.userNetInfo.gsID );
			if ( null == csgsInfo )
			{
				Logger.Warn( $"GS({this.userNetInfo.gsID}) not found." );
				return ErrorCode.NullGateServer;
			}

			CS.instance.PostMsgToGS( csgsInfo, sMsg, msgID, this.userNetInfo.gcNetID );
			return ErrorCode.Success;
		}

		public void PostCSNotice()
		{
			GSToGC.GameNotice pMsg = new GSToGC.GameNotice();
			CS.instance.csUserMgr.ForeachNotice( tempNotice =>
			{
				if ( tempNotice.msg.Length < 1 )
					return;

				//平台判断
				if ( tempNotice.platform != UserPlatform.Platform_All )
					if ( tempNotice.platform != this.userDbData.sPODUsrDBData.userPlatform )
						return;

				//过期判断
				long temp_date = TimeUtils.utcTime;
				long temp_end = tempNotice.end_time - temp_date;
				if ( temp_end < 0 )
					return;

				//是否到发送时间
				long temp_star = tempNotice.star_time - temp_date;
				if ( temp_star > 0 )
					return;

				GSToGC.GameNotice.Types.Notice notice = new GSToGC.GameNotice.Types.Notice
				{
					Title = tempNotice.title,
					Flag = ( uint )tempNotice.flag,
					Status = ( uint )tempNotice.state,
					Priority = tempNotice.priority,
					Notice_ = tempNotice.msg
				};
				pMsg.Notice.Add( notice );
			} );
			this.PostMsgToGC( pMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNotice );
		}

		private ErrorCode NotifyUserPlayState()
		{
			foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.friendListMap )
			{
				CSUser piUser = CS.instance.csUserMgr.GetUser( kv.Value.guididx );
				if ( null == piUser )
					continue;
				if ( !piUser.CheckIfInFriendList( this.guid ) )
					continue;
				if ( UserPlayingStatus.UserPlayingStatusPlaying == piUser._userPlayingStatus )
					piUser.SynUserSNSList( this.guid, kv.Value.relationShip );
			}
			return ErrorCode.Success;
		}

		private ErrorCode SynUser_IsOnSS()
		{
			//todo
			//GSToGC.NotifyIsOnSS msg = new GSToGC.NotifyIsOnSS();
			//CSBattle pBattle = GetBattleMgrInstance().GetBattle( GetUserBattleInfoEx().GetBattleID() );
			//msg.set_ssid( pBattle == null ? 0 : pBattle.GetSSID() );
			//this.PostMsgToGC( msg, msg.msgid() );
			return ErrorCode.Success;
		}

		private ErrorCode SynCommidityCfgInfo()
		{
			GSToGC.GoodsBuyCfgInfo sMsg = new GSToGC.GoodsBuyCfgInfo();
			CS.instance.csCfg.ForeachHeroBuyCfg( kv =>
			{
				HeroBuyCfg pCfg = kv.Value;
				if ( !pCfg.bIfShowInShop )
					return;

				GSToGC.GoodsCfgInfo sGoodsCfgInfo = new GSToGC.GoodsCfgInfo { Goodid = ( int )kv.Key };
				foreach ( ConsumeStruct consumeStruct in pCfg.sConsumeList )
				{
					GSToGC.GoodsCfgInfo.Types.Consume pConsume =
						new GSToGC.GoodsCfgInfo.Types.Consume
						{
							Consumetype = ( int )consumeStruct.type,
							Price = consumeStruct.price
						};
					sGoodsCfgInfo.Consume.Add( pConsume );
				}

				sGoodsCfgInfo.CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.Common;
				sMsg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.csCfg.ForeachRunesCfg( kv =>
			{
				SRunesCfg pCfg = kv.Value;
				if ( !pCfg.bIfShowInShop )
					return;
				GSToGC.GoodsCfgInfo sGoodsCfgInfo = new GSToGC.GoodsCfgInfo { Goodid = ( int )kv.Key };
				foreach ( ConsumeStruct consumeStruct in pCfg.sConsumeList )
				{
					GSToGC.GoodsCfgInfo.Types.Consume pConsume =
						new GSToGC.GoodsCfgInfo.Types.Consume
						{
							Consumetype = ( int )consumeStruct.type,
							Price = consumeStruct.price
						};
					sGoodsCfgInfo.Consume.Add( pConsume );
				}

				sGoodsCfgInfo.CfgType = ( GSToGC.GoodsCfgInfo.Types.CfgType.Common );
				sMsg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.csCfg.ForeachDiscountCfg( kv =>
			{
				SDiscountCfg pCfg = kv.Value;
				GSToGC.GoodsCfgInfo sGoodsCfgInfo = new GSToGC.GoodsCfgInfo { Goodid = ( int )kv.Key };
				foreach ( ConsumeStruct consumeStruct in pCfg.sConsumeList )
				{
					GSToGC.GoodsCfgInfo.Types.Consume pConsume =
						new GSToGC.GoodsCfgInfo.Types.Consume
						{
							Consumetype = ( int )consumeStruct.type,
							Price = consumeStruct.price
						};
					sGoodsCfgInfo.Consume.Add( pConsume );
				}

				sGoodsCfgInfo.CfgType = ( GSToGC.GoodsCfgInfo.Types.CfgType.Discount );
				sMsg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.csCfg.ForeachNewGoodsCfg( newGoodsCfg =>
			{
				GSToGC.GoodsCfgInfo sGoodsCfgInfo = new GSToGC.GoodsCfgInfo
				{
					Goodid = ( int )newGoodsCfg,
					CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.New
				};
				sMsg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.csCfg.ForeachHotGoodsCfg( hotGoodsCfg =>
			{
				GSToGC.GoodsCfgInfo sGoodsCfgInfo = new GSToGC.GoodsCfgInfo
				{
					Goodid = ( int )hotGoodsCfg,
					CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.Hot
				};
				sMsg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.PostMsgToGC( this.userNetInfo, sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyGoodsCfgInfo );
			return ErrorCode.Success;
		}

		private ErrorCode SynUserCLDays()
		{
			GSToGC.NotifyUserCLDays sMsg = new GSToGC.NotifyUserCLDays();
			DateTime today = CS.instance.csUserMgr.today;
			sMsg.Month = ( uint )today.Month;//当前月
			sMsg.Today = ( uint )today.Day;//当前日
			sMsg.TotalCldays = ( uint )DateTime.DaysInMonth( today.Year, today.Month );//月总天数
			sMsg.Cldays = this.userDbData.sPODUsrDBData.un16Cldays;//已领天数
			sMsg.IsTodayCan = this.userDbData.sPODUsrDBData.un32LastGetLoginRewardDay != today.DayOfYear;//今天是否可以领取
			return this.PostMsgToGC( sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserCldays );
		}

		private ErrorCode SynUser_AllHeroList()
		{
			GSToGC.NotifyCSHeroList sMsg = new GSToGC.NotifyCSHeroList();
			List<HeroListStruct> heroVec = new List<HeroListStruct>();
			this.GetHeroVec( heroVec );
			foreach ( HeroListStruct sHeroListStruct in heroVec )
			{
				GSToGC.NotifyCSHeroList.Types.HeroListCfg pCfg = new GSToGC.NotifyCSHeroList.Types.HeroListCfg();
				HeroBuyCfg t_cfg = CS.instance.csCfg.GetHeroClientMatchCfg( sHeroListStruct.heroid );
				if ( t_cfg == null )
					continue;

				if ( sHeroListStruct.expiredTime <= 0 && sHeroListStruct.expiredTime != -1 && sHeroListStruct.ifFree != true )
					continue;

				pCfg.Heroid = t_cfg.un32CommondityID;
				pCfg.ExpiredTime = sHeroListStruct.expiredTime;
				pCfg.IfFree = sHeroListStruct.ifFree;
				sMsg.Herocfg.Add( pCfg );
			}
			return this.PostMsgToGC( sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyHeroList );
		}

		private ErrorCode SynUserSNSList( ulong guid_friends, RelationShip eRSType )
		{
			GSToGC.NotifyUserSNSList sNotifyUserSNSList = new GSToGC.NotifyUserSNSList();
			if ( guid_friends != 0 )
			{
				CSUser piUser = CS.instance.csUserMgr.GetUser( guid_friends );
				if ( null == piUser )
				{
					return ErrorCode.UserNotExist;
				}

				GSToGC.SNSInfo pInfo = new GSToGC.SNSInfo
				{
					Type = ( int )eRSType,
					Nickname = piUser.nickname,
					Headid = piUser.headID,
					Status = ( int )piUser._userPlayingStatus,
					Guididx = ( guid_friends ),
					Viplv = ( uint )piUser.userDbData.sPODUsrDBData.un16VipLv
				};
				sNotifyUserSNSList.Info.Add( pInfo );
			}
			else
			{
				if ( RelationShip.RsTypeFriends == eRSType )
				{
					foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.friendListMap )
					{
						GSToGC.SNSInfo pInfo = new GSToGC.SNSInfo();
						pInfo.Type = ( int )eRSType;
						pInfo.Guididx = kv.Value.guididx;
						CSUser pTempUser = CS.instance.csUserMgr.GetUser( kv.Value.guididx );
						if ( null != pTempUser )
						{
							pInfo.Headid = ( pTempUser.headID );
							pInfo.Nickname = ( pTempUser.nickname );
							pInfo.Status = ( int )( pTempUser._userPlayingStatus );
							pInfo.Viplv = ( uint )( pTempUser.userDbData.sPODUsrDBData.un16VipLv );
						}
						else
						{
							pInfo.Headid = ( kv.Value.nHeadId );
							pInfo.Nickname = ( kv.Value.stNickName );
							pInfo.Status = ( int )( UserPlayingStatus.UserPlayingStatusOffLine );
							pInfo.Viplv = ( kv.Value.viplv );
						}
						sNotifyUserSNSList.Info.Add( pInfo );
					}
				}
				else
				{
					foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.blackListMap )
					{
						GSToGC.SNSInfo pInfo = new GSToGC.SNSInfo();
						pInfo.Type = ( int )( eRSType );
						pInfo.Headid = ( kv.Value.nHeadId );
						pInfo.Guididx = ( kv.Value.guididx );
						CSUser pTempUser = CS.instance.csUserMgr.GetUser( kv.Value.guididx );
						pInfo.Nickname = null != pTempUser ? pTempUser.nickname : kv.Value.stNickName;
						sNotifyUserSNSList.Info.Add( pInfo );
					}
				}
			}
			this.PostMsgToGC( sNotifyUserSNSList, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserSnslist );
			return ErrorCode.Success;
		}
	}
}