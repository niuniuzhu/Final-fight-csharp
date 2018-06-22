using Shared;
using System.Collections.Generic;

namespace CentralServer.Net
{
	public class GSMsgManager
	{
		private delegate EResult MsgHandler( CSGSInfo cpiGSInfo, byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public GSMsgManager()
		{
			#region 注册消息处理函数
			this._handlers[( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste] = this.OnMsgFromGSAskRegiste;
			this._handlers[( int )GSToCS.MsgID.EMsgToCsfromGsAskPing] = this.OnMsgFromGSAskPing;
			this._handlers[( int )GSToCS.MsgID.EMsgToCsfromGsReportGcmsg] = this.OnMsgFromGSReportGCMsg;
			#endregion
		}

		private EResult OnMsgFromGSAskRegiste( CSGSInfo cpigsinfo, byte[] data, int offset, int size )
		{
			throw new System.NotImplementedException();
		}

		private EResult OnMsgFromGSAskPing( CSGSInfo cpigsinfo, byte[] data, int offset, int size )
		{
			throw new System.NotImplementedException();
		}

		private EResult OnMsgFromGSReportGCMsg( CSGSInfo cpigsinfo, byte[] data, int offset, int size )
		{
			throw new System.NotImplementedException();
		}
	}
}