using Core;
using Core.Misc;
using Core.Net;

namespace Shared.Net
{
	public abstract class NetSession : INetSession
	{
		public NetSessionMgr owner { get; set; }
		public uint id { get; }
		public int logicID { get; set; }
		public SessionType type { get; set; }
		public IConnection connection { get; protected set; }

		protected readonly HandlerContainer _handlerContainer;

		/// <summary>
		/// 本地连接是否已经初始化的标记
		/// </summary>
		protected bool _inited;

		/// <summary>
		/// 远端连接是否已经初始化的标记
		/// </summary>
		protected bool _remoteInited;

		/// <summary>
		/// 重连标记
		/// </summary>
		protected bool _reconFlag;

		/// <summary>
		/// 连接关闭标记
		/// </summary>
		protected bool _closed;

		/// <summary>
		/// 建立连接的时间戳
		/// </summary>
		protected long _activeTime;

		/// <summary>
		/// 关闭连接的时间戳
		/// </summary>
		protected long _deactiveTime;

		protected NetSession( uint id )
		{
			this.id = id;
			this.connection = new Connection( this );

			this._closed = true;
			this._handlerContainer = new HandlerContainer();
		}

		public virtual void Dispose()
		{
			this.Close();
		}

		/// <summary>
		/// 关闭session
		/// </summary>
		public void Close()
		{
			if ( this._closed )
				return;
			this._closed = true;
			this._inited = false;
			this._remoteInited = false;
			this._deactiveTime = TimeUtils.utcTime;
			this._reconFlag = true;
			this.InternalClose();
			this.connection.Close();
			this.OnClose();
			NetSessionPool.instance.Push( this );
		}

		protected virtual void InternalClose()
		{
		}

		/// <summary>
		/// 收到远端的初始化消息后调用
		/// </summary>
		protected void SetInited( bool isInited, bool isTrigger )
		{
			this._inited = isInited;
			if ( !isTrigger )
				return;
			if ( !this._inited )
				return;

			//已经标记远端连接已经初始化则不必再次发送了
			if ( !this._remoteInited )
			{
				//标记远端连接已经初始化
				this._remoteInited = true;
				//向远端发送初始化数据
				this.SendInitData();
			}
			//此时才认为真正建立了可信的连接
			this.OnRealEstablish();
		}

		/// <summary>
		/// 建立连接后调用
		/// </summary>
		public virtual void OnEstablish()
		{
			this._inited = false;
			this._remoteInited = false;
			this._reconFlag = false;
			this._closed = false;
			this._activeTime = TimeUtils.utcTime;
			this._deactiveTime = 0;
		}

		/// <summary>
		/// 通信过程出现错误后调用
		/// </summary>
		public void OnError( string error )
		{
			Logger.Error( error );
			this.Close();
		}

		/// <summary>
		/// 收到数据后调用
		/// </summary>
		public void OnRecv( byte[] data, int offset, int size )
		{
			if ( this._closed )
				return;
			if ( this._handlerContainer == null )
				return;

			//剥离第一层消息ID
			int msgID = 0;
			offset += ByteUtils.Decode32i( data, offset, ref msgID );
			size -= offset;
			//检查是否注册了处理函数,否则调用未处理数据的函数
			if ( this._handlerContainer.TryGetHandler( msgID, out MsgHandler handler ) )
				handler.Invoke( data, offset, size, msgID );
			else
				this.HandleUnhandledMsg( data, offset, size, msgID );
		}

		/// <summary>
		/// 发送数据后调用
		/// </summary>
		public void OnSend()
		{
		}

		public bool Send( byte[] data, int size ) => this.connection.Send( data, size );

		/// <summary>
		/// 向远端发送初始化消息
		/// </summary>
		protected abstract void SendInitData();

		/// <summary>
		/// 建立可信的连接后调用
		/// </summary>
		protected abstract void OnRealEstablish();

		/// <summary>
		/// 关闭连接后调用
		/// </summary>
		protected abstract void OnClose();

		/// <summary>
		/// 处理此实例未处理的消息(通常该消息是一条转发消息)
		/// </summary>
		/// <returns></returns>
		protected abstract bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID );

		/// <summary>
		/// 每次心跳调用
		/// </summary>
		public virtual void OnHeartBeat( UpdateContext context )
		{
		}
	}
}