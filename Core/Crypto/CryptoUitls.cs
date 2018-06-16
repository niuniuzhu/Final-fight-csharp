using System;
using System.Text;

namespace Core.Crypto
{
	public static class CryptoUitls
	{
		public static string EncodeURI( string url )
		{
			return Uri.EscapeUriString( url );
		}

		public static string Base64Encode( byte[] bytes )
		{
			return Convert.ToBase64String( bytes );
		}

		public static byte[] Base64Decode( string str )
		{
			return Convert.FromBase64String( str );
		}

		public static string Base64EncodeToString( string str )
		{
			return Base64Encode( Encoding.UTF8.GetBytes( str ) );
		}

		public static string Base64DecodeFromString( string str )
		{
			return Encoding.UTF8.GetString( Base64Decode( str ) );
		}
	}
}