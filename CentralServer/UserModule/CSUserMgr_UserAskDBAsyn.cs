using CentralServer.Tools;
using Core.Misc;
using Google.Protobuf;
using ProtoBuf;
using Shared;
using Shared.DB;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		/// <summary>
		/// 异步查询/更新玩家数据
		/// </summary>
		private void UserAskDBAsynHandler( GBuffer buffer )
		{
			DBActiveWrapper db = this.GetDBSource( buffer.actorID );
			CSToDB.MsgID msgID = ( CSToDB.MsgID )buffer.data;
			switch ( msgID )
			{
				case CSToDB.MsgID.EQueryUserDbcallBack:
					this.DBAsynQueryUserCallBack( buffer, db );
					break;

				case CSToDB.MsgID.EUpdateGameMailDbcallBack:
					this.DBAsynUpdateGameMail( buffer, db );
					break;

				case CSToDB.MsgID.EChangeNickNameDbcall:
					this.DBAsynChangeNickNameCallBack( buffer, db );
					break;

				case CSToDB.MsgID.EExeSqlCall:
					this.DBAsynExeSQL( buffer, db );
					break;

				case CSToDB.MsgID.EInsertCdkeyEvents:
					this.InsertCDKeyEvent( buffer, db );
					break;

				case CSToDB.MsgID.EUpdateUserGameMailDbcallBack:
					this.DBAsynUpdateUserGameMail( buffer, db );
					break;

				case CSToDB.MsgID.EUpdateCdkeyInfo:
					this.UpdateCDKey( buffer, db );
					break;

				case CSToDB.MsgID.EInsertCdkeyInfo:
					this.InsertCDKey( buffer, db );
					break;

				default:
					Logger.Error( "unknown msg" );
					break;
			}

		}

		private ErrorCode DBAsynQueryUserCallBack( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.QueryUserReq msg = new CSToDB.QueryUserReq();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			GCToCS.Login login = new GCToCS.Login();
			login.MergeFrom( ByteString.CopyFromUtf8( msg.Logininfo ) );

			UserDBData userDbData = new UserDBData();
			userDbData.usrDBData.un64ObjIdx = ( ulong )msg.Objid;
			userDbData.usrDBData.userPlatform = ( UserPlatform )login.Sdk;

			DBToCS.QueryUser queryUser = new DBToCS.QueryUser();
			this.DBAsynQueryUser( userDbData, queryUser, db );

			using ( MemoryStream ms = new MemoryStream() )
			{
				Serializer.Serialize( ms, userDbData.usrDBData );
				queryUser.Db = Encoding.UTF8.GetString( ms.GetBuffer(), 0, ( int )ms.Length );
				queryUser.Login = msg.Logininfo;
				queryUser.Gcnetid = msg.Gcnetid;
				queryUser.Gsid = msg.Gsid;
				queryUser.Nickname = userDbData.szNickName;
			}
			CS.instance.userMgr.EncodeAndSendToLogicThread( queryUser, ( int )DBToCS.MsgID.EQueryUserDbcallBack );
			return ErrorCode.Success;
		}

		private ErrorCode DBAsynQueryUser( UserDBData userData, DBToCS.QueryUser queryUser, DBActiveWrapper db )
		{
			string sqlStr =
				$"select * from gameuser, gameuser_runne,gameuser_guide where gameuser.obj_id = {userData.usrDBData.un64ObjIdx} and " +
				$"gameuser_runne.user_id ={userData.usrDBData.un64ObjIdx} and gameuser_guide.obj_id = {userData.usrDBData.un64ObjIdx};";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				if ( !dataReader.Read() )
				{
					Logger.Warn( "could not find user data" );
					return ErrorCode.UserDataNotFound;
				}

				userData.szNickName = dataReader.GetString( "obj_name" );
				userData.usrDBData.n16Sex = dataReader.GetInt16( "obj_sex" );
				userData.usrDBData.userPlatform = ( UserPlatform )dataReader.GetInt32( "sdk_id" );
				userData.usrDBData.un16HeaderID = dataReader.GetUInt16( "obj_headid" );

				userData.usrDBData.n64Score = dataReader.GetInt64( "obj_score" );
				userData.usrDBData.n64Diamond = dataReader.GetInt64( "obj_diamond" );
				userData.usrDBData.n64Gold = dataReader.GetInt64( "obj_gold" );
				userData.usrDBData.un32TotalGameInns = dataReader.GetUInt32( "obj_game_inns" );
				userData.usrDBData.un32TotalWinInns = dataReader.GetUInt32( "obj_game_winns" );
				userData.usrDBData.un32TotalHeroKills = dataReader.GetUInt32( "obj_kill_hero_num" );
				userData.usrDBData.un32TotalAssist = dataReader.GetUInt32( "obj_ass_kill_num" );
				userData.usrDBData.un32TotalDestoryBuildings = dataReader.GetUInt32( "obj_dest_building_num" );
				userData.usrDBData.un32TotalDeadTimes = dataReader.GetUInt32( "obj_dead_num" );
				userData.usrDBData.un32UserCurLvExp = dataReader.GetUInt32( "obj_cur_lv_exp" );

				userData.usrDBData.un8UserLv = dataReader.GetByte( "obj_lv" );
				userData.usrDBData.un16Cldays = dataReader.GetUInt16( "obj_cldays" );
				userData.usrDBData.un16VipLv = dataReader.GetInt16( "obj_vip_lv" );

				userData.usrDBData.un32LastGetLoginRewardDay = dataReader.GetInt32( "obj_last_loginreward_time" );
				userData.usrDBData.tRegisteUTCMillisec = dataReader.GetInt64( "obj_register_time" );
				userData.szTaskData = dataReader.GetString( "obj_task_data" );

				queryUser.TaskData = userData.szTaskData;

				DBToCS.RuneInfo runedB = new DBToCS.RuneInfo
				{
					SlotStr = dataReader.GetString( "runeslot_json" ),
					BagStr = dataReader.GetString( "runnebag_json" )
				};

				queryUser.Runeinfo.Add( runedB );
				queryUser.Guidestr = dataReader.GetString( "obj_cs_guide_com_steps" );

				return ErrorCode.Success;
			} );

			if ( errorCode != ErrorCode.Success )
				return errorCode;

			errorCode = this.GetUserHeros( db, userData.usrDBData.un64ObjIdx, queryUser );
			if ( errorCode != ErrorCode.Success )
				return errorCode;

			Dictionary<ulong, uint> t_map = new Dictionary<ulong, uint>();
			errorCode = this.DBAsynQueryUserSNSList( db, userData.usrDBData.un64ObjIdx, t_map );
			if ( errorCode == ErrorCode.Success )
			{
				foreach ( KeyValuePair<ulong, uint> kv in t_map )
				{
					DBToCS.RSinfo info = new DBToCS.RSinfo
					{
						RelatedId = kv.Key,
						Relation = kv.Value
					};
					queryUser.Rsinfo.Add( info );
					this.DBAsynQueryUserHeaderAndNickname( db, kv.Value, info );
				}
			}
			else
				Logger.Error( $"DBAsynQueryUserSNSList error:{errorCode}" );

			errorCode = this.DBAsynQueryUserItems( db, userData.usrDBData.un64ObjIdx, queryUser );
			if ( errorCode != ErrorCode.Success )
				Logger.Error( $"DBAsynQueryUserItems error:{errorCode}" );

			errorCode = this.GetUserShortGiftMail( db, userData, queryUser );
			if ( errorCode != ErrorCode.Success )
				Logger.Error( $"GetUserShortGiftMail error:{errorCode}" );

			return errorCode;
		}

		private ErrorCode GetUserHeros( DBActiveWrapper db, ulong un64ObjIdx, DBToCS.QueryUser sQueryUser )
		{
			string sqlStr = $"select hero_id,hero_end_time,hero_buy_time from gameuser_hero where user_id={un64ObjIdx};";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					DBToCS.HeroCfg heroDB = new DBToCS.HeroCfg
					{
						Buytime = dataReader.GetInt64( "hero_buy_time" ),
						Expiredtime = dataReader.GetInt64( "hero_end_time" ),
						Commodityid = dataReader.GetUInt32( "hero_id" )
					};
					sQueryUser.Herocfg.Add( heroDB );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryUserSNSList( DBActiveWrapper db, ulong un64ObjIdx, Dictionary<ulong, uint> t_map )
		{
			string sqlStr = $"select * from gameuser_sns where user_id={un64ObjIdx};";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					t_map[dataReader.GetUInt64( "related_id" )] = dataReader.GetUInt32( "relation" );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryUserHeaderAndNickname( DBActiveWrapper db, ulong un64ObjIdx, DBToCS.RSinfo rs_info )
		{
			string sqlStr = $"select obj_name,obj_headid,obj_vip_lv from gameuser where obj_id={un64ObjIdx};";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				if ( dataReader.Read() )
				{
					rs_info.RelatedName = dataReader.GetString( "obj_name" );
					rs_info.RelatedHeader = dataReader.GetUInt32( "obj_headid" );
					rs_info.RelatedVip = dataReader.GetUInt32( "obj_vip_lv" );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryUserItems( DBActiveWrapper db, ulong user_id, DBToCS.QueryUser sQueryUser )
		{
			string sqlStr = $"select * from gameuser_item where user_id={user_id};";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					DBToCS.ItemInfo item_info = new DBToCS.ItemInfo
					{
						ItemId = dataReader.GetInt32( "item_id" ),
						ItemNum = dataReader.GetUInt32( "item_num" ),
						BuyTime = dataReader.GetInt32( "buy_time" ),
						EndTime = dataReader.GetInt32( "end_time" )
					};
					sQueryUser.ItemInfo.Add( item_info );

				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode GetUserShortGiftMail( DBActiveWrapper db, UserDBData sUserData, DBToCS.QueryUser sQueryUser )
		{
			string sqlStr =
				$"select mail_id,mail_state from gameuser_mail where user_id = {sUserData.usrDBData.un64ObjIdx} order by mail_id DESC;";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					DBToCS.MailInfo mailInfo = new DBToCS.MailInfo
					{
						Mailid = dataReader.GetInt32( "mail_id" ),
						State = dataReader.GetInt32( "mail_state" )
					};
					sQueryUser.MailInfo.Add( mailInfo );
				}
				this.DBAsynQueryGameMailList( db, sUserData.usrDBData.un64ObjIdx );
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryGameMailList( DBActiveWrapper db, ulong objIdx )
		{
			string sqlStr = "select * from game_mail where mail_del_state<> " + ( int )MailCurtState.Del;
			if ( objIdx > 0 )
				sqlStr += $" and mail_user_id = {objIdx}";
			else
				sqlStr += " and (mail_user_id is NULL or mail_user_id < 1) ";
			sqlStr += " and unix_timestamp(mail_over_time ) > unix_timestamp(NOW()) order by mail_id DESC;";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					MailDBData mailDb = new MailDBData
					{
						mailId = dataReader.GetInt32( "mail_id" ),
						mailType = ( MailType )dataReader.GetInt32( "mail_type" ),
						channelId = dataReader.GetInt32( "mail_sdk" ),
						mailTitle = dataReader.GetString( "mail_title" ),
						mailContent = dataReader.GetString( "mail_content" ),
						mailGift = dataReader.GetString( "mail_gift" ),
						szSender = dataReader.GetString( "mail_send" ),
						mCreateTime = dataReader.GetString( "mail_create_time" ),
						mEndTime = dataReader.GetString( "mail_over_time" )
					};
					mailDb.objIdx = objIdx > 0 ? ( long )objIdx : mailDb.objIdx;

					DBToCS.MailCallBack pMsg = new DBToCS.MailCallBack
					{
						Mailid = mailDb.mailId,
						Mailtype = ( int )mailDb.mailType,
						Channel = mailDb.channelId,
						Title = mailDb.mailTitle,
						Content = mailDb.mailContent,
						Gift = mailDb.mailGift,
						Sender = mailDb.szSender,
						Createtime = mailDb.mCreateTime,
						Objid = mailDb.objIdx
					};
					CS.instance.userMgr.EncodeAndSendToLogicThread( pMsg, ( int )DBToCS.MsgID.EMailCallBack );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynUpdateGameMail( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.UpdateGameMail pMsg = new CSToDB.UpdateGameMail();
			pMsg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			int total = pMsg.Maillist.Count;
			if ( total <= 0 )
				return ErrorCode.Success;

			string[] sqlStrs = new string[3];
			sqlStrs[0] = "begin; set autocommit=0;";
			for ( int i = 0; i < total; i++ )
			{
				CSToDB.GameMailInfo mail = pMsg.Maillist[i];
				if ( mail.Curtstate == CSToDB.EMailCurtState.EMailStateDel )
					sqlStrs[1] = $"update game_mail set mail_del_state ={mail.Curtstate}  where  mail_id={mail.MailId};";
				else if ( mail.Curtstate == CSToDB.EMailCurtState.EMailStateNew )
				{
					sqlStrs[1] =
						$"insert into game_mail(mail_id,mail_sdk,mail_type,mail_user_id,mail_title,mail_content,mail_gift,mail_send,mail_create_time,mail_over_time,mail_del_state) values({mail.MailId},{mail.Sdkidx},{mail.Type},{mail.Userid},\'{mail.Title}\',\'{mail.Content}\',\'{mail.Giftstr}\',\'{mail.Sender}\',\'{mail.Createtime}\',\'{mail.Overtime}\',{mail.Curtstate});";
				}
			}
			sqlStrs[2] = "commit;";
			return db.SqlExecNonQuery( sqlStrs );
		}

		private ErrorCode DBAsynChangeNickNameCallBack( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.ChangeNickName sChangeNickName = new CSToDB.ChangeNickName();
			sChangeNickName.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );
			string mystream =
				$"update account_user set user_name=\'{sChangeNickName.Nickname}\' where id={sChangeNickName.Guid};";
			ErrorCode errorCode = db.SqlExecNonQuery( mystream );
			return errorCode;
		}

		private ErrorCode DBAsynExeSQL( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.ExeSQL_Call sMsg = new CSToDB.ExeSQL_Call();
			sMsg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );
			return db.SqlExecNonQuery( sMsg.Sql );
		}

		private ErrorCode InsertCDKeyEvent( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.CDKeyEvents sMsg = new CSToDB.CDKeyEvents();
			sMsg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );
			return db.SqlExecNonQuery( sMsg.SqlStr );
		}

		private ErrorCode DBAsynUpdateUserGameMail( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.UpdateUserMail pMsg = new CSToDB.UpdateUserMail();
			pMsg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			string sqlStr = $"select id from gameuser_mail where mail_id={pMsg.Mailid} and user_id={pMsg.Objid};";
			int ntotal = 0;
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				ntotal = dataReader.GetInt32( "id" );
				return ErrorCode.Success;
			} );
			if ( errorCode != ErrorCode.Success )
				return errorCode;
			sqlStr = ntotal > 0
						 ? $"update gameuser_mail set mail_state={pMsg.Cstate} where mail_id={pMsg.Mailid} and user_id={pMsg.Objid};"
						 : $"insert into gameuser_mail (mail_id,user_id, mail_state) values({pMsg.Mailid},{pMsg.Objid},{pMsg.Cstate});";

			errorCode = db.SqlExecNonQuery( sqlStr );
			return errorCode;
		}

		private ErrorCode UpdateCDKey( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.UpdateCDKeyInfo sMsg = new CSToDB.UpdateCDKeyInfo();
			sMsg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );
			return db.SqlExecNonQuery( sMsg.SqlStr );
		}

		private ErrorCode InsertCDKey( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.InsertCDKeyInfo sMsg = new CSToDB.InsertCDKeyInfo();
			sMsg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );
			return db.SqlExecNonQuery( sMsg.SqlStr );
		}
	}
}