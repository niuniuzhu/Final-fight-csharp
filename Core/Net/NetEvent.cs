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
		public byte[] data;
		public int offset;
		public int size;
	}
}