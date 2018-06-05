using System.Collections.Generic;

namespace Net
{
	public enum NetworkProtoType
	{
		TCP,
		KCP
	}

	public class NetSessionMgr
	{
		private readonly List<IListener> _listeners = new List<IListener>();
		private readonly Dictionary<int, ISession> _idToSession = new Dictionary<int, ISession>();

		public bool CreateListener( NetworkProtoType protoType, int port, int recvBufSize, out int pos )
		{
			pos = this._listeners.Count;
			IListener listener = this.CreateListener( protoType );
			listener.sessionCreateHandler = () => SessionPool.instance.Pop<Session>( SessionType.ServerGS );
			listener.packetEncodeHandler = LengthEncoder.Decode;
			listener.recvBufSize = recvBufSize;
			this._listeners.Add( listener );
			return this._listeners[pos].Start( "0", port );
		}

		public bool CreateConnector( NetworkProtoType protoType, SessionType sessionType, string ip, int port, int recvsize, int logicId )
		{
			IConnSession session = this.CreateConnectorSession( protoType, sessionType );
			session.logicID = logicId;
			session.recvBufSize = recvsize;
			session.packetEncodeHandler = LengthEncoder.Decode;
			return session.Connect( protoType, ip, port );
		}

		private IListener CreateListener( NetworkProtoType protoType )
		{
			switch ( protoType )
			{
				case NetworkProtoType.TCP:
					return new TCPListener();

				case NetworkProtoType.KCP:
					return null;

				default:
					throw new System.Exception( "not support listener type" );
			}
		}

		private IConnSession CreateConnectorSession( NetworkProtoType protoType, SessionType sessionType )
		{
			IConnSession session;
			switch ( protoType )
			{
				case NetworkProtoType.TCP:
					session = SessionPool.instance.Pop<TCPConnSession>( sessionType );
					break;

				case NetworkProtoType.KCP:
					session = null;
					break;

				default:
					throw new System.Exception( "not support listener type" );
			}

			this._idToSession.Add( session.id, session );
			return session;
		}
	}
}