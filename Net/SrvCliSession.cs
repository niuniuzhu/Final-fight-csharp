using Core.Net;

namespace Net
{
	public abstract class SrvCliSession : NetSession
	{
		protected SrvCliSession( int id ) : base( id )
		{
		}
	}
}