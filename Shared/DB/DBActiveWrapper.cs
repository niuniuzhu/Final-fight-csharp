using Core.Misc;
using Google.Protobuf;
using MySql.Data.MySqlClient;
using System;

namespace Shared.DB
{
	public delegate ErrorCode SqlExecQueryHandler( MySqlDataReader dataReader );

	public class DBActiveWrapper
	{
		public int actorID => this._active.actorID;

		/// <summary>
		/// Native MySQL connection
		/// </summary>
		private readonly MySqlConnection _db;
		/// <summary>
		/// 消息的生产和消费处理器
		/// </summary>
		private readonly DBActive _active;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="callback">消息消费后的回调函数(通常和生产者不在同一个线程上)</param>
		/// <param name="cfg">数据库配置信息</param>
		/// <param name="beginCallback">开始处理消息的回调函数</param>
		public DBActiveWrapper( Action<GBuffer> callback, DBCfg cfg, Action beginCallback )
		{
			this._active = new DBActive( callback, beginCallback );
			this._db = new MySqlConnection(
				$"server={cfg.aszDBHostIP};user id={cfg.aszDBUserName};password={cfg.aszDBUserPwd};port={cfg.un32DBHostPort};database={cfg.aszDBName}" );

		}

		/// <summary>
		/// 开始连接数据库
		/// </summary>
		public void Start() => this._active.Run();

		/// <summary>
		/// 断开数据库的连接
		/// </summary>
		public void Stop()
		{
		}

		/// <summary>
		/// 把消息编码到缓冲区
		/// </summary>
		public static ErrorCode EncodeProtoMsgToBuffer( IMessage msg, int msgID, GBuffer buffer )
		{
			buffer.Write( msg.ToByteArray() );
			buffer.position = 0;
			buffer.data = msgID;
			return ErrorCode.Success;
		}

		/// <summary>
		/// 把消息编码到缓冲区并投递到消息处理器
		/// </summary>
		public ErrorCode EncodeAndSendToDBThread( IMessage msg, int msgID )
		{
			GBuffer buffer = this._active.GetBuffer();
			ErrorCode errCode = EncodeProtoMsgToBuffer( msg, msgID, buffer );
			if ( errCode != ErrorCode.Success )
			{
				this._active.ReleaseBuffer( buffer );
				return ErrorCode.EncodeMsgToBufferFailed;
			}
			buffer.actorID = this._active.actorID;
			this._active.Send( buffer );
			return ErrorCode.Success;
		}

		/// <summary>
		/// 执行查询指令
		/// </summary>
		/// <param name="command">sql指令</param>
		/// <param name="handler">查询结果的回调函数</param>
		/// <returns>错误信息</returns>
		public ErrorCode SqlExecQuery( string command, SqlExecQueryHandler handler )
		{
			if ( null == this._db )
			{
				Logger.Warn( "invalid db" );
				return ErrorCode.InvalidDatabase;
			}

			MySqlCommand sqlCmd = this._db.CreateCommand();
			MySqlDataReader dataReader = null;
			try
			{
				this._db.Open();
				sqlCmd.CommandText = command;
				sqlCmd.ExecuteNonQuery();
				dataReader = sqlCmd.ExecuteReader();
			}
			catch ( Exception e )
			{
				Logger.Warn( $"sql:{sqlCmd.CommandText} execute error:{e}", 2, 2 );
				dataReader?.Close();
				this._db.Close();
				return ErrorCode.SqlExecError;
			}

			ErrorCode errorCode = ErrorCode.Success;
			if ( handler != null )
				errorCode = handler.Invoke( dataReader );

			dataReader?.Close();
			this._db.Close();
			return errorCode;
		}

		/// <summary>
		/// 执行查询指令
		/// </summary>
		/// <param name="command">sql指令</param>
		/// <returns>错误信息</returns>
		public ErrorCode SqlExecNonQuery( string command )
		{
			if ( null == this._db )
			{
				Logger.Warn( "invalid db" );
				return ErrorCode.InvalidDatabase;
			}

			MySqlCommand sqlCmd = this._db.CreateCommand();
			try
			{
				this._db.Open();
				sqlCmd.CommandText = command;
				sqlCmd.ExecuteNonQuery();
			}
			catch ( Exception e )
			{
				Logger.Warn( $"sql:{sqlCmd.CommandText} execute error:{e}", 2, 2 );
				return ErrorCode.SqlExecError;
			}
			finally
			{
				this._db.Close();
			}
			return ErrorCode.Success;
		}

		/// <summary>
		/// 执行连串查询指令
		/// </summary>
		/// <param name="commands">sql指令集合</param>
		/// <returns>错误信息</returns>
		public ErrorCode SqlExecNonQuery( string[] commands )
		{
			if ( null == this._db )
			{
				Logger.Warn( "invalid db" );
				return ErrorCode.InvalidDatabase;
			}

			MySqlCommand sqlCmd = this._db.CreateCommand();
			try
			{
				this._db.Open();
				foreach ( string command in commands )
				{
					sqlCmd.CommandText = command;
					sqlCmd.ExecuteNonQuery();
				}
			}
			catch ( Exception e )
			{
				Logger.Warn( $"sql:{sqlCmd.CommandText} execute error:{e}", 2, 2 );
				return ErrorCode.SqlExecError;
			}
			finally
			{
				this._db.Close();
			}
			return ErrorCode.Success;
		}
	}
}