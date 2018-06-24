using System.Collections.Generic;

namespace Shared.Net
{
	/// <summary>
	/// 转发消息处理器的容器
	/// </summary>
	public class MsgCenter
	{
		public delegate ErrorCode MsgHandler( byte[] data, int offset, int size, int msgID );
		public delegate ErrorCode TransHandler( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID );

		private readonly Dictionary<int, MsgHandler> _msgHandlers = new Dictionary<int, MsgHandler>();
		private readonly Dictionary<int, TransHandler> _transHandlers = new Dictionary<int, TransHandler>();

		public void Register( int msgID, MsgHandler handler )
		{
			this._msgHandlers[msgID] = handler;
		}

		public void Register( int msgID, TransHandler handler )
		{
			this._transHandlers[msgID] = handler;
		}

		public bool TryGetHandler( int msgID, out MsgHandler handler )
		{
			return this._msgHandlers.TryGetValue( msgID, out handler );
		}

		public bool TryGetHandler( int msgID, out TransHandler handler )
		{
			return this._transHandlers.TryGetValue( msgID, out handler );
		}
	}
}