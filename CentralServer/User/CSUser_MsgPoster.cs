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
				Sex = this.userDbData.usrDBData.n16Sex,
				Curscore = this.userDbData.usrDBData.n64Score,
				Curgold = this.userDbData.usrDBData.n64Gold,
				Curdiamoand = this.userDbData.usrDBData.n64Diamond,
				Guid = this.userDbData.usrDBData.un64ObjIdx,
				//todo
				//Mapid = GetBattleMgrInstance().GetBattleMapID( m_sUserBattleInfoEx.GetBattleID() ),
				//Battleid = m_sUserBattleInfoEx.GetBattleID(),
				Mapid = 0,
				Battleid = 0,
				Ifreconnect = false,
				Level = this.userDbData.usrDBData.un8UserLv,
				Headid = this.userDbData.usrDBData.un16HeaderID,
				VipLevel = this.userDbData.usrDBData.un16VipLv,
				VipScore = this.userDbData.usrDBData.vipScore,
				CurExp = ( int )this.userDbData.usrDBData.un32UserCurLvExp
			};
			return this.PostMsgToGC( sUserBaseInfo, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserBaseInfo );
		}

		private ErrorCode PostMsgToGC( IMessage msg, int msgID )
		{
			CSGSInfo csgsInfo = CS.instance.GetGSInfoByGSID( ( uint )this.userNetInfo.gsID );
			if ( null == csgsInfo )
			{
				Logger.Warn( $"GS({this.userNetInfo.gsID}) not found." );
				return ErrorCode.NullGateServer;
			}

			CS.instance.PostMsgToGS( csgsInfo, msg, msgID, this.userNetInfo.gcNetID );
			return ErrorCode.Success;
		}

		public ErrorCode PostMsgToGCAskRetMsg( int n32AskProtocalID, ErrorCode errorCode )
		{
			return CS.instance.userMgr.PostMsgToGCAskReturn( this.userNetInfo, n32AskProtocalID, errorCode );
		}

		public ErrorCode PostMsgToGCNotifyNewNickname( ulong guid, string nickname )
		{
			GSToGC.NotifyNewNickname msg = new GSToGC.NotifyNewNickname
			{
				Guid = guid,
				Newnickname = nickname
			};
			return this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNewNickname );
		}

		private ErrorCode PostMsgToGCNotifyNewHeaderid( ulong guid, uint headerID )
		{
			GSToGC.NotifyNewHeaderid msg = new GSToGC.NotifyNewHeaderid
			{
				Guid = guid,
				Newheaderid = headerID
			};
			return this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNewHeaderid );
		}

		public void PostCSNotice()
		{
			GSToGC.GameNotice msg = new GSToGC.GameNotice();
			foreach ( Notice tempNotice in CS.instance.userMgr.notices )
			{
				if ( tempNotice.msg.Length < 1 )
					return;

				//平台判断
				if ( tempNotice.platform != UserPlatform.All )
					if ( tempNotice.platform != this.userDbData.usrDBData.userPlatform )
						return;

				//过期判断
				long tempDate = TimeUtils.utcTime;
				long tempEnd = tempNotice.end_time - tempDate;
				if ( tempEnd < 0 )
					return;

				//是否到发送时间
				long tempStar = tempNotice.star_time - tempDate;
				if ( tempStar > 0 )
					return;

				GSToGC.GameNotice.Types.Notice notice = new GSToGC.GameNotice.Types.Notice
				{
					Title = tempNotice.title,
					Flag = ( uint )tempNotice.flag,
					Status = ( uint )tempNotice.state,
					Priority = tempNotice.priority,
					Notice_ = tempNotice.msg
				};
				msg.Notice.Add( notice );
			}
			this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNotice );
		}

		private ErrorCode NotifyUserPlayState()
		{
			foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.friendListMap )
			{
				CSUser user = CS.instance.userMgr.GetUser( kv.Value.guididx );
				if ( null == user )
					continue;
				if ( !user.CheckIfInFriendList( this.guid ) )
					continue;
				if ( UserPlayingStatus.Playing == user.userPlayingStatus )
					user.SynUserSNSList( this.guid, kv.Value.relationShip );
			}
			return ErrorCode.Success;
		}

		private ErrorCode SynUserIsOnSS()
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
			GSToGC.GoodsBuyCfgInfo msg = new GSToGC.GoodsBuyCfgInfo();
			foreach ( KeyValuePair<uint, HeroBuyCfg> kv in CS.instance.csCfg.heroBuyCfgMap )
			{
				HeroBuyCfg pCfg = kv.Value;
				if ( !pCfg.bIfShowInShop )
					continue;
				GSToGC.GoodsCfgInfo goodsCfgInfo = new GSToGC.GoodsCfgInfo { Goodid = ( int )kv.Key };
				foreach ( ConsumeStruct consumeStruct in pCfg.consumeList )
				{
					GSToGC.GoodsCfgInfo.Types.Consume pConsume =
						new GSToGC.GoodsCfgInfo.Types.Consume
						{
							Consumetype = ( int )consumeStruct.type,
							Price = consumeStruct.price
						};
					goodsCfgInfo.Consume.Add( pConsume );
				}

				goodsCfgInfo.CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.Common;
				msg.Info.Add( goodsCfgInfo );
			}

			foreach ( KeyValuePair<uint, RunesCfg> kv in CS.instance.csCfg.runesCfgMap )
			{
				RunesCfg pCfg = kv.Value;
				if ( !pCfg.bIfShowInShop )
					continue;
				GSToGC.GoodsCfgInfo goodsCfgInfo = new GSToGC.GoodsCfgInfo { Goodid = ( int )kv.Key };
				foreach ( ConsumeStruct consumeStruct in pCfg.sConsumeList )
				{
					GSToGC.GoodsCfgInfo.Types.Consume pConsume =
						new GSToGC.GoodsCfgInfo.Types.Consume
						{
							Consumetype = ( int )consumeStruct.type,
							Price = consumeStruct.price
						};
					goodsCfgInfo.Consume.Add( pConsume );
				}

				goodsCfgInfo.CfgType = ( GSToGC.GoodsCfgInfo.Types.CfgType.Common );
				msg.Info.Add( goodsCfgInfo );
			}

			foreach ( KeyValuePair<uint, DiscountCfg> kv in CS.instance.csCfg.discountCfgMap )
			{
				DiscountCfg pCfg = kv.Value;
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
				msg.Info.Add( sGoodsCfgInfo );
			}

			foreach ( uint newGoodsCfg in CS.instance.csCfg.newGoodsCfgVec )
			{
				GSToGC.GoodsCfgInfo goodsCfgInfo = new GSToGC.GoodsCfgInfo
				{
					Goodid = ( int )newGoodsCfg,
					CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.New
				};
				msg.Info.Add( goodsCfgInfo );
			}

			foreach ( uint hotGoodsCfg in CS.instance.csCfg.hotGoodsCfgVec )
			{

				GSToGC.GoodsCfgInfo goodsCfgInfo = new GSToGC.GoodsCfgInfo
				{
					Goodid = ( int )hotGoodsCfg,
					CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.Hot
				};
				msg.Info.Add( goodsCfgInfo );
			}

			CS.instance.PostMsgToGC( this.userNetInfo, msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyGoodsCfgInfo );
			return ErrorCode.Success;
		}

		private ErrorCode SynUserCLDays()
		{
			GSToGC.NotifyUserCLDays msg = new GSToGC.NotifyUserCLDays();
			DateTime today = CS.instance.userMgr.today;
			msg.Month = ( uint )today.Month;//当前月
			msg.Today = ( uint )today.Day;//当前日
			msg.TotalCldays = ( uint )DateTime.DaysInMonth( today.Year, today.Month );//月总天数
			msg.Cldays = this.userDbData.usrDBData.un16Cldays;//已领天数
			msg.IsTodayCan = this.userDbData.usrDBData.un32LastGetLoginRewardDay != today.DayOfYear;//今天是否可以领取
			return this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserCldays );
		}

		private ErrorCode SynUserAllHeroList()
		{
			GSToGC.NotifyCSHeroList msg = new GSToGC.NotifyCSHeroList();
			List<HeroListStruct> heroVec = new List<HeroListStruct>();
			this.GetHeroVec( heroVec );
			foreach ( HeroListStruct heroListStruct in heroVec )
			{
				GSToGC.NotifyCSHeroList.Types.HeroListCfg pCfg = new GSToGC.NotifyCSHeroList.Types.HeroListCfg();
				HeroBuyCfg heroBuyCfg = CS.instance.csCfg.GetHeroClientMatchCfg( heroListStruct.heroid );
				if ( heroBuyCfg == null )
					continue;

				if ( heroListStruct.expiredTime <= 0 && heroListStruct.expiredTime != -1 && heroListStruct.ifFree != true )
					continue;

				pCfg.Heroid = heroBuyCfg.un32CommondityID;
				pCfg.ExpiredTime = heroListStruct.expiredTime;
				pCfg.IfFree = heroListStruct.ifFree;
				msg.Herocfg.Add( pCfg );
			}
			return this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyHeroList );
		}

		public ErrorCode SynUserSNSList( ulong guidFriends, RelationShip type )
		{
			GSToGC.NotifyUserSNSList notifyUserSNSList = new GSToGC.NotifyUserSNSList();
			if ( guidFriends != 0 )
			{
				CSUser user = CS.instance.userMgr.GetUser( guidFriends );
				if ( null == user )
					return ErrorCode.UserNotExist;

				GSToGC.SNSInfo info = new GSToGC.SNSInfo
				{
					Type = ( int )type,
					Nickname = user.nickname,
					Headid = user.headID,
					Status = ( int )user.userPlayingStatus,
					Guididx = ( guidFriends ),
					Viplv = ( uint )user.userDbData.usrDBData.un16VipLv
				};
				notifyUserSNSList.Info.Add( info );
			}
			else
			{
				if ( RelationShip.Friends == type )
				{
					foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.friendListMap )
					{
						GSToGC.SNSInfo info = new GSToGC.SNSInfo
						{
							Type = ( int )type,
							Guididx = kv.Value.guididx
						};
						CSUser pTempUser = CS.instance.userMgr.GetUser( kv.Value.guididx );
						if ( null != pTempUser )
						{
							info.Headid = ( pTempUser.headID );
							info.Nickname = ( pTempUser.nickname );
							info.Status = ( int )( pTempUser.userPlayingStatus );
							info.Viplv = ( uint )( pTempUser.userDbData.usrDBData.un16VipLv );
						}
						else
						{
							info.Headid = ( kv.Value.nHeadId );
							info.Nickname = ( kv.Value.stNickName );
							info.Status = ( int )( UserPlayingStatus.OffLine );
							info.Viplv = ( kv.Value.viplv );
						}
						notifyUserSNSList.Info.Add( info );
					}
				}
				else
				{
					foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.blackListMap )
					{
						CSUser pTempUser = CS.instance.userMgr.GetUser( kv.Value.guididx );
						GSToGC.SNSInfo info = new GSToGC.SNSInfo
						{
							Type = ( int )( type ),
							Headid = ( kv.Value.nHeadId ),
							Guididx = ( kv.Value.guididx ),
							Nickname = null != pTempUser ? pTempUser.nickname : kv.Value.stNickName
						};
						notifyUserSNSList.Info.Add( info );
					}
				}
			}
			this.PostMsgToGC( notifyUserSNSList, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserSnslist );
			return ErrorCode.Success;
		}

		public ErrorCode SynCurDiamond()
		{
			GSToGC.NotifyCurDiamond msg =
				new GSToGC.NotifyCurDiamond { Diamond = ( ulong )this.userDbData.usrDBData.n64Diamond };
			return this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyCurDiamond );
		}
	}
}