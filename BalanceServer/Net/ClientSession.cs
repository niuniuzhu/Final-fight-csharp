using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace BalanceServer.Net
{
	public class ClientSession : SrvCliSession
	{
		protected ClientSession( uint id ) : base( id )
		{
			 this.msgCenter.Register( ( int )GCToBS.MsgNum.EMsgToBsfromGcOneClinetLogin, this.MSGOneClientLogin );
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

		private ErrorCode MSGOneClientLogin( byte[] data, int offset, int size, int msgID )
		{
			//收到第2消息：客户端连接BS，向LS请求客户端是否合法登陆
			GCToBS.OneClinetLogin oneClientLogin = new GCToBS.OneClinetLogin();
			oneClientLogin.MergeFrom( data, offset, size );

			Logger.Log( $"user({oneClientLogin.Uin})({oneClientLogin.Sessionid})({this.id}) ask login bs" );
			oneClientLogin.Nsid = this.id;

			this.owner.SendMsgToSession( SessionType.ClientB2L, oneClientLogin, ( int )BSToLS.MsgID.EMsgToLsfromBcOneClinetLoginCheck );

			return ErrorCode.Success;
		}
	}
}