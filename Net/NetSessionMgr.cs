namespace Net
{
	public class NetSessionMgr
	{
		private const int MAX_COUNT_LISTENER = 3;

		private readonly IListener[] _listeners = new IListener[MAX_COUNT_LISTENER];

		public bool CreateListener( int port, int recvBufSize, int pos, ListenerFactory.ListenerType type )
		{
			if ( pos >= MAX_COUNT_LISTENER ) return false;
			if ( this._listeners[pos] != null ) return false;
			this._listeners[pos] = ListenerFactory.Create( type );
			this._listeners[pos].sessionCreateHandler = () => SessionPool.instance.Pop<Session>( SessionType.ServerGS );
			this._listeners[pos].packetEncodeHandler = LengthEncoder.Decode;
			this._listeners[pos].recvBufSize = recvBufSize;
			return this._listeners[pos].Start( "0", port );
		}
	}
}