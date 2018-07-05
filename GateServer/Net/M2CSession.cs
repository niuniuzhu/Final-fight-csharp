using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace GateServer.Net
{
	public class M2CSession : CliSession
	{
		private long _lastPingCSTickCounter;

		protected M2CSession( uint id ) : base( id )
		{
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsAskRegisteRet, this.MsgInitHandler );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsOneSsconnected, this.MsgOneSSConnectedHandler );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsAskPingRet, this.OnMsgFromCSAskPingRet );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsOrderOpenListen, this.OnMsgFromCSOrderOpenListen );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsOrderCloseListen, this.OnMsgFromCSOrderCloseListen );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsOrderKickoutGc, this.OnMsgFromCSOrderKickoutGC );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsUserConnectedSs, this.OnMsgFromCSUserConnectedToSS );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsUserDisConnectedSs, this.OnMsgFromCSUserDisConnectedToSS );
			this.msgCenter.Register( ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc, this.OnMsgToGsfromCsOrderPostToGc );
		}

		protected override void SendInitData()
		{
			this._lastPingCSTickCounter = 0;
			Logger.Info( "CS Connected, try to register me." );
			GSToCS.AskRegiste askRegiste = new GSToCS.AskRegiste();
			askRegiste.Port = GS.instance.gsConfig.n32GCListenPort;
			askRegiste.Ip = GS.instance.gsConfig.sGCListenIP;
			askRegiste.Gsid = GS.instance.gsConfig.n32GSID;
			askRegiste.Usepwd = GS.instance.gsConfig.aszMyUserPwd;
			byte[] data = askRegiste.ToByteArray();
			this.owner.TranMsgToSession( this.id, data, 0, data.Length, ( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste, 0, 0 );
		}

		protected override void OnRealEstablish()
		{
			Logger.Info( "CS Connected and register ok" );
			GS.instance.gsStorage.csNetSessionId = this.id;
		}

		protected override void OnClose()
		{
			Logger.Info( "CS DisConnect." );
			GS.instance.gsStorage.csNetSessionId = 0;
		}

		public override void OnHeartBeat( UpdateContext context )
		{
			base.OnHeartBeat( context );

			if ( !this._inited || context.utcTime - this._lastPingCSTickCounter < Consts.DEFAULT_PING_CD_TICK )
				return;
			GSToCS.Asking sPing = new GSToCS.Asking { Time = context.utcTime };
			byte[] data = sPing.ToByteArray();
			this.owner.TranMsgToSession( this.id, data, 0, data.Length, ( int )GSToCS.MsgID.EMsgToCsfromGsAskPing, 0, 0 );
			this._lastPingCSTickCounter = context.utcTime;
		}

		#region msg handlers
		private ErrorCode MsgInitHandler( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			CSToGS.AskRegisteRet askRegisteRet = new CSToGS.AskRegisteRet();
			askRegisteRet.MergeFrom( data, offset, size );

			if ( ( int )ErrorCode.Success != askRegisteRet.Registe )
			{
				Logger.Warn( $"CS Register Error(ret={askRegisteRet.Registe})!" );
				return ( ErrorCode )askRegisteRet.Registe;
			}

			long csMilsec = askRegisteRet.Curtime;
			long selfMilsec = TimeUtils.utcTime;
			GS.instance.gsStorage.csTimeError = csMilsec - selfMilsec;
			GS.instance.gsStorage.ssBaseIdx = askRegisteRet.Ssbaseid;

			int ssinfoCount = askRegisteRet.Ssinfo.Count;
			if ( 0 < ssinfoCount )
			{
				GS.instance.gsStorage.ssConnectNum = 0;
				for ( int i = 0; i < ssinfoCount; i++ )
				{
					if ( 0 == askRegisteRet.Ssinfo[i].Ssid )
						continue;

					if ( GS.instance.gsStorage.ContainsSSInfo( askRegisteRet.Ssinfo[i].Ssid ) )
						continue;

					GSSSInfo ssInfo = new GSSSInfo();
					ssInfo.ssID = askRegisteRet.Ssinfo[i].Ssid;
					ssInfo.listenIp = askRegisteRet.Ssinfo[i].Ip.Replace( "\0", string.Empty );
					ssInfo.listenPort = askRegisteRet.Ssinfo[i].Port;
					ssInfo.ssNetState = ( ServerNetState )askRegisteRet.Ssinfo[i].Netstate;
					GS.instance.gsStorage.AddSSInfo( ssInfo.ssID, ssInfo );
					if ( ssInfo.ssNetState == ServerNetState.Closed )
						continue;

					this.owner.CreateConnector( SessionType.ClientG2S, ssInfo.listenIp, ssInfo.listenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240, ssInfo.ssID );
					++GS.instance.gsStorage.ssConnectNum;
				}
			}
			this.SetInited( true, true );
			return ErrorCode.Success;
		}

		private ErrorCode MsgOneSSConnectedHandler( byte[] data, int offset, int size, int msgID )
		{
			offset += 2 * sizeof( int );
			size -= 2 * sizeof( int );

			CSToGS.OneSSConnected oneSsConnected = new CSToGS.OneSSConnected();
			oneSsConnected.MergeFrom( data, offset, size );

			GSSSInfo pcSSInfo = GS.instance.gsStorage.GetSSInfo( oneSsConnected.Ssid );
			if ( pcSSInfo != null )
			{
				pcSSInfo.listenIp = oneSsConnected.Ip.Replace( "\0", string.Empty );
				pcSSInfo.listenPort = oneSsConnected.Port;
				if ( pcSSInfo.ssNetState == ServerNetState.Closed )
				{
					pcSSInfo.ssNetState = ( ServerNetState )oneSsConnected.Netstate;
					pcSSInfo.nsID = 0;
					this.owner.CreateConnector( SessionType.ClientG2S, pcSSInfo.listenIp, pcSSInfo.listenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240, oneSsConnected.Ssid );
				}
			}
			return ErrorCode.Success;
		}

		/// <summary>
		/// 处理中心服务器返回的ping消息
		/// </summary>
		private ErrorCode OnMsgFromCSAskPingRet( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			CSToGS.AskPing pingRet = new CSToGS.AskPing();
			pingRet.MergeFrom( data, offset, size );

			long curMilsec = TimeUtils.utcTime;
			long tickSpan = curMilsec - pingRet.Time;

			Logger.Info( $"Ping CS returned, tick span {tickSpan}." );

			return ErrorCode.Success;
		}

		/// <summary>
		/// 中心服务器通知开服
		/// </summary>
		private ErrorCode OnMsgFromCSOrderOpenListen( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			GS.instance.CreateListener( GS.instance.gsConfig.n32GCListenPort, 10240, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 0 );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 中心服务器通知关服
		/// </summary>
		private ErrorCode OnMsgFromCSOrderCloseListen( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			GS.instance.StopListener( 0 );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 中心服务器通知强制客户端下线
		/// </summary>
		private ErrorCode OnMsgFromCSOrderKickoutGC( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			CSToGS.OrderKickoutGC orderKickoutGC = new CSToGS.OrderKickoutGC();
			orderKickoutGC.MergeFrom( data, offset, size );
			GS.instance.PostGameClientDisconnect( ( uint )orderKickoutGC.Gcnid );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 中心服务器通知场景服务器内的客户端信息
		/// </summary>
		private ErrorCode OnMsgFromCSUserConnectedToSS( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			CSToGS.UserConnectedSS userConnectedSS = new CSToGS.UserConnectedSS();
			userConnectedSS.MergeFrom( data, offset, size );

			GSSSInfo ssInfo = GS.instance.gsStorage.GetSSInfo( userConnectedSS.Ssid );
			if ( null == ssInfo )
			{
				Logger.Error( $"ssInfo is null with ssid({userConnectedSS.Ssid})" );
				return ErrorCode.SSNotFound;
			}

			//客户端id和场景服务器信息建立映射关系
			int count = userConnectedSS.Gcnid.Count;
			for ( int i = 0; i < count; ++i )
			{
				uint gc = ( uint )userConnectedSS.Gcnid[i];
				GS.instance.gsStorage.AddUserSSInfo( gc, ssInfo );
				Logger.Log( $"user netID({gc}) connect with SS({ssInfo.ssID})" );
			}
			return ErrorCode.Success;
		}

		/// <summary>
		/// 中心服务器通知客户端和场景服务器的连接断开
		/// </summary>
		private ErrorCode OnMsgFromCSUserDisConnectedToSS( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			CSToGS.UserDisConnectedSS userConnectedSS = new CSToGS.UserDisConnectedSS();
			userConnectedSS.MergeFrom( data, offset, size );

			int count = userConnectedSS.Gcnid.Count;
			for ( int i = 0; i < count; ++i )
				GS.instance.gsStorage.RemoveUserSSInfo( ( uint )userConnectedSS.Gcnid[i] );
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgToGsfromCsOrderPostToGc( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			if ( gcNetID == 0 )
				GS.instance.BroadcastToGameClient( data, offset, size, msgID );
			else
				GS.instance.PostToGameClient( gcNetID, data, offset, size, msgID );
			return ErrorCode.Success;
		}
		#endregion
	}
}