using Google.Protobuf;
using Shared.Net;

namespace GateServer.Net
{
	public abstract class MsgManager
	{
		/// <summary>
		/// 消息广播到所有客户端
		/// </summary>
		protected int BroadcastToGameClient( byte[] data, int offset, int size, int msgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ServerGS, data, offset, size, msgID, false );
			return 0;
		}

		/// <summary>
		/// 消息分发到指定id的客户端
		/// </summary>
		protected int PostToGameClient( uint sessionID, byte[] data, int offset, int size, int msgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( sessionID, data, offset, size, msgID );
			return 0;
		}

		protected int PostToGameClient( SessionType sessionType, byte[] data, int offset, int size, int msgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( sessionType, data, offset, size, msgID );
			return 0;
		}

		protected int PostToGameClient( uint sessionID, IMessage msg, int msgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( sessionID, msg, msgID );
			return 0;
		}

		protected int PostToGameClient( SessionType sessionType, IMessage msg, int msgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( sessionType, msg, msgID );
			return 0;
		}

		/// <summary>
		/// 强制客户端下线
		/// </summary>
		protected int PostGameClientDisconnect( uint nsID )
		{
			GSKernel.instance.netSessionMrg.DisconnectOne( ( uint )nsID );
			return 0;
		}
	}
}