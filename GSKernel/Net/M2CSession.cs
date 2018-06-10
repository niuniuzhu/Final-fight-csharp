using Net;

namespace GateServer.Net
{
	public class M2CSession : CliSession
	{
		protected M2CSession( int id ) : base( id )
		{
		}

		protected override void SendInitData()
		{
		}
	}
}