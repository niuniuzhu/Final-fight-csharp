using System.Net.Sockets;

namespace Core.Net
{
	public class NetEvent
	{
		public enum Type
		{
			Invalid = 0,
			Establish,
			ConnErr,
			Error,
			Terminate,
			Recv,
			Send,
			BindErr,
		}

		public Type type;
		public INetSession session;
		public string error;
		public byte[] data;
	}
}