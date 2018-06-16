using Core.Misc;
using Core.Net;
using System.Net.Sockets;

namespace Shared.Net
{
	/// <summary>
	/// 作为客户端的session,通常是主动发起连接后创建的session
	/// </summary>
	public abstract class CliSession : NetSession
	{

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

		private void Reconnect()
		{
			if ( !this._reconFlag || !this.reconnectTag )
				return;

			long curTime = TimeUtils.utcTime;
			if ( curTime < this._reconnTime )
				return;

			this._reconnTime = curTime + Consts.RECONN_DETECT_INTERVAL;
			if ( !this.connector.ReConnect() )
				return;

			this._reconFlag = false;
		}

		private bool Reconnect( string ip, int port, SocketType socketType, ProtocolType protoType )
		{
			return this.connector.Connect( ip, port, socketType, protoType );
		}

		public void OnConnError( string error )
		{
			Logger.Error( error );
			this._reconFlag = true;
		}

		public override void OnEstablish()
		{
			base.OnEstablish();
			//标记远端连接已经初始化,那么在往后收到的远端初始化消息后,不会重复发送初始化消息,否则会进入死循环
			//参考NetSession.SetInited
			this._remoteInited = true;
			//向远端发送初始化数据
			this.SendInitData();
		}

		public override void OnHeartBeat( UpdateContext context )
		{
			this.Reconnect();
		}
	}
}