using System;
using System.Net;
using System.Net.Sockets;
using Core;

namespace Net
{
	public class TCPConnSession : Session, IConnSession
	{
		public int logicID { get; set; }

		private readonly SocketAsyncEventArgs _connectEventArgs;

		protected TCPConnSession( int id, SessionType type )
			: base( id, type )
		{
			this._connectEventArgs = new SocketAsyncEventArgs { UserToken = this };
			this._connectEventArgs.Completed += this.OnIOComplete;
		}

		public override void Dispose()
		{
			base.Dispose();
			this._connectEventArgs.Completed -= this.OnIOComplete;
			this._connectEventArgs.Dispose();
		}

		public bool Connect( NetworkProtoType networkProtoType, string ip, int port )
		{
			try
			{
				this.socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			}
			catch ( SocketException e )
			{
				Logger.Error( $"create socket error, code:{e.SocketErrorCode}" );
				return false;
			}

			this.socket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.NoDelay, true );
			this.socket.NoDelay = true;

			this._connectEventArgs.RemoteEndPoint = new IPEndPoint( IPAddress.Parse( ip ), port );
			bool asyncResult;
			try
			{
				asyncResult = this.socket.ConnectAsync( this._connectEventArgs );
			}
			catch ( SocketException e )
			{
				Logger.Error( $"socket connect error, code:{e.SocketErrorCode} " );
				this.Close();
				return false;
			}
			if ( !asyncResult )
				this.ProcessConnect( this._connectEventArgs );
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
				NetEvent netEvent;
				netEvent.type = NetEvent.Type.Establish;
				netEvent.session = this;
				EventManager.instance.Push( netEvent );
			}
		}
	}
}