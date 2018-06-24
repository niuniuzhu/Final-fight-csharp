using System.Net;
using Core.Misc;
using Core.Net;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace GateServer.Net
{
	public class ClientSession : SrvCliSession
	{
		private bool _logicInited;

		protected ClientSession( uint id ) : base( id )
		{
			this._msgCenter.Register( ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin, this.OnMsgToGstoCsfromGcAskLogin );
			this._msgCenter.Register( ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskReconnectGame, this.OnMsgToGstoCsfromGcAskReconnectGame );
			this._msgCenter.Register( ( int )GCToSS.MsgNum.EMsgToGstoSsfromGcAskPingSs, this.OnMsgToGstoSsfromGcAskPingSs );
			int start = ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcBegin + 1;
			int end = ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcEnd;
			for ( int i = start; i < end; ++i )
				this._msgCenter.Register( i, this.OnTransToCS );
			start = ( int )GCToSS.MsgNum.EMsgToGstoSsfromGcBegin;
			end = ( int )GCToSS.MsgNum.EMsgToGstoSsfromGcEnd;
			for ( int i = start; i < end; ++i )
				this._msgCenter.Register( i, this.OnTransToSS );
		}

		protected override void SendInitData()
		{
		}

		protected override void OnRealEstablish()
		{
			if ( this._logicInited )
				return;
			this._logicInited = true;
			Logger.Log( $"client({this.id})({this.connection.remoteEndPoint}) connected." );
		}

		protected override void OnClose()
		{
			if ( !this._logicInited )
				return;
			Logger.Log( $"client({this.id})({this.connection.remoteEndPoint}) disconnected." );
			GS.instance.gsStorage.OnUserLost( this.id );
			this._logicInited = false;
		}

		#region msg handlers
		/// <summary>
		/// 客户端请求登录网关服务器
		/// </summary>
		private ErrorCode OnMsgToGstoCsfromGcAskLogin( byte[] data, int offset, int size, int msgID )
		{
			if ( !this._logicInited )
				this.SetInited( true, true );

			bool logMsgFlag = false;
			GCToCS.Login loginMsg = new GCToCS.Login();

			GCToCS.Login login = new GCToCS.Login();
			login.MergeFrom( data, offset, size );

			//验证token
			//正常登录流程是连接到登录服务器,再通过负载均衡服务器把消息转发给合适的网关服务器
			//如果客户端绕过上述过程直接连接网关服务器并请求登录,则是非法操作
			if ( !GS.instance.gsStorage.IsUserCanLogin( login.Name, login.Passwd, this.id ) )
			{
				Logger.Error( $"user {login.Name} can't login with token {login.Passwd}" );
				GSToGC.NetClash msg = new GSToGC.NetClash();
				//断开连接
				GS.instance.PostToGameClient( this.id, msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
				GS.instance.PostGameClientDisconnect( this.id );
				return ErrorCode.Success;
			}
			//获取IP
			INetSession pClient = GS.instance.GetSession( this.id );
			if ( null != pClient )
			{
				EndPoint endPoint = pClient.connection.remoteEndPoint;
				if ( endPoint == null )
				{
					Logger.Error( $"user {login.Name} can't login with IP is null" );
					GS.instance.PostGameClientDisconnect( this.id );
					return ErrorCode.Success;
				}
				logMsgFlag = true;
				loginMsg.Platform = login.Platform;
				loginMsg.Sdk = login.Sdk;
				loginMsg.Name = login.Name;
				loginMsg.Passwd = login.Passwd;
				loginMsg.Equimentid = login.Equimentid;
				loginMsg.Ipaddress = endPoint.ToString();
				Logger.Log( $"client({this.id}) ask login({loginMsg.Name})({loginMsg.Passwd})" );
			}
			//把登录信息转发到中心服务器
			this.TransToCS( this.id, data, offset, size, msgID, logMsgFlag, loginMsg );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 客户端请求重新登录
		/// </summary>
		private ErrorCode OnMsgToGstoCsfromGcAskReconnectGame( byte[] data, int offset, int size, int msgID )
		{
			if ( !this._logicInited )
				this.SetInited( true, true );

			GCToCS.ReconnectToGame reconnectToGame = new GCToCS.ReconnectToGame();
			reconnectToGame.MergeFrom( data, offset, size );
			//验证token
			if ( !GS.instance.gsStorage.IsUserCanLogin( reconnectToGame.Name, reconnectToGame.Passwd, this.id ) )
			{
				Logger.Error( $"user {reconnectToGame.Name} can't login with token {reconnectToGame.Passwd}" );
				GSToGC.NetClash msg = new GSToGC.NetClash();
				GS.instance.PostToGameClient( this.id, msg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNetClash );
				GS.instance.PostGameClientDisconnect( this.id );
			}
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgToGstoSsfromGcAskPingSs( byte[] data, int offset, int size, int msgID )
		{
			if ( !this._logicInited )
				this.SetInited( true, true );

			GCToSS.AskPingSS msgPing = new GCToSS.AskPingSS();
			msgPing.MergeFrom( data, offset, size );
			GSToGC.PingRet retMsg = new GSToGC.PingRet
			{
				Time = msgPing.Time,
				Flag = 1
			};
			GS.instance.PostToGameClient( this.id, retMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsGcaskPingRet );
			return ErrorCode.Success;
		}

		private ErrorCode OnTransToCS( byte[] data, int offset, int size, int msgID )
		{
			return this.TransToCS( this.id, data, offset, size, msgID, false, null );
		}

		private ErrorCode TransToCS( uint nsID, byte[] data, int offset, int size, int msgID, bool logMsgFlag, GCToCS.Login loginMsg )
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

				return ErrorCode.Success;
			}
			Logger.Warn( $"invalid CSNetSessionId:{GS.instance.gsStorage.csNetSessionId}" );
			return ErrorCode.InvalidNSID;
		}

		private ErrorCode OnTransToSS( byte[] data, int offset, int size, int msgID )
		{
			GSSSInfo ssInfo = GS.instance.gsStorage.GetUserSSInfo( this.id );
			if ( ssInfo == null )
			{
				Logger.Error( $"nsid:{this.id} send msg:{msgID} error, can't get ssinfo." );
				return ErrorCode.SSNotFound;
			}
			if ( ssInfo.nsID > 0 )
			{
				GS.instance.TranMsgToSession( ssInfo.nsID, data, offset, size, msgID,
											  ( int )GSToSS.MsgID.EMsgToSsfromGsReportGcmsg, this.id );
				return ErrorCode.Success;
			}
			Logger.Error( $"invalid ssID:{ssInfo.nsID}!!" );
			return ErrorCode.InvalidSSID;
		}
		#endregion
	}
}