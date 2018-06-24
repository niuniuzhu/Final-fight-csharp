using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace BalanceServer.Net
{
	public class GateSession : SrvCliSession
	{
		protected GateSession( uint id ) : base( id )
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
			if ( !BS.instance.bsConfig.allGsInfo.TryGetValue( this.logicID, out OneGsInfo gsInfo ) )
				return;

			gsInfo.gs_isLost = true;
			gsInfo.gs_nets = 0;
			Logger.Info( $"GS({this.logicID}) DisConnect." );
		}

		/// <summary>
		/// 登记GS
		/// </summary>
		private ErrorCode MsgAskRegister( byte[] data, int offset, int size, int msgID )
		{
			GSToBS.AskRegister askRegister = new GSToBS.AskRegister();
			askRegister.MergeFrom( data, offset, size );

			//GS的分配号
			int gsid = askRegister.Gsid;
			int gsListener = askRegister.Listenport;

			//找到对应分配号的GS信息
			if ( !BS.instance.bsConfig.allGsInfo.TryGetValue( gsid, out OneGsInfo gsInfo ) )
			{
				this.Close();
				return ErrorCode.InvalidGSID;
			}

			if ( !gsInfo.gs_isLost ||
				 gsInfo.gs_Port != gsListener ||
				 gsInfo.gs_Ip != this.connection.remoteEndPoint.ToString().Split( ':' )[0] )
			{
				this.Close();
				return ErrorCode.GSNotFound;
			}

			gsInfo.gs_isLost = false;
			gsInfo.gs_nets = this.id;
			this.logicID = gsid;
			this.SetInited( true, true );

			return ErrorCode.Success;
		}

		/// <summary>
		/// 向BS汇报GS的状态
		/// </summary>
		private ErrorCode MsgHandleReportGsInfo( byte[] data, int offset, int size, int msgID )
		{
			if ( !BS.instance.bsConfig.allGsInfo.TryGetValue( this.logicID, out OneGsInfo gsInfo ) )
				return ErrorCode.GSNotFound;

			GSToBS.ReportAllClientInf msg = new GSToBS.ReportAllClientInf();
			msg.MergeFrom( data, offset, size );
			gsInfo.gs_gc_count = msg.TokenlistSize;

			return ErrorCode.Success;
		}

		/// <summary>
		/// GS回应客户端已登陆
		/// </summary>
		private ErrorCode MsgHandleOneUserLoginTokenRet( byte[] data, int offset, int size, int msgID )
		{
			if ( !BS.instance.bsConfig.allGsInfo.TryGetValue( this.logicID, out OneGsInfo _ ) )
			{
				Logger.Error( "can not find GS for loginer." );
				return ErrorCode.GSNotFound;
			}

			BSToGS.OneUserLoginToken msg = new BSToGS.OneUserLoginToken();
			msg.MergeFrom( data, offset, size );
			BSToGC.AskGateAddressRet msgSend = new BSToGC.AskGateAddressRet
			{
				UserName = msg.UserName,
				Token = msg.Token,
				//这是网关的地址
				Ip = msg.Ip,
				Port = msg.Port
			};
			//通知客户端GS的地址
			this.owner.SendMsgToSession( ( uint )msg.Gateclient, msgSend, ( int )BSToGC.MsgID.EMsgToGcfromBsAskGateAddressRet );

			return ErrorCode.Success;
		}

	}
}