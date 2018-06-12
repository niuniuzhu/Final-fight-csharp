using System.Collections.Generic;

namespace Shared.Net
{
	public delegate bool MsgHandler( byte[] data, int offset, int size, int msgID );

	public class MsgCenter
	{
		private readonly Dictionary<int, MsgHandler> _nodes = new Dictionary<int, MsgHandler>();

		public void RegisterMsgFunc( int msgID, MsgHandler handler )
		{
			this._nodes[msgID] = handler;
		}

		public bool Contains( int msgID )
		{
			return this._nodes.ContainsKey( msgID );
		}

		public bool TryGetHandler( int msgID, out MsgHandler handler )
		{
			return this._nodes.TryGetValue( msgID, out handler );
		}

		public void Clear()
		{
			this._nodes.Clear();
		}
	}
}