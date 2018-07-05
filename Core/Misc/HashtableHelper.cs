using System;
using System.Collections;

namespace Core.Misc
{
	public static class HashtableHelper
	{
		public static void Concat( this Hashtable h1, Hashtable h2 )
		{
			if ( h2 == null )
				return;
			foreach ( DictionaryEntry de in h2 )
			{
				if ( h1.ContainsKey( de.Key ) )
					continue;
				h1[de.Key] = de.Value;
			}
		}

		public static int GetInt( this Hashtable ht, string key )
		{
			return Convert.ToInt32( ht[key] );
		}

		public static long GetLong( this Hashtable ht, string key )
		{
			return Convert.ToInt64( ht[key] );
		}

		public static Hashtable GetMap( this Hashtable ht, string key )
		{
			return ht[key] as Hashtable;
		}

		public static ArrayList GetList( this Hashtable ht, string key )
		{
			return ht[key] as ArrayList;
		}

		public static string GetString( this Hashtable ht, string key )
		{
			return Convert.ToString( ht[key] );
		}

		public static byte GetByte( this Hashtable ht, string key )
		{
			return Convert.ToByte( ht[key] );
		}

		public static double GetDouble( this Hashtable ht, string key )
		{
			return Convert.ToDouble( ht[key] );
		}

		public static ushort GetUShort( this Hashtable ht, string key )
		{
			return Convert.ToUInt16( ht[key] );
		}

		public static short GetShort( this Hashtable ht, string key )
		{
			return Convert.ToInt16( ht[key] );
		}

		public static uint GetUInt( this Hashtable ht, string key )
		{
			return Convert.ToUInt32( ht[key] );
		}

		public static ulong GetULong( this Hashtable ht, string key )
		{
			return Convert.ToUInt64( ht[key] );
		}

		public static bool GetBoolean( this Hashtable ht, string key )
		{
			return Convert.ToBoolean( ht[key] );
		}

		public static float GetFloat( this Hashtable ht, string key )
		{
			return Convert.ToSingle( ht[key] );
		}

		public static char GetChar( this Hashtable ht, string key )
		{
			return Convert.ToChar( ht[key] );
		}

		public static string[] GetStringArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			string[] f = new string[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToString( v[i] );
			return f;
		}

		public static bool[] GetBooleanArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			bool[] f = new bool[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToBoolean( v[i] );
			return f;
		}

		public static byte[] GetByteArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			byte[] f = new byte[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToByte( v[i] );
			return f;
		}

		public static short[] GetShortArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			short[] f = new short[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToInt16( v[i] );
			return f;
		}

		public static int[] GetIntArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			int[] f = new int[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToInt32( v[i] );
			return f;
		}

		public static float[] GetFloatArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			float[] f = new float[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToSingle( v[i] );
			return f;
		}

		public static double[] GetDoubleArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			double[] f = new double[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToDouble( v[i] );
			return f;
		}

		public static long[] GetLongArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			long[] f = new long[c];
			for ( int i = 0; i < c; i++ )
				f[i] = Convert.ToInt64( v[i] );
			return f;
		}

		public static Hashtable[] GetMapArray( this Hashtable ht, string key )
		{
			if ( !ht.ContainsKey( key ) )
				return null;
			ArrayList v = ( ArrayList )ht[key];
			int c = v.Count;
			Hashtable[] f = new Hashtable[c];
			for ( int i = 0; i < c; i++ )
				f[i] = ( Hashtable )v[i];
			return f;
		}
	}
}