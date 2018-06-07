using System.Collections;
using System.Collections.Generic;

namespace Core.Structure
{
	public class SwitchEnumerator<T> where T : IEnumerable, ICollection
	{
		protected T _consumeQueue;
		protected T _produceQueue;

		public bool isEmpty => 0 == this._consumeQueue.Count;
		public int count => this._consumeQueue.Count;

		public void Switch()
		{
			lock ( this._produceQueue )
			{
				Swap( ref this._consumeQueue, ref this._produceQueue );
			}
		}

		private static void Swap<T1>( ref T1 t1, ref T1 t2 )
		{
			T1 temp = t1;
			t1 = t2;
			t2 = temp;
		}
	}

	public class SwitchList<T> : SwitchEnumerator<List<T>>, IEnumerable<T>
	{
		public T this[int index] => this._consumeQueue[index];

		public SwitchList()
		{
			this._consumeQueue = new List<T>();
			this._produceQueue = new List<T>();
		}

		public void Add( T obj )
		{
			lock ( this._produceQueue )
			{
				this._produceQueue.Add( obj );
			}
		}

		public void RemoveAt( int index )
		{
			this._consumeQueue.RemoveAt( index );
		}

		public void Insert( int index, T obj )
		{
			this._consumeQueue.Insert( index, obj );
		}

		public void Clear()
		{
			lock ( this._produceQueue )
			{
				this._consumeQueue.Clear();
				this._produceQueue.Clear();
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this._consumeQueue.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._consumeQueue.GetEnumerator();
		}
	}

	public class SwitchQueue<T> : SwitchEnumerator<Queue<T>>, IEnumerable<T>
	{
		public SwitchQueue()
		{
			this._consumeQueue = new Queue<T>();
			this._produceQueue = new Queue<T>();
		}

		public void Push( T obj )
		{
			lock ( this._produceQueue )
			{
				this._produceQueue.Enqueue( obj );
			}
		}

		public T Pop()
		{
			return this._consumeQueue.Dequeue();
		}

		public void Clear()
		{
			lock ( this._produceQueue )
			{
				this._consumeQueue.Clear();
				this._produceQueue.Clear();
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this._consumeQueue.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._consumeQueue.GetEnumerator();
		}
	}
}