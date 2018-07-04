using System;
using CentralServer.User;
using Core.Misc;
using Shared;
using System.Collections.Generic;
using CentralServer.Tools;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		private const string LOG_SIGN = "#";

		public struct UserCombineKey
		{
			public string username { get; }
			public int sdkid { get; }

			public UserCombineKey( string username, int sdkid )
			{
				this.username = username;
				this.sdkid = sdkid;
			}

			public static bool operator <( UserCombineKey a, UserCombineKey b )
			{
				int res = string.CompareOrdinal( a.username, b.username );
				if ( res == 0 )
					return a.sdkid < b.sdkid;
				return res < 1;
			}

			public static bool operator >( UserCombineKey a, UserCombineKey b )
			{
				return !( a < b );
			}
		}
		public DateTime today { get; private set; }

		private delegate ErrorCode GCMsgHandler( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size );

		private readonly Dictionary<int, GCMsgHandler> _gcMsgHandlers = new Dictionary<int, GCMsgHandler>();

		private readonly Dictionary<UserNetInfo, CSUser> _userNetMap = new Dictionary<UserNetInfo, CSUser>();
		private readonly Dictionary<ulong, CSUser> _userGUIDMap = new Dictionary<ulong, CSUser>();
		private readonly Dictionary<ulong, CSUser> _userOnlineMap = new Dictionary<ulong, CSUser>();
		private readonly Dictionary<string, CSUser> _nickNameMap = new Dictionary<string, CSUser>();
		private readonly Dictionary<UserCombineKey, ulong> _allUserName2GUIDMap = new Dictionary<UserCombineKey, ulong>();
		private readonly HashSet<string> _allNickNameSet = new HashSet<string>();
		private readonly List<Notice> _notices = new List<Notice>();
		private int _maxGuid;

		public CSUserMgr()
		{
			this.today = new DateTime();
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskComleteUserInfo] = this.OnMsgToGstoCsfromGcAskComleteUserInfo;
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin] = this.OnMsgToGstoCsfromGcAskLogin;
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskReconnectGame] = this.OnMsgToGstoCsfromGcAskReconnectGame;
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskChangeNickName] = this.OnMsgToGstoCsfromGcAskChangeNickName;
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskChangeheaderId] = this.OnMsgToGstoCsfromGcAskChangeheaderId;
		}

		public bool ContainsUser( UserNetInfo userNetInfo ) => this._userNetMap.ContainsKey( userNetInfo );

		public ErrorCode AddUser( CSUser csUser )
		{
			if ( null == csUser )
				return ErrorCode.NullUser;

			csUser.ResetPingTimer();

			ulong un64ObjIndex = csUser.guid;

			if ( string.IsNullOrEmpty( csUser.username ) )
			{
				Logger.Error( "invalid username" );
				return ErrorCode.InvalidUserName;
			}

			if ( !this._userGUIDMap.TryAdd( un64ObjIndex, csUser ) )
			{
				Logger.Error( "add username fail" );
				return ErrorCode.AddUserNameFailed;
			}

			if ( !string.IsNullOrEmpty( csUser.nickname ) )
				this._nickNameMap.Add( csUser.nickname, csUser );

			long timerID = CS.instance.AddTimer( csUser.CheckDbSaveTimer, 1000, true );//todo CCSCfgMgr::getInstance().GetCSGlobalCfg().dbSaveTimeSpace * 1000
			csUser.timerID = timerID;

			return ErrorCode.Success;
		}

		public ErrorCode RemoveUser( CSUser pUser )
		{
			if ( pUser == null )
				return ErrorCode.NullUser;

			CS.instance.RemoveTimer( pUser.timerID );
			pUser.CheckHeroValidTimer( TimeUtils.utcTime );
			//todo
			//DBPoster_UpdateUser( pUser );//存盘// 
			//CSSGameLogMgr::GetInstance().AddGameLog( eLog_UserDiscon, pUser.GetUserDBData() );
			pUser.SaveToRedis();
			//m_MailMgr.RemoveObjId( pUser.GetUserDBData().sPODUsrDBData.un64ObjIdx );
			this._nickNameMap.Remove( pUser.nickname );
			this._userGUIDMap.Remove( pUser.guid );

			return ErrorCode.Success;
		}

		public bool CheckIfCanRemoveUser( CSUser pUser )
		{
			if ( pUser == null )
				return false;
			//todo
			if ( pUser.userPlayingStatus == UserPlayingStatus.UserPlayingStatusOffLine /*&& pUser.GetUserBattleInfoEx().GetBattleState() == eBattleState_Free*/ )
			{
				this.RemoveUser( pUser );
				Logger.Log( "user removed" );
				return true;
			}
			return false;
		}

		public CSUser GetUser( UserNetInfo userNetInfo )
		{
			this._userNetMap.TryGetValue( userNetInfo, out CSUser user );
			return user;
		}

		public CSUser GetUser( ulong guid )
		{
			this._userGUIDMap.TryGetValue( guid, out CSUser user );
			return user;
		}

		public CSUser GetUser( string nickName )
		{
			this._nickNameMap.TryGetValue( nickName, out CSUser user );
			return user;
		}

		public CSUser GetUser( CSGSInfo csgsInfo, uint gcNetID )
		{
			UserNetInfo userNetInfo = new UserNetInfo( csgsInfo.m_n32GSID, gcNetID );
			return this.GetUser( userNetInfo );
		}

		public CSUser CheckAndGetUserByNetInfo( CSGSInfo csgsInfo, uint gcNetID )
		{
			CSUser user = this.GetUser( csgsInfo, gcNetID );
			if ( null == user )
			{
				Logger.Error( $"could not find gcNetID:{gcNetID}" );
				return null;
			}
			user.ResetPingTimer();
			return user;
		}

		private bool CheckIfInGuideBattle( CSUser csUser )
		{
			//todo
			return true;
		}

		private ulong CombineGUID()
		{
			++this._maxGuid;
			//todo
			//return this._maxGuid * GUID_Devide + CS.instance.csKernelCfg.unCSId;
			return ( ulong )this._maxGuid;
		}

		private void ChangeUserNickName( CSUser csUser, string nickname )
		{
			if ( string.IsNullOrEmpty( nickname ) )
			{
				Logger.Error( "nickname is empty!" );
				return;
			}
			if ( !string.IsNullOrEmpty( csUser.nickname ) )
			{
				this._nickNameMap.Remove( csUser.nickname );
				this._allNickNameSet.Remove( csUser.nickname );
			}
			csUser.userDbData.ChangeUserDbData( UserDBDataType.NickName, nickname );
			this._allNickNameSet.Add( nickname );
			this._nickNameMap.Add( nickname, csUser );

			this.UpdateUserNickNameToDB( csUser );

			Logger.Log( $"nickname changed, username:{csUser.userDbData.szUserName}, nickname:{nickname}" );
		}

		private void UpdateUserNickNameToDB( CSUser csUser )
		{
			CSToDB.ChangeNickName sChangeNickName = new CSToDB.ChangeNickName
			{
				Nickname = csUser.nickname,
				Guid = ( long )csUser.guid
			};
			//todo
			//m_CdkeyWrapper.EncodeAndSendToDBThread( sChangeNickName, sChangeNickName.msgid() );
		}

		public void ForeachNotice( Action<Notice> handler )
		{
			int count = this._notices.Count;
			for ( int i = 0; i < count; i++ )
				handler.Invoke( this._notices[i] );
		}

		public ErrorCode OnUserOnline( CSUser csUser, UserNetInfo netInfo )
		{
			this._userNetMap.Add( netInfo, csUser );
			this._userOnlineMap.Add( csUser.guid, csUser );
			csUser.SetUserNetInfo( netInfo );
			Logger.Log( $"add user netinfo({netInfo.gcNetID})" );
			return ErrorCode.Success;
		}

		public void OnUserOffline( CSUser csUser )
		{
			this._userNetMap.Remove( csUser.userNetInfo );
			this._userOnlineMap.Remove( csUser.guid );
			csUser.ClearNetInfo();
		}

		private void InsertNewUserToMysql( GCToCS.Login login, CSUser csUser )
		{
		}

		public ErrorCode Invoke( CSGSInfo csgsInfo, int msgID, uint gcNetID, byte[] data, int offset, int size )
		{
			if ( this._gcMsgHandlers.TryGetValue( msgID, out GCMsgHandler handler ) )
				return handler.Invoke( csgsInfo, gcNetID, data, offset, size );
			Logger.Warn( $"invalid msg:{msgID}." );
			return ErrorCode.InvalidMsgProtocalID;
		}

		private void OnTimeUpdate()
		{
			DateTime oldDay = this.today;
			this.today = new DateTime();
			if ( this.today.Year != oldDay.Year ) this.OnNewYear();
			else if ( this.today.Month != oldDay.Month ) this.OnNewMonth();
			else if ( this.today.Day != oldDay.Day ) this.OnNewDay();
		}

		private void OnNewYear()
		{
		}

		private void OnNewMonth()
		{
		}

		private void OnNewDay()
		{
		}
	}
}