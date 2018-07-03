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
	public partial class CSUserMgr
	{
		private async Task<ErrorCode> QueryUserAsync( CSToDB.QueryUserReq queryUser )
		{
			ErrorCode errorCode;
			//若Redis缓存可用，则查询；不可用就直接查询数据库（通过RedisIP配置控制）
			ConnectionMultiplexer redis = CS.instance.GetUserDBCacheRedisHandler();
			if ( redis.IsConnected )
				errorCode = await redis.GetDatabase().StringGetAsync( $"usercache:{queryUser.Objid}" ).ContinueWith( this.OnRedisQueryUser, queryUser );
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
				UserNetInfo netinfo = new UserNetInfo( pQueryUser.Gsid, ( uint )pQueryUser.Gcnetid );
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

				UserDBData userDbData;
				using ( MemoryStream ms = new MemoryStream( task.Result ) )
				{
					userDbData = Serializer.Deserialize<UserDBData>( ms );
				}
				userDbData.usrDBData.un64ObjIdx = ( ulong )pQueryUser.Objid;
				userDbData.szUserName = sLoginMsg.Name;

				pcUser = new CSUser();
				pcUser.LoadDBData( userDbData );
				//todo
				//pcUser.GetUserBattleInfoEx().mDebugName = userDbData.szUserName;
				//pcUser.GetUserDBData().guideSteps.szCSContinueGuide = dbUserCache.guideStr;

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