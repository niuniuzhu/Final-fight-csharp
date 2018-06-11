using Core;
using GSToBS;
using Shared.Net;

namespace GateServer.Net
{
	public class M2BSession : CliSession
	{
		protected M2BSession( int id ) : base( id )
		{
		}

		protected override void SendInitData()
		{
			Logger.Info( "BS Connected, try to register me." );
			GSToBS.AskRegister askRegister = new AskRegister
			{
				Gsid = GSKernel.instance.gsConfig.n32GSID,
				Listenport = GSKernel.instance.gsConfig.n32GCListenPort
			};
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ClientG2B, 0, askRegister, ( int )MsgID.EMsgToBsfromGsAskRegister );
		}
	}
}