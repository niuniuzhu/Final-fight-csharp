using CentralServer.Tools;
using Core.Misc;
using Google.Protobuf;
using ProtoBuf;
using Shared;
using Shared.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		/// <summary>
		/// 异步查询玩家数据
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
					this.DBAsyn_ExeSQL( buffer, db );
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
			CS.instance.csUserMgr.EncodeAndSendToLogicThread( queryUser, ( int )DBToCS.MsgID.EQueryUserDbcallBack );
			return ErrorCode.Success;
		}

		private ErrorCode DBAsynQueryUser( UserDBData userData, DBToCS.QueryUser queryUser, DBActiveWrapper db )
		{
			string sqlStr =
				$"SELECT * from gameuser, gameuser_runne,gameuser_guide where gameuser.obj_id = {userData.usrDBData.un64ObjIdx} and " +
				$"gameuser_runne.user_id ={userData.usrDBData.un64ObjIdx} and gameuser_guide.obj_id = {userData.usrDBData.un64ObjIdx};";
			ErrorCode errorCode = db.SqlExecQuery( sqlStr, dataReader =>
			{
				if ( !dataReader.Read() )
				{
					Logger.Warn( "could not find user data" );
					return ErrorCode.UserDataNotFound;
				}

				userData.szNickName = Convert.ToString( dataReader["obj_name"] );
				userData.usrDBData.n16Sex = Convert.ToInt16( dataReader["obj_sex"] );
				userData.usrDBData.userPlatform = ( UserPlatform )Convert.ToInt32( dataReader["sdk_id"] );
				userData.usrDBData.un16HeaderID = Convert.ToUInt16( dataReader["obj_headid"] );

				userData.usrDBData.n64Score = Convert.ToInt64( dataReader["obj_score"] );
				userData.usrDBData.n64Diamond = Convert.ToInt64( dataReader["obj_diamond"] );
				userData.usrDBData.n64Gold = Convert.ToInt64( dataReader["obj_gold"] );
				userData.usrDBData.un32TotalGameInns = Convert.ToUInt32( dataReader["obj_game_inns"] );
				userData.usrDBData.un32TotalWinInns = Convert.ToUInt32( dataReader["obj_game_winns"] );
				userData.usrDBData.un32TotalHeroKills = Convert.ToUInt32( dataReader["obj_kill_hero_num"] );
				userData.usrDBData.un32TotalAssist = Convert.ToUInt32( dataReader["obj_ass_kill_num"] );
				userData.usrDBData.un32TotalDestoryBuildings = Convert.ToUInt32( dataReader["obj_dest_building_num"] );
				userData.usrDBData.un32TotalDeadTimes = Convert.ToUInt32( dataReader["obj_dead_num"] );
				userData.usrDBData.un32UserCurLvExp = Convert.ToUInt32( dataReader["obj_cur_lv_exp"] );

				userData.usrDBData.un8UserLv = Convert.ToByte( dataReader["obj_lv"] );
				userData.usrDBData.un16Cldays = Convert.ToUInt16( dataReader["obj_cldays"] );
				userData.usrDBData.un16VipLv = Convert.ToInt16( dataReader["obj_vip_lv"] );

				userData.usrDBData.un32LastGetLoginRewardDay = Convert.ToInt32( dataReader["obj_last_loginreward_time"] );
				userData.usrDBData.tRegisteUTCMillisec = Convert.ToInt64( dataReader["obj_register_time"] );
				userData.szTaskData = Convert.ToString( dataReader["obj_task_data"] );

				queryUser.TaskData = userData.szTaskData;

				DBToCS.RuneInfo runedB = new DBToCS.RuneInfo
				{
					SlotStr = Convert.ToString( dataReader["runeslot_json"] ),
					BagStr = Convert.ToString( dataReader["runnebag_json"] )
				};

				queryUser.Runeinfo.Add( runedB );
				queryUser.Guidestr = Convert.ToString( dataReader["obj_cs_guide_com_steps"] );

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

		private ErrorCode GetUserHeros( DBActiveWrapper pConn, ulong un64ObjIdx, DBToCS.QueryUser sQueryUser )
		{
			string sqlStr = $"select hero_id,hero_end_time,hero_buy_time from gameuser_hero where user_id={un64ObjIdx};";
			ErrorCode errorCode = pConn.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					DBToCS.HeroCfg heroDB = new DBToCS.HeroCfg
					{
						Buytime = Convert.ToInt64( dataReader["hero_buy_time"] ),
						Expiredtime = Convert.ToInt64( dataReader["hero_end_time"] ),
						Commodityid = Convert.ToUInt32( dataReader["hero_id"] )
					};
					sQueryUser.Herocfg.Add( heroDB );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryUserSNSList( DBActiveWrapper pConn, ulong un64ObjIdx, Dictionary<ulong, uint> t_map )
		{
			string sqlStr = $"select * from gameuser_sns where user_id={un64ObjIdx};";
			ErrorCode errorCode = pConn.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					t_map[Convert.ToUInt64( dataReader["related_id"] )] = Convert.ToUInt32( dataReader["relation"] );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryUserHeaderAndNickname( DBActiveWrapper pConn, ulong un64ObjIdx, DBToCS.RSinfo rs_info )
		{
			string sqlStr = $"select obj_name,obj_headid,obj_vip_lv from gameuser where obj_id={un64ObjIdx};";
			ErrorCode errorCode = pConn.SqlExecQuery( sqlStr, dataReader =>
			{
				if ( dataReader.Read() )
				{
					rs_info.RelatedName = Convert.ToString( dataReader["obj_name"] );
					rs_info.RelatedHeader = Convert.ToUInt32( dataReader["obj_headid"] );
					rs_info.RelatedVip = Convert.ToUInt32( dataReader["obj_vip_lv"] );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryUserItems( DBActiveWrapper pConn, ulong user_id, DBToCS.QueryUser sQueryUser )
		{
			string sqlStr = $"select * from gameuser_item where user_id={user_id};";
			ErrorCode errorCode = pConn.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					DBToCS.ItemInfo item_info = new DBToCS.ItemInfo
					{
						ItemId = Convert.ToInt32( dataReader["item_id"] ),
						ItemNum = Convert.ToUInt32( dataReader["item_num"] ),
						BuyTime = Convert.ToInt32( dataReader["buy_time"] ),
						EndTime = Convert.ToInt32( dataReader["end_time"] )
					};
					sQueryUser.ItemInfo.Add( item_info );

				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode GetUserShortGiftMail( DBActiveWrapper pConn, UserDBData sUserData, DBToCS.QueryUser sQueryUser )
		{
			string sqlStr =
				$"select mail_id,mail_state from gameuser_mail where user_id = {sUserData.usrDBData.un64ObjIdx} order by mail_id DESC;";
			ErrorCode errorCode = pConn.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					DBToCS.MailInfo mailInfo = new DBToCS.MailInfo
					{
						Mailid = Convert.ToInt32( dataReader["mail_id"] ),
						State = Convert.ToInt32( dataReader["mail_state"] )
					};
					sQueryUser.MailInfo.Add( mailInfo );
				}
				this.DBAsynQueryGameMailList( pConn, sUserData.usrDBData.un64ObjIdx );
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynQueryGameMailList( DBActiveWrapper pConn, ulong objIdx )
		{
			string sqlStr = "select *  from game_mail where mail_del_state<> " + ( int )MailCurtState.Del;
			if ( objIdx > 0 )
				sqlStr += $" and mail_user_id = {objIdx}";
			else
				sqlStr += " and (mail_user_id is NULL or mail_user_id < 1) ";
			sqlStr += " AND unix_timestamp(mail_over_time ) > unix_timestamp(NOW()) order by mail_id DESC;";
			ErrorCode errorCode = pConn.SqlExecQuery( sqlStr, dataReader =>
			{
				while ( dataReader.Read() )
				{
					MailDBData mailDb = new MailDBData
					{
						mailId = Convert.ToInt32( dataReader["mail_id"] ),
						mailType = ( MailType )Convert.ToInt32( dataReader["mail_type"] ),
						channelId = Convert.ToInt32( dataReader["mail_sdk"] ),
						mailTitle = Convert.ToString( dataReader["mail_title"] ),
						mailContent = Convert.ToString( dataReader["mail_content"] ),
						mailGift = Convert.ToString( dataReader["mail_gift"] ),
						szSender = Convert.ToString( dataReader["mail_send"] ),
						mCreateTime = Convert.ToString( dataReader["mail_create_time"] ),
						mEndTime = Convert.ToString( dataReader["mail_over_time"] )
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
					CS.instance.csUserMgr.EncodeAndSendToLogicThread( pMsg, ( int )DBToCS.MsgID.EMailCallBack );
				}
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private ErrorCode DBAsynUpdateGameMail( GBuffer buffer, DBActiveWrapper db )
		{
			return ErrorCode.Success;
		}

		private ErrorCode DBAsynChangeNickNameCallBack( GBuffer buffer, DBActiveWrapper db )
		{
			return ErrorCode.Success;
		}

		private ErrorCode DBAsyn_ExeSQL( GBuffer buffer, DBActiveWrapper db )
		{
			return ErrorCode.Success;
		}

		private ErrorCode InsertCDKeyEvent( GBuffer buffer, DBActiveWrapper db )
		{
			return ErrorCode.Success;
		}

		private ErrorCode DBAsynUpdateUserGameMail( GBuffer buffer, DBActiveWrapper db )
		{
			return ErrorCode.Success;
		}

		private ErrorCode UpdateCDKey( GBuffer buffer, DBActiveWrapper db )
		{
			return ErrorCode.Success;
		}

		private ErrorCode InsertCDKey( GBuffer buffer, DBActiveWrapper db )
		{
			return ErrorCode.Success;
		}
	}
}