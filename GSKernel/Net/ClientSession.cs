using Core;
using Shared.Net;

namespace GateServer.Net
{
	public abstract class ClientSession : SrvCliSession
	{
		private bool _logicInited;

		protected ClientSession( uint id ) : base( id )
		{
			this._handlerContainer.Register( ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcBegin, this.MsgInitHandler );
		}

		protected override void OnRealEstablish()
		{
			if ( this._logicInited )
				return;
			this._logicInited = true;
			Logger.Log( $"client({this.id})({this.connection.socket.RemoteEndPoint}) connected." );
		}

		protected override void OnClose()
		{
			if ( this._logicInited )
			{
				Logger.Log( $"client({this.id})({this.connection.socket.RemoteEndPoint}) disconnected." );
				GSKernel.instance.gsStorage.OnUserLost( this.id );
				this._logicInited = false;
			}
		}

		#region msg handlers
		private bool MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			this.SetInited( true, true );
			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			if ( !this._logicInited )
				this.SetInited( true, true );
			GSKernel.instance.gcMsgManager.HandleUnhandledMsg( this.id, data, offset, size, msgID );
			return true;
		}
		#endregion
	}
}