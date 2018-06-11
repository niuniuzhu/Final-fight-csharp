using System;
using System.Net.Sockets;
using Core.Net;

namespace Shared.Net
{
	public abstract class CliSession : NetSession
	{
		public IConnector connector { get; set; }

		public bool reconnectTag { get; set; }

		protected CliSession( int id ) : base( id )
		{
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

		public override void OnEstablish()
		{
			base.OnEstablish();
			this.SendInitData();
		}

		protected abstract void SendInitData();
	}
}