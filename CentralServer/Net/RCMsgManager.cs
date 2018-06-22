using System.Collections.Generic;
using Shared;

namespace CentralServer.Net
{
	public class RCMsgManager
	{
		private delegate EResult MsgHandler( CSRCInfo cpiGSInfo, byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();
	}
}