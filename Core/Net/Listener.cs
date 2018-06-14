using System;
using System.Net;
using System.Net.Sockets;

namespace Core.Net
{
	public class Listener : IListener
	{
		public PacketEncodeHandler packetEncodeHandler { get; set; }
		public PacketDecodeHandler packetDecodeHandler { get; set; }
		public SessionCreateHandler sessionCreateHandler { get; set; }

		public int recvBufSize { get; set; } = 10240;

		public bool noDelay
		{
			get => this._socket.NoDelay;
			set => this._socket.NoDelay = value;
		}

		private Socket _socket;

		public void Dispose() => this.Stop();

		public void SetOpt( SocketOptionName optionName, object opt ) => this._socket.SetSocketOption( SocketOptionLevel.Socket, optionName, opt );

		public bool Start( string ip, int port, SocketType socketType, ProtocolType protoType, bool reuseAddr = true )
		{
			Logger.Log( $"Start Listen {ip}:{port}, reuseAddr {reuseAddr}" );
			try
			{
				this._socket = new Socket( AddressFamily.InterNetwork, socketType, protoType );
			}
			catch ( SocketException e )
			{
				Logger.Error( $"create socket error, code:{e.SocketErrorCode}" );
				return false;
			}
			this._socket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddr );
			this._socket.NoDelay = true;
			try
			{
				this._socket.Bind( new IPEndPoint( IPAddress.Parse( ip ), port ) );
			}
			catch ( SocketException e )
			{
				Logger.Error( $"socket bind at {ip}:{port} fail, code:{e.SocketErrorCode}" );
				return false;
			}
			try
			{
				this._socket.Listen( 128 );
			}
			catch ( SocketException e )
			{
				Logger.Error( $"socket listen at {ip}:{port} fail, code:{e.SocketErrorCode}" );
				return false;
			}
			this.StartAccept( null );
			return true;
		}

		public bool Stop()
		{
			Socket socket = this._socket;
			this._socket = null;
			return this.Close( socket );
		}

		private bool Close( Socket socket )
		{
			if ( socket == null )
				return false;
			if ( socket.Connected )
				socket.Shutdown( SocketShutdown.Both );
			socket.Close();
			return true;
		}

		private void StartAccept( SocketAsyncEventArgs acceptEventArgs )
		{
			if ( this._socket == null )
				return;

			if ( acceptEventArgs == null )
			{
				acceptEventArgs = new SocketAsyncEventArgs();
				acceptEventArgs.RemoteEndPoint = new IPEndPoint( IPAddress.Any, 0 );
				acceptEventArgs.Completed += this.OnAcceptComplete;
			}
			else
				acceptEventArgs.AcceptSocket = null;

			bool asyncResult;
			try
			{
				asyncResult = this._socket.AcceptAsync( acceptEventArgs );
			}
			catch ( ObjectDisposedException )
			{
				return;
			}
			catch ( SocketException e )
			{
				Logger.Error( $"socket accept fail, code:{e.SocketErrorCode}" );
				this.Close( this._socket );
				return;
			}
			if ( !asyncResult )
				this.ProcessAccept( acceptEventArgs );
		}

		private void OnAcceptComplete( object sender, SocketAsyncEventArgs acceptEventArgs ) => this.ProcessAccept( acceptEventArgs );

		private void ProcessAccept( SocketAsyncEventArgs acceptEventArgs )
		{
			if ( acceptEventArgs.SocketError != SocketError.Success )
			{
				//网络错误
				Logger.Error( $"process accept fail,code{acceptEventArgs.SocketError}" );
				this.Close( acceptEventArgs.AcceptSocket );
				this.StartAccept( acceptEventArgs );
				return;
			}

			if ( this._socket == null )
			{
				this.Close( acceptEventArgs.AcceptSocket );
				this.StartAccept( acceptEventArgs );
				return;
			}

			//调用委托创建session
			INetSession session = this.sessionCreateHandler();
			if ( session == null )
			{
				Logger.Error( "create session failed" );
				this.Close( acceptEventArgs.AcceptSocket );
				this.StartAccept( acceptEventArgs );
				return;
			}
			session.connection.socket = acceptEventArgs.AcceptSocket;
			session.connection.packetEncodeHandler = this.packetEncodeHandler;
			session.connection.packetDecodeHandler = this.packetDecodeHandler;
			session.connection.recvBufSize = this.recvBufSize;

			NetEvent netEvent = NetEventMgr.instance.pool.Pop();
			netEvent.type = NetEvent.Type.Establish;
			netEvent.session = session;
			NetEventMgr.instance.Push( netEvent );

			//开始接收数据
			session.connection.StartReceive();

			this.StartAccept( acceptEventArgs );
		}
	}
}