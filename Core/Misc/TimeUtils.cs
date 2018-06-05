using System;

namespace Core.Misc
{
	public static class TimeUtils
	{
		private static readonly DateTime UTC_TIME_BEGIN = DateTime.MinValue;

		public static long utcTime => ( long )DateTime.UtcNow.Subtract( UTC_TIME_BEGIN ).TotalMilliseconds;

		public static string GetLocalTime( long milliseconds )
		{
			return UTC_TIME_BEGIN.AddMilliseconds( milliseconds ).ToLocalTime().ToString( "HH:mm:ss:fff" );
		}
	}
}