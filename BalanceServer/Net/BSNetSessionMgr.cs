using Core.Net;
using Shared.Net;
using System;

namespace BalanceServer.Net
{
	public class BSNetSessionMgr : NetSessionMgr
	{
		internal SrvCliSession CreateGateSession()
		{
			GateSession session = NetSessionPool.instance.Pop<GateSession>();
			session.owner = this;
			session.type = SessionType.ServerGS;
			return session;
		}

		internal SrvCliSession CreateClientSession()
		{
			ClientSession session = NetSessionPool.instance.Pop<ClientSession>();
			session.owner = this;
			session.type = SessionType.ServerGS;
			return session;
		}

		protected override CliSession CreateConnectorSession( SessionType sessionType )
		{
			CliSession session;
			switch ( sessionType )
			{
				case SessionType.ClientB2L:
					session = NetSessionPool.instance.Pop<B2LSession>();
					session.owner = this;
					break;

				default:
					throw new NotImplementedException();
			}
			session.type = sessionType;
			return session;
		}
	}
}