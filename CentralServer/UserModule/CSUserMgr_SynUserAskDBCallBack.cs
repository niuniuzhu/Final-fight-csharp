using CentralServer.Tools;
using CentralServer.User;
using Core.Misc;
using Core.Net;
using Shared;
using System.Collections.Generic;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		private void SynUserAskDBCallBack()
		{
			//this._dbCallbackQueue.Switch();
			//while ( !this._dbCallbackQueue.isEmpty )
			//{
			//	StreamBuffer pBuffer = this._dbCallbackQueue.Pop();
			//	switch ( pBuffer.m_LogLevel )
			//	{
			//		case DBToCS.eQueryUser_DBCallBack:
			//			SynHandleQueryUserCallback( pBuffer );
			//			break;
			//		case DBToCS.eQueryAllAccount_CallBack:
			//			SynHandleAllAccountCallback( pBuffer );
			//			break;
			//		case DBToCS.eMail_CallBack:
			//			SynHandleMailCallback( pBuffer );
			//			break;
			//		case DBToCS.eQueryNotice_CallBack:
			//			DBCallBack_QueryNotice( pBuffer );
			//			break;
			//		default:
			//			Logger.Warn( "not hv handler:%d", pBuffer.m_LogLevel );
			//			break;
			//	}
			//	this._dbCallbackQueuePool.Push( pBuffer );
			//}
		}

		//private void DBAsyn_QueryWhenThreadBegin()
		//{
		//	IDBConnector* piGameDBConnector = _userCacheDBActiveWrapper.GetDBConnector();
		//	if ( null == piGameDBConnector )
		//	{
		//		Logger.Error( " can not connect db!" );
		//		return;
		//	}

		//	ErrorCode errorCode = piGameDBConnector.ExecQuery( "select MAX(mail_id) as mailid   from game_mail ;" );
		//	if ( errorCode != ErrorCode.Success )
		//	{
		//		Logger.Error( "--errorCode:%d,errorStr:%s", piGameDBConnector.GetErrorCode(), piGameDBConnector.GetErrorStr() );
		//		return;
		//	}
		//	int tValue = 0;
		//	piGameDBConnector.GetQueryFieldData( "mailid", tValue );
		//	//服务器启动的时候 没有登录的玩家，可以这样设置
		//	m_MailMgr.setCurtMaxMailIdx( tValue );
		//	piGameDBConnector.CloseQuery();

		//	if ( tValue > 0 )
		//	{
		//		DBAsyn_QueryGameMailList( piGameDBConnector, 0 );
		//	}

		//	errorCode = piGameDBConnector.ExecQuery( "select * from notice;" );
		//	if ( errorCode != ErrorCode.Success )
		//	{
		//		Logger.Warn( "--errorCode:%d,errorStr:%s", piGameDBConnector.GetErrorCode(), piGameDBConnector.GetErrorStr() );
		//		return;
		//	}
		//	tValue = piGameDBConnector.GetQueryFieldNum();

		//	piGameDBConnector.CloseQuery();
		//	if ( tValue > 0 )
		//	{
		//		DBAsynQueryNoticeCallBack( piGameDBConnector );
		//	}
		//}

		//private void CDKThreadBeginCallback()
		//{
		//	/*MysqlDWrapper.*/
		//	IDBConnector* piGameDBConnector = _cdkeyWrapper.GetDBConnector();
		//	if ( null == piGameDBConnector )
		//	{
		//		Logger.Warn( " can not connect db!",  );
		//		return;
		//	}

		//	stringstream sqlStr;
		//	sqlStr << "select id,sdk_id,cdkey,user_name from account_user where cs_id=" << GetCSKernelInstance().GetCSID() << ";";
		//	int n32QueryRet = piGameDBConnector.ExecQuery( sqlStr.str() );
		//	if ( n32QueryRet != ErrorCode.Success )
		//	{
		//		Logger.Warn( "--errorCode:%d,errorStr:%s", piGameDBConnector.GetErrorCode(), piGameDBConnector.GetErrorStr() );
		//		return;
		//	}

		//	int row = piGameDBConnector.GetQueryRowNum();
		//	DBToCS.QueryAllAccount sQueryAllAccount;
		//	for ( int i = 0; i < row; ++i )
		//	{
		//		auto pAccount = sQueryAllAccount.add_account();

		//		long id = 0;
		//		piGameDBConnector.GetQueryFieldData( "id", id );
		//		pAccount.set_guid( id );

		//		string username;
		//		piGameDBConnector.GetQueryFieldData( "cdkey", username );
		//		pAccount.set_user_name( username );

		//		string name;
		//		piGameDBConnector.GetQueryFieldData( "user_name", name );
		//		pAccount.set_nickname( name );

		//		int sdkid = 0;
		//		piGameDBConnector.GetQueryFieldData( "sdk_id", sdkid );
		//		pAccount.set_sdkid( sdkid );

		//		piGameDBConnector.QueryNext();
		//	}
		//	if ( row > 0 )
		//	{
		//		GetCSUserMgrInstance().EncodeAndSendToLogicThread( sQueryAllAccount, sQueryAllAccount.mgsid() );
		//	}
		//}

		//private ErrorCode SynHandleQueryUserCallback( StreamBuffer pBuffer )
		//{
		//	DBToCS.QueryUser pMsg = ParseProtoMsgInThread<DBToCS.QueryUser>( pBuffer.GetDataHeader(), pBuffer.GetDataLength() );
		//	if ( !pMsg )
		//	{
		//		Logger.Error( "" );
		//		return 0;
		//	}
		//	GCToCS.Login pLogin = ParseProtoMsgInThread<GCToCS.Login>( pMsg.login().c_str(), pMsg.login().size() );
		//	if ( !pLogin )
		//	{
		//		Logger.Error( "" );
		//		return 0;
		//	}

		//	UserNetInfo netinfo = new UserNetInfo( pMsg.Gsid, ( uint )pMsg.Gcnetid );
		//	if ( this.ContainsUser( netinfo ) )
		//	{
		//		Logger.Warn( "" );
		//		return eEC_InvalidNetState;
		//	}

		//	UserDBData sUserDBData;
		//	memcpy( &sUserDBData, pMsg.db().c_str(), pMsg.db().size() );
		//	CSUser pcUser = this.GetUser( sUserDBData.usrDBData.un64ObjIdx );
		//	if ( null != pcUser )
		//	{
		//		pcUser.OnOnline( netinfo, pLogin, false, false );
		//		return ErrorCode.Success;
		//	}

		//	pcUser = new CSUser();

		//	sUserDBData.szUserName = pLogin.Name;
		//	sUserDBData.szUserPwd = pLogin.Passwd;
		//	sUserDBData.szNickName = pMsg.Nickname;
		//	sUserDBData.usrDBData.userPlatform = ( UserPlatform )pLogin.Sdk;
		//	sUserDBData.szTaskData = pMsg.TaskData;

		//	const bool bNewUser = sUserDBData.usrDBData.tRegisteUTCMillisec < 1;

		//	pcUser.LoadDBData( sUserDBData );
		//	pcUser.GetUserDBData().mGuideSteps.szCSContinueGuide = pMsg.guidestr();

		//	for ( int i = 0; i < pMsg.rsinfo_size(); i++ )
		//	{
		//		pcUser.LoadUserSNSList( pMsg.rsinfo( i ) );
		//	}

		//	for ( int i = 0; i < pMsg.item_info_size(); i++ )
		//	{
		//		pcUser.AddUserItems( *pMsg.mutable_item_info( i ) );
		//	}

		//	for ( int i = 0; i < pMsg.mail_info_size(); ++i )
		//	{
		//		m_MailMgr.updatePerMailList( pMsg.mail_info( i ).mailid(), sUserDBData.sPODUsrDBData.un64ObjIdx, ( EMailCurtState )pMsg.mail_info( i ).state() );
		//	}

		//	pcUser.GetUserBattleInfoEx().mDebugName = pLogin.name();

		//	if ( !bNewUser )
		//	{
		//		long curTime = TimeUtils.utcTime;
		//		for ( int i = 0; i < pMsg.herocfg_size(); ++i )
		//		{
		//			const DBToCS.HeroCfg&pHeroCfg = pMsg.herocfg( i );
		//			if ( pHeroCfg.expiredtime() != PersistTimeAlways && pHeroCfg.expiredtime() < curTime )
		//			{
		//				continue;
		//			}

		//			StoreUserdata.Types.SUserHeroDBData sSUserHeroDBData;
		//			sSUserHeroDBData.buyTime = pHeroCfg.buytime();
		//			sSUserHeroDBData.endTime = pHeroCfg.expiredtime();
		//			sSUserHeroDBData.un32HeroID = pHeroCfg.commodityid();
		//			pcUser.AddHero( sSUserHeroDBData );
		//		}

		//		for ( int i = 0; i < pMsg.runeinfo_size(); ++i )
		//		{
		//			const DBToCS.RuneInfo&sRuneInfo = pMsg.runeinfo( i );
		//			pcUser.InitRunes( sRuneInfo.bagstr(), sRuneInfo.slotstr() );
		//		}
		//	}

		//	ErrorCode errorCode = this.AddUser( pcUser );
		//	if ( errorCode != ErrorCode.Success )
		//		return errorCode;

		//	pcUser.OnOnline( netinfo, pLogin, bNewUser, true );
		//	//todo
		//	//if ( bNewUser )
		//	//{
		//	//	stringstream mystream;
		//	//	mystream << pLogin.name() << LOG_SIGN << pLogin.sdk() << LOG_SIGN;
		//	//	mystream << pLogin.platform() << LOG_SIGN << pLogin.equimentid() << LOG_SIGN;
		//	//	mystream << pLogin.ipaddress();
		//	//	CSSGameLogMgr.GetInstance().AddGameLog( eLog_Register, pcUser.GetUserDBData().sPODUsrDBData.un64ObjIdx, mystream.str() );
		//	//}
		//	return ErrorCode.Success;
		//}

		//private ErrorCode DBPosterUpdateUser( CSUser pcUser )
		//{
		//	UserDBData psUserDBData = pcUser.userDbData;
		//	pcUser.GetTaskMgr().PackTaskData( psUserDBData.szTaskData, psUserDBData.isTaskRush );//存数据库时增加任务数据
		//	CCSUserDbDataMgr.UpdateUserDbData( psUserDBData, m_SaveUserStream );
		//	if ( !m_SaveUserStream.str().empty() )
		//	{
		//		CSToDB.UpdateUser sUpdateUser = new CSToDB.UpdateUser();
		//		sUpdateUser.Guid = ( long )psUserDBData.usrDBData.un64ObjIdx;
		//		sUpdateUser.Sqlstr = ( m_SaveUserStream.str() );
		//		_userCacheDBActiveWrapper.EncodeAndSendToDBThread( sUpdateUser, CSToDB.MsgID.EUpdateUserDbcallBack );
		//	}
		//	return ErrorCode.Success;
		//}

		//private void PostSaveCmd()
		//{
		//	Logger.Info( "start post save data to db...." );
		//	foreach ( KeyValuePair<ulong, CSUser> kv in this._userGUIDMap )
		//		this.DBPosterUpdateUser( kv.Value );
		//	Logger.Error( "only finish post save data to db, don't close me at once." );
		//}

		private void DBAsynQueryWhenThreadBegin()
		{
		}

		private void CDKThreadBeginCallback()
		{
		}
	}
}