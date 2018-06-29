using CentralServer.Tools;
using Core.Misc;
using Shared;

namespace CentralServer.User
{
	public class CSUser
	{
		public ulong guid => this._userDBData.sPODUsrDBData.un64ObjIdx;
		public string username => this._userDBData.szUserName;
		public string nickname => this._userDBData.szNickName;
		public long timerID { get; set; }
		//todo
		//public CCSUserBattleInfo userBattleInfoEx { get; private set; }

		private readonly SUserDBData _userDBData = new SUserDBData();
		private long _gcLastPing;

		public void OnOnline( SUserNetInfo netinfo, GCToCS.Login login, bool isFirstInDB, bool isFirstInMem, bool isReLogin = false )
		{
		}

		public void LoadDBData( SUserDBData sUserDbData )
		{
		}

		public void ResetPingTimer()
		{
			this._gcLastPing = TimeUtils.utcTime;
		}

		public void CheckDbSaveTimer( long curtime, long tickspan )
		{
		}

		public void InitRunes( string bagStr, string slotStr )
		{
		}

		public void LoadFromRedisStr( string heroStr, string friendStr, string blackStr, string itemStr, string mailStr )
		{
		}
	}
}