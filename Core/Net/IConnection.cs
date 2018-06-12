using System.Net.Sockets;

namespace Core.Net
{
	public interface IConnection
	{
		Socket socket { set; }
		INetSession session { get; }
		int recvBufSize { set; }
		bool connected { get; }
		PacketEncodeHandler packetEncodeHandler { set; get; }
		PacketDecodeHandler packetDecodeHandler { set; }
		void Dispose();
		void Release();
		void Close();
		bool StartReceive();
		bool Send( byte[] data, int len );
		int SyncSend( byte[] data, uint len );
	}
}