using CentralServer.Tools;
using CentralServer.User;
using Core.Misc;
using Google.Protobuf;
using Shared;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		private static ErrorCode PostMsgToGCAskReturn( UserNetInfo crsUserNetInfo, int n32AskProtocalID, int n32RetFlag )
		{
			GSToGC.AskRet sMsg = new GSToGC.AskRet
			{
				Askid = n32AskProtocalID,
				Errorcode = n32RetFlag
			};
			return CS.instance.PostMsgToGC( crsUserNetInfo, sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsGcaskRet );
		}

		private static ErrorCode PostMsgToGCAskReturn( CSGSInfo csgsInfo, uint gcNetID, int askProtocalID, ErrorCode errorCode )
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
				PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin, errorCode );
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgToGstoCsfromGcAskReconnectGame( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			GCToCS.ReconnectToGame pReconnectToGame = new GCToCS.ReconnectToGame();
			pReconnectToGame.MergeFrom( data, offset, size );

			ErrorCode errorCode = this.UserAskReconnectGame( csgsInfo, gcNetID, pReconnectToGame.Name, pReconnectToGame.Passwd );
			if ( ErrorCode.Success != errorCode )
				PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskReconnectGame, errorCode );
			return errorCode;
		}

		private ErrorCode OnMsgToGstoCsfromGcAskComleteUserInfo( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			CSUser csUser = this.GetUser( csgsInfo, gcNetID );
			if ( null == csUser )
			{
				Logger.Error( $"user net({csgsInfo.m_n32NSID}) can't find" );
				return ErrorCode.NullUser;
			}

			csUser.ResetPingTimer();
			ErrorCode errorCode = ErrorCode.Success;
			do
			{
				GCToCS.CompleteInfo pCompleteInfo = new GCToCS.CompleteInfo();
				pCompleteInfo.MergeFrom( data, offset, size );

				if ( string.IsNullOrEmpty( pCompleteInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameNotAllowed;
					break;
				}

				if ( pCompleteInfo.Nickname.Length > Consts.DEFAULT_NICK_NAME_LEN )
				{
					errorCode = ErrorCode.NickNameNotAllowed;
					break;
				}

				if ( CS.instance.csCfg.CheckInvalidWorld( pCompleteInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameNotAllowed;
					break;
				}

				if ( CS.instance.csCfg.CheckAIRobotName( pCompleteInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameCollision;
					break;
				}

				if ( this._allNickNameSet.Contains( pCompleteInfo.Nickname ) )
				{
					errorCode = ErrorCode.NickNameCollision;
					break;
				}

				csUser.userDbData.ChangeUserDbData( UserDBDataType.UserDBType_Sex, pCompleteInfo.Sex );
				csUser.userDbData.ChangeUserDbData( UserDBDataType.UserDBType_HeaderId, pCompleteInfo.Headid );

				this.ChangeUserNickName( csUser, pCompleteInfo.Nickname );

				csUser.SynUser_UserBaseInfo();
				csUser.PostCSNotice();

				//todo
				//DBPoster_UpdateUser( csUser );//存盘// 

				string log = $"{csUser.userDbData.szUserName}{LOG_SIGN}{pCompleteInfo.Headid}{pCompleteInfo.Nickname}";
				//todo
				//CSSGameLogMgr::GetInstance().AddGameLog( eLog_HeadUse, mystream.str(), 0 );
			} while ( false );

			if ( ErrorCode.Success != errorCode )
				PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskComleteUserInfo, errorCode );

			return errorCode;
		}
	}
}