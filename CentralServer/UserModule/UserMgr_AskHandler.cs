using CentralServer.Tools;
using CentralServer.User;
using Core.Misc;
using Google.Protobuf;
using Shared;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		private ErrorCode UserAskLogin( CSGSInfo csgsInfo, uint gcNetID, GCToCS.Login login )
		{
			if ( string.IsNullOrEmpty( login.Name ) || login.Name.Length > Consts.DEFAULT_NAME_LEN )
				return ErrorCode.InvalidUserName;

			UserNetInfo netinfo = new UserNetInfo( csgsInfo.m_n32GSID, gcNetID );
			if ( this.ContainsUser( netinfo ) )
				return ErrorCode.InvalidNetState;

			ErrorCode errorCode = ErrorCode.Success;

			UserCombineKey sUserCombineKey = new UserCombineKey( login.Name, login.Sdk );
			if ( this._allUserName2GUIDMap.TryGetValue( sUserCombineKey, out ulong guid ) )
			{
				//老玩家
				//如果还在内存里
				CSUser user = this.GetUser( guid );
				if ( null != user )
				{
					bool bFlag = this.CheckIfInGuideBattle( user );
					if ( bFlag )
					{
						Logger.Warn( "新手引导玩家不允许顶号" );
						return ErrorCode.GuideUserForbit;
					}

					user.OnOnline( netinfo, login, false, false );
					return ErrorCode.Success;
				}

				//异步查询玩家数据
				CSToDB.QueryUserReq queryUser = new CSToDB.QueryUserReq
				{
					Logininfo = login.ToByteString().ToStringUtf8(),
					Gsid = csgsInfo.m_n32GSID,
					Gcnetid = ( int )gcNetID,
					Csid = ( int )CS.instance.csCfg.unCSId,
					Objid = ( long )guid
				};
				errorCode = this.QueryUserAsync( queryUser ).Result;
			}
			else
			{
				//新玩家，产生GUID
				guid = this.CombineGUID();

				CSUser pcUser = new CSUser();
				SUserDBData sUserDBData = new SUserDBData();
				sUserDBData.sPODUsrDBData.un64ObjIdx = guid;
				sUserDBData.szUserName = login.Name;
				sUserDBData.szUserPwd = login.Passwd;
				sUserDBData.sPODUsrDBData.userPlatform = ( UserPlatform )login.Sdk;
				sUserDBData.sPODUsrDBData.tRegisteUTCMillisec = TimeUtils.utcTime;

				//加入全局表
				this._allUserName2GUIDMap.Add( sUserCombineKey, guid );

				pcUser.LoadDBData( sUserDBData );
				//todo
				//pcUser.userBattleInfoEx.mDebugName = login.Name;

				ErrorCode nRet = this.AddUser( pcUser );
				if ( nRet != 0 )
					return nRet;

				pcUser.OnOnline( netinfo, login, true, true );

				this.InsertNewUserToMysql( login, pcUser );
			}
			//todo
			//log
			//{
			//	string mystream = string.Empty;
			//	mystream << login.name() << LOG_SIGN;
			//	mystream << login.sdk() << LOG_SIGN;
			//	mystream << login.platform() << LOG_SIGN;
			//	mystream << login.equimentid() << LOG_SIGN;
			//	mystream << login.ipaddress();
			//	CSSGameLogMgr::GetInstance().AddGameLog( eLog_Login, guid, mystream.str() );
			//}
			return errorCode;
		}

		private ErrorCode UserAskReconnectGame( CSGSInfo csgsInfo, uint gcNetID, string name, string passwd )
		{
			UserNetInfo netinfo = new UserNetInfo( csgsInfo.m_n32GSID, gcNetID );
			if ( this.ContainsUser( netinfo ) )
				return ErrorCode.InvalidNetState;

			//需要从消息获取
			const int sdkID = 0;
			UserCombineKey sUserCombineKey = new UserCombineKey( name, sdkID );
			if ( !this._allUserName2GUIDMap.TryGetValue( sUserCombineKey, out ulong guid ) )
				return ErrorCode.NullUser;

			CSUser pcUser = this.GetUser( guid );
			if ( null == pcUser )
				return ErrorCode.NullUser;

			GCToCS.Login login = new GCToCS.Login();
			pcUser.OnOnline( netinfo, login, false, false, true );
			return ErrorCode.Success;
		}
	}
}