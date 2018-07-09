namespace CentralServer.Mail
{
	public class CSMailMgr
	{
		private int _curtMaxMailIdx;

		public void setCurtMaxMailIdx( int index ) => this._curtMaxMailIdx += index;

		public int getCurtMailIdx() => ++this._curtMaxMailIdx;
	}
}