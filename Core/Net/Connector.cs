using System.Net;
using System.Net.Sockets;

namespace Core.Net
{
	public class Connector : IConnector
	{
		public Socket socket { get; set; }
		public INetSession session { get; set; }
		public int recvBufSize { get; set; }
		public PacketEncodeHandler packetEncodeHandler { get; set; }
		public bool connected => this.socket != null && this.socket.Connected;

		private readonly SocketAsyncEventArgs _connEventArgs;

		public Connector()
		{
			this._connEventArgs = new SocketAsyncEventArgs { UserToken = this };
			this._connEventArgs.Completed += this.OnIOComplete;
		}

		public void Dispose()
		{
			this._connEventArgs.Completed -= this.OnIOComplete;
			this._connEventArgs.Dispose();
		}

		private void Close()
		{
			if ( this.connected )
			{
				this.socket.Shutdown( SocketShutdown.Both );
			}
			this.socket.Close();
			this.socket = null;
		}

		public bool Connect( string ip, int port, SocketType socketType, ProtocolType protoType )
		{
			try
			{
				this.socket = new Socket( AddressFamily.InterNetwork, socketType, protoType );
			}
			catch ( SocketException e )
			{
				Logger.Error( $"create socket error, code:{e.SocketErrorCode}" );
				return false;
			}

			this.socket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.NoDelay, true );
			this.socket.NoDelay = true;

			this._connEventArgs.RemoteEndPoint = new IPEndPoint( IPAddress.Parse( ip ), port );
			bool asyncResult;
			try
			{
				asyncResult = this.socket.ConnectAsync( this._connEventArgs );
			}
			catch ( SocketException e )
			{
				Logger.Error( $"socket connect error, code:{e.SocketErrorCode} " );
				this.Close();
				return false;
			}
			if ( !asyncResult )
				this.ProcessConnect( this._connEventArgs );
			return true;
		}

		private void OnIOComplete( object sender, SocketAsyncEventArgs asyncEventArgs )
		{
			switch ( asyncEventArgs.LastOperation )
			{
				case SocketAsyncOperation.Connect:
					this.ProcessConnect( asyncEventArgs );
					break;
			}
		}

		private void ProcessConnect( SocketAsyncEventArgs connectEventArgs )
		{
			if ( connectEventArgs.SocketError != SocketError.Success )
			{
				Logger.Error( $"socket connect error, code:{connectEventArgs.SocketError}" );
				this.Close();
				return;
			}
			NetEvent netEvent;
			netEvent.type = NetEvent.Type.Establish;
			netEvent.session = this.session;
			this.session.connection.socket = this.socket;
			this.session.connection.recvBufSize = this.recvBufSize;
			this.session.connection.packetEncodeHandler = this.packetEncodeHandler;
			EventManager.instance.Push( netEvent );
		}
	}
}