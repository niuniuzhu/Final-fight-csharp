using System.Net.Sockets;

namespace Core.Net
{
	public interface IConnection
	{
		INetSession session { get; set; }
		bool connected { get; }
		Socket socket { set; }
		int recvBufSize { set; }
		PacketEncodeHandler packetEncodeHandler { set; }
		void Dispose();
		void Release();
		bool StartReceive();
		bool Send( byte[] data, int len );
		int SyncSend( byte[] data, uint len );
	}
}