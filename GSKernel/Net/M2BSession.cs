using Core;
using Google.Protobuf;
using GSToBS;
using Net;

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
			GSToBS.AskRegister askRegister = new AskRegister();
			//askRegister.Mgsid = MsgID.EMsgToBsfromGsAskRegister;
			askRegister.Gsid = GSKernel.instance.gsConfig.n32GSID;
			askRegister.Listenport = GSKernel.instance.gsConfig.n32GCListenPort;
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ClientG2B, 0, askRegister, ( int )MsgID.EMsgToBsfromGsAskRegister );
		}
	}
}