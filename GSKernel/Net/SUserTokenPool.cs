using System.Collections.Generic;

namespace GateServer.Net
{
	public class SUserTokenPool
	{
		private static SUserTokenPool _instance;
		public static SUserTokenPool instance => _instance ?? ( _instance = new SUserTokenPool() );

		private readonly Queue<SUserToken> _pool = new Queue<SUserToken>();

		private SUserTokenPool()
		{
		}

		public SUserToken Pop()
		{
			if ( this._pool.Count == 0 )
				return new SUserToken();
			return this._pool.Dequeue();
		}

		public void Push( SUserToken userToken )
		{
			this._pool.Enqueue( userToken );
		}
	}
}