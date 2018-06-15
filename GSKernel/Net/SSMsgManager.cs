using Core;
using Core.Misc;
using Google.Protobuf;
using Shared;
using System.Collections.Generic;

namespace GateServer.Net
{
	public class SSMsgManager : MsgManager
	{
		private delegate EResult MsgHandler( GSSSInfo ssInfo, byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public SSMsgManager()
		{
			#region 注册消息处理函数
			this._handlers[( int )SSToGS.MsgID.EMsgToGsfromSsAskPingRet] = this.OnMsgFromSS_AskPingRet;
			this._handlers[( int )SSToGS.MsgID.EMsgToGsfromSsOrderKickoutGc] = this.OnMsgFromSS_OrderKickoutGC;
			#endregion
		}

		/// <summary>
		/// 处理场景服务器返回的ping消息
		/// </summary>
		private EResult OnMsgFromSS_AskPingRet( GSSSInfo ssInfo, byte[] data, int offset, int size )
		{
			if ( null == ssInfo )
				return EResult.NullPointer;

			SSToGS.AskPingRet pPingRet = new SSToGS.AskPingRet();
			pPingRet.MergeFrom( data, offset, size );

			long curMilsec = TimeUtils.utcTime;
			long tickSpan = curMilsec - pPingRet.Time;
			Logger.Info( $"Ping SS {ssInfo.ssID} returned, Tick span {tickSpan}." );
			return EResult.Normal;
		}

		/// <summary>
		/// 场景服务器通知踢走客户端
		/// </summary>
		private EResult OnMsgFromSS_OrderKickoutGC( GSSSInfo ssInfo, byte[] data, int offset, int size )
		{
			SSToGS.OrderKickoutGC orderKickoutGc = new SSToGS.OrderKickoutGC();
			orderKickoutGc.MergeFrom( data, offset, size );

			GSKernel.instance.PostGameClientDisconnect( ( uint )orderKickoutGc.Gsnid );
			return EResult.Normal;
		}

		/// <summary>
		/// 处理需要转发的消息
		/// </summary>
		public ErrorCode HandleUnhandledMsg( GSSSInfo ssInfo, byte[] data, int offset, int size, int msgID, int transID, uint gcNetID )
		{
			switch ( transID )
			{
				case ( int )SSToGS.MsgID.EMsgToGsfromSsOrderPostToGc when gcNetID == 0:
					GSKernel.instance.BroadcastToGameClient( data, offset, size, msgID );
					break;

				case ( int )SSToGS.MsgID.EMsgToGsfromSsOrderPostToGc:
					if ( msgID == ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyBattleBaseInfo )
					{
						//该消息的路由:ss-cs-gs-gc
						//该消息从ss发出,目标端是网络id为gcNetID的客户端,消息体是场景信息
						//当该消息流经gs时建立该客户端和场景信息的映射关系
						GSKernel.instance.gsStorage.AddUserSSInfo( gcNetID, ssInfo );
					}
					GSKernel.instance.PostToGameClient( gcNetID, data, offset, size, msgID );
					break;

				default:
					//检查是否注册了的该消息的处理函数
					if ( this._handlers.TryGetValue( transID, out MsgHandler handler ) )
						handler( ssInfo, data, offset, size );
					else
						return ErrorCode.EC_InvalidMsgProtocalID;
					break;
			}
			return ErrorCode.EC_Success;
		}
	}
}