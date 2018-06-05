using System;

namespace Core.Misc
{
	public static class GuidHash
	{
		public static string GetString()
		{
			return Convert.ToBase64String( GetBytes() );
		}

		public static byte[] GetBytes()
		{
			return Guid.NewGuid().ToByteArray();
		}

		public static uint GetUInt32()
		{
			return BitConverter.ToUInt32( GetBytes(), 0 );
		}

		public static ulong GetUInt64()
		{
			return BitConverter.ToUInt64( GetBytes(), 0 );
		}
	}
}