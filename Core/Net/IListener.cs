using System.Net.Sockets;

namespace Core.Net
{
	public interface IListener
	{
		/// <summary>
		/// 包编码器
		/// </summary>
		PacketEncodeHandler packetEncodeHandler { get; set; }

		/// <summary>
		/// 包解码器
		/// </summary>
		PacketDecodeHandler packetDecodeHandler { get; set; }

		/// <summary>
		/// 当有连接到达时调用该委托创建session
		/// </summary>
		SessionCreateHandler sessionCreateHandler { get; set; }

		/// <summary>
		/// 连接建立后的接收缓冲区大小
		/// </summary>
		int recvBufSize { get; set; }

		/// <summary>
		/// 销毁此实例
		/// </summary>
		void Dispose();

		/// <summary>
		/// 设置套接字参数
		/// </summary>
		void SetOpt( SocketOptionName optionName, object opt );

		/// <summary>
		/// 开始监听
		/// </summary>
		/// <param name="port">本地端口</param>
		/// <param name="socketType">套接字类型</param>
		/// <param name="protoType">协议类型</param>
		/// <param name="reuseAddr">是否端口重用</param>
		/// <returns>操作是否成功</returns>
		bool Start( int port, SocketType socketType, ProtocolType protoType, bool reuseAddr = true );

		/// <summary>
		/// 停止监听
		/// </summary>
		/// <returns></returns>
		bool Stop();
	}
}