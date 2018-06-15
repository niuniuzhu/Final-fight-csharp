using System.Net;
using System.Net.Sockets;

namespace Core.Net
{
	public class Connector : IConnector
	{
		public Socket socket { get; set; }
		public INetSession session { get; }
		public int recvBufSize { get; set; }
		public PacketEncodeHandler packetEncodeHandler { get; set; }
		public PacketDecodeHandler packetDecodeHandler { get; set; }
		public bool connected => this.socket != null && this.socket.Connected;

		private readonly SocketAsyncEventArgs _connEventArgs;
		private string _ip;
		private int _port;
		private SocketType _socketType;
		private ProtocolType _protocolType;

		public Connector( INetSession session )
		{
			this.session = session;
			this._connEventArgs = new SocketAsyncEventArgs { UserToken = this };
			this._connEventArgs.Completed += this.OnIOComplete;
		}

		public void Dispose()
		{
			this._connEventArgs.Completed -= this.OnIOComplete;
			this._connEventArgs.Dispose();
		}

		public void Close()
		{
			if ( this.connected )
				this.socket.Shutdown( SocketShutdown.Both );
			this.socket.Close();
			this.socket = null;
		}

		public bool Connect( string ip, int port, SocketType socketType, ProtocolType protoType )
		{
			this._ip = ip;
			this._port = port;
			this._socketType = socketType;
			this._protocolType = protoType;
			return this.ReConnect();
		}

		public bool ReConnect()
		{
			try
			{
				this.socket = new Socket( AddressFamily.InterNetwork, this._socketType, this._protocolType );
			}
			catch ( SocketException e )
			{
				this.OnError( $"create socket error, code:{e.SocketErrorCode}" );
				return false;
			}

			this.socket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.NoDelay, true );
			this.socket.NoDelay = true;

			this._connEventArgs.RemoteEndPoint = new IPEndPoint( IPAddress.Parse( this._ip ), this._port );
			bool asyncResult;
			try
			{
				asyncResult = this.socket.ConnectAsync( this._connEventArgs );
			}
			catch ( SocketException e )
			{
				this.OnError( $"socket connect error, address:{this._ip}:{this._port}, code:{e.SocketErrorCode} " );
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
				this.OnError( $"socket connect error, address:{this._ip}:{this._port}, code:{connectEventArgs.SocketError}" );
				return;
			}
			this.session.connection.socket = this.socket;
			this.session.connection.localEndPoint = this.socket.LocalEndPoint;
			this.session.connection.remoteEndPoint = this.socket.RemoteEndPoint;
			this.session.connection.recvBufSize = this.recvBufSize;
			this.session.connection.packetEncodeHandler = this.packetEncodeHandler;
			this.session.connection.packetDecodeHandler = this.packetDecodeHandler;
			this.session.connection.StartReceive();
			this.socket = null;

			NetEvent netEvent = NetEventMgr.instance.pool.Pop();
			netEvent.type = NetEvent.Type.Establish;
			netEvent.session = this.session;
			NetEventMgr.instance.Push( netEvent );
		}

		private void OnError( string error )
		{
			NetEvent netEvent = NetEventMgr.instance.pool.Pop();
			netEvent.type = NetEvent.Type.ConnErr;
			netEvent.session = this.session;
			netEvent.error = error;
			NetEventMgr.instance.Push( netEvent );
		}
	}
}