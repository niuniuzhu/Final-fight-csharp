using System.Net.Sockets;

namespace Net
{
	public interface ISession
	{
		int id { get; }
		SessionType type { get; }
		Socket socket { set; }
		PacketEncodeHandler packetEncodeHandler { set; }
		int recvBufSize { set; }
		bool connected { get; }
		void Dispose();
		void Release();
		bool StartReceive();
		bool Send( byte[] data, int len );
		int SyncSend( byte[] data, uint len );
	}
}