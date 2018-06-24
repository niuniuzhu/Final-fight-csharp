using Google.Protobuf;
using Shared;
using Shared.Net;

namespace LoginServer.Net
{
	public class ClientSession : SrvCliSession
	{
		protected ClientSession( uint id ) : base( id )
		{
			//完整登陆流程:
			//1,客户端连接LS,请求登陆
			//2,LS下发BS列表
			//3,客户端连接BS,请求登陆
			//4,BS请求LS验证登陆合法性
			//5,LS返回验证结果
			//6,BS处理结果,不合法则断开连接,合法则找出空闲GS,发送登陆信息
			//7,GS回应BS客户端已登陆
			//8,BS通知客户端GS地址
			//9,客户端连接GS
			//10,GS把登陆信息转发到CS
			 this._msgCenter.Register( ( int )GCToLS.MsgID.EMsgToLsfromGcAskLogin, this.MsgInitHandler );
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

		private ErrorCode MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			//收到第1消息：请求登录，放入登录队列
			GCToLS.AskLogin login = new GCToLS.AskLogin();
			login.MergeFrom( data, offset, size );

			LS.instance.sdkAsynHandler.CheckLogin( login, ( int )GCToLS.MsgID.EMsgToLsfromGcAskLogin, this.id );
			this.SetInited( true, true );
			return ErrorCode.Success;
		}
	}
}