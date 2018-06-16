using System;
using System.IO;
using System.Security.Cryptography;

namespace Core.Crypto
{
	/// <summary>
	/// DES/TripleDES的相关便捷方法
	/// </summary>
	public static class DesUtil
	{
		/// <summary>
		/// 通过密码得到一个通用Des密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] GenDesKey( byte[] passwd )
		{
			byte[] dg = MD5Util.GetMd5Digest( passwd );
			return dg.SubBytes( 0, 8 );
		}

		/// <summary>
		/// 通过密码得到一个通用TripleDes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] GenTripleDesKey( byte[] passwd )
		{
			byte[] dg = MD5Util.GetMd5Digest( passwd );
			byte[] key = new byte[24];
			Array.Copy( dg, 0, key, 0, 16 );
			Array.Copy( dg, 0, key, 16, 8 );
			return key;
		}

		/// <summary>
		/// 生成随机密匙
		/// </summary>
		/// <returns></returns>
		public static byte[] GenDesKey()
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			des.GenerateKey();
			return des.Key;
		}

		/// <summary>
		/// 生成随机向量
		/// </summary>
		/// <returns></returns>
		public static byte[] GenDesIV()
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			des.GenerateIV();
			return des.IV;
		}

		/// <summary>
		/// 生成3重des密匙
		/// </summary>
		/// <returns></returns>
		public static byte[] GenTripleDesKey()
		{
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			des.GenerateKey();
			return des.Key;
		}

		public static byte[] GenTripleDesIV()
		{
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			des.GenerateIV();
			return des.IV;
		}

		public static void DesEncrypt( Stream i, Stream o, byte[] key )
		{
			Des des = new Des( key );
			des.Encrypt( i, o );
		}

		public static byte[] DesEncrypt( byte[] data, byte[] key )
		{
			Des des = new Des( key );
			return des.Encrypt( data );
		}

		public static void DesEncrypt( Stream i, Stream o, byte[] key, byte[] iv )
		{
			Des des = new Des( key, iv );
			des.Encrypt( i, o );
		}

		public static byte[] DesEncrypt( byte[] data, byte[] key, byte[] iv )
		{
			Des des = new Des( key, iv );
			return des.Encrypt( data );
		}

		public static void DesDecrypt( Stream i, Stream o, byte[] key )
		{
			Des des = new Des( key );
			des.Decrypt( i, o );
		}

		public static byte[] DesDecrypt( byte[] data, byte[] key )
		{
			Des des = new Des( key );
			return des.Decrypt( data );
		}

		public static void DesDecrypt( Stream i, Stream o, byte[] key, byte[] iv )
		{
			Des des = new Des( key, iv );
			des.Decrypt( i, o );
		}

		public static byte[] DesDecrypt( byte[] data, byte[] key, byte[] iv )
		{
			Des des = new Des( key, iv );
			return des.Decrypt( data );
		}

		public static void TripleDesEncrypt( Stream i, Stream o, byte[] key )
		{
			TripleDes des = new TripleDes( key );
			des.Encrypt( i, o );
		}

		public static byte[] TripleDesEncrypt( byte[] data, byte[] key )
		{
			TripleDes des = new TripleDes( key );
			return des.Encrypt( data );
		}

		public static void TripleDesEncrypt( Stream i, Stream o, byte[] key, byte[] iv )
		{
			TripleDes des = new TripleDes( key, iv );
			des.Encrypt( i, o );
		}

		public static byte[] TripleDesEncrypt( byte[] data, byte[] key, byte[] iv )
		{
			TripleDes des = new TripleDes( key, iv );
			return des.Encrypt( data );
		}

		public static void TripleDesDecrypt( Stream i, Stream o, byte[] key )
		{
			TripleDes des = new TripleDes( key );
			des.Decrypt( i, o );
		}

		public static byte[] TripleDesDecrypt( byte[] data, byte[] key )
		{
			TripleDes des = new TripleDes( key );
			return des.Decrypt( data );
		}

		public static void TripleDesDecrypt( Stream i, Stream o, byte[] key, byte[] iv )
		{
			TripleDes des = new TripleDes( key, iv );
			des.Decrypt( i, o );
		}

		public static byte[] TripleDesDecrypt( byte[] data, byte[] key, byte[] iv )
		{
			TripleDes des = new TripleDes( key, iv );
			return des.Decrypt( data );
		}

	}

	/// <summary>
	/// DES对象便捷封装
	/// </summary>
	public class Des : IDisposable
	{
		private readonly DES _des = new DESCryptoServiceProvider();

		public DES des
		{
			get { return this._des; }
		}

		public Des()
		{
			this._des.Mode = CipherMode.ECB;
			this._des.Padding = PaddingMode.PKCS7;
			this._des.GenerateKey();
		}

		public Des( byte[] key )
		{
			this._des.Mode = CipherMode.ECB;
			this._des.Padding = PaddingMode.PKCS7;
			this._des.Key = key;
		}

		public Des( byte[] key, byte[] iv )
		{
			this._des.Mode = CipherMode.CBC;
			this._des.Padding = PaddingMode.PKCS7;
			this._des.Key = key;
			this._des.IV = iv;
		}

		public void SetKey( byte[] key )
		{
			this._des.Key = key;
		}

		public byte[] GetKey()
		{
			return this._des.Key;
		}

		public void SetIV( byte[] iv )
		{
			this._des.IV = iv;
		}

		public byte[] GetIV()
		{
			return this._des.IV;
		}

		public void SetCipherMode( CipherMode mode )
		{
			this._des.Mode = mode;
		}

		public void SetPadingMode( PaddingMode mode )
		{
			this._des.Padding = mode;
		}

		public void Encrypt( Stream i, Stream o )
		{
			CryptoStream cs = new CryptoStream( o, this._des.CreateEncryptor(), CryptoStreamMode.Write );
			Stdio.CopyStream( i, cs );
			//cs.Flush();
			cs.Close();
			o.Flush();
		}

		public void Encrypt( byte[] data, byte[] output )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream( output );
			this.Encrypt( i, o );
			i.Close();
			o.Close();
		}

		public byte[] Encrypt( byte[] data )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream();
			this.Encrypt( i, o );
			i.Close();
			o.Close();
			return o.ToArray();
		}

		public void Encrypt( FileInfo src, FileInfo dst )
		{
			FileStream fi = new FileStream( src.FullName, FileMode.Open, FileAccess.Read, FileShare.Read );
			try
			{
				FileStream fo = new FileStream( dst.FullName, FileMode.Create, FileAccess.Write, FileShare.None );
				try
				{
					this.Encrypt( fi, fo );
				}
				finally
				{
					fo.Close();
				}
			}
			finally
			{
				fi.Close();
			}
		}

		public void Decrypt( Stream i, Stream o )
		{
			CryptoStream cs = new CryptoStream( i, this._des.CreateDecryptor(), CryptoStreamMode.Read );
			Stdio.CopyStream( cs, o );
			//cs.Flush();
			cs.Close();
			o.Flush();
		}

		public void Decrypt( byte[] data, byte[] output )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream( output );
			this.Decrypt( i, o );
			i.Close();
			o.Close();
		}

		public byte[] Decrypt( byte[] data )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream();
			this.Decrypt( i, o );
			i.Close();
			o.Close();
			return o.ToArray();
		}

		public void Decrypt( FileInfo src, FileInfo dst )
		{
			FileStream fi = new FileStream( src.FullName, FileMode.Open, FileAccess.Read, FileShare.Read );
			try
			{
				FileStream fo = new FileStream( dst.FullName, FileMode.Create, FileAccess.Write, FileShare.None );
				try
				{
					this.Decrypt( fi, fo );
				}
				finally
				{
					fo.Close();
				}
			}
			finally
			{
				fi.Close();
			}
		}

		public void Dispose()
		{
			this._des.Clear();
		}
	}

	/// <summary>
	/// TripleDES对象便捷封装
	/// </summary>
	public class TripleDes : IDisposable
	{
		private TripleDES _des = new TripleDESCryptoServiceProvider();

		public TripleDES des
		{
			get { return this._des; }
		}

		public TripleDes()
		{
			this._des.Mode = CipherMode.ECB;
			this._des.Padding = PaddingMode.PKCS7;
			this._des.GenerateKey();
		}

		public TripleDes( byte[] key )
		{
			this._des.Mode = CipherMode.ECB;
			this._des.Padding = PaddingMode.PKCS7;
			this._des.Key = key;
		}

		public TripleDes( byte[] key, byte[] iv )
		{
			this._des.Mode = CipherMode.CBC;
			this._des.Padding = PaddingMode.PKCS7;
			this._des.Key = key;
			this._des.IV = iv;
		}

		public void SetKey( byte[] key )
		{
			this._des.Key = key;
		}

		public byte[] GetKey()
		{
			return this._des.Key;
		}

		public void SetIV( byte[] iv )
		{
			this._des.IV = iv;
		}

		public byte[] GetIV()
		{
			return this._des.IV;
		}

		public void SetCipherMode( CipherMode mode )
		{
			this._des.Mode = mode;
		}

		public void SetPadingMode( PaddingMode mode )
		{
			this._des.Padding = mode;
		}

		public void Encrypt( Stream i, Stream o )
		{
			CryptoStream cs = new CryptoStream( o, this._des.CreateEncryptor(), CryptoStreamMode.Write );
			Stdio.CopyStream( i, cs );
			//cs.Flush();
			cs.Close();
			o.Flush();
		}

		public void Encrypt( byte[] data, byte[] output )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream( output );
			this.Encrypt( i, o );
			i.Close();
			o.Close();
		}

		public byte[] Encrypt( byte[] data )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream();
			this.Encrypt( i, o );
			i.Close();
			o.Close();
			return o.ToArray();
		}

		public void Encrypt( FileInfo src, FileInfo dst )
		{
			FileStream fi = new FileStream( src.FullName, FileMode.Open, FileAccess.Read, FileShare.Read );
			try
			{
				FileStream fo = new FileStream( dst.FullName, FileMode.Create, FileAccess.Write, FileShare.None );
				try
				{
					this.Encrypt( fi, fo );
				}
				finally
				{
					fo.Close();
				}
			}
			finally
			{
				fi.Close();
			}
		}

		public void Decrypt( Stream i, Stream o )
		{
			CryptoStream cs = new CryptoStream( i, this._des.CreateDecryptor(), CryptoStreamMode.Read );
			Stdio.CopyStream( cs, o );
			//cs.Flush();
			cs.Close();
			o.Flush();
		}

		public void Decrypt( byte[] data, byte[] output )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream( output );
			this.Decrypt( i, o );
			i.Close();
			o.Close();
		}

		public byte[] Decrypt( byte[] data )
		{
			MemoryStream i = new MemoryStream( data );
			MemoryStream o = new MemoryStream();
			this.Decrypt( i, o );
			i.Close();
			o.Close();
			return o.ToArray();
		}

		public void Decrypt( FileInfo src, FileInfo dst )
		{
			FileStream fi = new FileStream( src.FullName, FileMode.Open, FileAccess.Read, FileShare.Read );
			try
			{
				FileStream fo = new FileStream( dst.FullName, FileMode.Create, FileAccess.Write, FileShare.None );
				try
				{
					this.Decrypt( fi, fo );
				}
				finally
				{
					fo.Close();
				}
			}
			finally
			{
				fi.Close();
			}
		}

		public void Dispose()
		{
			this._des.Clear();
		}
	}
}
