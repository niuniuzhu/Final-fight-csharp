using System;

namespace Net
{
	public static class ListenerFactory
	{
		public enum ListenerType
		{
			TCP,
			KCP
		}

		public static IListener Create( ListenerType type )
		{
			switch ( type )
			{
				case ListenerType.TCP:
					return new TCPListener();
				case ListenerType.KCP:
					break;
			}
			throw new Exception( "not support listener type" );
		}
	}
}