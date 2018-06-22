using Core.Misc;
using Core.Net;
using Google.Protobuf;
using Shared;
using Shared.Net;
using System.Collections.Generic;
using System.Net;

namespace GateServer.Net
{
	public class GCMsgManager
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

		/// <summary>
		/// 客户端请求登录网关服务器
		/// </summary>
		private EResult OnMsgToGstoCsfromGcAskLogin( uint nsID, byte[] data, int offset, int size, int msgID )
		{
			bool logMsgFlag = false;
			GCToCS.Login loginMsg = new GCToCS.Login();

			GCToCS.Login login = new GCToCS.Login();
			login.MergeFrom( data, offset, size );

			//验证token
			//正常登录流程是连接到登录服务器,再通过负载均衡服务器把消息转发给合适的网关服务器
			//如果客户端绕过上述过程直接连接网关服务器并请求登录,则是非法操作
			if ( !GS.instance.gsStorage.IsUserCanLogin( login.Name, login.Passwd, nsID ) )
			{
				Logger.Error( $"user {login.Name} can't login with token {login.Passwd}" );
				GSToGC.NetClash msg = new GSToGC.NetClash();
				//断开连接
				GS.instance.PostToGameClient( nsID, msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
				GS.instance.PostGameClientDisconnect( nsID );
				return EResult.Normal;
			}
			//获取IP
			INetSession pClient = GS.instance.GetSession( nsID );
			if ( null != pClient )
			{
				EndPoint endPoint = pClient.connection.remoteEndPoint;
				if ( endPoint == null )
				{
					Logger.Error( $"user {login.Name} can't login with IP is null" );
					GS.instance.PostGameClientDisconnect( nsID );
					return EResult.Normal;
				}
				logMsgFlag = true;
				loginMsg.Platform = login.Platform;
				loginMsg.Sdk = login.Sdk;
				loginMsg.Name = login.Name;
				loginMsg.Passwd = login.Passwd;
				loginMsg.Equimentid = login.Equimentid;
				loginMsg.Ipaddress = endPoint.ToString();
				Logger.Log( $"client({nsID}) ask login({loginMsg.Name})({loginMsg.Passwd})" );
			}
			//把登录信息转发到中心服务器
			this.TransToCS( nsID, data, offset, size, msgID, logMsgFlag, loginMsg );
			return EResult.Normal;
		}

		/// <summary>
		/// 客户端请求重新登录
		/// </summary>
		private EResult OnMsgToGstoCsfromGcAskReconnectGame( uint nsID, byte[] data, int offset, int size, int msgID )
		{
			GCToCS.ReconnectToGame reconnectToGame = new GCToCS.ReconnectToGame();
			reconnectToGame.MergeFrom( data, offset, size );
			//验证token
			if ( !GS.instance.gsStorage.IsUserCanLogin( reconnectToGame.Name, reconnectToGame.Passwd, nsID ) )
			{
				Logger.Error( $"user {reconnectToGame.Name} can't login with token {reconnectToGame.Passwd}" );
				GSToGC.NetClash msg = new GSToGC.NetClash();
				GS.instance.PostToGameClient( nsID, msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
				GS.instance.PostGameClientDisconnect( nsID );
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
			GS.instance.PostToGameClient( nsID, retMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsGcaskPingRet );
			return EResult.Normal;
		}

		private void TransToCS( uint nsID, byte[] data, int offset, int size, int msgID, bool logMsgFlag, GCToCS.Login loginMsg )
		{
			//确保已经连接到中心服务器
			if ( GS.instance.gsStorage.csNetSessionId > 0 )
			{
				if ( logMsgFlag )
				{
					byte[] data2 = loginMsg.ToByteArray();
					GS.instance.TranMsgToSession( SessionType.ClientG2C, data2, 0, data2.Length, msgID, ( int )GSToCS.MsgID.EMsgToCsfromGsReportGcmsg, nsID );
				}
				else
					GS.instance.TranMsgToSession( SessionType.ClientG2C, data, offset, size, msgID, ( int )GSToCS.MsgID.EMsgToCsfromGsReportGcmsg, nsID );
			}
			else
				Logger.Warn( $"invalid CSNetSessionId:{GS.instance.gsStorage.csNetSessionId}" );
		}

		public ErrorCode HandleUnhandledMsg( uint nsID, byte[] data, int offset, int size, int msgID )
		{
			if ( this._handlers.TryGetValue( msgID, out MsgHandler handler ) )
				handler( nsID, data, offset, size, msgID );
			else
			{
				if ( msgID > ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcBegin &&
					 msgID < ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcEnd )
				{
					this.TransToCS( nsID, data, offset, size, msgID, false, null );
				}
				else if ( msgID > ( int )GCToSS.MsgNum.EMsgToGstoSsfromGcBegin &&
						  msgID < ( int )GCToSS.MsgNum.EMsgToGstoSsfromGcEnd )
				{
					GSSSInfo ssInfo = GS.instance.gsStorage.GetUserSSInfo( nsID );
					if ( ssInfo == null )
						Logger.Error( $"nsid:{nsID} send msg:{msgID} error, can't get ssinfo." );
					else
					{
						if ( ssInfo.nsID > 0 )
							GS.instance.TranMsgToSession( ssInfo.nsID, data, offset, size, msgID,
																( int )GSToSS.MsgID.EMsgToSsfromGsReportGcmsg, nsID );
						else
							Logger.Error( $"invalid ssID:{ssInfo.nsID}!!" );
					}
				}
				else
				{
					Logger.Error( $"unknown msg with protocal id:{msgID}, offset:{offset}, size:{size}, nsID:{nsID}" );
					return ErrorCode.EC_InvalidMsgProtocalID;
				}
			}
			return ErrorCode.EC_Success;
		}
	}
}