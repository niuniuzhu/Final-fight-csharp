using Core;
using Shared.Net;
using AskRegister = GSToBS.AskRegister;
using MsgID = GSToBS.MsgID;

namespace GateServer.Net
{
	public class M2BSession : CliSession
	{
		protected M2BSession( uint id ) : base( id )
		{
		}

		protected override void SendInitData()
		{
			Logger.Info( "BS Connected, try to register me." );
			AskRegister askRegister = new AskRegister
			{
				Gsid = GSKernel.instance.gsConfig.n32GSID,
				Listenport = GSKernel.instance.gsConfig.n32GCListenPort
			};
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ClientG2B, 0, askRegister, ( int )MsgID.EMsgToBsfromGsAskRegister );
		}

		protected override void OnRealEstablish()
		{
			throw new System.NotImplementedException();
		}

		protected override void OnClose()
		{
			throw new System.NotImplementedException();
		}

		#region msg handlers
		private bool MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			this.SetInited( true, true );
			return true;
		}

		private bool MsgOneUserLoginTokenHandler( byte[] data, int offset, int size, NetSession vthis, int n32MsgID )
		{
			//OneUserLoginToken pReportAllClientInf = new OneUserLoginToken();
			//pReportAllClientInf.MergeFrom( data );

			//GSKernel.instance.AddUserToken( pReportAllClientInf.UserName, pReportAllClientInf.Token );
			//GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ClientG2B, 0, pReportAllClientInf, ( int )MsgID.EMsgToBsfromGsOneUserLoginTokenRet );
			return true;
		}

		protected override bool OnUnknowMsg( byte[] data, int offset, int size, int msgID )
		{
			return true;
		}
		#endregion
	}
}