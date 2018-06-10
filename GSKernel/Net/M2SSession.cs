using Net;

namespace GateServer.Net
{
	public class M2SSession : CliSession
	{
		protected M2SSession( int id ) : base( id )
		{
		}

		protected override void SendInitData()
		{
		}
	}
}