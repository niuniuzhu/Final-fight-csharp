using System.Collections.Generic;
using Core.Misc;

namespace CentralServer.Tools
{
	public class ThreadTimer
	{
		long nextexpiredTime;
		long lastHandleTime;
		long sequence;
		HeartbeatCallback pHeartbeatCallback;
		long interval;
		bool ifPersist;

		public ThreadTimer( long nextexpiredTime, long lastHandleTime, HeartbeatCallback pHeartbeatCallback, long interval, long sequence, bool ifPersist )
		{
			this.nextexpiredTime = nextexpiredTime;
			this.lastHandleTime = lastHandleTime;
			this.pHeartbeatCallback = pHeartbeatCallback;
			this.interval = interval;
			this.sequence = sequence;
			this.ifPersist = ifPersist;
		}

		public static bool operator <( ThreadTimer a, ThreadTimer b )
		{
			if ( a.nextexpiredTime != b.nextexpiredTime )
				return a.nextexpiredTime > b.nextexpiredTime;
			return a.sequence > b.sequence;
		}

		public static bool operator >( ThreadTimer a, ThreadTimer b ) => !( a < b );
	}

	public class BattleTimer
	{
		private readonly HashSet<long> _invalidTimerSet = new HashSet<long>();
		private readonly List<ThreadTimer> _toAddTimer = new List<ThreadTimer>();
		private long _initTime;
		private long _timerSeq;

		public long internalTime => TimeUtils.utcTime - this._initTime;
		public long timerSequence => ++this._timerSeq;

		public long AddTimer( HeartbeatCallback pHeartbeatCallback, long interval, bool ifPersist )
		{
			long nowTime = this.internalTime;
			long seqID = this.timerSequence;
			long nextTime = nowTime + interval;
			ThreadTimer lThreadTimer = new ThreadTimer( nextTime, nowTime, pHeartbeatCallback, interval, seqID, ifPersist );
			this._toAddTimer.Add( lThreadTimer );
			return seqID;
		}

		public void RemoveTimer( long timerID ) => this._invalidTimerSet.Add( timerID );
	}
}