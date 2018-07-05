using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Shared.DB
{
	public class ProducerConsumer
	{
		private static int _gid;

		private readonly Action<GBuffer> _callback;
		private readonly BufferBlock<GBuffer> _buffer = new BufferBlock<GBuffer>();
		private bool _running;

		public int count => this._buffer.Count;
		public bool isEmpty => this._buffer.Count == 0;
		public int actorID { get; }

		public ProducerConsumer( Action<GBuffer> callback )
		{
			this.actorID = _gid++;
			this._callback = callback;
		}

		public void Send( GBuffer buffer )
		{
			if ( null != buffer )
				this._buffer.Post( buffer );
		}

		public void Run()
		{
			this._running = true;
			Task.Run( () => this.Consume() );
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
			}
		}
	}
}