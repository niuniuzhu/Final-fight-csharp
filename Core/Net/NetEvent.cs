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
			Recv,
			Send,
		}

		public Type type;
		public INetSession session;
		public string error;
		public byte[] data;
	}
}