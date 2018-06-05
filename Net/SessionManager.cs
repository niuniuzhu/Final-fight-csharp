using System.Collections.Concurrent;
using System.Threading;

namespace Net
{
	public enum SessionType
	{
		StNone,
		///////////////////////////
		StServerCsOnlySs,
		StServerCsOnlyGS,
		StServerSs,
		StServerGS,
		StServerLog,
		StServerBSOnlyGS,
		StServerBSOnlyGc,
		StServerLsOnlyBS,
		StServerLsOnlyGc,
		StClientB2L,
		StClientG2C,
		StClientG2S,
		StClientG2B,
		StClientS2C,//as client to link gs
		StServerCsOnlyRc,
		StClientC2Lg,
		StClientS2Lg,
		StClientC2L,// link login server
		StClientC2B,//link balance server
		StClientC2G,//link gate server
		StClientC2R,
		StClientS2Log,
		StClientC2Log,
		StClientC2LogicRedis,
	};

	public class SessionManager
	{
		private static SessionManager _instance;
		public static SessionManager instance => _instance ?? ( _instance = new SessionManager() );

		private static int _gid;

		private readonly ConcurrentQueue<Session> _pool = new ConcurrentQueue<Session>();

		public Session Pop( SessionType type )
		{
			if ( this._pool.IsEmpty )
				return new Session( Interlocked.Increment( ref _gid ), type );
			this._pool.TryDequeue( out Session session );
			return session;
		}

		public void Push( Session session )
		{
			this._pool.Enqueue( session );
		}

		public void Dispose()
		{
			while ( !this._pool.IsEmpty )
			{
				this._pool.TryDequeue( out Session connData );
				connData.Dispose();
			}
		}
	}
}