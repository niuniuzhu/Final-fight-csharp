using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace GateServer.Net
{
	public class M2SSession : CliSession
	{
		protected M2SSession( uint id ) : base( id )
		{
			 this.msgCenter.Register( ( int )SSToGS.MsgID.EMsgToGsfromSsAskRegisteRet, this.MsgInitHandler );
			 this.msgCenter.Register( ( int )SSToGS.MsgID.EMsgToGsfromSsAskPingRet, this.OnMsgFromSSAskPingRet );
			 this.msgCenter.Register( ( int )SSToGS.MsgID.EMsgToGsfromSsOrderKickoutGc, this.OnMsgFromSSOrderKickoutGC );
			 this.msgCenter.Register( ( int )SSToGS.MsgID.EMsgToGsfromSsOrderPostToGc, this.OnMsgToGsfromSsOrderPostToGc );
		}

		protected override void SendInitData()
		{
			GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.ssID}) Connected, try to register me." );
			ssInfo.nsID = this.id;
			GSToSS.AskRegiste askRegiste = new GSToSS.AskRegiste()
			{
				Gsid = GS.instance.gsConfig.n32GSID,
				Pwd = GS.instance.gsConfig.aszMyUserPwd
			};
			byte[] data = askRegiste.ToByteArray();
			this.owner.TranMsgToSession( ssInfo.nsID, data, 0, data.Length, ( int )GSToSS.MsgID.EMsgToSsfromGsAskRegiste, 0, 0 );
		}

		protected override void OnRealEstablish()
		{
			GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.ssID}) Connected and register ok." );
			ssInfo.nsID = this.id;
			ssInfo.lastConnMilsec = TimeUtils.utcTime;
			ssInfo.pingTickCounter = 0;
		}

		protected override void OnClose()
		{
			GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.ssID}) DisConnect." );
			GS.instance.gsStorage.OnSSClosed( ssInfo );
			ssInfo.nsID = 0;
		}

		#region msg handlers
		private ErrorCode MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null || data == null )
			{
				Logger.Error( string.Empty );
				return ErrorCode.InvaildLogicID;
			}

			offset += 2 * sizeof( int );
			size -= 2 * sizeof( int );
			SSToGS.AskRegisteRet askRegisteRet = new SSToGS.AskRegisteRet();
			askRegisteRet.MergeFrom( data, offset, size );

			if ( ( int )ErrorCode.Success != askRegisteRet.State )
			{
				Logger.Warn( $"register to SS {ssInfo.ssID} Fail with error code {askRegisteRet.State}." );
				return ( ErrorCode )askRegisteRet.State;
			}

			ssInfo.ssNetState = ServerNetState.Free;
			Logger.Info( $"register to SS {ssInfo.ssID} success at session {ssInfo.nsID}." );
			this.SetInited( true, true );

			return ErrorCode.Success;
		}

		/// <summary>
		/// 处理场景服务器返回的ping消息
		/// </summary>
		private ErrorCode OnMsgFromSSAskPingRet( byte[] data, int offset, int size, int msgID )
		{
			GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null )
				return ErrorCode.SSNotFound;

			SSToGS.AskPingRet pPingRet = new SSToGS.AskPingRet();
			pPingRet.MergeFrom( data, offset, size );

			long curMilsec = TimeUtils.utcTime;
			long tickSpan = curMilsec - pPingRet.Time;
			Logger.Info( $"Ping SS {ssInfo.ssID} returned, Tick span {tickSpan}." );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 场景服务器通知踢走客户端
		/// </summary>
		private ErrorCode OnMsgFromSSOrderKickoutGC( byte[] data, int offset, int size, int msgID )
		{
			SSToGS.OrderKickoutGC orderKickoutGc = new SSToGS.OrderKickoutGC();
			orderKickoutGc.MergeFrom( data, offset, size );

			GS.instance.PostGameClientDisconnect( ( uint )orderKickoutGc.Gsnid );
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgToGsfromSsOrderPostToGc( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			if ( gcNetID == 0 )
				GS.instance.BroadcastToGameClient( data, offset, size, msgID );
			else
			{
				if ( msgID == ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyBattleBaseInfo )
				{
					GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( this.logicID );
					if ( ssInfo == null )
						return ErrorCode.SSNotFound;
					//该消息的路由:ss-cs-gs-gc
					//该消息从ss发出,目标端是网络id为gcNetID的客户端,消息体是场景信息
					//当该消息流经gs时建立该客户端和场景信息的映射关系
					GS.instance.gsStorage.AddUserSSInfo( gcNetID, ssInfo );
				}
				GS.instance.PostToGameClient( gcNetID, data, offset, size, msgID );
			}
			return ErrorCode.Success;
		}
		#endregion
	}
}