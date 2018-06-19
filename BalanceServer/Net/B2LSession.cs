using Core.Misc;
using Google.Protobuf;
using Shared.Net;
using System.Collections.Generic;

namespace BalanceServer.Net
{
	public class B2LSession : CliSession
	{
		protected B2LSession( uint id ) : base( id )
		{
			this._msgHandler.Register( ( int )LSToBS.MsgID.EMsgToBsfromLsAskRegisterRet, this.MsgInitHandler );
			this._msgHandler.Register( ( int )LSToBS.MsgID.EMsgToBsfromLsOneClinetLoginCheckRet, this.MsgUserLogin );
		}

		protected override void SendInitData()
		{
			Logger.Info( "LS Connected, try to register me." );
			BSToLS.AskRegister msg = new BSToLS.AskRegister();
			this.owner.SendMsgToSession( this.id, msg, ( int )BSToLS.MsgID.EMsgToLsfromBsAskRegister );
		}

		protected override void OnRealEstablish()
		{
			Logger.Info( "LS Connected and register ok" );
		}

		protected override void OnClose()
		{
			Logger.Info( "LS DisConnect." );
		}

		private bool MsgInitHandler( byte[] data, int offset, int size, int msgid )
		{
			this.SetInited( true, true );
			return true;
		}

		private bool MsgUserLogin( byte[] data, int offset, int size, int msgid )
		{
			GCToBS.OneClinetLogin userLoginInfo = new GCToBS.OneClinetLogin();
			userLoginInfo.MergeFrom( data, offset, size );

			// 发送第3消息：用户验证是否成功，如果验证成功，请求分配gs给用户
			BSToGC.ClinetLoginCheckRet msg = new BSToGC.ClinetLoginCheckRet();
			msg.LoginSuccess = userLoginInfo.LoginSuccess;
			this.owner.SendMsgToSession( userLoginInfo.Nsid, msg, ( int )BSToGC.MsgID.EMsgToGcfromBsOneClinetLoginCheckRet );

			Logger.Log( $"user({userLoginInfo.Uin})({userLoginInfo.Sessionid})({userLoginInfo.Nsid}) ask login ret" );

			if ( userLoginInfo.LoginSuccess == 0 )
				this.owner.DisconnectOne( userLoginInfo.Nsid );
			else
			{
				OneGsInfo littleOne = null;
				foreach ( KeyValuePair<uint, OneGsInfo> kv in BS.instance.bsConfig.allGsInfo )
				{
					OneGsInfo theGsInfo = kv.Value;
					if ( theGsInfo.gs_isLost )
						continue;
					if ( littleOne == null || theGsInfo.gs_gc_count < littleOne.gs_gc_count )
						littleOne = theGsInfo;
				}

				if ( littleOne == null )
					return false;
				++littleOne.gs_gc_count;//only ++ as cache.

				string guid = GuidHash.GetString();
				BSToGS.OneUserLoginToken sOneUserLoginToken =
					new BSToGS.OneUserLoginToken
					{
						Gateclient = ( int )userLoginInfo.Nsid,
						Token = guid,
						UserName = userLoginInfo.Uin,
						Ip = littleOne.gs_IpExport,
						Port = littleOne.gs_Port
					};
				this.owner.SendMsgToSession( littleOne.gs_nets, sOneUserLoginToken, ( int )BSToGS.MsgID.EMsgToGsfromBsOneUserLoginToken );
			}

			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			return true;
		}
	}
}