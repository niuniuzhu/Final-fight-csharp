using Core.Misc;
using Google.Protobuf;
using Shared;
using System.Collections.Generic;

namespace GateServer.Net
{
	public class CSMsgManager
	{
		private delegate EResult MsgHandler( byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public CSMsgManager()
		{
			#region 注册消息处理函数
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsAskPingRet] = this.OnMsgFromCS_AskPingRet;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsOrderOpenListen] = this.OnMsgFromCS_OrderOpenListen;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsOrderCloseListen] = this.OnMsgFromCS_OrderCloseListen;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsOrderKickoutGc] = this.OnMsgFromCS_OrderKickoutGC;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsUserConnectedSs] = this.OnMsgFromCS_UserConnectedToSS;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsUserDisConnectedSs] = this.OnMsgFromCS_UserDisConnectedToSS;
			#endregion
		}

		/// <summary>
		/// 处理中心服务器返回的ping消息
		/// </summary>
		private EResult OnMsgFromCS_AskPingRet( byte[] data, int offset, int size )
		{
			CSToGS.AskPing pingRet = new CSToGS.AskPing();
			pingRet.MergeFrom( data, offset, size );

			long curMilsec = TimeUtils.utcTime;
			long tickSpan = curMilsec - pingRet.Time;

			Logger.Info( $"Ping CS returned, tick span {tickSpan}." );

			return EResult.Normal;
		}

		/// <summary>
		/// 中心服务器通知开服
		/// </summary>
		private EResult OnMsgFromCS_OrderOpenListen( byte[] data, int offset, int size )
		{
			GS.instance.CreateListener( GS.instance.gsConfig.n32GCListenPort, 10240, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 0 );
			return EResult.Normal;
		}

		/// <summary>
		/// 中心服务器通知关服
		/// </summary>
		private EResult OnMsgFromCS_OrderCloseListen( byte[] data, int offset, int size )
		{
			GS.instance.StopListener( 0 );
			return EResult.Normal;
		}

		/// <summary>
		/// 中心服务器通知强制客户端下线
		/// </summary>
		private EResult OnMsgFromCS_OrderKickoutGC( byte[] data, int offset, int size )
		{
			CSToGS.OrderKickoutGC orderKickoutGC = new CSToGS.OrderKickoutGC();
			orderKickoutGC.MergeFrom( data, offset, size );
			GS.instance.PostGameClientDisconnect( ( uint )orderKickoutGC.Gcnid );
			return EResult.Normal;
		}

		/// <summary>
		/// 中心服务器通知场景服务器内的客户端信息
		/// </summary>
		private EResult OnMsgFromCS_UserConnectedToSS( byte[] data, int offset, int size )
		{
			CSToGS.UserConnectedSS userConnectedSS = new CSToGS.UserConnectedSS();
			userConnectedSS.MergeFrom( data, offset, size );

			GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( userConnectedSS.Ssid );
			if ( null == ssInfo )
			{
				Logger.Error( $"ssInfo is null with ssid({userConnectedSS.Ssid})" );
				return EResult.Normal;
			}

			//客户端id和场景服务器信息建立映射关系
			int count = userConnectedSS.Gcnid.Count;
			for ( int i = 0; i < count; ++i )
			{
				uint gcNetID = ( uint )userConnectedSS.Gcnid[i];
				GS.instance.gsStorage.AddUserSSInfo( gcNetID, ssInfo );
				Logger.Log( $"user netID({gcNetID}) connect with SS({ssInfo.ssID})" );
			}
			return EResult.Normal;
		}

		/// <summary>
		/// 中心服务器通知客户端和场景服务器的连接断开
		/// </summary>
		private EResult OnMsgFromCS_UserDisConnectedToSS( byte[] data, int offset, int size )
		{
			CSToGS.UserDisConnectedSS userConnectedSS = new CSToGS.UserDisConnectedSS();
			userConnectedSS.MergeFrom( data, offset, size );

			int count = userConnectedSS.Gcnid.Count;
			for ( int i = 0; i < count; ++i )
				GS.instance.gsStorage.RemoveUserSSInfo( ( uint )userConnectedSS.Gcnid[i] );
			return EResult.Normal;
		}

		/// <summary>
		/// 处理需要转发的消息
		/// </summary>
		public ErrorCode HandleUnhandledMsg( byte[] data, int offset, int size, int msgID, int transID, uint gcNetID )
		{
			switch ( transID )
			{
				case ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc when gcNetID == 0:
					GS.instance.BroadcastToGameClient( data, offset, size, msgID );
					break;

				case ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc:
					GS.instance.PostToGameClient( gcNetID, data, offset, size, msgID );
					break;

				default:
					//检查是否注册了的该消息的处理函数
					if ( this._handlers.TryGetValue( transID, out MsgHandler handler ) )
						handler( data, offset, size );
					else
						return ErrorCode.EC_InvalidMsgProtocalID;
					break;
			}
			return ErrorCode.EC_Success;
		}
	}
}