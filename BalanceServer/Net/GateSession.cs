using Core.Misc;
using Google.Protobuf;
using Shared.Net;

namespace BalanceServer.Net
{
	public class GateSession : SrvCliSession
	{
		public GateSession( uint id ) : base( id )
		{
			this._msgHandler.Register( ( int )GSToBS.MsgID.EMsgToBsfromGsAskRegister, this.MsgAskRegister );
			this._msgHandler.Register( ( int )GSToBS.MsgID.EMsgToBsfromGsReportAllClientInfo, this.MsgHandleReportGsInfo );
			this._msgHandler.Register( ( int )GSToBS.MsgID.EMsgToBsfromGsOneUserLoginTokenRet, this.MsgHandleOneUserLoginTokenRet );
		}

		protected override void SendInitData()
		{
			BSToGS.AskRegister sAskRegister = new BSToGS.AskRegister();
			this.owner.SendMsgToSession( this.id, sAskRegister, ( int )BSToGS.MsgID.EMsgToGsfromBsAskRegisterRet );
		}

		protected override void OnRealEstablish()
		{
			Logger.Info( $"GS({this.logicID}) Connected." );
		}

		protected override void OnClose()
		{
			if ( !BS.instance.bsConfig.gAllGsInfo.TryGetValue( ( uint )this.logicID, out sOneGsInfo gsInfo ) )
				return;

			gsInfo.gs_isLost = true;
			gsInfo.gs_nets = 0;
			Logger.Info( $"GS({this.logicID}) DisConnect." );
		}

		private bool MsgAskRegister( byte[] data, int offset, int size, int msgid )
		{
			GSToBS.AskRegister sAskRegister = new GSToBS.AskRegister();
			sAskRegister.MergeFrom( data, offset, size );

			int gsid = sAskRegister.Gsid;
			int gsListener = sAskRegister.Listenport;

			if ( !BS.instance.bsConfig.gAllGsInfo.TryGetValue( ( uint )this.logicID, out sOneGsInfo gsInfo ) )
			{
				this.Close();
				return true;
			}

			if ( !gsInfo.gs_isLost )
			{
				this.Close();
				return true;
			}

			if ( gsInfo.gs_Port != gsListener || gsInfo.gs_Ip != this.connection.remoteEndPoint.ToString() )
			{
				this.Close();
				return true;
			}

			gsInfo.gs_isLost = false;
			gsInfo.gs_nets = this.id;
			this.logicID = gsid;
			this.SetInited( true, true );
			return true;
		}

		private bool MsgHandleReportGsInfo( byte[] data, int offset, int size, int msgid )
		{
			if ( !BS.instance.bsConfig.gAllGsInfo.TryGetValue( ( uint )this.logicID, out sOneGsInfo gsInfo ) )
				return false;
			GSToBS.ReportAllClientInf sMsg = new GSToBS.ReportAllClientInf();
			sMsg.MergeFrom( data, offset, size );
			gsInfo.gs_gc_count = sMsg.TokenlistSize;
			return true;
		}

		private bool MsgHandleOneUserLoginTokenRet( byte[] data, int offset, int size, int msgid )
		{
			if ( !BS.instance.bsConfig.gAllGsInfo.TryGetValue( ( uint )this.logicID, out sOneGsInfo gsInfo ) )
			{
				Logger.Error( "can not find GS for loginer." );
				return false;
			}

			BSToGS.OneUserLoginToken sMsg = new BSToGS.OneUserLoginToken();
			sMsg.MergeFrom( data, offset, size );
			BSToGC.AskGateAddressRet sMsgSend = new BSToGC.AskGateAddressRet
			{
				UserName = sMsg.UserName,
				Token = sMsg.Token,
				Ip = sMsg.Ip,
				Port = sMsg.Port
			};
			this.owner.SendMsgToSession( ( uint )sMsg.Gateclient, sMsgSend, ( int )BSToGC.MsgID.EMsgToGcfromBsAskGateAddressRet );
			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			return true;
		}
	}
}