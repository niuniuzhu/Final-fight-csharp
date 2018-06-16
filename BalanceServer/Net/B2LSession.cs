using Shared.Net;

namespace BalanceServer.Net
{
	public class B2LSession : CliSession
	{
		public B2LSession( uint id ) : base( id )
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