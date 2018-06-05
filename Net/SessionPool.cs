using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Net
{
	public enum SessionType
	{
		None,
		ServerCsOnlySs,
		ServerCsOnlyGS,
		ServerSs,
		ServerGS,
		ServerLog,
		ServerBSOnlyGS,
		ServerBSOnlyGc,
		ServerLsOnlyBS,
		ServerLsOnlyGc,
		ClientB2L,
		ClientG2C,
		ClientG2S,
		ClientG2B,
		ClientS2C,//as client to link gs
		ServerCsOnlyRc,
		ClientC2Lg,
		ClientS2Lg,
		ClientC2L,// link login server
		ClientC2B,//link balance server
		ClientC2G,//link gate server
		ClientC2R,
		ClientS2Log,
		ClientC2Log,
		ClientC2LogicRedis,
	};

	public class SessionPool
	{
		private static SessionPool _instance;
		public static SessionPool instance => _instance ?? ( _instance = new SessionPool() );


		private static int _gid;

		private readonly ConcurrentDictionary<Type, ConcurrentQueue<Session>> _typeToObjects = new ConcurrentDictionary<Type, ConcurrentQueue<Session>>();

		public T Pop<T>( SessionType sessionType ) where T : Session
		{
			Type type = typeof( T );
			if ( !this._typeToObjects.TryGetValue( type, out ConcurrentQueue<Session> objs ) )
			{
				objs = new ConcurrentQueue<Session>();
				this._typeToObjects[type] = objs;
			}

			if ( objs.Count == 0 )
			{
				return ( T )Activator.CreateInstance( typeof( T ), BindingFlags.NonPublic | BindingFlags.Instance,
														   Type.DefaultBinder,
														   new object[] { Interlocked.Increment( ref _gid ) }, sessionType );
			}
			objs.TryDequeue( out Session session );
			return ( T )session;
		}

		public void Push( Session session )
		{
			this._typeToObjects[session.GetType()].Enqueue( session );
		}

		public void Dispose()
		{
			foreach ( KeyValuePair<Type, ConcurrentQueue<Session>> kv in this._typeToObjects )
			{
				ConcurrentQueue<Session> queue = kv.Value;
				while ( !queue.IsEmpty )
				{
					queue.TryDequeue( out Session session );
					session.Dispose();
				}
			}
			this._typeToObjects.Clear();
		}
	}
}