using System.Collections.Generic;

namespace Shared.Net
{
	public delegate bool MsgHandler( byte[] data, int offset, int size, int msgID );

	/// <summary>
	/// 消息处理器的容器
	/// </summary>
	public class HandlerContainer
	{
		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public void Register( int msgID, MsgHandler handler )
		{
			this._handlers[msgID] = handler;
		}

		public bool Contains( int msgID )
		{
			return this._handlers.ContainsKey( msgID );
		}

		public bool TryGetHandler( int msgID, out MsgHandler handler )
		{
			return this._handlers.TryGetValue( msgID, out handler );
		}

		public void Clear()
		{
			this._handlers.Clear();
		}
	}
}