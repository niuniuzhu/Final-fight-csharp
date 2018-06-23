using System.Collections.Generic;
using Shared;

namespace CentralServer.Net
{
	public class GCMsgManager
	{
		private delegate ErrorCode MsgHandler( CSGSInfo cpiGSInfo, uint gcnsID, byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();
	}
}