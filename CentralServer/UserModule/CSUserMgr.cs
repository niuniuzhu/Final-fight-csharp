﻿using System;
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

		private delegate ErrorCode GCMsgHandler( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size );

		private readonly Dictionary<int, GCMsgHandler> _gcMsgHandlers = new Dictionary<int, GCMsgHandler>();

		private readonly Dictionary<UserNetInfo, CSUser> _userNetMap = new Dictionary<UserNetInfo, CSUser>();
		private readonly Dictionary<ulong, CSUser> _userGUIDMap = new Dictionary<ulong, CSUser>();
		private readonly Dictionary<string, CSUser> _nickNameMap = new Dictionary<string, CSUser>();
		private readonly Dictionary<UserCombineKey, ulong> _allUserName2GUIDMap = new Dictionary<UserCombineKey, ulong>();
		private readonly HashSet<string> _allNickNameSet = new HashSet<string>();
		private readonly List<Notice> _notices = new List<Notice>();
		private int _maxGuid;

		public CSUserMgr()
		{
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin] = this.OnMsgToGstoCsfromGcAskLogin;
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskReconnectGame] = this.OnMsgToGstoCsfromGcAskReconnectGame;
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskComleteUserInfo] = this.OnMsgToGstoCsfromGcAskComleteUserInfo;
		}

		public bool ContainsUser( UserNetInfo userNetInfo )
		{
			return this._userNetMap.ContainsKey( userNetInfo );
		}

		private ErrorCode AddUser( CSUser csUser )
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

		public CSUser GetUser( UserNetInfo userNetInfo )
		{
			this._userNetMap.TryGetValue( userNetInfo, out CSUser user );
			return user;
		}

		private CSUser GetUser( ulong guid )
		{
			this._userGUIDMap.TryGetValue( guid, out CSUser user );
			return user;
		}

		private CSUser GetUser( string nickName )
		{
			this._nickNameMap.TryGetValue( nickName, out CSUser user );
			return user;
		}
		private CSUser GetUser( CSGSInfo csgsInfo, uint gcNetID )
		{
			UserNetInfo userNetInfo = new UserNetInfo( csgsInfo.m_n32GSID, gcNetID );
			return this.GetUser( userNetInfo );
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
			//return this._maxGuid * GUID_Devide + CS.instance.csCfg.unCSId;
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
			csUser.userDbData.ChangeUserDbData( EUserDBDataType.eUserDBType_NickName, nickname );
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
			//m_CdkeyWrapper->EncodeAndSendToDBThread( sChangeNickName, sChangeNickName.msgid() );
		}

		public void ForeachNotice( Action<Notice> handler )
		{
			int count = this._notices.Count;
			for ( int i = 0; i < count; i++ )
				handler.Invoke( this._notices[i] );
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
	}
}