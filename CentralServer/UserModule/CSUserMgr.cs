using CentralServer.Tools;
using CentralServer.User;
using Core.Misc;
using Core.Structure;
using Google.Protobuf;
using Shared;
using Shared.DB;
using System;
using System.Collections.Generic;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		private const string LOG_SIGN = "#";
		private const int G_THREAD = 2;
		private const int GUID_Devide = 1000;

		public struct UserCombineKey
		{
			public string username;
			public int sdkid;

			public UserCombineKey( string username, int sdkid )
			{
				this.username = username;
				this.sdkid = sdkid;
			}
		}

		public delegate ErrorCode GCMsgHandler( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size );

		/// <summary>
		/// 消息处理函数容器
		/// </summary>
		public Dictionary<int, GCMsgHandler> gcMsgHandlers { get; } = new Dictionary<int, GCMsgHandler>();
		/// <summary>
		/// 玩家和网络信息的映射
		/// </summary>
		public Dictionary<UserNetInfo, CSUser> userNetMap { get; } = new Dictionary<UserNetInfo, CSUser>();
		/// <summary>
		/// 玩家和唯一id的映射
		/// </summary>
		public Dictionary<ulong, CSUser> userGuidMap { get; } = new Dictionary<ulong, CSUser>();
		/// <summary>
		/// 在线玩家容器
		/// </summary>
		public Dictionary<ulong, CSUser> userOnlineMap { get; } = new Dictionary<ulong, CSUser>();
		/// <summary>
		/// 玩家和昵称的映射
		/// </summary>
		public Dictionary<string, CSUser> nickNameMap { get; } = new Dictionary<string, CSUser>();
		/// <summary>
		/// 玩家名称(包括SDK ID)和唯一id的映射
		/// </summary>
		public Dictionary<UserCombineKey, ulong> allUserName2GuidMap { get; } = new Dictionary<UserCombineKey, ulong>();
		/// <summary>
		/// 昵称集合
		/// </summary>
		public HashSet<string> allNickNameSet { get; } = new HashSet<string>();
		public List<Notice> notices { get; } = new List<Notice>();

		public DateTime today { get; private set; }

		private ulong _maxGuid;
		private readonly DBActiveWrapper _userCacheDBActiveWrapper;
		private readonly DBActiveWrapper _cdkeyWrapper;
		private readonly List<DBActiveWrapper> _userAskDBActiveWrappers = new List<DBActiveWrapper>();

		private readonly SwitchQueue<GBuffer> _dbCallbackQueue = new SwitchQueue<GBuffer>();
		private readonly ThreadSafeObejctPool<GBuffer> _dbCallbackQueuePool = new ThreadSafeObejctPool<GBuffer>();

		public CSUserMgr()
		{
			this.today = new DateTime();

			DBCfg cfgGameDB = CS.instance.csCfg.GetDBCfg( DBType.Game );
			DBCfg cfgCdkeyDB = CS.instance.csCfg.GetDBCfg( DBType.Cdkey );

			//第一个参数是更新玩家数据的方法,这是一个异步回调,通常由别的线程投递更新请求
			//此实例会开启一个线程专门消费这些更新请求
			//第三个参数是当这个线程开启时的回调函数,也是一个异步回调,这个回调只会执行一次,也就是服务器开启的时候
			//具体该方法是处理什么消息,以后继续探讨
			this._userCacheDBActiveWrapper = new DBActiveWrapper( this.UserCacheDBAsynHandler, cfgGameDB, this.DBAsynQueryWhenThreadBegin );
			this._userCacheDBActiveWrapper.Start();

			//服务端启动就开启一个线程查询玩家数据
			this._cdkeyWrapper = new DBActiveWrapper( this.UserAskDBAsynHandler, cfgCdkeyDB, this.CDKThreadBeginCallback );
			this._cdkeyWrapper.Start();

			for ( int i = 0; i < G_THREAD; i++ )
			{
				DBActiveWrapper threadDBWrapper = new DBActiveWrapper( this.UserAskDBAsynHandler, cfgGameDB, null );
				threadDBWrapper.Start();
				this._userAskDBActiveWrappers.Add( threadDBWrapper );
			}

			this.gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskComleteUserInfo] = this.OnMsgToGstoCsfromGcAskComleteUserInfo;
			this.gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin] = this.OnMsgToGstoCsfromGcAskLogin;
			this.gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskReconnectGame] = this.OnMsgToGstoCsfromGcAskReconnectGame;
			this.gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskChangeNickName] = this.OnMsgToGstoCsfromGcAskChangeNickName;
			this.gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskChangeheaderId] = this.OnMsgToGstoCsfromGcAskChangeheaderId;
		}

		/// <summary>
		/// 把消息编码并投递到处理队列(异步生产,同步消费)
		/// </summary>
		private ErrorCode EncodeAndSendToLogicThread( IMessage msg, int msgID )
		{
			GBuffer buffer = this._dbCallbackQueuePool.Pop();
			ErrorCode errorCode = DBActiveWrapper.EncodeProtoMsgToBuffer( msg, msgID, buffer );
			if ( errorCode != ErrorCode.Success )
			{
				this._dbCallbackQueuePool.Push( buffer );
				return ErrorCode.EncodeMsgToBufferFailed;
			}
			this._dbCallbackQueue.Push( buffer );
			return ErrorCode.Success;
		}

		public ErrorCode Invoke( CSGSInfo csgsInfo, int msgID, uint gcNetID, byte[] data, int offset, int size )
		{
			if ( this.gcMsgHandlers.TryGetValue( msgID, out GCMsgHandler handler ) )
				return handler.Invoke( csgsInfo, gcNetID, data, offset, size );
			Logger.Warn( $"invalid msg:{msgID}." );
			return ErrorCode.InvalidMsgProtocalID;
		}

		public bool ContainsUser( UserNetInfo userNetInfo ) => this.userNetMap.ContainsKey( userNetInfo );

		public ErrorCode AddUser( CSUser user )
		{
			if ( null == user )
				return ErrorCode.NullUser;

			user.ResetPingTimer();

			ulong un64ObjIndex = user.guid;

			if ( string.IsNullOrEmpty( user.username ) )
			{
				Logger.Error( "invalid username" );
				return ErrorCode.InvalidUserName;
			}

			if ( !this.userGuidMap.TryAdd( un64ObjIndex, user ) )
			{
				Logger.Error( "add username fail" );
				return ErrorCode.AddUserNameFailed;
			}

			if ( !string.IsNullOrEmpty( user.nickname ) )
				this.nickNameMap.Add( user.nickname, user );

			long timerID = CS.instance.AddTimer( user.CheckDbSaveTimer, 1000, true );//todo CCSCfgMgr::getInstance().GetCSGlobalCfg().dbSaveTimeSpace * 1000
			user.timerID = timerID;

			return ErrorCode.Success;
		}

		public ErrorCode RemoveUser( CSUser user )
		{
			if ( user == null )
				return ErrorCode.NullUser;

			CS.instance.RemoveTimer( user.timerID );
			user.CheckHeroValidTimer( TimeUtils.utcTime );
			//todo
			//DBPosterUpdateUser( user );//存盘// 
			//CSSGameLogMgr::GetInstance().AddGameLog( eLog_UserDiscon, user.GetUserDBData() );
			user.SaveToRedis();
			//m_MailMgr.RemoveObjId( user.GetUserDBData().sPODUsrDBData.un64ObjIdx );
			this.nickNameMap.Remove( user.nickname );
			this.userGuidMap.Remove( user.guid );

			return ErrorCode.Success;
		}

		public bool CheckIfCanRemoveUser( CSUser user )
		{
			if ( user == null )
				return false;
			//todo
			if ( user.userPlayingStatus == UserPlayingStatus.OffLine /*&& user.GetUserBattleInfoEx().GetBattleState() == eBattleState_Free*/ )
			{
				this.RemoveUser( user );
				Logger.Log( "user removed" );
				return true;
			}
			return false;
		}

		public CSUser GetUser( UserNetInfo userNetInfo )
		{
			this.userNetMap.TryGetValue( userNetInfo, out CSUser user );
			return user;
		}

		public CSUser GetUser( ulong guid )
		{
			this.userGuidMap.TryGetValue( guid, out CSUser user );
			return user;
		}

		public CSUser GetUser( string nickName )
		{
			this.nickNameMap.TryGetValue( nickName, out CSUser user );
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

		private bool CheckIfInGuideBattle( CSUser user )
		{
			//todo
			return true;
		}

		private ulong CombineGUID()
		{
			++this._maxGuid;
			//todo
			//return this._maxGuid * GUID_Devide + CS.instance.csKernelCfg.unCSId;
			return this._maxGuid;
		}

		private void ChangeUserNickName( CSUser user, string nickname )
		{
			if ( string.IsNullOrEmpty( nickname ) )
			{
				Logger.Error( "nickname is empty!" );
				return;
			}
			if ( !string.IsNullOrEmpty( user.nickname ) )
			{
				this.nickNameMap.Remove( user.nickname );
				this.allNickNameSet.Remove( user.nickname );
			}
			user.userDbData.ChangeUserDbData( UserDBDataType.NickName, nickname );
			this.allNickNameSet.Add( nickname );
			this.nickNameMap.Add( nickname, user );

			this.UpdateUserNickNameToDB( user );

			Logger.Log( $"nickname changed, username:{user.userDbData.szUserName}, nickname:{nickname}" );
		}

		public ErrorCode OnUserOnline( CSUser csUser, UserNetInfo netInfo )
		{
			this.userNetMap.Add( netInfo, csUser );
			this.userOnlineMap.Add( csUser.guid, csUser );
			csUser.SetUserNetInfo( netInfo );
			Logger.Log( $"add user netinfo({netInfo.gcNetID})" );
			return ErrorCode.Success;
		}

		public void OnUserOffline( CSUser user )
		{
			this.userNetMap.Remove( user.userNetInfo );
			this.userOnlineMap.Remove( user.guid );
			user.ClearNetInfo();
		}

		public void UserAskUdateItem( UserItemInfo tempInfo, DBOperation del, ulong guid )
		{
			//todo
		}

		private void UpdateUserNickNameToDB( CSUser user )
		{
			CSToDB.ChangeNickName changeNickName = new CSToDB.ChangeNickName
			{
				Nickname = user.nickname,
				Guid = ( long )user.guid
			};
			this._cdkeyWrapper.EncodeAndSendToDBThread( changeNickName, ( int )CSToDB.MsgID.EChangeNickNameDbcall );
		}

		private void InsertNewUserToMysql( GCToCS.Login login, CSUser user )
		{
			if ( user == null )
				return;

			CSToDB.ExeSQL_Call sqlCall = new CSToDB.ExeSQL_Call
			{
				Sql = $"insert account_user(id,cs_id,sdk_id,cdkey) values({user.guid},{CS.instance.csKernelCfg.unCSId},{login.Sdk},\'{login.Name}\');"
			};
			this._cdkeyWrapper.EncodeAndSendToDBThread( sqlCall, ( int )CSToDB.MsgID.EExeSqlCall );

			string op =
				$"insert gameuser(obj_id,sdk_id,obj_cdkey, obj_register_time) values({user.guid},{login.Sdk},\'{login.Name}\',{user.userDbData.usrDBData.tRegisteUTCMillisec});" +
				$"insert gameuser_runne(user_id) values({user.guid});" +
				$"insert gameuser_guide(obj_id) values({user.guid});";
			CSToDB.UpdateUser updateUser = new CSToDB.UpdateUser
			{
				Sqlstr = op,
				Guid = ( long )user.guid
			};
			this._userCacheDBActiveWrapper.EncodeAndSendToDBThread( updateUser, ( int )CSToDB.MsgID.EUpdateUserDbcallBack );
		}

		public DBActiveWrapper GetDBSource( int actorID )
		{
			if ( this._userCacheDBActiveWrapper.actorID == actorID )
				return this._userCacheDBActiveWrapper;

			if ( this._cdkeyWrapper.actorID == actorID )
				return this._cdkeyWrapper;

			foreach ( DBActiveWrapper userAskDbActiveWrapper in this._userAskDBActiveWrappers )
			{
				if ( userAskDbActiveWrapper.actorID == actorID )
					return userAskDbActiveWrapper;
			}
			return null;
		}

		private void OnTimeUpdate()
		{
			DateTime oldDay = this.today;
			this.today = DateTime.Now;
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

		public void OnHeartBeatImmediately()
		{
			this.OnTimeUpdate();
			this.SynUserAskDBCallBack();
		}
	}
}