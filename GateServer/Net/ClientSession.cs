using Core.Misc;
using Shared;
using Shared.Net;

namespace GateServer.Net
{
	public class ClientSession : SrvCliSession
	{
		private bool _logicInited;

		protected ClientSession( uint id ) : base( id )
		{
			this._msgHandler.Register( ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcBegin, this.MsgInitHandler );
		}

		protected override void SendInitData()
		{
		}

		protected override void OnRealEstablish()
		{
			if ( this._logicInited )
				return;
			this._logicInited = true;
			Logger.Log( $"client({this.id})({this.connection.remoteEndPoint}) connected." );
		}

		protected override void OnClose()
		{
			if ( this._logicInited )
			{
				Logger.Log( $"client({this.id})({this.connection.remoteEndPoint}) disconnected." );
				GS.instance.gsStorage.OnUserLost( this.id );
				this._logicInited = false;
			}
		}

		#region msg handlers
		private ErrorCode MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			this.SetInited( true, true );
			return ErrorCode.Success;
		}

		protected override ErrorCode HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			if ( !this._logicInited )
				this.SetInited( true, true );
			GS.instance.gcMsgManager.HandleUnhandledMsg( this.id, data, offset, size, msgID );
			return ErrorCode.Success;
		}
		#endregion
	}
}