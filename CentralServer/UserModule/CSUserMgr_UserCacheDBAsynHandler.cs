using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.DB;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		/// <summary>
		/// 异步方式储存玩家数据
		/// </summary>
		private void UserCacheDBAsynHandler( GBuffer buffer )
		{
			DBActiveWrapper db = this.GetDBSource( buffer.actorID );
			CSToDB.MsgID msgID = ( CSToDB.MsgID )buffer.data;
			switch ( msgID )
			{
				case CSToDB.MsgID.EUpdateUserDbcallBack:
					this.DBAsynUpdateUserCallback( buffer, db );
					break;

				case CSToDB.MsgID.EAlterSnslistDbcall:
					this.DBAsynAlterSNSList( buffer, db );
					break;

				case CSToDB.MsgID.EAlterItemDbcall:
					this.DBAsyAlterItemCallBack( buffer, db );
					break;

				case CSToDB.MsgID.EInsertNoticeDbcall:
					this.DBAsynInsertNoticeCall( buffer, db );
					break;

				default:
					Logger.Error( "unknown msg" );
					break;
			}
		}

		private ErrorCode DBAsynUpdateUserCallback( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.UpdateUser msg = new CSToDB.UpdateUser();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			ErrorCode errorCode = db.SqlExecNonQuery( new[] { "begin;set autocommit=0;", msg.Sqlstr, "commit;" } );
			if ( errorCode != ErrorCode.Success )
				return errorCode;
			Logger.Log( $"udate user {msg.Guid} to db" );
			return ErrorCode.Success;
		}

		private ErrorCode DBAsynAlterSNSList( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.AlterSNSList msg = new CSToDB.AlterSNSList();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			return this.AlterUserSNSList( db, msg.UserId, msg.RelatedId, ( RelationShip )msg.Related, ( DBOperation )msg.Opration );
		}

		private ErrorCode AlterUserSNSList( DBActiveWrapper db, ulong askerGuid, ulong relatedID, RelationShip rsType, DBOperation opType )
		{
			if ( DBOperation.Add == opType )
			{
				Logger.Info( $"add user:{askerGuid} to user:{relatedID} SNS as type:{rsType}" );
				ErrorCode errorCode = db.SqlExecNonQuery( $"insert into gameuser_sns(user_id,related_id,relation) values({askerGuid},{relatedID},{rsType});" );
				if ( errorCode != ErrorCode.Success )
					return errorCode;
				Logger.Log( $"user:{askerGuid} add user:{relatedID} to SNS List as type:{rsType}" );
			}
			else
			{
				Logger.Info( $"remove user:{relatedID} from user:{askerGuid} SNS as type:{rsType}" );
				ErrorCode errorCode = db.SqlExecNonQuery( $"delete from gameuser_sns where user_id={askerGuid} and related_id={relatedID};" );
				if ( errorCode != ErrorCode.Success )
					return errorCode;

				if ( rsType == RelationShip.Friends )
				{
					Logger.Info( $"remove user:{askerGuid} from user:{relatedID} SNS as type:{rsType}" );
					errorCode = db.SqlExecNonQuery( $"delete from gameuser_sns where user_id={relatedID} and related_id={askerGuid};" );
					if ( errorCode != ErrorCode.Success )
						return errorCode;
				}
			}
			return ErrorCode.Success;
		}

		private ErrorCode DBAsyAlterItemCallBack( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.AlterItem msg = new CSToDB.AlterItem();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			return this.DBAsynAlterUserItem( db, msg.SqlStr );
		}

		private ErrorCode DBAsynAlterUserItem( DBActiveWrapper db, string command )
		{
			return db.SqlExecNonQuery( command );
		}

		private ErrorCode DBAsynInsertNoticeCall( GBuffer buffer, DBActiveWrapper db )
		{
			CSToDB.InsertNotice msg = new CSToDB.InsertNotice();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			return db.SqlExecNonQuery( msg.SqlStr );
		}
	}
}