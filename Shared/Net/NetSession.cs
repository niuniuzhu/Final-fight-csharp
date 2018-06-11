using Core;
using Core.Net;

namespace Shared.Net
{
	public abstract class NetSession : INetSession
	{
		public int id { get; }
		public int logicID { get; set; }
		public SessionType type { get; set; }
		public IConnection connection { get; }

		protected NetSession( int id )
		{
			this.id = id;
			this.connection = new Connection();
			this.connection.session = this;
		}

		public virtual void Dispose()
		{
		}

		public virtual void Release()
		{
		}

		public virtual void OnEstablish()
		{
			//todo active time
		}

		public void OnTerminate()
		{
		}

		public void OnError( int moduleErr, int sysErr )
		{
		}

		public void OnRecv( byte[] buf, int size )
		{
			Logger.Log( size );
		}

		public bool Send( byte[] data, int size )
		{
			return this.connection.Send( data, size );
		}
	}
}