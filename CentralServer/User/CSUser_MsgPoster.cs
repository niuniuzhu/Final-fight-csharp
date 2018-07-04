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
			return CS.instance.csUserMgr.PostMsgToGCAskReturn( this.userNetInfo, n32AskProtocalID, errorCode );
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
			return this.PostMsgToGC( msg, ( int ) GSToGC.MsgID.EMsgToGcfromGsNotifyNewHeaderid );
		}

		public void PostCSNotice()
		{
			GSToGC.GameNotice msg = new GSToGC.GameNotice();
			CS.instance.csUserMgr.ForeachNotice( tempNotice =>
			{
				if ( tempNotice.msg.Length < 1 )
					return;

				//平台判断
				if ( tempNotice.platform != UserPlatform.Platform_All )
					if ( tempNotice.platform != this.userDbData.usrDBData.userPlatform )
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
				msg.Notice.Add( notice );
			} );
			this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNotice );
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
				if ( UserPlayingStatus.UserPlayingStatusPlaying == piUser.userPlayingStatus )
					piUser.SynUserSNSList( this.guid, kv.Value.relationShip );
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
				msg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.csCfg.ForeachRunesCfg( kv =>
			{
				RunesCfg pCfg = kv.Value;
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
				msg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.csCfg.ForeachDiscountCfg( kv =>
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
			} );

			CS.instance.csCfg.ForeachNewGoodsCfg( newGoodsCfg =>
			{
				GSToGC.GoodsCfgInfo sGoodsCfgInfo = new GSToGC.GoodsCfgInfo
				{
					Goodid = ( int )newGoodsCfg,
					CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.New
				};
				msg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.csCfg.ForeachHotGoodsCfg( hotGoodsCfg =>
			{
				GSToGC.GoodsCfgInfo sGoodsCfgInfo = new GSToGC.GoodsCfgInfo
				{
					Goodid = ( int )hotGoodsCfg,
					CfgType = GSToGC.GoodsCfgInfo.Types.CfgType.Hot
				};
				msg.Info.Add( sGoodsCfgInfo );
			} );

			CS.instance.PostMsgToGC( this.userNetInfo, msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyGoodsCfgInfo );
			return ErrorCode.Success;
		}

		private ErrorCode SynUserCLDays()
		{
			GSToGC.NotifyUserCLDays msg = new GSToGC.NotifyUserCLDays();
			DateTime today = CS.instance.csUserMgr.today;
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
				msg.Herocfg.Add( pCfg );
			}
			return this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyHeroList );
		}

		public ErrorCode SynUserSNSList( ulong guidFriends, RelationShip type )
		{
			GSToGC.NotifyUserSNSList notifyUserSNSList = new GSToGC.NotifyUserSNSList();
			if ( guidFriends != 0 )
			{
				CSUser user = CS.instance.csUserMgr.GetUser( guidFriends );
				if ( null == user )
					return ErrorCode.UserNotExist;

				GSToGC.SNSInfo pInfo = new GSToGC.SNSInfo
				{
					Type = ( int )type,
					Nickname = user.nickname,
					Headid = user.headID,
					Status = ( int )user.userPlayingStatus,
					Guididx = ( guidFriends ),
					Viplv = ( uint )user.userDbData.usrDBData.un16VipLv
				};
				notifyUserSNSList.Info.Add( pInfo );
			}
			else
			{
				if ( RelationShip.RsTypeFriends == type )
				{
					foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.friendListMap )
					{
						GSToGC.SNSInfo info = new GSToGC.SNSInfo();
						info.Type = ( int )type;
						info.Guididx = kv.Value.guididx;
						CSUser pTempUser = CS.instance.csUserMgr.GetUser( kv.Value.guididx );
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
							info.Status = ( int )( UserPlayingStatus.UserPlayingStatusOffLine );
							info.Viplv = ( kv.Value.viplv );
						}
						notifyUserSNSList.Info.Add( info );
					}
				}
				else
				{
					foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in this.userDbData.blackListMap )
					{
						GSToGC.SNSInfo info = new GSToGC.SNSInfo();
						info.Type = ( int )( type );
						info.Headid = ( kv.Value.nHeadId );
						info.Guididx = ( kv.Value.guididx );
						CSUser pTempUser = CS.instance.csUserMgr.GetUser( kv.Value.guididx );
						info.Nickname = null != pTempUser ? pTempUser.nickname : kv.Value.stNickName;
						notifyUserSNSList.Info.Add( info );
					}
				}
			}
			this.PostMsgToGC( notifyUserSNSList, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserSnslist );
			return ErrorCode.Success;
		}

		public ErrorCode SynCurDiamond()
		{
			GSToGC.NotifyCurDiamond msg = new GSToGC.NotifyCurDiamond();
			msg.Diamond = ( ulong )this.userDbData.usrDBData.n64Diamond;
			return this.PostMsgToGC( msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyCurDiamond );
		}
	}
}