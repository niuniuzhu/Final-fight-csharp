using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Core.Net
{
	public class NetSessionPool
	{
		private static NetSessionPool _instance;
		public static NetSessionPool instance => _instance ?? ( _instance = new NetSessionPool() );
		private static int _gid;

		private readonly ConcurrentDictionary<Type, ConcurrentQueue<INetSession>> _typeToObjects = new ConcurrentDictionary<Type, ConcurrentQueue<INetSession>>();

		private NetSessionPool()
		{
		}

		public T Pop<T>() where T : INetSession
		{
			Type type = typeof( T );
			if ( !this._typeToObjects.TryGetValue( type, out ConcurrentQueue<INetSession> objs ) )
			{
				objs = new ConcurrentQueue<INetSession>();
				this._typeToObjects[type] = objs;
			}

			if ( objs.Count == 0 )
			{
				uint id = ( uint )Interlocked.Increment( ref _gid );
				if ( id == uint.MaxValue )
					return default( T );

				return ( T )Activator.CreateInstance( typeof( T ), BindingFlags.NonPublic | BindingFlags.Instance,
													   null,
													   new object[] { id }, null );
			}
			objs.TryDequeue( out INetSession session );
			return ( T )session;
		}

		public void Push( INetSession session )
		{
			this._typeToObjects[session.GetType()].Enqueue( session );
		}

		public void Dispose()
		{
			foreach ( KeyValuePair<Type, ConcurrentQueue<INetSession>> kv in this._typeToObjects )
			{
				ConcurrentQueue<INetSession> queue = kv.Value;
				while ( !queue.IsEmpty )
				{
					queue.TryDequeue( out INetSession session );
					session.Dispose();
				}
			}
			this._typeToObjects.Clear();
		}
	}
}