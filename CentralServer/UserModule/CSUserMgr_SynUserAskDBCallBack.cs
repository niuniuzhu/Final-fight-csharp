using CentralServer.Tools;
using CentralServer.User;
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
					//todo
					//服务器启动的时候 没有登录的玩家，可以这样设置
					//m_MailMgr.setCurtMaxMailIdx( tValue );

					//if ( tValue > 0 )
					// DBAsyn_QueryGameMailList( this._userCacheDBActiveWrapper, 0 );
				}

				return ErrorCode.Success;
			} );

			if ( errorCode != ErrorCode.Success )
				return;

			this.DBAsynQueryNoticeCallBack( this._userCacheDBActiveWrapper );
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
				CS.instance.csUserMgr.EncodeAndSendToLogicThread( notice, ( int )DBToCS.MsgID.EQueryNoticeCallBack );
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
					CS.instance.csUserMgr.EncodeAndSendToLogicThread( queryAllAccount, ( int )DBToCS.MsgID.EQueryAllAccountCallBack );
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

			//for ( int i = 0; i < msg.MailInfo.Count; ++i )
			//	m_MailMgr.updatePerMailList( msg.MailInfo[i].Mailid, userDbData.usrDBData.un64ObjIdx, ( EMailCurtState )msg.MailInfo[i].State );

			//todo
			//pcUser.GetUserBattleInfoEx().mDebugName = pLogin.Name;

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
			//todo
			//if ( bNewUser )
			//{
			//	stringstream mystream;
			//	mystream << pLogin.name() << LOG_SIGN << pLogin.sdk() << LOG_SIGN;
			//	mystream << pLogin.platform() << LOG_SIGN << pLogin.equimentid() << LOG_SIGN;
			//	mystream << pLogin.ipaddress();
			//	CSSGameLogMgr.GetInstance().AddGameLog( eLog_Register, pcUser.GetUserDBData().sPODUsrDBData.un64ObjIdx, mystream.str() );
			//}
			return ErrorCode.Success;
		}

		private void SynHandleAllAccountCallback( GBuffer buffer )
		{
		}

		private void SynHandleMailCallback( GBuffer buffer )
		{
		}

		private void DBCallBackQueryNotice( GBuffer buffer )
		{
		}

		private void PostSaveCmd()
		{
			Logger.Info( "start post save data to db...." );
			foreach ( KeyValuePair<ulong, CSUser> kv in this.userGuidMap )
				this.DBPosterUpdateUser( kv.Value );
			Logger.Error( "only finish post save data to db, don't close me at once." );
		}

		private ErrorCode DBPosterUpdateUser( CSUser pcUser )
		{
			//UserDBData psUserDBData = pcUser.userDbData;
			//pcUser.GetTaskMgr().PackTaskData( psUserDBData.szTaskData, psUserDBData.isTaskRush );//存数据库时增加任务数据
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