using System.IO;
using System.Text;

namespace Core.Crypto
{
	/// <summary>
	/// 单行文本读取回调
	/// </summary>
	/// <param name="line">一行文本</param>
	/// <returns>返回true继续读取下一行，返回false则终止读取操作</returns>
	public delegate bool OnReadLine( string line );

	/// <summary>
	/// 文件和流相关IO操作的便捷方法
	/// </summary>
	public static class Stdio
	{

		/// <summary>
		/// 读取输入流中的数据,直到缓冲区满或输入结束
		/// </summary>
		/// <param name="s">输入流</param>
		/// <param name="buf">接收缓冲区</param>
		/// <returns>返回实际读取的大小,返回小于缓冲区大小时表示已到结束</returns>
		public static int ReadStream( Stream s, byte[] buf )
		{
			return ReadStream( s, buf, 0, buf.Length );
		}

		/// <summary>
		/// 读取输入流中的数据,直到缓冲区满或输入结束
		/// </summary>
		/// <param name="s">输入流</param>
		/// <param name="buf">接收缓冲区</param>
		/// <param name="offset">缓冲区起始位置</param>
		/// <param name="len">希望读取的数据大小</param>
		/// <returns>返回实际读取的大小,返回小于希望读取的大小时表示已到结束</returns>
		public static int ReadStream( Stream s, byte[] buf, int offset, int len )
		{
			int total = 0;
			do
			{
				int tmp = s.Read( buf, offset + total, len - total );
				if ( tmp > 0 )
				{
					total += tmp;
					if ( total >= len )
						break;
				}
				else
					break;
			} while ( true );
			return total;
		}

		/// <summary>
		/// 读取输入流中指定长度的数据(不建议读取大数据)
		/// </summary>
		/// <param name="s">输入流</param>
		/// <param name="len">希望读取的数据大小</param>
		/// <returns>返回实际读取的数据,返回数据小于希望读取的大小时表示已到结束</returns>
		public static byte[] ReadStream( Stream s, int len )
		{
			byte[] buf = new byte[len];
			len = ReadStream( s, buf );
			if ( len < buf.Length )
				return buf.SubBytes( 0, len );
			return buf;
		}

		/// <summary>
		/// 读取输入流中的所有数据(不建议读取大数据)
		/// </summary>
		/// <param name="s">输入流</param>
		/// <returns>返回读取的数据</returns>
		public static byte[] ReadStream( Stream s )
		{
			MemoryStream os = new MemoryStream();
			CopyStream( s, os );
			os.Close();
			return os.ToArray();
		}

		/// <summary>
		/// 将输入流的数据拷贝到输出流
		/// </summary>
		/// <param name="s">输入流</param>
		/// <param name="os">输出流</param>
		public static void CopyStream( Stream s, Stream os )
		{
			byte[] data = new byte[4096];
			do
			{
				int len = ReadStream( s, data );
				if ( len > 0 )
					os.Write( data, 0, len );
				if ( len < data.Length )
					break;
			}
			while ( true );
			os.Flush();
		}

		/// <summary>
		/// 逐行读取输入流
		/// </summary>
		/// <param name="s">输入流</param>
		/// <param name="cb">每读取一行的回调处理对象</param>
		/// <param name="encoding">字符编码</param>
		/// <param name="bz">读取缓冲区大小</param>
		/// <returns>返回读取的行数</returns>
		public static int ReadStreamByLine( Stream s, OnReadLine cb, Encoding encoding = null, int bz = 0 )
		{
			StreamReader sr;
			if ( encoding != null )
			{
				sr = bz > 0 ? new StreamReader( s, encoding, true, bz ) : new StreamReader( s, encoding );
			}
			else
				sr = new StreamReader( s );

			int lc = 0;
			do
			{
				string line = sr.ReadLine();
				if ( line == null )
					break;
				lc++;
				if ( !cb( line ) )
					break;
			}
			while ( !sr.EndOfStream );
			return lc;
		}

		/// <summary>
		/// 写数据到文件
		/// </summary>
		/// <param name="fileName">文件名</param>
		/// <param name="data">数据</param>
		/// <param name="append">是否追加到文件末尾</param>
		public static void WriteFile( string fileName, byte[] data, bool append = false )
		{
			FileStream fs = File.Open( fileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read );
			try
			{
				fs.Write( data, 0, data.Length );
			}
			finally
			{
				fs.Close();
			}
		}

		/// <summary>
		/// 读取文件数据
		/// </summary>
		/// <param name="fileName">文件名</param>
		/// <returns>返回数据</returns>
		public static byte[] ReadFile( string fileName )
		{
			FileStream fs = File.Open( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
			try
			{
				return ReadStream( fs );
			}
			finally
			{
				fs.Close();
			}
		}

		/// <summary>
		/// 逐行读取文件
		/// </summary>
		/// <param name="fileName">文件名</param>
		/// <param name="cb">每读取一行的回调处理对象</param>
		/// <param name="encoding">字符编码</param>
		/// <param name="bz">读取缓冲区大小</param>
		/// <returns>返回读取的行数</returns>
		public static int ReadFileByLine( string fileName, OnReadLine cb, Encoding encoding = null, int bz = 0 )
		{
			int lc;
			FileStream fs = File.Open( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
			try
			{
				lc = ReadStreamByLine( fs, cb, encoding, bz );
			}
			finally
			{
				fs.Close();
			}
			return lc;
		}

		/// <summary>
		/// 将字符串写入文本文件
		/// </summary>
		/// <param name="fileName">文件名</param>
		/// <param name="content">字符串内容</param>
		/// <param name="append">是否追加到文件末尾</param>
		/// <param name="encoding">字符编码</param>
		/// <param name="bz">缓冲区大小</param>
		public static void WriteTextFile( string fileName, string content, bool append = false, Encoding encoding = null, int bz = 0 )
		{
			FileStream fs = File.Open( fileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read );
			StreamWriter sw;
			if ( encoding != null )
			{
				sw = bz > 0 ? new StreamWriter( fs, encoding, bz ) : new StreamWriter( fs, encoding );
			}
			else
				sw = new StreamWriter( fs );
			try
			{
				sw.Write( content );
			}
			finally
			{
				sw.Close();
				fs.Close();
			}
		}

		/// <summary>
		/// 读取文本文件内容
		/// </summary>
		/// <param name="fileName">文件名</param>
		/// <param name="encoding">字符编码</param>
		/// <param name="bz">缓冲区大小</param>
		/// <returns>返回读进的字符串内容</returns>
		public static string ReadTextFile( string fileName, Encoding encoding = null, int bz = 0 )
		{
			FileStream fs = File.Open( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
			StreamReader sr;
			if ( encoding != null )
			{
				sr = bz > 0 ? new StreamReader( fs, encoding, true, bz ) : new StreamReader( fs, encoding );
			}
			else
				sr = new StreamReader( fs );

			StringBuilder buf = new StringBuilder();
			char[] cs = new char[1024];
			int len;
			try
			{
				while ( ( len = sr.Read( cs, 0, cs.Length ) ) > 0 )
					buf.Append( cs, 0, len );
			}
			finally
			{
				sr.Close();
				fs.Close();
			}
			return buf.ToString();
		}

	}
}
