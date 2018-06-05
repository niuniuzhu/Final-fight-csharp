using System.Net;

namespace Net
{
	public interface IConnection
	{
		/**
		 * @brief Retrieves local endpoint for the connection
		 * @return IP address four bytes unsigned integer
		 */
		EndPoint localEndPoint { get; set; }

		/**
		 * @brief Retrieves remote endpoint for the connection
		 * @return port number
		 */
		EndPoint remoteEndPoint { get; set; }

		/**
		 * @brief Retrieves local send buffer's free size
		 * @return local send buffer's free size in bytes
		 */
		uint sendBufFree { get; }
		/**
		 * @brief Check if the connection is connected
		 * @return true means connected, otherwise, false
		 */
		bool IsConnected();

		/**
		 * @brief Send data on the connection
		 * @param pBuf : data buffer need to send
		 * @param dwLen : data buffer length
		 */
		void Send( byte[] buf, uint len );

		/**
		 * @brief Set extension options
		 * @param dwType : option type
		 * @param pOpt : option value
		 */
		void SetOpt( uint dwType, object pOpt );

		/**
		 * @brief Close the TCP connection
		 */
		void Disconnect();
	}
}