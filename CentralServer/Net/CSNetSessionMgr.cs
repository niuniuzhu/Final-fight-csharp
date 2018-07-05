using Core.Net;
using Shared.Net;
using System;
using System.Net.Sockets;

namespace CentralServer.Net
{
	public class CSNetSessionMgr : NetSessionMgr
	{
		public override bool CreateConnector( SessionType sessionType, string ip, int port, SocketType socketType, ProtocolType protoType, int recvsize, int logicId )
		{
			CliSession session = this.CreateConnectorSession( sessionType );
			this.AddSession( session );
			session.logicID = logicId;
			session.connector.recvBufSize = recvsize;
			session.connector.packetDecodeHandler = LengthEncoder.Decode;
			return session.Connect( ip, port, socketType, protoType );
		}

		internal SrvCliSession CreateGateSession()
		{
			SrvCliSession session = NetSessionPool.instance.Pop<GateSession>();
			session.owner = this;
			session.type = SessionType.ServerCsOnlyGS;
			return session;
		}

		internal SrvCliSession CreateSceneSession()
		{
			SrvCliSession session = NetSessionPool.instance.Pop<SceneSession>();
			session.owner = this;
			session.type = SessionType.ServerCsOnlySs;
			return session;
		}

		internal SrvCliSession CreateRemoteConsoleSession()
		{
			SrvCliSession session = NetSessionPool.instance.Pop<RemoteConsoleSession>();
			session.owner = this;
			session.type = SessionType.ServerCsOnlyRc;
			return session;
		}

		protected override CliSession CreateConnectorSession( SessionType sessionType )
		{
			CliSession session;
			switch ( sessionType )
			{
				case SessionType.ClientC2Log:
					session = NetSessionPool.instance.Pop<CSLogSession>();
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