using Core.Net;
using Shared.Net;

namespace LoginServer.Net
{
	public class LSNetSessionMgr : NetSessionMgr
	{
		public INetSession CreateBlanceSession()
		{
			BalanceSession session = NetSessionPool.instance.Pop<BalanceSession>();
			session.owner = this;
			session.type = SessionType.ServerLsOnlyBS;
			return session;
		}

		public INetSession CreateClientSession()
		{
			ClientSession session = NetSessionPool.instance.Pop<ClientSession>();
			session.owner = this;
			session.type = SessionType.ServerLsOnlyGc;
			return session;
		}

		protected override CliSession CreateConnectorSession( SessionType sessionType )
		{
			return null;
		}
	}
}