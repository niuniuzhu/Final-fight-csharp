using System.Collections.Concurrent;

namespace Core.Misc
{
	public class ThreadSafeObejctPool<T> where T : new()
	{
		private readonly ConcurrentQueue<T> _pool = new ConcurrentQueue<T>();

		public bool isEmpty => this._pool.IsEmpty;

		public T Pop()
		{
			if ( this._pool.IsEmpty )
				return new T();
			this._pool.TryDequeue( out T result );
			return result;
		}

		public void Push( T obj )
		{
			this._pool.Enqueue( obj );
		}
	}
}