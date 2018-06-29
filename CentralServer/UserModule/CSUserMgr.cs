using CentralServer.User;
using Core.Misc;
using Google.Protobuf;
using Shared;
using System.Collections.Generic;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
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

		private readonly Dictionary<SUserNetInfo, CSUser> _userNetMap = new Dictionary<SUserNetInfo, CSUser>();
		private readonly Dictionary<ulong, CSUser> _userGUIDMap = new Dictionary<ulong, CSUser>();
		private readonly Dictionary<string, CSUser> _nickNameMap = new Dictionary<string, CSUser>();
		private readonly Dictionary<UserCombineKey, ulong> _allUserName2GUIDMap = new Dictionary<UserCombineKey, ulong>();
		private int _maxGuid;

		public CSUserMgr()
		{
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin] = this.OnMsgToGstoCsfromGcAskLogin;
		}

		public bool ContainsUser( SUserNetInfo userNetInfo )
		{
			return this._userNetMap.ContainsKey( userNetInfo );
		}

		private ErrorCode AddUser( CSUser pcUser )
		{
			if ( null == pcUser )
				return ErrorCode.NullUser;

			pcUser.ResetPingTimer();

			ulong un64ObjIndex = pcUser.guid;

			if ( string.IsNullOrEmpty( pcUser.username ) )
			{
				Logger.Error( "invalid username" );
				return ErrorCode.InvalidUserName;
			}

			if ( !this._userGUIDMap.TryAdd( un64ObjIndex, pcUser ) )
			{
				Logger.Error( "add username fail" );
				return ErrorCode.AddUserNameFailed;
			}

			if ( !string.IsNullOrEmpty( pcUser.nickname ) )
				this._nickNameMap.Add( pcUser.nickname, pcUser );

			long timerID = CS.instance.AddTimer( pcUser.CheckDbSaveTimer, 1000, true );//todo CCSCfgMgr::getInstance().GetCSGlobalCfg().dbSaveTimeSpace * 1000
			pcUser.timerID = timerID;

			return ErrorCode.Success;
		}

		public CSUser GetUser( SUserNetInfo userNetInfo )
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

		private bool CheckIfInGuideBattle( CSUser pUser )
		{
			//todo
			return true;
		}

		private ulong CombineGUID()
		{
			++this._maxGuid;
			//todo
			//return this._maxGuid * GUID_Devide + CS.instance.m_sCSKernelCfg.unCSId;
			return ( ulong )this._maxGuid;
		}

		private void InsertNewUserToMysql( GCToCS.Login login, CSUser pcUser )
		{
		}

		public ErrorCode Invoke( CSGSInfo csgsInfo, int msgID, uint gcNetID, byte[] data, int offset, int size )
		{
			if ( this._gcMsgHandlers.TryGetValue( msgID, out GCMsgHandler handler ) )
				return handler.Invoke( csgsInfo, gcNetID, data, offset, size );
			Logger.Warn( $"invalid msg:{msgID}." );
			return ErrorCode.InvalidMsgProtocalID;
		}

		private static void PostMsgToGCAskReturn( CSGSInfo csgsInfo, uint gcNetID, int askProtocalID, ErrorCode errorCode )
		{
			GSToGC.AskRet msg = new GSToGC.AskRet
			{
				Askid = askProtocalID,
				Errorcode = ( int )errorCode
			};
			CS.instance.netSessionMgr.TranMsgToSession( csgsInfo.m_n32NSID, msg,
														( int )GSToGC.MsgID.EMsgToGcfromGsGcaskRet,
														gcNetID == 0 ? 0 : ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc,
														gcNetID );
		}

		private ErrorCode OnMsgToGstoCsfromGcAskLogin( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			GCToCS.Login login = new GCToCS.Login();
			login.MergeFrom( data, offset, size );
			Logger.Log( $"--new login({login.Name})--" );
			ErrorCode errorCode = this.UserAskLogin( csgsInfo, gcNetID, login );
			if ( ErrorCode.Success != errorCode )
				PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin, errorCode );
			return ErrorCode.Success;
		}
	}
}