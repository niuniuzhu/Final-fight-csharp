using Core.Structure;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Core.Net
{
	public class NetEventMgr
	{
		public class Pool
		{
			private readonly ConcurrentQueue<NetEvent> _pool = new ConcurrentQueue<NetEvent>();

			public NetEvent Pop()
			{
				if ( this._pool.IsEmpty )
					return new NetEvent();
				this._pool.TryDequeue( out NetEvent netEvent );
				return netEvent;
			}

			public void Push( NetEvent netEvent )
			{
				this._pool.Enqueue( netEvent );
			}
		}

		private static NetEventMgr _instance;
		public static NetEventMgr instance => _instance ?? ( _instance = new NetEventMgr() );

		private readonly SwitchQueue<NetEvent> _queue = new SwitchQueue<NetEvent>();

		public Pool pool { get; } = new Pool();

		private NetEventMgr()
		{
		}

		public void Push( NetEvent netEvent )
		{
			this._queue.Push( netEvent );
		}

		public void PopEvents( Queue<NetEvent> container )
		{
			this._queue.Switch();
			while ( !this._queue.isEmpty )
				container.Enqueue( this._queue.Pop() );
		}
	}
}