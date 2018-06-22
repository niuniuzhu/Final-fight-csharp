using Shared;
using System.Collections.Generic;

namespace CentralServer.Net
{
	public class SSMsgManager
	{
		private delegate EResult MsgHandler( CSSSInfo cpiGSInfo, byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public SSMsgManager()
		{
			#region 注册消息处理函数
			this._handlers[( int )SSToCS.MsgID.EMsgToCsfromSsAskPing] = this.OnMsgFromSSAskPing;
			#endregion
		}

		private EResult OnMsgFromSSAskPing( CSSSInfo cpigsinfo, byte[] data, int offset, int size )
		{
			throw new System.NotImplementedException();
		}
	}
}