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
			this._msgCenter.RegisterMsgFunc( ( int )MsgID.EMsgToGsfromCsAskRegisteRet, this.MsgInitHandler );
			this._msgCenter.RegisterMsgFunc( ( int )MsgID.EMsgToGsfromCsOneSsconnected, this.MsgOneSSConnectedHandler );
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
			this.owner.TranMsgToSession( SessionType.ClientG2C, this.id, data, 0, data.Length, ( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste, 0, 0 );
		}

		protected override void OnRealEstablish()
		{
			Logger.Info( "CS Connected and register ok" );
		}

		protected override void OnClose()
		{
			Logger.Info( "CS DisConnect." );
		}

		protected override void InternalOnHeartBeat( UpdateContext context )
		{
			if ( context.utcTime - this._lastPingCSTickCounter < Consts.C_T_DEFAULT_PING_CD_TICK )
				return;
			GSToCS.Asking sPing = new GSToCS.Asking { Time = context.utcTime };
			byte[] data = sPing.ToByteArray();
			this.owner.TranMsgToSession( SessionType.ClientG2C, this.id, data, 0, data.Length, ( int )GSToCS.MsgID.EMsgToCsfromGsAskPing, 0, 0 );
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
			GSKernel.instance.csTimeError = csMilsec - selfMilsec;
			GSKernel.instance.ssBaseIdx = askRegisteRet.Ssbaseid;
			int ssinfoCount = askRegisteRet.Ssinfo.Count;
			Logger.Debug( $"m_un32MaxSSNum:{ssinfoCount}" );
			if ( ssinfoCount > 100000 )
			{
				Logger.Warn( $"CS Register Error(ss max={ssinfoCount})!" );
				return false;
			}

			if ( 0 < ssinfoCount )
			{
				GSKernel.instance.ssConnectNum = 0;
				for ( int i = 0; i < ssinfoCount; i++ )
				{
					// 消息解码//
					if ( 0 == askRegisteRet.Ssinfo[i].Ssid )
						continue;

					if ( GSKernel.instance.ContainsSSInfo( askRegisteRet.Ssinfo[i].Ssid ) )
						continue;

					GSSSInfo ssInfo = new GSSSInfo();
					ssInfo.m_n32SSID = askRegisteRet.Ssinfo[i].Ssid;
					ssInfo.m_sListenIP = askRegisteRet.Ssinfo[i].Ip.Replace( "\0", string.Empty );
					ssInfo.m_n32ListenPort = askRegisteRet.Ssinfo[i].Port;
					ssInfo.m_eSSNetState = ( EServerNetState )askRegisteRet.Ssinfo[i].Netstate;
					ssInfo.m_n32NSID = 0;
					ssInfo.m_un32ConnTimes = 0;
					ssInfo.m_tLastConnMilsec = 0;
					ssInfo.m_tPingTickCounter = 0;
					ssInfo.m_n64MsgReceived = 0;
					ssInfo.m_n64MsgSent = 0;
					ssInfo.m_n64DataReceived = 0;
					ssInfo.m_n64DataSent = 0;
					GSKernel.instance.AddGSSInfo( ssInfo.m_n32SSID, ssInfo );
					if ( ssInfo.m_eSSNetState == EServerNetState.SnsClosed )
						continue;

					this.owner.CreateConnector( SessionType.ClientG2S, ssInfo.m_sListenIP, ssInfo.m_n32ListenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240, ssInfo.m_n32SSID );
					++GSKernel.instance.ssConnectNum;
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

			GSSSInfo pcSSInfo = GSKernel.instance.GetGSSSInfo( oneSsConnected.Ssid );
			if ( pcSSInfo != null )
			{
				pcSSInfo.m_sListenIP = oneSsConnected.Ip.Replace( "\0", string.Empty ); ;
				pcSSInfo.m_n32ListenPort = oneSsConnected.Port;
				if ( pcSSInfo.m_eSSNetState == EServerNetState.SnsClosed )
				{
					pcSSInfo.m_eSSNetState = ( EServerNetState )oneSsConnected.Netstate;
					pcSSInfo.m_n32NSID = 0;
					this.owner.CreateConnector( SessionType.ClientG2S, pcSSInfo.m_sListenIP, pcSSInfo.m_n32ListenPort,
												Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 10240, oneSsConnected.Ssid );
				}
			}
			return true;
		}

		protected override bool OnUnknowMsg( byte[] data, int offset, int size, int msgID )
		{
			int n32RealMsgID = 0;
			uint n32GcNetID = 0;
			offset += ByteUtils.Decode32i( data, offset, ref n32RealMsgID );
			offset += ByteUtils.Decode32u( data, offset, ref n32GcNetID );
			size -= 2 * sizeof( int );
			GSKernel.instance.csMsgHandler.HandleUnhandledMsg( data, offset, size, n32RealMsgID, msgID, n32GcNetID );
			return true;
		}
		#endregion
	}
}