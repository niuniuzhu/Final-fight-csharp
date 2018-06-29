using CentralServer.Tools;
using CentralServer.User;
using Core.Misc;
using Google.Protobuf;
using ProtoBuf;
using Shared;
using StackExchange.Redis;
using System.IO;
using System.Threading.Tasks;

namespace CentralServer.UserModule
{
	[ProtoContract]
	public struct DBUserCache
	{
		[ProtoMember( 1 )]
		public ulong guid;
		[ProtoMember( 2 )]
		public string szNickName;
		[ProtoMember( 3 )]
		public string bagStr;
		[ProtoMember( 4 )]
		public string slotStr;
		[ProtoMember( 5 )]
		public string heroStr;
		[ProtoMember( 6 )]
		public string friendStr;
		[ProtoMember( 7 )]
		public string blackStr;
		[ProtoMember( 8 )]
		public string itemStr;
		[ProtoMember( 9 )]
		public string mailStr;
		[ProtoMember( 10 )]
		public long n64Diamond;
		[ProtoMember( 11 )]
		public long n64Gold;
		[ProtoMember( 12 )]
		public long n64Score;
		[ProtoMember( 13 )]
		public ulong tLastFirstWinTime;
		[ProtoMember( 14 )]
		public uint un32LastGetLoginRewardDay;
		[ProtoMember( 15 )]
		public long tRegisteUTCMillisec;
		[ProtoMember( 16 )]
		public ushort un16Cldays;
		[ProtoMember( 17 )]
		public byte un8UserLv;
		[ProtoMember( 18 )]
		public short un16VipLv;
		[ProtoMember( 19 )]
		public ushort un16HeaderID;
		[ProtoMember( 20 )]
		public uint un32TotalAssist;
		[ProtoMember( 21 )]
		public uint un32TotalDeadTimes;
		[ProtoMember( 22 )]
		public uint un32TotalDestoryBuildings;
		[ProtoMember( 23 )]
		public uint un32TotalGameInns;
		[ProtoMember( 24 )]
		public uint un32TotalHeroKills;
		[ProtoMember( 25 )]
		public uint un32TotalWinInns;
		[ProtoMember( 26 )]
		public uint un32UserCurLvExp;
		[ProtoMember( 27 )]
		public int vipScore;
		[ProtoMember( 28 )]
		public string guideStr;
		[ProtoMember( 29 )]
		public short n16Sex;
		[ProtoMember( 30 )]
		public string szTaskData;
	}

	public partial class CSUserMgr
	{
		private async Task<ErrorCode> QueryUserAsync( CSToDB.QueryUserReq queryUser )
		{
			ErrorCode errorCode;
			//若Redis缓存可用，则查询；不可用就直接查询数据库（通过RedisIP配置控制）
			ConnectionMultiplexer redis = CS.instance.GetUserDBCacheRedisHandler();
			if ( redis.IsConnected )
			{
				errorCode = await redis.GetDatabase().StringGetAsync( $"usercache:{queryUser.Objid}" ).ContinueWith( this.OnRedisQueryUser, queryUser );
			}
			else
			{
				//todo
				//查询数据库
				//errorCode = GetNowWorkActor().EncodeAndSendToDBThread( *queryUser, queryUser.msgid() );
				errorCode = ErrorCode.Success;
			}

			return errorCode;
		}

		private ErrorCode OnRedisQueryUser( Task<RedisValue> task, object state )
		{
			CSToDB.QueryUserReq pQueryUser = ( CSToDB.QueryUserReq )state;
			ErrorCode res;
			do
			{
				//这里为什么还要检测一次?
				SUserNetInfo netinfo = new SUserNetInfo( pQueryUser.Gsid, ( uint )pQueryUser.Gcnetid );
				if ( this.ContainsUser( netinfo ) )
				{
					res = ErrorCode.InvalidNetState;
					break;
				}

				GCToCS.Login sLoginMsg = new GCToCS.Login();
				sLoginMsg.MergeFrom( ByteString.CopyFromUtf8( pQueryUser.Logininfo ) );

				CSUser pcUser = this.GetUser( ( ulong )pQueryUser.Objid );
				if ( null != pcUser )
				{
					pcUser.OnOnline( netinfo, sLoginMsg, false, false );
					return ErrorCode.Success;
				}

				if ( task.Result.IsNullOrEmpty || !task.Result.HasValue )
				{
					Logger.Error( "Null Reply" );
					res = ErrorCode.RedisReplyNil;
					break;
				}

				SUserDBData sSUserDBData = new SUserDBData();
				PODUsrDBData sPODUsrDBData = sSUserDBData.sPODUsrDBData;
				sPODUsrDBData.un64ObjIdx = ( ulong )pQueryUser.Objid;
				sSUserDBData.szUserName = sLoginMsg.Name;

				MemoryStream ms = new MemoryStream( task.Result );
				DBUserCache dbUserCache = Serializer.Deserialize<DBUserCache>( ms );
				System.Diagnostics.Debug.Assert( dbUserCache.guid == sPODUsrDBData.un64ObjIdx );
				sSUserDBData.szNickName = dbUserCache.szNickName;
				sPODUsrDBData.n64Diamond = dbUserCache.n64Diamond;
				sPODUsrDBData.n64Gold = dbUserCache.n64Gold;
				sPODUsrDBData.n64Score = dbUserCache.n64Score;
				sPODUsrDBData.tLastFirstWinTime = dbUserCache.tLastFirstWinTime;
				sPODUsrDBData.un32LastGetLoginRewardDay = dbUserCache.un32LastGetLoginRewardDay;
				sPODUsrDBData.tRegisteUTCMillisec = dbUserCache.tRegisteUTCMillisec;
				sPODUsrDBData.un16Cldays = dbUserCache.un16Cldays;
				sPODUsrDBData.un8UserLv = dbUserCache.un8UserLv;
				sPODUsrDBData.un16VipLv = dbUserCache.un16VipLv;
				sPODUsrDBData.un16HeaderID = dbUserCache.un16HeaderID;
				sPODUsrDBData.un32TotalAssist = dbUserCache.un32TotalAssist;
				sPODUsrDBData.un32TotalDeadTimes = dbUserCache.un32TotalDeadTimes;
				sPODUsrDBData.un32TotalDestoryBuildings = dbUserCache.un32TotalDestoryBuildings;
				sPODUsrDBData.un32TotalGameInns = dbUserCache.un32TotalGameInns;
				sPODUsrDBData.un32TotalHeroKills = dbUserCache.un32TotalHeroKills;
				sPODUsrDBData.un32TotalWinInns = dbUserCache.un32TotalWinInns;
				sPODUsrDBData.un32UserCurLvExp = dbUserCache.un32UserCurLvExp;
				sPODUsrDBData.vipScore = dbUserCache.vipScore;
				sPODUsrDBData.n16Sex = dbUserCache.n16Sex;
				sSUserDBData.szTaskData = dbUserCache.szTaskData;

				pcUser = new CSUser();
				pcUser.LoadDBData( sSUserDBData );
				pcUser.InitRunes( dbUserCache.bagStr, dbUserCache.slotStr );
				pcUser.LoadFromRedisStr( dbUserCache.heroStr, dbUserCache.friendStr, dbUserCache.blackStr, dbUserCache.itemStr, dbUserCache.mailStr );
				//todo
				//pcUser.GetUserBattleInfoEx().mDebugName = sSUserDBData.szUserName;
				//pcUser.GetUserDBData().mGuideSteps.szCSContinueGuide = dbUserCache.guideStr;

				res = this.AddUser( pcUser );
				if ( res != ErrorCode.Success )
				{
					Logger.Error( "add user fail!" );
					return res;
				}

				pcUser.OnOnline( netinfo, sLoginMsg, false, true );
				this.RemoveUserFromRedisLRU( pcUser );
			} while ( false );

			if ( res != ErrorCode.Success )
			{
				Logger.Log( "没命中cache,查询数据库" );
				//todo
				//GetNowWorkActor().EncodeAndSendToDBThread( pQueryUser, CSToDB.MsgID.EQueryUserDbcallBack );
			}
			return ErrorCode.Success;
		}

		private bool RemoveUserFromRedisLRU( CSUser pUser )
		{
			ConnectionMultiplexer redis = CS.instance.GetUserDBCacheRedisHandler();
			if ( !redis.IsConnected )
				return false;
			redis.GetDatabase().KeyDeleteAsync( $"usercache:{pUser.guid}", CommandFlags.FireAndForget );
			Logger.Log( $"delete redis cache guid:{pUser.guid}" );
			return true;
		}
	}
}