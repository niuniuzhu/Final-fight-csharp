using System.Net;
using System.Net.Sockets;

namespace Core.Net
{
	public class Connection : IConnection
	{
		public Socket socket { get; set; }
		public EndPoint remoteEndPoint { get; set; }
		public EndPoint localEndPoint { get; set; }
		public INetSession session { get; }
		public int recvBufSize { set => this._recvEventArgs.SetBuffer( new byte[value], 0, value ); }
		public PacketEncodeHandler packetEncodeHandler { get; set; }
		public PacketDecodeHandler packetDecodeHandler { get; set; }
		public bool connected => this.socket != null && this.socket.Connected;

		private readonly SocketAsyncEventArgs _sendEventArgs;
		private readonly SocketAsyncEventArgs _recvEventArgs;
		private readonly StreamBuffer _cache = new StreamBuffer();

		public Connection( INetSession session )
		{
			this.session = session;
			this._sendEventArgs = new SocketAsyncEventArgs { UserToken = this };
			this._recvEventArgs = new SocketAsyncEventArgs { UserToken = this };
			this._sendEventArgs.Completed += this.OnIOComplete;
			this._recvEventArgs.Completed += this.OnIOComplete;
		}

		public void Dispose()
		{
			this._sendEventArgs.Completed -= this.OnIOComplete;
			this._recvEventArgs.Completed -= this.OnIOComplete;
			this._sendEventArgs.Dispose();
			this._recvEventArgs.Dispose();
		}

		public void Close()
		{
			if ( this.socket == null )
				return;
			if ( this.connected )
				this.socket.Shutdown( SocketShutdown.Both );
			this.socket.Close();
			this.socket = null;
			this._cache.Clear();
			this.packetEncodeHandler = null;
			this.packetDecodeHandler = null;
		}

		public void SetOpt( SocketOptionName optionName, object opt ) => this.socket.SetSocketOption( SocketOptionLevel.Socket, optionName, opt );

		public bool StartReceive()
		{
			bool asyncResult;
			try
			{
				asyncResult = this.socket.ReceiveAsync( this._recvEventArgs );
			}
			catch ( SocketException e )
			{
				this.OnError( $"socket receive error, code:{e.SocketErrorCode} " );
				return false;
			}
			if ( !asyncResult )//有一个挂起的IO操作需要马上处理
				this.ProcessReceive( this._recvEventArgs );
			return true;
		}

		public bool Send( byte[] data, int len )
		{
			if ( !this.connected )
				return false;

			this._sendEventArgs.SetBuffer( data, 0, len );
			bool asyncResult;
			try
			{
				asyncResult = this.socket.SendAsync( this._sendEventArgs );
			}
			catch ( SocketException e )
			{
				this.OnError( $"socket send error, code:{e.SocketErrorCode} " );
				return false;
			}
			if ( !asyncResult )
				this.ProcessSend( this._sendEventArgs );
			return true;
		}

		public int SyncSend( byte[] data, uint len )
		{
			int sendLen;
			try
			{
				sendLen = this.socket.Send( data, ( int )len, SocketFlags.None );
			}
			catch ( SocketException e )
			{
				this.OnError( $"SyncSend buffer error, code:{e.SocketErrorCode} " );
				return -1;
			}
			return sendLen;
		}

		private void OnIOComplete( object sender, SocketAsyncEventArgs asyncEventArgs )
		{
			switch ( asyncEventArgs.LastOperation )
			{
				case SocketAsyncOperation.Receive:
					this.ProcessReceive( asyncEventArgs );
					break;

				case SocketAsyncOperation.Send:
					this.ProcessSend( asyncEventArgs );
					break;
			}
		}

		private void ProcessSend( SocketAsyncEventArgs sendEventArgs )
		{
			if ( sendEventArgs.SocketError != SocketError.Success )
			{
				//网络错误
				this.OnError( $"socket send error, code:{sendEventArgs.SocketError}" );
				return;
			}

			NetEvent netEvent = NetEventMgr.instance.pool.Pop();
			netEvent.type = NetEvent.Type.Send;
			netEvent.session = this.session;
			NetEventMgr.instance.Push( netEvent );
		}

		private void ProcessReceive( SocketAsyncEventArgs recvEventArgs )
		{
			if ( recvEventArgs.SocketError != SocketError.Success )
			{
				//网络错误
				this.OnError( $"receive error, remote endpoint:{this.remoteEndPoint}, code:{recvEventArgs.SocketError}" );
				return;
			}
			int size = recvEventArgs.BytesTransferred;
			if ( size == 0 )
			{
				//远端可能已经关闭连接
				this.OnError( $"Receive zero bytes, remote endpoint: {this.remoteEndPoint}, code:{SocketError.NoData}" );
				return;
			}
			//写入缓冲区
			this._cache.Write( recvEventArgs.Buffer, recvEventArgs.Offset, recvEventArgs.BytesTransferred );
			//处理数据
			this.ProcessData();
			//重新开始接收
			this.StartReceive();
		}

		private void ProcessData()
		{
			if ( this._cache.length == 0 )
				return;

			byte[] data;
			if ( this.packetDecodeHandler != null )
			{
				//解码数据,返回解码后的数据长度
				//完成解码后数据的包头(整个数据的长度)已经被剥离
				int len = this.packetDecodeHandler( this._cache.GetBuffer(), 0, this._cache.position, out data );
				if ( data == null )
					return;
				//截断当前缓冲区
				this._cache.Strip( len, ( int )this._cache.length - len );
			}
			else
				data = this._cache.ToArray();

			NetEvent netEvent = NetEventMgr.instance.pool.Pop();
			netEvent.type = NetEvent.Type.Recv;
			netEvent.session = this.session;
			netEvent.data = data;
			NetEventMgr.instance.Push( netEvent );

			//缓冲区里可能还有未处理的数据,继续递归处理
			this.ProcessData();
		}

		private void OnError( string error )
		{
			NetEvent netEvent = NetEventMgr.instance.pool.Pop();
			netEvent.type = NetEvent.Type.Error;
			netEvent.session = this.session;
			netEvent.error = error;
			NetEventMgr.instance.Push( netEvent );
		}
	}
}