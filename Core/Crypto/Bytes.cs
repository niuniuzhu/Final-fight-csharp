using System;

namespace Core.Crypto
{
	/// <summary>
	/// 字节顺序
	/// </summary>
	public enum ByteOrder
	{
		LittleEndian,
		BigEndian
	}

	/// <summary>
	/// 字节流操作的一些便捷方法
	/// </summary>
	public static class Bytes
	{
		/// <summary>
		/// 将字节数组全置
		/// </summary>
		/// <param name="data">字节数组</param>
		public static void ZeroBytes( this byte[] data )
		{
			ZeroBytes( data, 0, data.Length );
		}

		/// <summary>
		/// 将字节数组指定位置后全置0
		/// </summary>
		/// <param name="data">字节数组</param>
		/// <param name="offset">起始位置</param>
		public static void ZeroBytes( this byte[] data, int offset )
		{
			ZeroBytes( data, offset, data.Length - offset );
		}

		/// <summary>
		/// 将字节数组部分置0
		/// </summary>
		/// <param name="data">字节数组</param>
		/// <param name="offset">起始位置</param>
		/// <param name="len">长度</param>
		public static void ZeroBytes( this byte[] data, int offset, int len )
		{
			SetBytes( data, offset, len, 0x00 );
		}

		/// <summary>
		/// 将字节数组全部置为指定值
		/// </summary>
		/// <param name="data">字节数组</param>
		/// <param name="value">设置值</param>
		public static void SetBytes( this byte[] data, byte value )
		{
			SetBytes( data, 0, data.Length, value );
		}

		/// <summary>
		/// 将字节数组指定位置后置为指定值
		/// </summary>
		/// <param name="data">字节数组</param>
		/// <param name="offset">起始位置</param>
		/// <param name="value">设置值</param>
		public static void SetBytes( this byte[] data, int offset, byte value )
		{
			SetBytes( data, offset, data.Length - offset, value );
		}

		/// <summary>
		/// 将字节数组部分置为指定值
		/// </summary>
		/// <param name="data">字节数组</param>
		/// <param name="offset">起始位置</param>
		/// <param name="len">长度</param>
		/// <param name="value">设置值</param>
		public static void SetBytes( this byte[] data, int offset, int len, byte value )
		{
			for ( int i = offset; i < len + offset; i++ )
				data[i] = value;
		}

		/// <summary>
		/// 取字节数组子数组
		/// </summary>
		/// <param name="data">源字节数组</param>
		/// <param name="offset">起始位置</param>
		/// <param name="len">长度</param>
		/// <returns>返回指定源数组offset位置后长度len的一份拷贝，不足len长度时，右补0</returns>
		public static byte[] SubBytes( this byte[] data, int offset, int len )
		{
			byte[] bb = new byte[len];
			return SubBytes( data, offset, bb );
		}

		/// <summary>
		/// 取字节数组子数组
		/// </summary>
		/// <param name="data">源字节数组</param>
		/// <param name="offset">起始位置</param>
		/// <returns>返回offset到结束的子数组</returns>
		public static byte[] SubBytes( this byte[] data, int offset )
		{
			return SubBytes( data, offset, data.Length - offset );
		}

		/// <summary>
		/// 取字节数组子数组
		/// </summary>
		/// <param name="data">源字节数组</param>
		/// <param name="offset">起始位置</param>
		/// <param name="bb">目标,指定源数组offset位置后长度bb.length的一份拷贝，不足bb.length长度时，右补0</param>
		/// <returns>返回bb</returns>
		public static byte[] SubBytes( this byte[] data, int offset, byte[] bb )
		{
			int len = bb.Length;
			if ( data.Length - offset >= len )
			{
				Array.Copy( data, offset, bb, 0, len );
			}
			else
			{
				Array.Copy( data, offset, bb, 0, data.Length - offset );
				ZeroBytes( bb, data.Length - offset );
			}
			return bb;
		}

		/// <summary>
		/// 合并两个字节数组
		/// </summary>
		/// <param name="bs1">数组1</param>
		/// <param name="bs2">数组2</param>
		/// <returns>返回合并后的字节数组</returns>
		public static byte[] JoinBytes( byte[] bs1, byte[] bs2 )
		{
			return JoinBytes( bs1, 0, bs1.Length, bs2, 0, bs2.Length );
		}

		/// <summary>
		/// 合并两个字节数组
		/// </summary>
		/// <param name="bs1">数组1</param>
		/// <param name="offset1">第一个数组起始位置</param>
		/// <param name="len1">第一个数组长度</param>
		/// <param name="bs2">数组2</param>
		/// <param name="offset2">第二个数组起始位置</param>
		/// <param name="len2">第二个数组起始位置</param>
		/// <returns>返回合并后的字节数组</returns>
		public static byte[] JoinBytes( byte[] bs1, int offset1, int len1, byte[] bs2, int offset2, int len2 )
		{
			byte[] bs = new byte[len1 + len2];
			Array.Copy( bs1, offset1, bs, 0, len1 );
			Array.Copy( bs2, offset2, bs, len1, len2 );
			return bs;
		}

		/// <summary>
		/// 比较两字节数组是否相等
		/// </summary>
		/// <param name="b1">第一个比较字节数组</param>
		/// <param name="b2">第二个比较字节数组</param>
		/// <returns>相等返回true</returns>
		public static bool BytesEquals( this byte[] b1, byte[] b2 )
		{
			if ( b1 == b2 )
				return true;
			if ( b1 == null || b2 == null || b1.Length != b2.Length )
				return false;
			return BytesEquals( b1, 0, b2, 0, b1.Length );
		}

		/// <summary>
		/// 比较两字节数组是否相等
		/// </summary>
		/// <param name="b1">第一个比较字节数组</param>
		/// <param name="offset1">第一个比较字节数组起始位置</param>
		/// <param name="b2">第二个比较字节数组</param>
		/// <param name="offset2">第二个比较字节数组起始位置</param>
		/// <returns>相等返回true</returns>
		public static bool BytesEquals( byte[] b1, int offset1, byte[] b2, int offset2 )
		{
			if ( b1 != b2 && ( b1 == null || b2 == null ) )
				return false;
			if ( b1.Length - offset1 != b2.Length - offset2 )
				return false;
			return BytesEquals( b1, offset1, b2, offset2, b1.Length - offset1 );
		}

		/// <summary>
		/// 比较两字节数组是否相等
		/// </summary>
		/// <param name="b1">第一个比较字节数组</param>
		/// <param name="offset1">第一个比较字节数组起始位置</param>
		/// <param name="b2">第二个比较字节数组</param>
		/// <param name="offset2">第二个比较字节数组起始位置</param>
		/// <param name="len">比较长度</param>
		/// <returns>相等返回true</returns>
		public static bool BytesEquals( byte[] b1, int offset1, byte[] b2,
				int offset2, int len )
		{
			if ( b1 != b2 && ( b1 == null || b2 == null ) )
				return false;
			if ( b1.Length < offset1 + len || b2.Length < offset2 + len )
				return false;
			for ( int i = 0; i < len; i++ )
			{
				if ( b1[offset1 + i] != b2[offset2 + i] )
					return false;
			}
			return true;
		}

		/// <summary>
		/// 是否与本地主机字节相符
		/// </summary>
		/// <param name="littleEndian"></param>
		/// <returns></returns>
		public static bool IsSystemEndian( bool littleEndian )
		{
			return ( BitConverter.IsLittleEndian == littleEndian );
		}

		/// <summary>
		/// 字节数组转为字符
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static char BytesToChar( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[2];
				Array.Copy( bb, 0, _bb, 0, 2 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToChar( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为短整数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static short BytesToShort( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[2];
				Array.Copy( bb, 0, _bb, 0, 2 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToInt16( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为整数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static int BytesToInt( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[4];
				Array.Copy( bb, 0, _bb, 0, 4 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToInt32( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为长整数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static long BytesToLong( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[8];
				Array.Copy( bb, 0, _bb, 0, 8 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToInt64( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为无符号短整数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static ushort BytesToUShort( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[2];
				Array.Copy( bb, 0, _bb, 0, 2 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToUInt16( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为无符号整数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static uint BytesToUInt( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[4];
				Array.Copy( bb, 0, _bb, 0, 4 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToUInt32( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为无符号长整数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static ulong BytesToULong( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[8];
				Array.Copy( bb, 0, _bb, 0, 8 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToUInt64( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为单精度浮点数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static float BytesToFloat( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[4];
				Array.Copy( bb, 0, _bb, 0, 4 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToSingle( _bb, 0 );
		}

		/// <summary>
		/// 字节数组转为双精度浮点数
		/// </summary>
		/// <param name="bb">字节数组</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static double BytesToDouble( byte[] bb, bool littleEndian = true )
		{
			byte[] _bb = bb;
			if ( !IsSystemEndian( littleEndian ) )
			{
				_bb = new byte[8];
				Array.Copy( bb, 0, _bb, 0, 8 );
				Array.Reverse( _bb );
			}
			return BitConverter.ToDouble( _bb, 0 );
		}

		/// <summary>
		/// 字符转为字节数组
		/// </summary>
		/// <param name="value">字符</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] CharToBytes( char value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 短整数转为字节数组
		/// </summary>
		/// <param name="value">短整数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] ShortToBytes( short value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 整数转为字节数组
		/// </summary>
		/// <param name="value">整数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] IntToBytes( int value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 长整数转为字节数组
		/// </summary>
		/// <param name="value">长整数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] LongToBytes( long value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 无符号短整数转为字节数组
		/// </summary>
		/// <param name="value">无符号短整数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] UShortToBytes( ushort value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 无符号整数转为字节数组
		/// </summary>
		/// <param name="value">无符号整数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] UIntToBytes( uint value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 无符号长整数转为字节数组
		/// </summary>
		/// <param name="value">无符号长整数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] ULongToBytes( ulong value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 单精度浮点数转为字节数组
		/// </summary>
		/// <param name="value">单精度浮点数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] FloatToBytes( float value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

		/// <summary>
		/// 双精度浮点数转为字节数组
		/// </summary>
		/// <param name="value">双精度浮点数</param>
		/// <param name="littleEndian">是否LITTLE_ENDIAN字节顺序，默认为true</param>
		/// <returns></returns>
		public static byte[] DoubleToBytes( double value, bool littleEndian = true )
		{
			byte[] bb = BitConverter.GetBytes( value );
			if ( !IsSystemEndian( littleEndian ) )
				Array.Reverse( bb );
			return bb;
		}

	}
}
