using Core;
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
			this._handlerContainer.Register( ( int )MsgID.EMsgToGsfromCsAskRegisteRet, this.MsgInitHandler );
			this._handlerContainer.Register( ( int )MsgID.EMsgToGsfromCsOneSsconnected, this.MsgOneSSConnectedHandler );
		}

		protected override void SendInitData()
		{
			this._lastPingCSTickCounter = 0;
			Logger.Info( "CS Connected, try to register me." );
			GSToCS.AskRegiste askRegiste = new GSToCS.AskRegiste();
			askRegiste.Port = GSKernel.instance.gsConfig.n32GCListenPort;
			askRegiste.Ip = GSKernel.instance.gsConfig.sGCListenIP;
			askRegiste.Gsid = GSKernel.instance.gsConfig.n32GSID;
			askRegiste.Usepwd = GSKernel.instance.gsConfig.aszMyUserPwd;
			byte[] data = askRegiste.ToByteArray();
			this.owner.TranMsgToSession( this.id, data, 0, data.Length, ( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste, 0, 0 );
		}

		protected override void OnRealEstablish()
		{
			Logger.Info( "CS Connected and register ok" );
			GSKernel.instance.gsStorage.csNetSessionId = this.id;
		}

		protected override void OnClose()
		{
			Logger.Info( "CS DisConnect." );
			GSKernel.instance.gsStorage.csNetSessionId = 0;
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
		private bool MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			offset += 2 * sizeof( int );
			size -= 2 * sizeof( int );

			//don't send any message until it init success.
			AskRegisteRet askRegisteRet = new AskRegisteRet();
			askRegisteRet.MergeFrom( data, offset, size );

			if ( ( int )EResult.Normal != askRegisteRet.Registe )
			{
				Logger.Warn( $"CS Register Error(ret={askRegisteRet.Registe})!" );
				return false;
			}

			long csMilsec = askRegisteRet.Curtime;
			long selfMilsec = TimeUtils.utcTime;
			GSKernel.instance.gsStorage.csTimeError = csMilsec - selfMilsec;
			GSKernel.instance.gsStorage.ssBaseIdx = askRegisteRet.Ssbaseid;
			int ssinfoCount = askRegisteRet.Ssinfo.Count;
			if ( ssinfoCount > 100000 )
			{
				Logger.Warn( $"CS Register Error(ss max={ssinfoCount})!" );
				return false;
			}

			if ( 0 < ssinfoCount )
			{
				GSKernel.instance.gsStorage.ssConnectNum = 0;
				for ( int i = 0; i < ssinfoCount; i++ )
				{
					// 消息解码//
					if ( 0 == askRegisteRet.Ssinfo[i].Ssid )
						continue;

					if ( GSKernel.instance.gsStorage.ContainsSSInfo( askRegisteRet.Ssinfo[i].Ssid ) )
						continue;

					GSSSInfo ssInfo = new GSSSInfo();
					ssInfo.ssID = askRegisteRet.Ssinfo[i].Ssid;
					ssInfo.listenIp = askRegisteRet.Ssinfo[i].Ip.Replace( "\0", string.Empty );
					ssInfo.listenPort = askRegisteRet.Ssinfo[i].Port;
					ssInfo.ssNetState = ( EServerNetState )askRegisteRet.Ssinfo[i].Netstate;
					ssInfo.nsID = 0;
					ssInfo.connTimes = 0;
					ssInfo.lastConnMilsec = 0;
					ssInfo.pingTickCounter = 0;
					ssInfo.msgReceived = 0;
					ssInfo.msgSent = 0;
					ssInfo.dataReceived = 0;
					ssInfo.dataSent = 0;
					GSKernel.instance.gsStorage.AddSSInfo( ssInfo.ssID, ssInfo );
					if ( ssInfo.ssNetState == EServerNetState.SnsClosed )
						continue;

					this.owner.CreateConnector( SessionType.ClientG2S, ssInfo.listenIp, ssInfo.listenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240, ssInfo.ssID );
					++GSKernel.instance.gsStorage.ssConnectNum;
				}
			}
			this.SetInited( true, true );
			return true;
		}

		private bool MsgOneSSConnectedHandler( byte[] data, int offset, int size, int msgID )
		{
			offset += 2 * sizeof( int );
			size -= 2 * sizeof( int );

			OneSSConnected oneSsConnected = new OneSSConnected();
			oneSsConnected.MergeFrom( data, offset, size );

			GSSSInfo pcSSInfo = GSKernel.instance.gsStorage.GetSSInfo( oneSsConnected.Ssid );
			if ( pcSSInfo != null )
			{
				pcSSInfo.listenIp = oneSsConnected.Ip.Replace( "\0", string.Empty ); ;
				pcSSInfo.listenPort = oneSsConnected.Port;
				if ( pcSSInfo.ssNetState == EServerNetState.SnsClosed )
				{
					pcSSInfo.ssNetState = ( EServerNetState )oneSsConnected.Netstate;
					pcSSInfo.nsID = 0;
					this.owner.CreateConnector( SessionType.ClientG2S, pcSSInfo.listenIp, pcSSInfo.listenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240, oneSsConnected.Ssid );
				}
			}
			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			int realMsgID = 0;
			uint gcNetID = 0;
			//剥离第二层数据ID
			offset += ByteUtils.Decode32i( data, offset, ref realMsgID );
			//剥离客户端网络ID
			offset += ByteUtils.Decode32u( data, offset, ref gcNetID );
			size -= 2 * sizeof( int );
			GSKernel.instance.csMsgManager.HandleUnhandledMsg( data, offset, size, realMsgID, msgID, gcNetID );
			return true;
		}
		#endregion
	}
}