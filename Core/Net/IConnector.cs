using System.Net.Sockets;

namespace Core.Net
{
	public interface IConnector
	{
		Socket socket { get; set; }
		INetSession session { get; }
		int recvBufSize { get; set; }
		PacketEncodeHandler packetEncodeHandler { set; get; }
		PacketDecodeHandler packetDecodeHandler { get; set; }
		bool connected { get; }
		void Dispose();
		void Close();
		bool Connect( string ip, int port, SocketType socketType, ProtocolType protoType );
		bool ReConnect();
	}
}