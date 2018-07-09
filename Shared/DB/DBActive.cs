using Core.Misc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Shared.DB
{
	public class DBActive
	{
		private static int _gid;

		private readonly ThreadSafeObejctPool<GBuffer> _pool = new ThreadSafeObejctPool<GBuffer>();
		private readonly BufferBlock<GBuffer> _buffer = new BufferBlock<GBuffer>();
		private readonly Action<GBuffer> _callback;
		private readonly Action _beginCallback;
		private bool _running;

		public int count => this._buffer.Count;
		public bool isEmpty => this._buffer.Count == 0;
		public int actorID { get; }

		public DBActive( Action<GBuffer> callback, Action beginCallback )
		{
			this.actorID = _gid++;
			this._callback = callback;
			this._beginCallback = beginCallback;
		}

		public void Send( GBuffer buffer )
		{
			if ( null != buffer )
				this._buffer.Post( buffer );
		}

		public void Run()
		{
			this._running = true;
			Task.Run( () =>
			{
				this._beginCallback?.Invoke();
				this.Consume();
			} );
		}

		public void Stop()
		{
			if ( this._buffer.TryReceiveAll( out IList<GBuffer> buffers ) )
			{
				foreach ( GBuffer buffer in buffers )
					this._callback?.Invoke( buffer );
			}
			this._running = false;
		}

		private async void Consume()
		{
			while ( this._running )
			{
				GBuffer buffer = await this._buffer.ReceiveAsync();
				this._callback?.Invoke( buffer );
				this.ReleaseBuffer( buffer );
			}
		}

		public GBuffer GetBuffer()
		{
			return this._pool.Pop();
		}

		public void ReleaseBuffer( GBuffer buffer )
		{
			if ( buffer == null )
				return;
			buffer.Clear();
			this._pool.Push( buffer );
		}
	}
}