using System;
using Core.Misc;
using Google.Protobuf;
using MySql.Data.MySqlClient;
using Shared;

namespace CentralServer.UserModule
{
	public partial class CSUserMgr
	{
		private void UserCacheDBAsynHandler( GBuffer buffer )
		{
			MySqlConnection db = this.GetDBSource( buffer.actorID );
			if ( null == db )
			{
				Logger.Error( "db is null" );
				return;
			}

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
			}
		}

		private ErrorCode DBAsynUpdateUserCallback( GBuffer buffer, MySqlConnection db )
		{
			if ( null == db )
			{
				Logger.Warn( "invalid db" );
				return ErrorCode.InvalidDatabase;
			}

			CSToDB.UpdateUser msg = new CSToDB.UpdateUser();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			MySqlCommand myCommand = db.CreateCommand();
			try
			{
				db.Open();
				myCommand.CommandText = "begin;set autocommit=0;";
				myCommand.ExecuteNonQuery();
				myCommand.CommandText = msg.Sqlstr;
				myCommand.ExecuteNonQuery();
				myCommand.CommandText = "commit;";
				myCommand.ExecuteNonQuery();
			}
			catch ( Exception e )
			{
				Logger.Warn( $"sql execute error: user:{msg.Guid}, msg:{e}" );
				return ErrorCode.SqlExecError;
			}
			finally
			{
				db.Close();
			}
			Logger.Log( $"udate user {msg.Guid} to db" );
			return ErrorCode.Success;
		}

		private ErrorCode DBAsynAlterSNSList( GBuffer buffer, MySqlConnection db )
		{
			CSToDB.AlterSNSList msg = new CSToDB.AlterSNSList();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			return this.AlterUserSNSList( db, msg.UserId, msg.RelatedId, ( RelationShip )msg.Related, ( DBOperation )msg.Opration );
		}

		private ErrorCode AlterUserSNSList( MySqlConnection db, ulong askerGuid, ulong relatedID, RelationShip rsType, DBOperation opType )
		{
			if ( null == db )
			{
				Logger.Warn( "invalid db" );
				return ErrorCode.InvalidDatabase;
			}

			if ( DBOperation.Add == opType )
			{
				Logger.Info( $"add user:{askerGuid} to user:{relatedID} SNS as type:{rsType}" );
				ErrorCode errorCode = this.SqlExec( db, $"insert into gameuser_sns(user_id,related_id,relation) values({askerGuid},{relatedID},{rsType});" );
				if ( errorCode != ErrorCode.Success )
					return errorCode;
				Logger.Log( $"user:{askerGuid} add user:{relatedID} to SNS List as type:{rsType}" );
			}
			else
			{
				Logger.Info( $"remove user:{relatedID} from user:{askerGuid} SNS as type:{rsType}" );
				ErrorCode errorCode = this.SqlExec( db, $"delete from gameuser_sns where user_id={askerGuid} and related_id={relatedID};" );
				if ( errorCode != ErrorCode.Success )
					return errorCode;

				if ( rsType == RelationShip.Friends )
				{
					Logger.Info( $"remove user:{askerGuid} from user:{relatedID} SNS as type:{rsType}" );
					errorCode = this.SqlExec( db, $"delete from gameuser_sns where user_id={relatedID} and related_id={askerGuid};" );
					if ( errorCode != ErrorCode.Success )
						return errorCode;
				}
			}
			return ErrorCode.Success;
		}

		private ErrorCode DBAsyAlterItemCallBack( GBuffer buffer, MySqlConnection db )
		{
			CSToDB.AlterItem msg = new CSToDB.AlterItem();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			return this.DBAsynAlterUserItem( db, msg.SqlStr );
		}

		private ErrorCode DBAsynAlterUserItem( MySqlConnection db, string command )
		{
			if ( null == db )
			{
				Logger.Warn( "invalid db" );
				return ErrorCode.InvalidDatabase;
			}
			return this.SqlExec( db, command );
		}

		private ErrorCode DBAsynInsertNoticeCall( GBuffer buffer, MySqlConnection db )
		{
			if ( null == db )
			{
				Logger.Warn( "invalid db" );
				return ErrorCode.InvalidDatabase;
			}

			CSToDB.InsertNotice msg = new CSToDB.InsertNotice();
			msg.MergeFrom( buffer.GetBuffer(), 0, ( int )buffer.length );

			return this.SqlExec( db, msg.SqlStr );
		}

		private ErrorCode SqlExec( MySqlConnection db, string command )
		{
			MySqlCommand myCommand = db.CreateCommand();
			try
			{
				db.Open();
				myCommand.CommandText = command;
				myCommand.ExecuteNonQuery();
			}
			catch ( Exception e )
			{
				Logger.Warn( $"sql execute error:{e}" );
				return ErrorCode.SqlExecError;
			}
			finally
			{
				db.Close();
			}
			return ErrorCode.Success;
		}
	}
}