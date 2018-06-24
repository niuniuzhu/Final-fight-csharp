using Shared;
using Shared.Net;

namespace CentralServer.Net
{
	public class LogicRedisSession : CliSession
	{
		protected LogicRedisSession( uint id ) : base( id )
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
	}
}