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

		private readonly Dictionary<int, GSSSInfo> _GSSSInfoMap = new Dictionary<int, GSSSInfo>();
		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public SSMsgManager()
		{
			#region 注册消息处理函数
			this._handlers[( int )SSToGS.MsgID.EMsgToGsfromSsAskPingRet] = this.OnMsgFromSS_AskPingRet;
			this._handlers[( int )SSToGS.MsgID.EMsgToGsfromSsOrderKickoutGc] = this.OnMsgFromSS_OrderKickoutGC;
			#endregion
		}

		/// <summary>
		/// 添加场景服务器信息
		/// </summary>
		public void AddGSSInfo( int ssID, GSSSInfo gsssInfo ) => this._GSSSInfoMap[ssID] = gsssInfo;

		/// <summary>
		/// 移除指定场景服务器信息
		/// </summary>
		public bool RemoveGSSInfo( int ssID ) => this._GSSSInfoMap.Remove( ssID );

		/// <summary>
		/// 获取本地场景服务器信息
		/// </summary>
		public GSSSInfo GetGSSSInfo( int ssID )
		{
			this._GSSSInfoMap.TryGetValue( ssID, out GSSSInfo ssInfo );
			return ssInfo;
		}

		/// <summary>
		/// 是否包含指定场景服务器id的信息
		/// </summary>
		public bool ContainsSSInfo( int ssID ) => this._GSSSInfoMap.ContainsKey( ssID );

		/// <summary>
		/// 向每个场景服务器发送ping
		/// </summary>
		public EResult PingSS( long utcTime )
		{
			if ( this._GSSSInfoMap.Count == 0 )
				return EResult.Normal;

			foreach ( KeyValuePair<int, GSSSInfo> kv in this._GSSSInfoMap )
			{
				GSSSInfo ssInfo = kv.Value;
				if ( 0 == ssInfo.ssID || 0 == ssInfo.nsID )
					continue;

				if ( utcTime - ssInfo.pingTickCounter < Consts.DEFAULT_PING_CD_TICK )
					continue;

				GSToSS.AskPing sMsg = new GSToSS.AskPing { Time = utcTime };
				byte[] data = sMsg.ToByteArray();
				GSKernel.instance.netSessionMrg.TranMsgToSession( ssInfo.nsID, data, 0, data.Length, ( int )GSToSS.MsgID.EMsgToSsfromGsAskPing, 0, 0 );
				ssInfo.pingTickCounter = utcTime;
			}
			return EResult.Normal;
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

			this.PostGameClientDisconnect( ( uint )orderKickoutGc.Gsnid );
			return EResult.Normal;
		}

		/// <summary>
		/// 处理需要转发的消息
		/// </summary>
		public ErrorCode HandleUnhandledMsg( GSSSInfo ssInfo, byte[] data, int offset, int size, int msgID, int transID, uint gcNetID )
		{
			switch ( transID )
			{
				case ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc when gcNetID == 0:
					this.BroadcastToGameClient( data, offset, size, msgID );
					break;

				case ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc:
					if ( msgID == ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyBattleBaseInfo )
					{
						//该消息的路由:ss-cs-gs-gc
						//该消息从ss发出,目标端是网络id为gcNetID的客户端,消息体是场景信息
						//当该消息流经gs时建立该客户端和场景信息的映射关系
						GSKernel.instance.csMsgManager.AddGSSInfo( gcNetID, ssInfo );
					}
					this.PostToGameClient( gcNetID, data, offset, size, msgID );
					break;

				default:
					//检查是否注册了的该消息的处理函数
					if ( this._handlers.TryGetValue( transID, out MsgHandler handler ) )
						handler( ssInfo, data, offset, size );
					else
						return ErrorCode.EC_InvalidMsgProtocalID;
					break;
			}
			return ErrorCode.EC_NullMsgHandler;
		}
	}
}