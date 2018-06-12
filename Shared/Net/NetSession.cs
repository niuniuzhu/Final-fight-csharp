using Core;
using Core.Misc;
using Core.Net;

namespace Shared.Net
{
	public abstract class NetSession : INetSession
	{
		public NetSessionMgr owner { get; set; }
		public uint id { get; private set; }
		public int logicID { get; set; }
		public SessionType type { get; set; }
		public IConnection connection { get; }

		protected readonly MsgCenter _msgCenter;
		protected bool _inited;
		protected bool _remoteInited;
		protected bool _isSrvCli;
		protected bool _reconFlag;
		protected bool _closed;
		protected long _activeTime;
		protected long _deactiveTime;

		protected NetSession( uint id )
		{
			this.id = id;
			this.connection = new Connection( this );

			this._closed = true;
			this._msgCenter = new MsgCenter();
		}

		public virtual void Dispose()
		{
		}

		public virtual void Release()
		{
		}

		public void Close()
		{
			if ( this._closed )
				return;
			this._closed = true;
			this._inited = false;
			this._remoteInited = false;
			this._deactiveTime = TimeUtils.utcTime;
			this._reconFlag = true;
			if ( this._isSrvCli )
			{
				this.owner.RemoveSession( this );
				this.id = Consts.PP_INVALID;
			}
			this.connection.Close();
			this.OnClose();
		}

		protected void SetInited( bool isInited, bool isTrigger )
		{
			this._inited = isInited;
			if ( !isTrigger )
				return;
			if ( !this._inited )
				return;
			if ( !this._remoteInited )
			{
				this._remoteInited = true;
				this.SendInitData();
			}
			this.OnRealEstablish();
		}

		public virtual void OnEstablish()
		{
			if ( this._isSrvCli )
				this.owner.AddSession( this );
			this._inited = false;
			this._remoteInited = false;
			this._reconFlag = false;
			this._closed = false;
			this._activeTime = TimeUtils.utcTime;
			this._deactiveTime = 0;
		}

		public void OnTerminate()
		{
		}

		public void OnError( string error )
		{
			Logger.Error( error );
			this.Close();
		}

		public void OnRecv( byte[] data, int offset, int size )
		{
			if ( this._closed )
				return;
			if ( this._msgCenter == null )
				return;

			int msgID = 0;
			offset += ByteUtils.Decode32i( data, offset, ref msgID );
			size -= offset;
			if ( this._msgCenter.TryGetHandler( msgID, out MsgHandler handler ) )
				handler.Invoke( data, offset, size, msgID );
			else
				this.OnUnknowMsg( data, offset, size, msgID );
		}

		public void OnSend()
		{
		}

		public bool Send( byte[] data, int size ) => this.connection.Send( data, size );

		protected abstract void SendInitData();

		protected abstract void OnRealEstablish();

		protected abstract void OnClose();

		protected abstract bool OnUnknowMsg( byte[] data, int offset, int size, int msgID );

		public void OnHeartBeat( UpdateContext context )
		{
			if ( !this._inited )
				return;
			this.InternalOnHeartBeat( context );
		}

		protected virtual void InternalOnHeartBeat( UpdateContext context )
		{
		}
	}
}