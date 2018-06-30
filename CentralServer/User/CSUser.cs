using CentralServer.Tools;
using Core.Misc;
using Shared;

namespace CentralServer.User
{
	public partial class CSUser
	{
		public SUserDBData userDbData { get; private set; }
		public UserNetInfo userNetInfo { get; private set; }
		public ulong guid => this.userDbData.sPODUsrDBData.un64ObjIdx;
		public string username => this.userDbData.szUserName;
		public string nickname => this.userDbData.szNickName;
		public long timerID { get; set; }
		//todo
		//public CCSUserBattleInfo userBattleInfoEx { get; private set; }

		private long _gcLastPing;

		public void OnOnline( UserNetInfo netinfo, GCToCS.Login login, bool isFirstInDB, bool isFirstInMem, bool isReLogin = false )
		{
		}

		public ErrorCode LoadDBData( SUserDBData sUserDbData )
		{
			this.userDbData = sUserDbData.Clone();
			if ( 0 == this.userDbData.sPODUsrDBData.un8UserLv )
			{
				this.userDbData.ChangeUserDbData( EUserDBDataType.eUserDBType_UserLv, 1 );
				this.userDbData.ChangeUserDbData( EUserDBDataType.eUserDBType_VIPLevel, 1 );
			}

			//auto itemID = RefreshCardBegin + i;
			//auto cfg = CCSCfgMgr::getInstance().GetOtherItemCfg( itemID );
			//if ( !cfg )
			//{
			//	ELOG( LOG_ERROR, "洗练全配置为null" );
			//	continue;
			//}
			//SUserItemInfo sSUserItemInfo;
			//sSUserItemInfo.item_id = itemID;
			//sSUserItemInfo.item_num = 5;
			//sSUserItemInfo.end_time = -1;
			//ELOG( LOG_DBBUG, "增加临时洗练" );
			//this._userDBData.item_Map[sSUserItemInfo.item_id] = sSUserItemInfo;

			//GetTaskMgr()->UnpackTaskData( this._userDBData.szTaskData );//解析任务数据

			return ErrorCode.Success;
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