using Shared;

namespace CentralServer.Mail
{
	public class CSMailMgr
	{
		private int _curtMaxMailIdx;

		public void setCurtMaxMailIdx( int index ) => this._curtMaxMailIdx += index;

		public int getCurtMailIdx() => ++this._curtMaxMailIdx;

		public void UpdatePerMailList( int mailid, ulong un64ObjIdx, MailCurtState state )
		{
		}

		public void AddGameMail( MailDBData mailDb )
		{
		}
	}
}