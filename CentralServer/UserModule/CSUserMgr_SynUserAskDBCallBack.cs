using CentralServer.Tools;
using CentralServer.User;
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
		private void SynUserAskDBCallBack()
		{
			this._dbCallbackQueue.Switch();
			while ( !this._dbCallbackQueue.isEmpty )
			{
				GBuffer buffer = this._dbCallbackQueue.Pop();
				DBToCS.MsgID msgID = ( DBToCS.MsgID )buffer.data;
				switch ( msgID )
				{
					case DBToCS.MsgID.EQueryUserDbcallBack:
						this.SynHandleQueryUserCallback( buffer );
						break;

					case DBToCS.MsgID.EQueryAllAccountCallBack:
						this.SynHandleAllAccountCallback( buffer );
						break;

					case DBToCS.MsgID.EMailCallBack:
						this.SynHandleMailCallback( buffer );
						break;

					case DBToCS.MsgID.EQueryNoticeCallBack:
						this.DBCallBackQueryNotice( buffer );
						break;

					default:
						Logger.Warn( $"not hv handler:{buffer.data}" );
						break;
				}
				this._dbCallbackQueuePool.Push( buffer );
			}
		}

		private void DBAsynQueryWhenThreadBegin()
		{
			ErrorCode errorCode = this._userCacheDBActiveWrapper.SqlExecQuery( "select MAX(mail_id) as mailid from game_mail;", dataReader =>
			{
				if ( dataReader.Read() && !dataReader.IsDBNull( 0 ) )
				{
					int value = dataReader.GetInt32( 0 );
					//服务器启动的时候 没有登录的玩家，可以这样设置
					CS.instance.mailMgr.setCurtMaxMailIdx( value );
					if ( value > 0 )
						this.DBAsynQueryGameMailList( this._userCacheDBActiveWrapper, 0 );
				}

				return ErrorCode.Success;
			} );

			if ( errorCode != ErrorCode.Success )
				return;

			this.DBAsynQueryNoticeCallBack( this._userCacheDBActiveWrapper );
		}

		private ErrorCode DBAsynQueryGameMailList( DBActiveWrapper db, long objIdx )
		{
			string sqlStr = "select * from game_mail where mail_del_state<> " + MailCurtState.Del;
			if ( objIdx > 0 )
				sqlStr += " and mail_user_id = " + objIdx;
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
					mailDb.objIdx = objIdx > 0 ? objIdx : mailDb.objIdx;

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

		private ErrorCode DBAsynQueryNoticeCallBack( DBActiveWrapper db )
		{
			ErrorCode errorCode = db.SqlExecQuery( "select * from notice;", dataReader =>
			{
				DBToCS.QueryNotice notice = new DBToCS.QueryNotice();
				while ( dataReader.Read() )
				{
					DBToCS.QueryNotice.Types.Notice info =
						new DBToCS.QueryNotice.Types.Notice
						{
							Id = dataReader.GetUInt32( "id" ),
							Platform = dataReader.GetUInt32( "platform_id" ),
							Title = dataReader.GetString( "title" ),
							Eflag = dataReader.GetInt32( "eflag" ),
							Estate = dataReader.GetInt32( "estate" ),
							Priority = dataReader.GetInt32( "priority" ),
							Notice_ = dataReader.GetString( "notice" ),
							StarTime = dataReader.GetUInt64( "star_time" ),
							EndTime = dataReader.GetUInt64( "end_time" )
						};
					notice.NoticeInfo.Add( info );
				}
				CS.instance.userMgr.EncodeAndSendToLogicThread( notice, ( int )DBToCS.MsgID.EQueryNoticeCallBack );
				return ErrorCode.Success;
			} );
			return errorCode;
		}

		private void CDKThreadBeginCallback()
		{
			this._cdkeyWrapper.SqlExecQuery( $"select id,sdk_id,cdkey,user_name from account_user where cs_id={CS.instance.csID};", dataReader =>
			{
				DBToCS.QueryAllAccount queryAllAccount = new DBToCS.QueryAllAccount();
				while ( dataReader.Read() )
				{
					DBToCS.QueryAllAccount.Types.Account account =
						new DBToCS.QueryAllAccount.Types.Account
						{
							Guid = dataReader.GetInt64( "id" ),
							UserName = dataReader.GetString( "cdkey" ),
							Nickname = dataReader.GetString( "user_name" ),
							Sdkid = dataReader.GetInt32( "sdk_id" )
						};
					queryAllAccount.Account.Add( account );
				}
				if ( dataReader.HasRows )
					CS.instance.userMgr.EncodeAndSendToLogicThread( queryAllAccount, ( int )DBToCS.MsgID.EQueryAllAccountCallBack );
				return ErrorCode.Success;
			} );
		}

		private ErrorCode SynHandleQueryUserCallback( GBuffer buffer )
		{
			DBToCS.QueryUser msg = new DBToCS.QueryUser();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			GCToCS.Login login = new GCToCS.Login();
			login.MergeFrom( ByteString.CopyFromUtf8( msg.Login ) );

			UserNetInfo netInfo = new UserNetInfo( msg.Gsid, ( uint )msg.Gcnetid );
			if ( this.ContainsUser( netInfo ) )
			{
				Logger.Warn( "invalid netInfo" );
				return ErrorCode.InvalidNetState;
			}

			UserDBData userDbData;
			using ( MemoryStream ms = new MemoryStream( Encoding.UTF8.GetBytes( msg.Db ) ) )
			{
				userDbData = Serializer.Deserialize<UserDBData>( ms );
			}

			CSUser user = this.GetUser( userDbData.usrDBData.un64ObjIdx );
			if ( null != user )
			{
				user.OnOnline( netInfo, login, false, false );
				return ErrorCode.Success;
			}

			user = new CSUser();

			userDbData.szUserName = login.Name;
			userDbData.szUserPwd = login.Passwd;
			userDbData.szNickName = msg.Nickname;
			userDbData.usrDBData.userPlatform = ( UserPlatform )login.Sdk;
			userDbData.szTaskData = msg.TaskData;

			bool newUser = userDbData.usrDBData.tRegisteUTCMillisec < 1;

			user.LoadDBData( userDbData );
			user.userDbData.guideSteps.szCSContinueGuide = msg.Guidestr;

			for ( int i = 0; i < msg.Rsinfo.Count; i++ )
				user.LoadUserSNSList( msg.Rsinfo[i] );

			for ( int i = 0; i < msg.ItemInfo.Count; i++ )
				user.AddUserItems( msg.ItemInfo[i] );

			for ( int i = 0; i < msg.MailInfo.Count; ++i )
				CS.instance.mailMgr.UpdatePerMailList( msg.MailInfo[i].Mailid, userDbData.usrDBData.un64ObjIdx, ( MailCurtState )msg.MailInfo[i].State );

			//todo
			//user.GetUserBattleInfoEx().mDebugName = pLogin.Name;

			if ( !newUser )
			{
				long curTime = TimeUtils.utcTime;
				for ( int i = 0; i < msg.Herocfg.Count; ++i )
				{
					DBToCS.HeroCfg heroCfg = msg.Herocfg[i];
					if ( heroCfg.Expiredtime != Consts.PERSIST_TIME_ALWAYS && heroCfg.Expiredtime < curTime )
						continue;

					UserHeroDBData userHeroDbData = new UserHeroDBData( heroCfg.Commodityid, heroCfg.Expiredtime, heroCfg.Buytime );
					user.AddHero( userHeroDbData );
				}
				for ( int i = 0; i < msg.Runeinfo.Count; ++i )
				{
					DBToCS.RuneInfo runeInfo = msg.Runeinfo[i];
					user.InitRunes( runeInfo.BagStr, runeInfo.SlotStr );
				}
			}

			ErrorCode errorCode = this.AddUser( user );
			if ( errorCode != ErrorCode.Success )
				return errorCode;

			user.OnOnline( netInfo, login, newUser, true );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 查询玩家数据的回调函数
		/// 把数据库的所有玩家数据取回内存
		/// </summary>
		private void SynHandleAllAccountCallback( GBuffer buffer )
		{
			DBToCS.QueryAllAccount msg = new DBToCS.QueryAllAccount();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			for ( int i = 0; i < msg.Account.Count; ++i )
			{
				DBToCS.QueryAllAccount.Types.Account pAccount = msg.Account[i];
				//取回所有昵称
				if ( !string.IsNullOrEmpty( pAccount.Nickname ) )
					this.allNickNameSet.Add( pAccount.Nickname );

				UserCombineKey userCombineKey;
				userCombineKey.sdkid = pAccount.Sdkid;
				userCombineKey.username = pAccount.UserName;

				ulong guid = ( ulong )pAccount.Guid;
				//取回所有guid
				this.allUserName2GuidMap.Add( userCombineKey, guid );
				if ( this._maxGuid < guid )
					this._maxGuid = guid;
			}
			this._maxGuid /= GUID_Devide;
			Logger.Log( $"Load maxguid {this._maxGuid}" );
		}

		private void SynHandleMailCallback( GBuffer buffer )
		{
			DBToCS.MailCallBack pMsg = new DBToCS.MailCallBack();
			pMsg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			MailDBData mailDb = new MailDBData();
			mailDb.mailId = pMsg.Mailid;
			mailDb.mailType = ( MailType )pMsg.Mailtype;
			mailDb.channelId = pMsg.Channel;
			mailDb.mailContent = pMsg.Content;
			mailDb.mailTitle = pMsg.Title;
			mailDb.mailGift = pMsg.Gift;
			mailDb.szSender = pMsg.Sender;
			mailDb.mCreateTime = pMsg.Createtime;
			//todo
			//mailDb.n64CreateTime = CFunction::FormatTime2TimeT( mailDb.mCreateTime );
			//mailDb.n64EndTime = CFunction::FormatTime2TimeT( mailDb.mEndTime );
			mailDb.objIdx = pMsg.Objid;
			CS.instance.mailMgr.AddGameMail( mailDb );
		}

		private void DBCallBackQueryNotice( GBuffer buffer )
		{
			DBToCS.QueryNotice msg = new DBToCS.QueryNotice();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			for ( int i = 0; i < msg.NoticeInfo.Count; i++ )
			{
				Notice notice = new Notice();
				DBToCS.QueryNotice.Types.Notice noticeInfo = msg.NoticeInfo[i];
				notice.id = noticeInfo.Id;
				notice.platform = ( UserPlatform )noticeInfo.Platform;
				notice.title = noticeInfo.Title;
				notice.flag = ( NoticeFlag )noticeInfo.Eflag;
				notice.state = ( NoticeState )noticeInfo.Estate;
				notice.priority = ( uint )noticeInfo.Priority;
				notice.msg = noticeInfo.Notice_;
				notice.star_time = ( long )noticeInfo.StarTime;
				notice.end_time = ( long )noticeInfo.EndTime;
				this.AddNotice( notice );
			}
		}

		private bool AddNotice( Notice notice )
		{
			long temp_date = TimeUtils.utcTime;
			long temp = temp_date - notice.end_time;
			if ( temp > 0 ) //公告过期判断
				return false;
			this.notices.Add( notice );
			return true;
		}

		private void PostSaveCmd()
		{
			Logger.Info( "start post save data to db...." );
			foreach ( KeyValuePair<ulong, CSUser> kv in this.userGuidMap )
				this.DBPosterUpdateUser( kv.Value );
			Logger.Error( "only finish post save data to db, don't close me at once." );
		}

		private ErrorCode DBPosterUpdateUser( CSUser user )
		{
			//UserDBData psUserDBData = user.userDbData;
			//user.GetTaskMgr().PackTaskData( psUserDBData.szTaskData, psUserDBData.isTaskRush );//存数据库时增加任务数据
			//CCSUserDbDataMgr.UpdateUserDbData( psUserDBData, m_SaveUserStream );
			//if ( !m_SaveUserStream.str().empty() )
			//{
			//	CSToDB.UpdateUser sUpdateUser = new CSToDB.UpdateUser();
			//	sUpdateUser.Guid = ( long )psUserDBData.usrDBData.un64ObjIdx;
			//	sUpdateUser.Sqlstr = ( m_SaveUserStream.str() );
			//	this._userCacheDBActiveWrapper.EncodeAndSendToDBThread( sUpdateUser, CSToDB.MsgID.EUpdateUserDbcallBack );
			//}
			return ErrorCode.Success;
		}
	}
}