using Shared.Net;

namespace CentralServer.Net
{
	public class GateSession: SrvCliSession
	{
		public GateSession( uint id ) : base( id )
		{
		}

		protected override void SendInitData()
		{
			throw new System.NotImplementedException();
		}

		protected override void OnRealEstablish()
		{
			throw new System.NotImplementedException();
		}

		protected override void OnClose()
		{
			throw new System.NotImplementedException();
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			throw new System.NotImplementedException();
		}
	}
}