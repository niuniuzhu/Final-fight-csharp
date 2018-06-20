using System.Collections.Generic;

namespace Core.Misc
{
	public class ObejctPool<T> where T : new()
	{
		private readonly Queue<T> _pool = new Queue<T>();

		public bool isEmpty => this._pool.Count == 0;

		public T Pop()
		{
			if ( this._pool.Count == 0 )
				return new T();
			return this._pool.Dequeue();
		}

		public void Push( T obj )
		{
			this._pool.Enqueue( obj );
		}
	}
}