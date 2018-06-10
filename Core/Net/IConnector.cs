using System.Net.Sockets;

namespace Core.Net
{
	public interface IConnector
	{
		Socket socket { get; set; }
		INetSession session { get; set; }
		int recvBufSize { get; set; }
		PacketEncodeHandler packetEncodeHandler { get; set; }
		bool connected { get; }

		bool Connect( string ip, int port, SocketType socketType, ProtocolType protoType );
	}
}