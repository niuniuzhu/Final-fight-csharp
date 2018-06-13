using Shared.Net;

namespace GateServer.Net
{
	public abstract class ClientSession : SrvCliSession
	{
		private bool _logicInited;

		protected ClientSession( uint id ) : base( id )
		{
		}

		#region msg handlers
		private bool MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			this.SetInited( true, true );
			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			//ClientSession cliSession = ( ClientSession )vthis;
			//if ( !cliSession._logicInited )
			//{
			//	cliSession.SetInited( true, true );
			//}
			//GSKernel.instance.HandleMsgFromGC( cliSession.id, data, n32MsgID );
			return true;
		}
		#endregion
	}
}