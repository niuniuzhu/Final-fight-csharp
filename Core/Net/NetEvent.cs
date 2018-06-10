namespace Core.Net
{
	public struct NetEvent
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
	}
}