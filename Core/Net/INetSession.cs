namespace Core.Net
{
	public interface INetSession
	{
		/// <summary>
		/// ID
		/// </summary>
		uint id { get; }

		/// <summary>
		/// 此实例持有的连接实例
		/// </summary>
		IConnection connection { get; }

		/// <summary>
		/// 销毁此实例
		/// </summary>
		void Dispose();

		/// <summary>
		/// 关闭Session
		/// </summary>
		void Close();

		/// <summary>
		/// 连接建立后调用
		/// </summary>
		void OnEstablish();

		/// <summary>
		/// 通信过程出现错误后调用
		/// </summary>
		void OnError( string error );

		/// <summary>
		/// 收到数据后调用
		/// </summary>
		void OnRecv( byte[] data, int offset, int size );

		/// <summary>
		/// 发送数据后调用
		/// </summary>
		void OnSend();
	}
}