using Core.Misc;
using CSToGS;
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
			this._msgHandler.Register( ( int )MsgID.EMsgToGsfromCsAskRegisteRet, this.MsgInitHandler );
			this._msgHandler.Register( ( int )MsgID.EMsgToGsfromCsOneSsconnected, this.MsgOneSSConnectedHandler );
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
		private ErrorCode MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			offset += 2 * sizeof( int );
			size -= 2 * sizeof( int );

			AskRegisteRet askRegisteRet = new AskRegisteRet();
			askRegisteRet.MergeFrom( data, offset, size );

			if ( ( int )ErrorCode.Success != askRegisteRet.Registe )
			{
				Logger.Warn( $"CS Register Error(ret={askRegisteRet.Registe})!" );
				return ( ErrorCode ) askRegisteRet.Registe;
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
					ssInfo.ssNetState = ( EServerNetState )askRegisteRet.Ssinfo[i].Netstate;
					GS.instance.gsStorage.AddSSInfo( ssInfo.ssID, ssInfo );
					if ( ssInfo.ssNetState == EServerNetState.SnsClosed )
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

			OneSSConnected oneSsConnected = new OneSSConnected();
			oneSsConnected.MergeFrom( data, offset, size );

			GSSSInfo pcSSInfo = GS.instance.gsStorage.GetSSInfo( oneSsConnected.Ssid );
			if ( pcSSInfo != null )
			{
				pcSSInfo.listenIp = oneSsConnected.Ip.Replace( "\0", string.Empty );
				pcSSInfo.listenPort = oneSsConnected.Port;
				if ( pcSSInfo.ssNetState == EServerNetState.SnsClosed )
				{
					pcSSInfo.ssNetState = ( EServerNetState )oneSsConnected.Netstate;
					pcSSInfo.nsID = 0;
					this.owner.CreateConnector( SessionType.ClientG2S, pcSSInfo.listenIp, pcSSInfo.listenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240, oneSsConnected.Ssid );
				}
			}
			return ErrorCode.Success;
		}

		protected override ErrorCode HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			int realMsgID = 0;
			uint gcNetID = 0;
			//剥离第二层消息ID
			offset += ByteUtils.Decode32i( data, offset, ref realMsgID );
			//剥离客户端网络ID
			offset += ByteUtils.Decode32u( data, offset, ref gcNetID );
			size -= 2 * sizeof( int );
			GS.instance.csMsgManager.HandleUnhandledMsg( data, offset, size, realMsgID, msgID, gcNetID );
			return ErrorCode.Success;
		}
		#endregion
	}
}