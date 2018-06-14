using Core;
using Core.Net;
using Google.Protobuf;
using Shared;
using Shared.Net;
using System.Collections.Generic;
using System.Net;

namespace GateServer.Net
{
	public class GCMsgManager : MsgManager
	{
		private delegate EResult MsgHandler( uint nsID, byte[] data, int offset, int size, int msgID );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public GCMsgManager()
		{
			#region 注册消息处理函数
			this._handlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin] = this.OnMsgToGstoCsfromGcAskLogin;
			this._handlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskReconnectGame] = this.OnMsgToGstoCsfromGcAskReconnectGame;
			this._handlers[( int )GCToSS.MsgNum.EMsgToGstoSsfromGcAskPingSs] = this.OnMsgToGstoSsfromGcAskPingSs;
			#endregion
		}

		private EResult OnMsgToGstoCsfromGcAskLogin( uint nsID, byte[] data, int offset, int size, int msgID )
		{
			bool bLogMsgFlag = false;
			GCToCS.Login ploginMsg = new GCToCS.Login();

			GCToCS.Login pLogin = new GCToCS.Login();
			pLogin.MergeFrom( data, offset, size );

			//验证token
			if ( !GSKernel.instance.userTokenMgr.IsUserCanLogin( pLogin.Name, pLogin.Passwd, nsID ) )
			{
				Logger.Error( $"user {pLogin.Name} can't login with token {pLogin.Passwd}" );
				GSToGC.NetClash sMsg = new GSToGC.NetClash();
				this.PostToGameClient( nsID, sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
				this.PostGameClientDisconnect( nsID );
				return EResult.Normal;
			}
			//获取IP
			INetSession pClient = GSKernel.instance.netSessionMrg.GetSession( nsID );
			if ( null != pClient )
			{
				EndPoint endPoint = pClient.connection.socket.RemoteEndPoint;
				if ( endPoint == null )
				{
					Logger.Error( $"user {pLogin.Name} can't login with IP is null" );
					this.PostGameClientDisconnect( nsID );
					return EResult.Normal;
				}
				bLogMsgFlag = true;
				ploginMsg.Platform = pLogin.Platform;
				ploginMsg.Sdk = pLogin.Sdk;
				ploginMsg.Name = pLogin.Name;
				ploginMsg.Passwd = pLogin.Passwd;
				ploginMsg.Equimentid = pLogin.Equimentid;
				ploginMsg.Ipaddress = endPoint.ToString();
				Logger.Log( $"client({nsID}) ask login({ploginMsg.Name})({ploginMsg.Passwd})" );
			}
			this.TransToCS( nsID, data, offset, size, msgID, bLogMsgFlag, ploginMsg );
			return EResult.Normal;
		}

		private EResult OnMsgToGstoCsfromGcAskReconnectGame( uint nsID, byte[] data, int offset, int size, int msgID )
		{
			GCToCS.ReconnectToGame pReconnectToGame = new GCToCS.ReconnectToGame();
			pReconnectToGame.MergeFrom( data, offset, size );
			//验证token
			if ( !GSKernel.instance.userTokenMgr.IsUserCanLogin( pReconnectToGame.Name, pReconnectToGame.Passwd, nsID ) )
			{
				Logger.Error( $"user {pReconnectToGame.Name} can't login with token {pReconnectToGame.Passwd}" );
				GSToGC.NetClash sMsg = new GSToGC.NetClash();
				this.PostToGameClient( nsID, sMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
				this.PostGameClientDisconnect( nsID );
			}
			return EResult.Normal;
		}

		private EResult OnMsgToGstoSsfromGcAskPingSs( uint nsID, byte[] data, int offset, int size, int msgID )
		{
			GCToSS.AskPingSS msgPing = new GCToSS.AskPingSS();
			msgPing.MergeFrom( data, offset, size );
			GSToGC.PingRet retMsg = new GSToGC.PingRet
			{
				Time = msgPing.Time,
				Flag = 1
			};
			this.PostToGameClient( nsID, retMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsGcaskPingRet );
			return EResult.Normal;
		}

		private void TransToCS( uint nsID, byte[] data, int offset, int size, int msgID, bool bLogMsgFlag, GCToCS.Login ploginMsg )
		{
			//确保已经连接到中心服务器
			if ( GSKernel.instance.csNetSessionId > 0 )
			{
				if ( bLogMsgFlag )
				{
					byte[] data2 = ploginMsg.ToByteArray();
					GSKernel.instance.netSessionMrg.TranMsgToSession( SessionType.ClientG2C, data2, 0, data2.Length, msgID, ( int )GSToCS.MsgID.EMsgToCsfromGsReportGcmsg, nsID );
				}
				else
					GSKernel.instance.netSessionMrg.TranMsgToSession( SessionType.ClientG2C, data, offset, size, msgID, ( int )GSToCS.MsgID.EMsgToCsfromGsReportGcmsg, nsID );
			}
			else
				Logger.Warn( $"invalid CSNetSessionId:{GSKernel.instance.csNetSessionId}" );
		}

		public ErrorCode HandleUnhandledMsg( uint nsID, byte[] data, int offset, int size, int msgID )
		{
			if ( this._handlers.TryGetValue( msgID, out MsgHandler handler ) )
				handler( nsID, data, offset, size, msgID );
			else
			{
				if ( msgID > ( int )GCToSS.MsgNum.EMsgToGstoSsfromGcBegin && msgID < ( int )GCToSS.MsgNum.EMsgToGstoSsfromGcEnd )
				{
					GSSSInfo ssInfo = GSKernel.instance.csMsgManager.GetSSInfo( nsID );
					if ( ssInfo == null )
						Logger.Error( $"nsid:{nsID} send msg:{msgID} error, can't get ssinfo." );
					else
					{
						if ( ssInfo.nsID > 0 )
							GSKernel.instance.netSessionMrg.TranMsgToSession( ssInfo.nsID, data, offset, size, msgID,
																			  ( int )GSToSS.MsgID.EMsgToSsfromGsReportGcmsg, nsID );
						else
							Logger.Error( $"invalid ssID:{ssInfo.nsID}!!" );
					}
				}
				else
				{
					Logger.Error( $"unknown msg with Protocal id:{msgID}, offset:{offset}, size:{size}, nsID:{nsID}" );
					return ErrorCode.EC_InvalidMsgProtocalID;
				}
			}
			return ErrorCode.EC_Success;
		}
	}
}