using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Crypto
{
	/// <summary>
	/// AES的相关便捷方法
	/// </summary>
	public static class AesUtil
	{
		/// <summary>
		/// 通过密码得到一个通用64位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] GenAesKey64( byte[] passwd )
		{
			byte[] dg = MD5Util.GetMd5Digest( passwd );
			return dg.SubBytes( 0, 8 );
		}

		/// <summary>
		/// 通过密码得到一个通用128位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] GenAesKey128( byte[] passwd )
		{
			return MD5Util.GetMd5Digest( passwd );
		}

		/// <summary>
		/// 通过密码得到一个通用192位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] GenAesKey192( byte[] passwd )
		{
			return GenAesKey( passwd ).SubBytes( 0, 24 );
		}

		/// <summary>
		/// 通过密码得到一个通用256位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] GenAesKey( byte[] passwd )
		{
			MD5 md5 = new MD5();
			byte[] dg = md5.GetDigest( passwd );
			byte[] passwd2 = passwd.SubBytes( 0 );
			Array.Reverse( passwd2 );
			byte[] dg2 = md5.GetDigest( passwd2 );
			return Bytes.JoinBytes( dg, dg2 );
		}

		/// <summary>
		/// 生成随机密匙
		/// </summary>
		/// <returns></returns>
		public static byte[] GenAesKey()
		{
			RijndaelManaged aes = new RijndaelManaged();
			aes.GenerateKey();
			return aes.Key;
		}

		/// <summary>
		/// 生成随机向量
		/// </summary>
		/// <returns></returns>
		public static byte[] GenAesIv()
		{
			RijndaelManaged aes = new RijndaelManaged();
			aes.GenerateIV();
			return aes.IV;
		}

		public static void AesEncrypt( Stream i, Stream o, byte[] key )
		{
			Aes aes = new Aes( key );
			aes.Encrypt( i, o );
		}

		public static byte[] AesEncrypt( byte[] data, byte[] key )
		{
			Aes aes = new Aes( key );
			return aes.Encrypt( data );
		}

		public static void AesEncrypt( Stream i, Stream o, byte[] key, byte[] iv )
		{
			Aes aes = new Aes( key, iv );
			aes.Encrypt( i, o );
		}

		public static byte[] AesEncrypt( byte[] data )
		{
			Aes aes = new Aes( CryptoConfig.AES_KEY, CryptoConfig.AES_IV );
			return aes.Encrypt( data );
		}

		public static byte[] AesEncrypt( byte[] data, byte[] key, byte[] iv )
		{
			Aes aes = new Aes( key, iv );
			return aes.Encrypt( data );
		}

		/// <summary>
		/// 加密数据
		/// </summary>
		/// <param name="source">明文</param>
		/// <returns>返回密文</returns>
		public static string AesEncrypt( string source )
		{
			byte[] bytes = Encoding.UTF8.GetBytes( source );
			byte[] v = AesEncrypt( bytes );
			if ( v != null )
				return Convert.ToBase64String( v );
			return source;
		}

		public static void AesDecrypt( Stream i, Stream o, byte[] key )
		{
			Aes aes = new Aes( key );
			aes.Decrypt( i, o );
		}

		public static byte[] AesDecrypt( byte[] data, byte[] key )
		{
			Aes aes = new Aes( key );
			return aes.Decrypt( data );
		}

		public static void AesDecrypt( Stream i, Stream o, byte[] key, byte[] iv )
		{
			Aes aes = new Aes( key, iv );
			aes.Decrypt( i, o );
		}

		public static byte[] AesDecrypt( byte[] data )
		{
			Aes aes = new Aes( CryptoConfig.AES_KEY, CryptoConfig.AES_IV );
			return aes.Decrypt( data );
		}

		public static byte[] AesDecrypt( byte[] data, byte[] key, byte[] iv )
		{
			Aes aes = new Aes( key, iv );
			return aes.Decrypt( data );
		}

		/// <summary>
		/// 解密数据
		/// </summary>
		/// <param name="source">密文</param>
		/// <returns>返回明文</returns>
		public static string AesDecrypt( string source )
		{
			byte[] v = Convert.FromBase64String( source );
			v = AesDecrypt( v );
			if ( v != null )
				return Encoding.UTF8.GetString( v );
			return source;
		}

	}

	/// <summary>
	/// AES对象便捷封装
	/// </summary>
	public class Aes : IDisposable
	{
		private readonly RijndaelManaged _aes = new RijndaelManaged();

		public RijndaelManaged aes
		{
			get { return this._aes; }
		}

		public Aes()
		{
			this._aes.Mode = CipherMode.ECB;
			this._aes.Padding = PaddingMode.PKCS7;
			this._aes.GenerateKey();
		}

		public Aes( byte[] key )
		{
			this._aes.Mode = CipherMode.ECB;
			this._aes.Padding = PaddingMode.PKCS7;
			this._aes.Key = key;
		}

		public Aes( byte[] key, byte[] iv )
		{
			this._aes.Mode = CipherMode.CBC;
			this._aes.Padding = PaddingMode.PKCS7;
			this._aes.Key = key;
			this._aes.IV = iv;
		}

		public void SetKey( byte[] key )
		{
			this._aes.Key = key;
		}

		public byte[] GetKey()
		{
			return this._aes.Key;
		}

		public void SetIV( byte[] iv )
		{
			this._aes.IV = iv;
		}

		public byte[] GetIV()
		{
			return this._aes.IV;
		}

		public void SetCipherMode( CipherMode mode )
		{
			this._aes.Mode = mode;
		}

		public void SetPadingMode( PaddingMode mode )
		{
			this._aes.Padding = mode;
		}

		public void Encrypt( Stream i, Stream o )
		{
			CryptoStream cs = new CryptoStream( o, this._aes.CreateEncryptor(), CryptoStreamMode.Write );
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
			CryptoStream cs = new CryptoStream( i, this._aes.CreateDecryptor(), CryptoStreamMode.Read );
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
			this._aes.Clear();
		}
	}

	/*
	/// <summary>
	/// AES的相关便捷方法
	/// </summary>
	public static class AesUtil
	{
		/// <summary>
		/// 通过密码得到一个通用64位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] genAesKey64(byte[] passwd)
		{
			byte[] dg = MD5Util.getMD5Digest(passwd);
			return dg.SubBytes(0, 8);
		}

		/// <summary>
		/// 通过密码得到一个通用128位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] genAesKey128(byte[] passwd)
		{
			return MD5Util.getMD5Digest(passwd);
		}

		/// <summary>
		/// 通过密码得到一个通用192位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] genAesKey192(byte[] passwd)
		{
			return genAesKey(passwd).SubBytes(0, 24);
		}

		/// <summary>
		/// 通过密码得到一个通用256位Aes密钥
		/// </summary>
		/// <param name="passwd">密码</param>
		/// <returns>返回密钥</returns>
		public static byte[] genAesKey(byte[] passwd)
		{
			MD5 md5 = new MD5();
			byte[] dg = md5.getDigest(passwd);
			byte[] passwd2 = passwd.SubBytes(0);
			passwd2.Reverse();
			byte[] dg2 = md5.getDigest(passwd2);
			return Bytes.JoinBytes(dg, dg2);
		}

		/// <summary>
		/// 生成随机密匙
		/// </summary>
		/// <returns></returns>
		public static byte[] genAesKey()
		{
			AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
			aes.GenerateKey();
			return aes.Key;
		}

		/// <summary>
		/// 生成随机向量
		/// </summary>
		/// <returns></returns>
		public static byte[] genAesIV()
		{
			AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
			aes.GenerateIV();
			return aes.IV;
		}

		public static void AesEncrypt(Stream i, Stream o, byte[] key)
		{
			Aes aes = new Aes(key);
			aes.Encrypt(i, o);
		}

		public static byte[] AesEncrypt(byte[] data, byte[] key)
		{
			Aes aes = new Aes(key);
			return aes.Encrypt(data);
		}

		public static void AesEncrypt(Stream i, Stream o, byte[] key, byte[] iv)
		{
			Aes aes = new Aes(key, iv);
			aes.Encrypt(i, o);
		}

		public static byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
		{
			Aes aes = new Aes(key, iv);
			return aes.Encrypt(data);
		}

		public static void AesDecrypt(Stream i, Stream o, byte[] key)
		{
			Aes aes = new Aes(key);
			aes.Decrypt(i, o);
		}

		public static byte[] AesDecrypt(byte[] data, byte[] key)
		{
			Aes aes = new Aes(key);
			return aes.Decrypt(data);
		}

		public static void AesDecrypt(Stream i, Stream o, byte[] key, byte[] iv)
		{
			Aes aes = new Aes(key, iv);
			aes.Decrypt(i, o);
		}

		public static byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
		{
			Aes aes = new Aes(key, iv);
			return aes.Decrypt(data);
		}
	}

	/// <summary>
	/// AES对象便捷封装
	/// </summary>
	public class Aes : IDisposable
	{
		private System.Security.Cryptography.Aes _aes = new AesCryptoServiceProvider();
		
		public System.Security.Cryptography.Aes aes
		{
			get { return _aes; }
		}

		public Aes()
		{
			_aes.Mode = CipherMode.ECB;
			_aes.Padding = PaddingMode.PKCS7;
			_aes.GenerateKey();
		}

		public Aes(byte[] key)
		{
			_aes.Mode = CipherMode.ECB;
			_aes.Padding = PaddingMode.PKCS7;
			_aes.Key = key;
		}

		public Aes(byte[] key, byte[] iv)
		{
			_aes.Mode = CipherMode.CBC;
			_aes.Padding = PaddingMode.PKCS7;
			_aes.Key = key;
			_aes.IV = iv;
		}

		public void SetKey(byte[] key)
		{
			_aes.Key = key;
		}

		public byte[] GetKey()
		{
			return _aes.Key;
		}

		public void SetIV(byte[] iv)
		{
			_aes.IV = iv;
		}

		public byte[] GetIV()
		{
			return _aes.IV;
		}

		public void SetCipherMode(CipherMode mode)
		{
			_aes.Mode = mode;
		}

		public void SetPadingMode(PaddingMode mode)
		{
			_aes.Padding = mode;
		}

		public void Encrypt(Stream i, Stream o)
		{
			CryptoStream cs = new CryptoStream(o, _aes.CreateEncryptor(), CryptoStreamMode.Write);
			Stdio.CopyStream(i, cs);
			//cs.Flush();
			cs.Close();
			o.Flush();
		}

		public void Encrypt(byte[] data, byte[] output)
		{
			MemoryStream i = new MemoryStream(data);
			MemoryStream o = new MemoryStream(output);
			Encrypt(i, o);
			i.Close();
			o.Close();
		}

		public byte[] Encrypt(byte[] data)
		{
			MemoryStream i = new MemoryStream(data);
			MemoryStream o = new MemoryStream();
			Encrypt(i, o);
			i.Close();
			o.Close();
			return o.ToArray();
		}

		public void Encrypt(FileInfo src, FileInfo dst)
		{
			FileStream fi = new FileStream(src.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				FileStream fo = new FileStream(dst.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
				try
				{
					Encrypt(fi, fo);
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

		public void Decrypt(Stream i, Stream o)
		{
			CryptoStream cs = new CryptoStream(i, _aes.CreateDecryptor(), CryptoStreamMode.Read);
			Stdio.CopyStream(cs, o);
			//cs.Flush();
			cs.Close();
			o.Flush();
		}

		public void Decrypt(byte[] data, byte[] output)
		{
			MemoryStream i = new MemoryStream(data);
			MemoryStream o = new MemoryStream(output);
			Decrypt(i, o);
			i.Close();
			o.Close();
		}

		public byte[] Decrypt(byte[] data)
		{
			MemoryStream i = new MemoryStream(data);
			MemoryStream o = new MemoryStream();
			Decrypt(i, o);
			i.Close();
			o.Close();
			return o.ToArray();
		}

		public void Decrypt(FileInfo src, FileInfo dst)
		{
			FileStream fi = new FileStream(src.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				FileStream fo = new FileStream(dst.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
				try
				{
					Decrypt(fi, fo);
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
			_aes.Clear();
		}
	}
	*/
}
