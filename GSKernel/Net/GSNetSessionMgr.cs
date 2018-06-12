using Core.Net;
using Shared.Net;
using System;

namespace GateServer.Net
{
	public class GSNetSessionMgr : NetSessionMgr
	{
		protected override SrvCliSession CreateListenerSession()
		{
			ClientSession session = SessionPool.instance.Pop<ClientSession>();
			session.owner = this;
			session.type = SessionType.ServerGS;
			return session;
		}

		protected override CliSession CreateConnectorSession( SessionType sessionType )
		{
			CliSession session;
			switch ( sessionType )
			{
				case SessionType.ClientG2C:
					session = SessionPool.instance.Pop<M2CSession>();
					session.owner = this;
					break;

				case SessionType.ClientG2S:
					session = SessionPool.instance.Pop<M2SSession>();
					session.owner = this;
					break;

				case SessionType.ClientG2B:
					session = SessionPool.instance.Pop<M2BSession>();
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