using Core;
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
			this._msgCenter.RegisterMsgFunc( ( int )SSToGS.MsgID.EMsgToGsfromSsAskRegisteRet, this.MsgInitHandler );
		}

		protected override void SendInitData()
		{
			GSSSInfo ssInfo = GSKernel.instance.GetGSSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.m_n32SSID}) Connected, try to register me." );
			ssInfo.m_n32NSID = this.id;
			GSToSS.AskRegiste askRegiste = new GSToSS.AskRegiste()
			{
				Gsid = GSKernel.instance.gsConfig.n32GSID,
				Pwd = GSKernel.instance.gsConfig.aszMyUserPwd
			};
			byte[] data = askRegiste.ToByteArray();
			this.owner.TranMsgToSession( SessionType.ClientG2S, ssInfo.m_n32NSID, data, 0, data.Length, ( int )GSToSS.MsgID.EMsgToSsfromGsAskRegiste, 0, 0 );
		}

		protected override void OnRealEstablish()
		{
			GSSSInfo ssInfo = GSKernel.instance.GetGSSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.m_n32SSID}) Connected and register ok." );
			ssInfo.m_n32NSID = this.id;
			ssInfo.m_tLastConnMilsec = TimeUtils.utcTime;
			ssInfo.m_tPingTickCounter = 0;
		}

		protected override void OnClose()
		{
			GSSSInfo ssInfo = GSKernel.instance.GetGSSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.m_n32SSID}) DisConnect." );
			//GSKernel.instance.OnEvent( EVT_ON_SS_DISCONNECT, pcSSInfo ); //todo
			ssInfo.m_n32NSID = 0;
		}

		#region msg handlers
		private bool MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			// don't send any message until it init success.
			GSSSInfo pcSSInfo = GSKernel.instance.GetGSSSInfo( this.logicID );
			if ( pcSSInfo == null || data == null )
			{
				Logger.Error( string.Empty );
				return false;
			}

			offset += 2 * sizeof( int );
			size -= 2 * sizeof( int );
			SSToGS.AskRegisteRet askRegisteRet = new SSToGS.AskRegisteRet();
			askRegisteRet.MergeFrom( data, offset, size );

			if ( ( int )EResult.Normal != askRegisteRet.State )
			{
				Logger.Warn( $"register to SS {pcSSInfo.m_n32SSID} Fail with error code {askRegisteRet.State}." );
				return false;
			}

			pcSSInfo.m_eSSNetState = EServerNetState.SnsFree;
			Logger.Info( $"register to SS {pcSSInfo.m_n32SSID} success at session {pcSSInfo.m_n32NSID}." );
			this.SetInited( true, true );

			return true;
		}

		protected override bool OnUnknowMsg( byte[] data, int offset, int size, int msgID )
		{
			int n32RealMsgID = 0;
			uint n32GcNetID = 0;
			offset += ByteUtils.Decode32i( data, offset, ref n32RealMsgID );
			offset += ByteUtils.Decode32u( data, offset, ref n32GcNetID );
			size -= 2 * sizeof( int );
			GSSSInfo pcSSInfo = GSKernel.instance.GetGSSSInfo( this.logicID );
			if ( pcSSInfo != null )
				GSKernel.instance.ssMsgHandler.HandleUnhandledMsg( pcSSInfo, data, offset, size, n32RealMsgID, msgID, n32GcNetID );
			return true;
		}
		#endregion
	}
}