using Google.Protobuf;
using MySql.Data.MySqlClient;
using System;

namespace Shared.DB
{
	public class DBActiveWrapper
	{
		public int actorID => this._active.actorID;
		public MySqlConnection db { get; private set; }

		private readonly DBActive _active;
		private readonly DBCfg _cfg;

		public DBActiveWrapper( Action<GBuffer> callback, DBCfg cfg, Action beginCallback )
		{
			this._cfg = cfg;
			this._active = new DBActive( callback, beginCallback );
		}

		public void Start()
		{
			this.db = new MySqlConnection( $"server={this._cfg.aszDBHostIP};user id={this._cfg.aszDBUserName};password={this._cfg.aszDBUserPwd};port={this._cfg.un32DBHostPort};database={this._cfg.aszDBName}" );
		}

		public void Stop()
		{
		}

		private bool EncodeProtoMsgToBuffer( IMessage msg, int msgID, GBuffer buffer )
		{
			buffer.Write( msg.ToByteArray() );
			buffer.position = 0;
			buffer.data = msgID;
			return true;
		}

		public bool EncodeAndSendToDBThread( IMessage msg, int msgID )
		{
			GBuffer buffer = this._active.GetBuffer();
			bool res = this.EncodeProtoMsgToBuffer( msg, msgID, buffer );
			if ( !res )
			{
				this._active.ReleaseBuffer( buffer );
				return false;
			}
			buffer.actorID = this._active.actorID;
			this._active.Send( buffer );
			return true;
		}
	}
}