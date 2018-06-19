using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;
using System.Collections.Generic;

namespace LoginServer.Net
{
	public class BalanceSession : SrvCliSession
	{
		private readonly List<OneBsInfo> m_BS_List = new List<OneBsInfo>();

		protected BalanceSession( uint id ) : base( id )
		{
			this._msgHandler.Register( ( int )BSToLS.MsgID.EMsgToLsfromBsAskRegister, this.MsgInitHandler );
			this._msgHandler.Register( ( int )BSToLS.MsgID.EMsgToLsfromBcOneClinetLoginCheck, this.MsgHandleOneClientLoginCheck );
		}

		protected override void SendInitData()
		{
			LSToBS.AskRegisterRet sMsg = new LSToBS.AskRegisterRet();
			this.owner.SendMsgToSession( this.id, sMsg, ( int )LSToBS.MsgID.EMsgToBsfromLsAskRegisterRet );
		}

		protected override void OnRealEstablish()
		{
			Logger.Info( $"BS({this.logicID}) Connected." );
		}

		protected override void OnClose()
		{
			Logger.Info( $"BS({this.logicID}) DisConnected." );
		}

		private bool MsgInitHandler( byte[] data, int offset, int size, int msgid )
		{
			this.SetInited( true, true );
			return true;
		}

		private bool MsgHandleOneClientLoginCheck( byte[] data, int offset, int size, int msgid )
		{
			GCToBS.OneClinetLogin sOneClientLogin = new GCToBS.OneClinetLogin();
			sOneClientLogin.MergeFrom( data, offset, size );

			string sessionid = string.Empty;
			if ( sOneClientLogin.Plat == ( uint )EUserPlatform.ePlatform_PC )
			{
				sessionid += sOneClientLogin.Plat;
				sessionid += sOneClientLogin.Uin;
			}
			else
				sessionid = sOneClientLogin.Sessionid;

			LoginUserInfo loginUserInfo = LS.instance.sdkConnector.GetLoginUserInfo( sessionid );
			if ( loginUserInfo != null )
			{
				sOneClientLogin.LoginSuccess = 1;
				sOneClientLogin.Uin = loginUserInfo.uin;
				LS.instance.sdkConnector.RemoveLoginUserInfo( sessionid );
				Logger.Log( $"Erase uid:{sOneClientLogin.Uin}" );
			}
			else
			{
				Logger.Warn( $"Fail With User with uin({sOneClientLogin.Uin}) Not Find." );
				sOneClientLogin.LoginSuccess = 0;
			}

			this.owner.SendMsgToSession( this.id, sOneClientLogin, ( int )LSToBS.MsgID.EMsgToBsfromLsOneClinetLoginCheckRet );

			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			return true;
		}
	}
}