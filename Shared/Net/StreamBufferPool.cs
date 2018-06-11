using Core.Net;
using System.Collections.Concurrent;

namespace Shared.Net
{
	public static class StreamBufferPool
	{
		private static readonly ConcurrentQueue<StreamBuffer> POOL = new ConcurrentQueue<StreamBuffer>();

		public static StreamBuffer Pop()
		{
			if ( POOL.IsEmpty )
				return new StreamBuffer();
			POOL.TryDequeue( out StreamBuffer buffer );
			return buffer;
		}

		public static void Push( StreamBuffer buffer )
		{
			buffer.Clear();
			POOL.Enqueue( buffer );
		}
	}
}