using System.Collections.Generic;

namespace Shared.Net
{
	/// <summary>
	/// 消息处理中心
	/// </summary>
	public class MsgCenter
	{
		public delegate ErrorCode GeneralHandler( byte[] data, int offset, int size, int msgID );
		public delegate ErrorCode TransHandler( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID );

		private readonly Dictionary<int, GeneralHandler> _generalHandlers = new Dictionary<int, GeneralHandler>();
		private readonly Dictionary<int, TransHandler> _transHandlers = new Dictionary<int, TransHandler>();

		public void Register( int msgID, GeneralHandler handler )
		{
			if ( this._generalHandlers.ContainsKey( msgID ) )
				return;
			this._generalHandlers[msgID] = handler;
		}

		public void Register( int msgID, TransHandler handler )
		{
			if ( this._transHandlers.ContainsKey( msgID ) )
				return;
			this._transHandlers[msgID] = handler;
		}

		public bool TryGetHandler( int msgID, out GeneralHandler handler )
		{
			return this._generalHandlers.TryGetValue( msgID, out handler );
		}

		public bool TryGetHandler( int msgID, out TransHandler handler )
		{
			return this._transHandlers.TryGetValue( msgID, out handler );
		}
	}
}