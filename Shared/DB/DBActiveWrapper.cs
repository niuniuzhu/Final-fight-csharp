using System;

namespace Shared.DB
{
	public class DBActiveWrapper
	{
		private ProducerConsumer _active;

		public DBActiveWrapper( Action<GBuffer> callback, DBCfg cfg )
		{
			this._active = new ProducerConsumer( callback );
		}

		public void Start()
		{
		}
	}
}