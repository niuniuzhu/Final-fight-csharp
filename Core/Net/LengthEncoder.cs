using Core.Misc;

namespace Core.Net
{
	public static class LengthEncoder
	{
		private const int LENGTH_SIZE = sizeof( ushort );

		public static byte[] Encode( byte[] data, int offset, int size )
		{
			byte[] result = new byte[size + LENGTH_SIZE];
			int mOffset = ByteUtils.Encode16u( result, 0, ( ushort )size );
			System.Buffer.BlockCopy( data, offset, result, mOffset, size );
			return result;
		}

		public static int Decode( byte[] data, int offset, int size, out byte[] result )
		{
			if ( size < LENGTH_SIZE )//包头还没有收完(ushort=4bytes)
			{
				result = null;
				return -1;
			}
			ushort length = 0;
			ByteUtils.Decode16u( data, offset, ref length );
			if ( length > size - LENGTH_SIZE )//还没有足够数组
			{
				result = null;
				return -1;
			}
			result = new byte[length];
			System.Buffer.BlockCopy( data, offset + LENGTH_SIZE, result, 0, length );
			return LENGTH_SIZE + length;
		}
	}
}