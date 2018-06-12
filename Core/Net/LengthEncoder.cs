using Core.Misc;

namespace Core.Net
{
	public static class LengthEncoder
	{
		private const int LENGTH_SIZE = sizeof( int );

		public static byte[] Encode( byte[] data, int offset, int size )
		{
			byte[] result = new byte[size + LENGTH_SIZE];
			int mOffset = ByteUtils.Encode32i( result, 0, size );
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
			int length = 0;
			ByteUtils.Decode32i( data, offset, ref length );
			if ( length > size )//还没有足够数组
			{
				result = null;
				return -1;
			}
			result = new byte[length - LENGTH_SIZE];
			System.Buffer.BlockCopy( data, offset + LENGTH_SIZE, result, 0, length - LENGTH_SIZE );
			return length;
		}
	}
}