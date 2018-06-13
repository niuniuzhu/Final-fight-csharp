using System.Net.Sockets;

namespace Core.Net
{
	public class Connection : IConnection
	{
		public Socket socket { get; set; }
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

		public void Release()
		{
			this._cache.Clear();
			this.packetDecodeHandler = null;
		}

		public void Close()
		{
			if ( this.connected )
				this.socket.Shutdown( SocketShutdown.Both );
			this.socket.Close();
			this.socket = null;
		}

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
			if ( !asyncResult )
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
				this.OnError( $"receive error, remote endpoint:{this.socket.RemoteEndPoint}, code:{recvEventArgs.SocketError}" );
				return;
			}
			int size = recvEventArgs.BytesTransferred;
			if ( size == 0 )
			{
				this.OnError( $"Receive zero bytes, remote endpoint: {this.socket.RemoteEndPoint}, code:{SocketError.NoData}" );
				return;
			}
			this._cache.Write( recvEventArgs.Buffer, recvEventArgs.Offset, recvEventArgs.BytesTransferred );
			this.ProcessData();
			this.StartReceive();
		}

		private void ProcessData()
		{
			if ( this._cache.length == 0 )
				return;

			int len = this.packetDecodeHandler( this._cache.GetBuffer(), 0, this._cache.position, out byte[] data );
			if ( data == null )
				return;
			this._cache.Strip( len, ( int )this._cache.length - len );

			NetEvent netEvent = NetEventMgr.instance.pool.Pop();
			netEvent.type = NetEvent.Type.Recv;
			netEvent.session = this.session;
			netEvent.data = data;
			NetEventMgr.instance.Push( netEvent );

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