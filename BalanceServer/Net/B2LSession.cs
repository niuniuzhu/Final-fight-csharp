using Core.Misc;
using Google.Protobuf;
using Shared;
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

		private ErrorCode MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			this.SetInited( true, true );
			return ErrorCode.Success;
		}

		/// <summary>
		/// LS回应BS的检查登陆合法性
		/// </summary>
		private ErrorCode MsgUserLogin( byte[] data, int offset, int size, int msgID )
		{
			GCToBS.OneClinetLogin userLoginInfo = new GCToBS.OneClinetLogin();
			userLoginInfo.MergeFrom( data, offset, size );

			//发送第3消息：验证登陆是否成功，如果成功，请求分配GS给客户端
			BSToGC.ClinetLoginCheckRet msg = new BSToGC.ClinetLoginCheckRet();
			msg.LoginSuccess = userLoginInfo.LoginSuccess;
			this.owner.SendMsgToSession( userLoginInfo.Nsid, msg, ( int )BSToGC.MsgID.EMsgToGcfromBsOneClinetLoginCheckRet );

			Logger.Log( $"user({userLoginInfo.Uin})({userLoginInfo.Sessionid})({userLoginInfo.Nsid}) ask login ret" );

			if ( userLoginInfo.LoginSuccess == 0 )
				this.owner.DisconnectOne( userLoginInfo.Nsid );
			else
			{
				//找到最空闲的网关服务器
				OneGsInfo littleOne = null;
				foreach ( KeyValuePair<int, OneGsInfo> kv in BS.instance.bsConfig.allGsInfo )
				{
					OneGsInfo theGsInfo = kv.Value;
					if ( theGsInfo.gs_isLost )
						continue;
					if ( littleOne == null || theGsInfo.gs_gc_count < littleOne.gs_gc_count )
						littleOne = theGsInfo;
				}

				if ( littleOne == null )
					return ErrorCode.GSNotFound;

				++littleOne.gs_gc_count;//仅仅作为缓存,GS会定时汇报服务器的状态

				//这条消息的路由:BS-GS-BS-GC
				BSToGS.OneUserLoginToken oneUserLoginToken = new BSToGS.OneUserLoginToken
				{
					Gateclient = ( int )userLoginInfo.Nsid,
					Token = GuidHash.GetString(),
					UserName = userLoginInfo.Uin,
					//这里ip和port最终会到达GC
					Ip = littleOne.gs_IpExport,
					Port = littleOne.gs_Port
				};
				//通知网关服务器有客户端登陆
				this.owner.SendMsgToSession( littleOne.gs_nets, oneUserLoginToken, ( int )BSToGS.MsgID.EMsgToGsfromBsOneUserLoginToken );
			}
			return ErrorCode.Success;
		}
	}
}