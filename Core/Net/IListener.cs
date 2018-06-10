using System.Net.Sockets;

namespace Core.Net
{
	public interface IListener
	{
		/**
		 * @brief Set an user implemented ISDPacketParser object, which process
		 *        packet parse logic of the connection accepted by the ISDListener
		 * @param poPacketParser ISDPacketParser instance pointer
		 */
		PacketEncodeHandler packetEncodeHandler { get; set; }

		/**
		 * @brief Set the user implemented ISDSessionFactory object to ISDListener
		 *
		 * When a TCP connection is accepted by ISDListener,
		 * ISDSessionFactory object will be asked to create a ISDSession
		 * @param poSessionFactory ISDSessionFactory instance factory
		 */
		SessionCreateHandler sessionCreateHandler { get; set; }

		/**
		 * @brief Set the send and receive buffer size of the connection accepted by the ISDListener object
		 * @param dwRecvBufSize : receiving buffer size in bytes
		 * @param dwSendBufSize : sending buffer size in bytes
		 */
		int recvBufSize { get; set; }

		/**
		 * @brief Set extension options
		 * @param dwType : option type
		 * @param pOpt : option value
		 */
		void SetOpt( SocketOptionName optionName, object pOpt );

		/**
		 * @brief Listen at specified IP and port
		 * @param pszIP : IP string
		 * @param wPort : port number
		 * @param bReUseAddr : the flag for re-using same address
		 * @return true means success, false means failure
		 */
		bool Start( string ip, int port, SocketType socketType, ProtocolType protoType, bool reUseAddr = true );

		/**
		 * @brief Stop listening
		 * @return true means success, false means failure
		 */
		bool Stop();
	}
}