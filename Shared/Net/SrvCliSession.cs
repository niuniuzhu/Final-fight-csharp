namespace Shared.Net
{
	/// <summary>
	/// 作为服务端的session,通常是监听器接受连接后创建的session
	/// </summary>
	public abstract class SrvCliSession : NetSession
	{
		protected SrvCliSession( uint id ) : base( id )
		{
		}

		protected override void InternalClose()
		{
			base.InternalClose();
			//由于此session是被动创建的
			this.owner.RemoveSession( this );
		}

		public override void OnEstablish()
		{
			//由于此session是被动创建的
			this.owner.AddSession( this );
			base.OnEstablish();
		}
	}
}