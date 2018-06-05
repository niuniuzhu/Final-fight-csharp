namespace Net
{
	public interface IConnSession : ISession
	{
		int logicID { get; set; }
		bool Connect( NetworkProtoType networkProtoType, string ip, int port );
	}
}