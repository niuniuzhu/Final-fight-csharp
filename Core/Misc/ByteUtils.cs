namespace Core.Misc
{
	public static class ByteUtils
	{
		public static int Encode8u( byte[] p, int offset, byte c )
		{
			p[0 + offset] = c;
			return 1;
		}

		public static int Decode8u( byte[] p, int offset, ref byte c )
		{
			c = p[0 + offset];
			return 1;
		}

		public static int Encode16u( byte[] p, int offset, ushort w )
		{
			p[0 + offset] = ( byte )( w >> 0 );
			p[1 + offset] = ( byte )( w >> 8 );
			return 2;
		}

		public static int Decode16u( byte[] p, int offset, ref ushort c )
		{
			ushort result = 0;
			result |= p[0 + offset];
			result |= ( ushort )( p[1 + offset] << 8 );
			c = result;
			return 2;
		}

		public static int Encode32u( byte[] p, int offset, uint l )
		{
			p[0 + offset] = ( byte )( l >> 0 );
			p[1 + offset] = ( byte )( l >> 8 );
			p[2 + offset] = ( byte )( l >> 16 );
			p[3 + offset] = ( byte )( l >> 24 );
			return 4;
		}

		public static int Decode32u( byte[] p, int offset, ref uint c )
		{
			uint result = 0;
			result |= p[0 + offset];
			result |= ( uint )( p[1 + offset] << 8 );
			result |= ( uint )( p[2 + offset] << 16 );
			result |= ( uint )( p[3 + offset] << 24 );
			c = result;
			return 4;
		}

		public static int Encode64u( byte[] p, int offset, ulong l )
		{
			uint l0 = ( uint )( l & 0xffffffff );
			uint l1 = ( uint )( l >> 32 );
			int offset2 = Encode32u( p, offset, l0 );
			Encode32u( p, offset + offset2, l1 );
			return 8;
		}

		public static int Decode64u( byte[] p, int offset, ref ulong c )
		{
			uint l0 = 0;
			uint l1 = 0;
			int offset2 = Decode32u( p, offset, ref l0 );
			Decode32u( p, offset + offset2, ref l1 );
			c = l0 | ( ( ulong )l1 << 32 );
			return 8;
		}

		public static byte Decode8u( byte[] p, int offset )
		{
			return p[0 + offset];
		}

		public static ushort Decode16u( byte[] p, int offset )
		{
			ushort result = 0;
			result |= p[0 + offset];
			result |= ( ushort )( p[1 + offset] << 8 );
			return result;
		}

		public static uint Decode32u( byte[] p, int offset )
		{
			uint result = 0;
			result |= p[0 + offset];
			result |= ( uint )( p[1 + offset] << 8 );
			result |= ( uint )( p[2 + offset] << 16 );
			result |= ( uint )( p[3 + offset] << 24 );
			return result;
		}

		public static ulong Decode64u( byte[] p, int offset )
		{
			uint l0 = 0;
			uint l1 = 0;
			int offset2 = Decode32u( p, offset, ref l0 );
			Decode32u( p, offset + offset2, ref l1 );
			return l0 | ( ( ulong )l1 << 32 );
		}
	}
}