using Core;
using Core.Net;
using System;
using System.Net.Sockets;

namespace Shared.Net
{
	public abstract class CliSession : NetSession
	{
		public IConnector connector { get; }

		public bool reconnectTag { get; set; }

		protected CliSession( uint id ) : base( id )
		{
			this.connector = new Connector( this );
		}

		public bool Connect( string ip, int port, SocketType socketType, ProtocolType protoType )
		{
			return this.Reconnect( ip, port, socketType, protoType );
		}

		public bool Reconnect()
		{
			//todo 处理自动重连
			throw new NotImplementedException();
		}

		private bool Reconnect( string ip, int port, SocketType socketType, ProtocolType protoType )
		{
			return this.connector.Connect( ip, port, socketType, protoType );
		}

		public void OnConnError( string error )
		{
			Logger.Error( error );
			if ( this._closed )
				return;
			this._closed = true;
			this.connector.Close();
			//todo handle reconnect
		}

		public override void OnEstablish()
		{
			base.OnEstablish();
			this._remoteInited = true;
			this.SendInitData();
		}
	}
}