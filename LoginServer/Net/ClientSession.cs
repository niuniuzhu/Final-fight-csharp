using Google.Protobuf;
using Shared.Net;

namespace LoginServer.Net
{
	public class ClientSession : SrvCliSession
	{
		protected ClientSession( uint id ) : base( id )
		{
			this._msgHandler.Register( ( int )GCToLS.MsgID.EMsgToLsfromGcAskLogin, this.MsgInitHandler );
		}

		protected override void SendInitData()
		{
		}

		protected override void OnRealEstablish()
		{
		}

		protected override void OnClose()
		{
		}

		public override void OnError( string error )
		{
			this.Close();
		}

		private bool MsgInitHandler( byte[] data, int offset, int size, int msgid )
		{
			//收到第1消息：请求登录，放入登录队列
			GCToLS.AskLogin login = new GCToLS.AskLogin();
			login.MergeFrom( data, offset, size );

			LS.instance.sdkAsynHandler.CheckLogin( login, ( int )GCToLS.MsgID.EMsgToLsfromGcAskLogin, this.id );
			this.SetInited( true, true );
			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			return true;
		}
	}
}