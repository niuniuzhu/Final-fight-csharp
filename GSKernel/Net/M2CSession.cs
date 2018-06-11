using Core;
using Google.Protobuf;
using Shared.Net;

namespace GateServer.Net
{
	public class M2CSession : CliSession
	{
		protected M2CSession( int id ) : base( id )
		{
		}

		protected override void SendInitData()
		{
			GSKernel.instance.ResetlastPingCSTickCounter();
			Logger.Info( "CS Connected, try to register me." );
			GSToCS.AskRegiste askRegiste = new GSToCS.AskRegiste();
			askRegiste.Port = GSKernel.instance.gsConfig.n32GCListenPort;
			askRegiste.Ip = GSKernel.instance.gsConfig.sGCListenIP;
			askRegiste.Gsid = GSKernel.instance.gsConfig.n32GSID;
			askRegiste.Usepwd = GSKernel.instance.gsConfig.aszMyUserPwd;
			ByteString bs = askRegiste.ToByteString();
			GSKernel.instance.TransToCS( bs, ( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste, 0, 0 );
		}
	}
}