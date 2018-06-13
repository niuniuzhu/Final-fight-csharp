using Core;
using Core.Misc;
using Core.Net;
using System.Net.Sockets;

namespace Shared.Net
{
	public abstract class CliSession : NetSession
	{
		private const long RECONN_DETECT_INTERVAL = 10000;

		public IConnector connector { get; }
		public bool reconnectTag { get; set; }

		private long _reconnTime;

		protected CliSession( uint id ) : base( id )
		{
			this.connector = new Connector( this );
			this.reconnectTag = true;
		}

		public bool Connect( string ip, int port, SocketType socketType, ProtocolType protoType )
		{
			return this.Reconnect( ip, port, socketType, protoType );
		}

		public bool Reconnect()
		{
			if ( !this._reconFlag || !this.reconnectTag )
				return false;

			long curTime = TimeUtils.utcTime;
			if ( curTime < this._reconnTime )
				return false;

			this._reconnTime = curTime + RECONN_DETECT_INTERVAL;
			if ( !this.connector.ReConnect() )
				return false;

			this._reconFlag = false;

			return true;
		}

		private bool Reconnect( string ip, int port, SocketType socketType, ProtocolType protoType )
		{
			return this.connector.Connect( ip, port, socketType, protoType );
		}

		public void OnConnError( string error )
		{
			Logger.Error( error );
			this._reconFlag = true;
			this.connector.Close();
		}

		public override void OnEstablish()
		{
			base.OnEstablish();
			this._remoteInited = true;
			this.SendInitData();
		}

		public override void OnHeartBeat( UpdateContext context )
		{
			this.Reconnect();
		}
	}
}