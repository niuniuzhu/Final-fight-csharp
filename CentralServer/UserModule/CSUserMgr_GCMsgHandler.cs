using CentralServer.Tools;
using CentralServer.User;
using Core.Misc;
using Google.Protobuf;
using Shared;
using System.Collections.Generic;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		public ErrorCode PostMsgToGCAskReturn( UserNetInfo crsUserNetInfo, int n32AskProtocalID, ErrorCode errorCode )
		{
			GSToGC.AskRet sMsg = new GSToGC.AskRet
			{
				Askid = n32AskProtocalID,
				Errorcode = ( int )errorCode
			};
			return CS.instance.PostMsgToGC( crsUserNetInfo, sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsGcaskRet );
		}

		public ErrorCode PostMsgToGCAskReturn( CSGSInfo csgsInfo, uint gcNetID, int askProtocalID, ErrorCode errorCode )
		{
			GSToGC.AskRet msg = new GSToGC.AskRet
			{
				Askid = askProtocalID,
				Errorcode = ( int )errorCode
			};
			return CS.instance.PostMsgToGS( csgsInfo, msg, ( int )GSToGC.MsgID.EMsgToGcfromGsGcaskRet, gcNetID );
		}

		private ErrorCode OnMsgToGstoCsfromGcAskLogin( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			GCToCS.Login login = new GCToCS.Login();
			login.MergeFrom( data, offset, size );
			Logger.Log( $"--new login({login.Name})--" );
			ErrorCode errorCode = this.UserAskLogin( csgsInfo, gcNetID, login );
			if ( ErrorCode.Success != errorCode )
				this.PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin, errorCode );
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgToGstoCsfromGcAskReconnectGame( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			GCToCS.ReconnectToGame pReconnectToGame = new GCToCS.ReconnectToGame();
			pReconnectToGame.MergeFrom( data, offset, size );

			ErrorCode errorCode = this.UserAskReconnectGame( csgsInfo, gcNetID, pReconnectToGame.Name, pReconnectToGame.Passwd );
			if ( ErrorCode.Success != errorCode )
				this.PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskReconnectGame, errorCode );
			return errorCode;
		}

		private ErrorCode OnMsgToGstoCsfromGcAskComleteUserInfo( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			CSUser csUser = this.GetUser( csgsInfo, gcNetID );
			if ( null == csUser )
			{
				Logger.Error( $"could not find user({csgsInfo.m_n32NSID})" );
				return ErrorCode.NullUser;
			}

			csUser.ResetPingTimer();
			ErrorCode errorCode = ErrorCode.Success;
			do
			{
				GCToCS.CompleteInfo completeInfo = new GCToCS.CompleteInfo();
				completeInfo.MergeFrom( data, offset, size );

				if ( string.IsNullOrEmpty( completeInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameNotAllowed;
					break;
				}
				//长度是否合法
				if ( completeInfo.Nickname.Length > Consts.DEFAULT_NICK_NAME_LEN )
				{
					errorCode = ErrorCode.NickNameNotAllowed;
					break;
				}
				//昵称是否合法
				if ( CS.instance.csCfg.CheckInvalidWorld( completeInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameNotAllowed;
					break;
				}
				//是否和ai昵称冲突
				if ( CS.instance.csCfg.CheckAIRobotName( completeInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameCollision;
					break;
				}
				//是否存在相同昵称
				if ( this._allNickNameSet.Contains( completeInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameCollision;
					break;
				}

				csUser.userDbData.ChangeUserDbData( UserDBDataType.Sex, completeInfo.Sex );
				csUser.userDbData.ChangeUserDbData( UserDBDataType.HeaderId, completeInfo.Headid );
				this.ChangeUserNickName( csUser, completeInfo.Nickname );

				csUser.SynUser_UserBaseInfo();
				csUser.PostCSNotice();

				//todo
				//DBPosterUpdateUser( csUser );//存盘// 

				string log = $"{csUser.userDbData.szUserName}{LOG_SIGN}{completeInfo.Headid}{completeInfo.Nickname}";
				//todo
				//CSSGameLogMgr::GetInstance().AddGameLog( eLog_HeadUse, mystream.str(), 0 );
			} while ( false );

			if ( ErrorCode.Success != errorCode )
				this.PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskComleteUserInfo, errorCode );

			return errorCode;
		}

		private ErrorCode OnMsgToGstoCsfromGcAskChangeNickName( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			CSUser user = this.CheckAndGetUserByNetInfo( csgsInfo, gcNetID );
			GCToCS.ChangeNickName pMsg = new GCToCS.ChangeNickName();
			pMsg.MergeFrom( data, offset, size );
			ErrorCode errorCode = ErrorCode.Success;
			do
			{
				if ( pMsg.Newnickname.Length < 3 )
					errorCode = ErrorCode.NickNameTooShort;

				if ( this._allNickNameSet.Contains( pMsg.Newnickname ) )
				{
					errorCode = ErrorCode.NickNameCollision;
					break;
				}
				if ( !user.CheckIfEnoughPay( PayType.Diamond, 20 ) )
				{
					errorCode = ErrorCode.DiamondNotEnough;
					break;
				}
			} while ( false );

			if ( errorCode != ErrorCode.Success )
				return user.PostMsgToGCAskRetMsg( ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskChangeNickName, errorCode );

			user.userDbData.ChangeUserDbData( UserDBDataType.Diamond, -20 );
			user.PostMsgToGCNotifyNewNickname( user.guid, pMsg.Newnickname );

			this.ChangeUserNickName( user, pMsg.Newnickname );

			foreach ( KeyValuePair<ulong, UserRelationshipInfo> kv in user.userDbData.friendListMap )
			{
				CSUser piUser = this.GetUser( kv.Value.guididx );
				if ( null != piUser && piUser.userPlayingStatus == UserPlayingStatus.Playing )
					piUser.SynUserSNSList( user.guid, RelationShip.Friends );
			}

			user.SynCurDiamond();
			//todo
			//CSSGameLogMgr::GetInstance().AddGameLog( eLog_ChangeUseName, user.GetGUID(), user.GetUserDBData().sPODUsrDBData.un8UserLv, user.GetUserDBData().sPODUsrDBData.un16VipLv );
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgToGstoCsfromGcAskChangeheaderId( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			CSUser pcUser = this.CheckAndGetUserByNetInfo( csgsInfo, gcNetID );
			GCToCS.AskChangeheaderId pMsg = new GCToCS.AskChangeheaderId();
			pMsg.MergeFrom( data, offset, size );
			pcUser.AskChangeHeaderId( pMsg.Newheaderid );
			return ErrorCode.Success;
		}
	}
}