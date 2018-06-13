using Core;
using Google.Protobuf;
using Shared.Net;

namespace GateServer.Net
{
	public class M2BSession : CliSession
	{
		protected M2BSession( uint id ) : base( id )
		{
			this._msgCenter.RegisterMsgFunc( ( int )BSToGS.MsgID.EMsgToGsfromBsAskRegisterRet, this.MsgInitHandler );
			this._msgCenter.RegisterMsgFunc( ( int )BSToGS.MsgID.EMsgToGsfromBsOneUserLoginToken, this.MsgOneUserLoginTokenHandler );
		}

		protected override void SendInitData()
		{
			Logger.Info( "BS Connected, try to register me." );
			GSToBS.AskRegister askRegister = new GSToBS.AskRegister
			{
				Gsid = GSKernel.instance.gsConfig.n32GSID,
				Listenport = GSKernel.instance.gsConfig.n32GCListenPort
			};
			GSKernel.instance.netSessionMrg.SendMsgToSession( this.id, askRegister, ( int )GSToBS.MsgID.EMsgToBsfromGsAskRegister );
		}

		protected override void OnRealEstablish()
		{
			Logger.Info( "BS Connected and register ok." );
		}

		protected override void OnClose()
		{
			Logger.Info( "BS DisConnect." );
		}

		#region msg handlers
		private bool MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			this.SetInited( true, true );
			return true;
		}

		private bool MsgOneUserLoginTokenHandler( byte[] data, int offset, int size, int msgID )
		{
			BSToGS.OneUserLoginToken reportAllClientInf = new BSToGS.OneUserLoginToken();
			reportAllClientInf.MergeFrom( data, offset, size );

			GSKernel.instance.userTokenMgr.AddUserToken( reportAllClientInf.UserName, reportAllClientInf.Token );
			this.owner.SendMsgToSession( this.id, reportAllClientInf, ( int )GSToBS.MsgID.EMsgToBsfromGsOneUserLoginTokenRet );
			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			return true;
		}
		#endregion
	}
}