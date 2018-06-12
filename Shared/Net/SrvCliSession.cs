namespace Shared.Net
{
	public abstract class SrvCliSession : NetSession
	{
		protected SrvCliSession( uint id ) : base( id )
		{
			this._isSrvCli = true;
		}
	}
}