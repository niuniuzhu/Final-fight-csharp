using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.Misc
{
	public class MiniJSON
	{
		private const string INDENT = "\t";
		private const int TOKEN_NONE = 0;
		private const int TOKEN_CURLY_OPEN = 1;
		private const int TOKEN_CURLY_CLOSE = 2;
		private const int TOKEN_SQUARED_OPEN = 3;
		private const int TOKEN_SQUARED_CLOSE = 4;
		private const int TOKEN_COLON = 5;
		private const int TOKEN_COMMA = 6;
		private const int TOKEN_STRING = 7;
		private const int TOKEN_NUMBER = 8;
		private const int TOKEN_TRUE = 9;
		private const int TOKEN_FALSE = 10;
		private const int TOKEN_NULL = 11;
		private const int BUILDER_CAPACITY = 2000;

		/// <summary>
		///     On decoding, this value holds the position at which the parse failed (-1 = no error).
		/// </summary>
		private static int _lastErrorIndex = -1;

		private static string _lastDecode = string.Empty;


		/// <summary>
		///     Parses the string json into a value
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <returns>An ArrayList, a Map, a float, a string, null, true, or false</returns>
		public static object JsonDecode( string json )
		{
			//Ron edit
			//var r = new Regex( @"/\*[\s\S]*?\*/" );
			//json = r.Replace( json, "" );

			//r = new Regex( @"/{2}.*" );
			//json = r.Replace( json, "" );

			// save the string for debug information
			_lastDecode = json;
			char[] charArray = json.ToCharArray();
			int index = 0;
			bool success = true;
			object value = ParseValue( charArray, ref index, ref success );

			if ( success )
				_lastErrorIndex = -1;
			else
				_lastErrorIndex = index;

			return value;
		}


		/// <summary>
		///     Converts a Map / ArrayList / Dictionary(string,string) object into a JSON string
		/// </summary>
		/// <param name="json">A Map / ArrayList</param>
		/// <param name="indent"></param>
		/// <param name="sortKey"></param>
		/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
		public static string JsonEncode( object json, bool indent = false, bool sortKey = false )
		{
			var builder = new StringBuilder( BUILDER_CAPACITY );
			bool success = SerializeValue( json, builder, 0, indent, sortKey );

			return ( success ? builder.ToString() : null );
		}

		/// <summary>
		///     On decoding, this function returns the position at which the parse failed (-1 = no error).
		/// </summary>
		/// <returns></returns>
		public static bool LastDecodeSuccessful()
		{
			return ( _lastErrorIndex == -1 );
		}


		/// <summary>
		///     On decoding, this function returns the position at which the parse failed (-1 = no error).
		/// </summary>
		/// <returns></returns>
		public static int GetLastErrorIndex()
		{
			return _lastErrorIndex;
		}


		/// <summary>
		///     If a decoding error occurred, this function returns a piece of the JSON string
		///     at which the error took place. To ease debugging.
		/// </summary>
		/// <returns></returns>
		public static string GetLastErrorSnippet()
		{
			if ( _lastErrorIndex == -1 )
				return "";
			int startIndex = _lastErrorIndex - 5;
			int endIndex = _lastErrorIndex + 15;
			if ( startIndex < 0 )
				startIndex = 0;

			if ( endIndex >= _lastDecode.Length )
				endIndex = _lastDecode.Length - 1;

			return _lastDecode.Substring( startIndex, endIndex - startIndex + 1 );
		}

		#region Parsing

		private static Hashtable ParseObject( char[] json, ref int index )
		{
			Hashtable table = new Hashtable();

			NextToken( json, ref index );

			while ( true )
			{
				int token = LookAhead( json, index );
				if ( token == TOKEN_NONE )
					return null;
				if ( token == TOKEN_COMMA )
					NextToken( json, ref index );
				else if ( token == TOKEN_CURLY_CLOSE )
				{
					NextToken( json, ref index );
					return table;
				}
				else
				{
					// name
					string name = ParseString( json, ref index );
					if ( name == null )
					{
						return null;
					}

					// :
					token = NextToken( json, ref index );
					if ( token != TOKEN_COLON )
						return null;

					// value
					bool success = true;
					object value = ParseValue( json, ref index, ref success );
					if ( !success )
						return null;

					table[name] = value;
				}
			}
		}


		private static ArrayList ParseArray( char[] json, ref int index )
		{
			var array = new ArrayList();

			// [
			NextToken( json, ref index );

			while ( true )
			{
				int token = LookAhead( json, index );
				if ( token == TOKEN_NONE )
					return null;
				if ( token == TOKEN_COMMA )
					NextToken( json, ref index );
				else if ( token == TOKEN_SQUARED_CLOSE )
				{
					NextToken( json, ref index );
					break;
				}
				else
				{
					bool success = true;
					object value = ParseValue( json, ref index, ref success );
					if ( !success )
						return null;

					array.Add( value );
				}
			}

			return array;
		}


		private static object ParseValue( char[] json, ref int index, ref bool success )
		{
			switch ( LookAhead( json, index ) )
			{
				case TOKEN_STRING:
					return ParseString( json, ref index );
				case TOKEN_NUMBER:
					return ParseNumber( json, ref index );
				case TOKEN_CURLY_OPEN:
					return ParseObject( json, ref index );
				case TOKEN_SQUARED_OPEN:
					return ParseArray( json, ref index );
				case TOKEN_TRUE:
					NextToken( json, ref index );
					return Boolean.Parse( "TRUE" );
				case TOKEN_FALSE:
					NextToken( json, ref index );
					return Boolean.Parse( "FALSE" );
				case TOKEN_NULL:
					NextToken( json, ref index );
					return null;
				case TOKEN_NONE:
					break;
			}

			success = false;
			return null;
		}


		private static string ParseString( char[] json, ref int index )
		{
			string s = "";

			EatWhitespace( json, ref index );

			// ReSharper disable RedundantAssignment
			char c = json[index++];
			// ReSharper restore RedundantAssignment
			bool complete = false;
			while ( true )
			{
				if ( index == json.Length )
					break;

				c = json[index++];
				if ( c == '"' )
				{
					complete = true;
					break;
				}
				if ( c == '\\' )
				{
					if ( index == json.Length )
						break;

					c = json[index++];
					if ( c == '"' )
					{
						s += '"';
					}
					else if ( c == '\\' )
					{
						s += '\\';
					}
					else if ( c == '/' )
					{
						s += '/';
					}
					else if ( c == 'b' )
					{
						s += '\b';
					}
					else if ( c == 'f' )
					{
						s += '\f';
					}
					else if ( c == 'n' )
					{
						s += '\n';
					}
					else if ( c == 'r' )
					{
						s += '\r';
					}
					else if ( c == 't' )
					{
						s += '\t';
					}
					else if ( c == 'u' )
					{
						int remainingLength = json.Length - index;
						if ( remainingLength >= 4 )
						{
							var unicodeCharArray = new char[4];
							Array.Copy( json, index, unicodeCharArray, 0, 4 );

							// Drop in the HTML markup for the unicode character
							s += "&#x" + new string( unicodeCharArray ) + ";";

							/*
	uint codePoint = UInt32.Parse(new string(unicodeCharArray), NumberStyles.HexNumber);
	// convert the integer codepoint to a unicode char and add to string
	s += Char.ConvertFromUtf32((int)codePoint);
	*/

							// skip 4 chars
							index += 4;
						}
						else
						{
							break;
						}
					}
				}
				else
				{
					s += c.ToString( CultureInfo.InvariantCulture );
				}
			}

			if ( !complete )
				return null;

			return s;
		}


		private static object ParseNumber( char[] json, ref int index )
		{
			EatWhitespace( json, ref index );

			int lastIndex = GetLastIndexOfNumber( json, index );
			int charLength = ( lastIndex - index ) + 1;
			var numberCharArray = new char[charLength];

			Array.Copy( json, index, numberCharArray, 0, charLength );
			index = lastIndex + 1;
			string s = new string( numberCharArray );
			if ( s.Contains( "." ) )
				return float.Parse( s, CultureInfo.InvariantCulture );
			return int.Parse( s, CultureInfo.InvariantCulture );
		}

		private static int GetLastIndexOfNumber( char[] json, int index )
		{
			int lastIndex;
			for ( lastIndex = index; lastIndex < json.Length; lastIndex++ )
				if ( "0123456789+-.eE".IndexOf( json[lastIndex] ) == -1 )
				{
					break;
				}
			return lastIndex - 1;
		}


		private static void EatWhitespace( char[] json, ref int index )
		{
			for ( ; index < json.Length; index++ )
				if ( " \t\n\r".IndexOf( json[index] ) == -1 )
				{
					break;
				}
		}

		private static int LookAhead( char[] json, int index )
		{
			int saveIndex = index;
			return NextToken( json, ref saveIndex );
		}

		private static int NextToken( char[] json, ref int index )
		{
			EatWhitespace( json, ref index );

			if ( index == json.Length )
			{
				return TOKEN_NONE;
			}

			char c = json[index];
			index++;
			switch ( c )
			{
				case '{':
					return TOKEN_CURLY_OPEN;
				case '}':
					return TOKEN_CURLY_CLOSE;
				case '[':
					return TOKEN_SQUARED_OPEN;
				case ']':
					return TOKEN_SQUARED_CLOSE;
				case ',':
					return TOKEN_COMMA;
				case '"':
					return TOKEN_STRING;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
					return TOKEN_NUMBER;
				case ':':
					return TOKEN_COLON;
			}
			index--;

			int remainingLength = json.Length - index;

			// false
			if ( remainingLength >= 5 )
			{
				if ( json[index] == 'f' &&
					 json[index + 1] == 'a' &&
					 json[index + 2] == 'l' &&
					 json[index + 3] == 's' &&
					 json[index + 4] == 'e' )
				{
					index += 5;
					return TOKEN_FALSE;
				}
			}

			// true
			if ( remainingLength >= 4 )
			{
				if ( json[index] == 't' &&
					 json[index + 1] == 'r' &&
					 json[index + 2] == 'u' &&
					 json[index + 3] == 'e' )
				{
					index += 4;
					return TOKEN_TRUE;
				}
			}

			// null
			if ( remainingLength >= 4 )
			{
				if ( json[index] == 'n' &&
					 json[index + 1] == 'u' &&
					 json[index + 2] == 'l' &&
					 json[index + 3] == 'l' )
				{
					index += 4;
					return TOKEN_NULL;
				}
			}

			return TOKEN_NONE;
		}

		#endregion

		#region Serialization

		/*
		protected static bool SerializeObjectOrArray(object objectOrArray, StringBuilder builder, int lv)
		{
			var o = objectOrArray as Map;
			if ( o != null )
				return SerializeObject( o, builder, lv );
			var list = objectOrArray as ArrayList;
			if ( list != null )
				return SerializeArray( list, builder, lv );
			return false;
		}
		*/

		private static bool SerializeObject( IDictionary anObject, StringBuilder builder, int lv, bool isIndent, bool sortKey )
		{
			string indent = Join( INDENT, lv );
			string indent2 = indent + INDENT;
			builder.Append( "{" );
			if ( anObject.Count > 0 )
			{
				if ( isIndent )
				{
					builder.AppendLine();
					builder.Append( indent2 );
				}

				bool first = true;
				ICollection cc;
				if ( sortKey )
				{
					ArrayList array = new ArrayList( anObject );
					array.Sort( new DictionaryEntryComparer() );
					cc = array;
				}
				else
					cc = anObject;
				foreach ( DictionaryEntry e in cc )
				{
					string key = e.Key.ToString();
					object value = e.Value;

					if ( !first )
					{
						builder.Append( "," );
						if ( isIndent )
						{
							builder.AppendLine();
							builder.Append( indent2 );
						}
						else
							builder.Append( " " );
					}

					SerializeString( key, builder );
					builder.Append( ":" );
					if ( isIndent )
						builder.Append( " " );
					if ( !SerializeValue( value, builder, lv + 1, isIndent, sortKey ) )
					{
						return false;
					}

					first = false;
				}

				if ( isIndent )
				{
					builder.AppendLine();
					builder.Append( indent );
				}
			}

			builder.Append( "}" );
			return true;
		}

		private static bool SerializeDictionary( Dictionary<string, string> dict, StringBuilder builder, int lv, bool isIndent, bool sortKey )
		{
			string indent = Join( INDENT, lv );
			string indent2 = indent + INDENT;
			builder.Append( "{" );
			if ( dict.Count > 0 )
			{
				if ( isIndent )
				{
					builder.AppendLine();
					builder.Append( indent2 );
				}
				bool first = true;
				ICollection<KeyValuePair<string, string>> cc;
				if ( sortKey )
				{
					List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>( dict );
					list.Sort( new KeyValuePairComparer() );
					cc = list;
				}
				else
					cc = dict;
				foreach ( var kv in cc )
				{
					if ( !first )
					{
						builder.Append( "," );
						if ( isIndent )
						{
							builder.AppendLine();
							builder.Append( indent2 );
						}
						else
							builder.Append( " " );
					}

					SerializeString( kv.Key, builder );
					builder.Append( ":" );
					if ( isIndent )
						builder.Append( " " );
					SerializeString( kv.Value, builder );

					first = false;
				}

				if ( isIndent )
				{
					builder.AppendLine();
					builder.Append( indent );
				}
			}

			builder.Append( "}" );
			return true;
		}


		private static bool SerializeArray( IList anArray, StringBuilder builder, int lv, bool isIndent, bool sortKey )
		{
			string indent = Join( INDENT, lv );
			string indent2 = indent + INDENT;
			builder.Append( "[" );
			if ( anArray.Count > 0 )
			{
				if ( isIndent )
				{
					builder.AppendLine();
					builder.Append( indent2 );
				}
				bool first = true;
				foreach ( object value in anArray )
				{
					if ( !first )
					{
						builder.Append( "," );
						if ( isIndent )
						{
							builder.AppendLine();
							builder.Append( indent2 );
						}
						else
							builder.Append( " " );
					}

					if ( !SerializeValue( value, builder, lv + 1, isIndent, sortKey ) )
					{
						return false;
					}

					first = false;
				}

				if ( isIndent )
				{
					builder.AppendLine();
					builder.Append( indent );
				}
			}
			builder.Append( "]" );
			return true;
		}


		private static bool SerializeValue( object value, StringBuilder builder, int lv, bool isIndent, bool sortKey )
		{
			if ( value == null )
			{
				builder.Append( "null" );
			}
			else if ( value.GetType().IsArray )
			{
				SerializeArray( new ArrayList( ( ICollection )value ), builder, lv, isIndent, sortKey );
			}
			else if ( value is string )
			{
				SerializeString( ( string )value, builder );
			}
			else if ( value.GetType().IsEnum )
			{
				SerializeNumber( Convert.ToInt32( value ), builder );
			}
			else if ( value is Char )
			{
				SerializeString( Convert.ToString( ( char )value ), builder );
			}
			else if ( value is Dictionary<string, string> )
			{
				SerializeDictionary( ( Dictionary<string, string> )value, builder, lv, isIndent, sortKey );
			}
			else if ( value is IDictionary )
			{
				SerializeObject( ( IDictionary )value, builder, lv, isIndent, sortKey );
			}
			else if ( value is IList )
			{
				SerializeArray( ( IList )value, builder, lv, isIndent, sortKey );
			}
			else if ( ( value is Boolean ) && ( Boolean )value )
			{
				builder.Append( "true" );
			}
			else if ( ( value is Boolean ) && ( ( Boolean )value == false ) )
			{
				builder.Append( "false" );
			}
			else if ( value.GetType().IsPrimitive )
			{
				SerializeNumber( Convert.ToSingle( value ), builder );
			}
			else
			{
				return false;
			}

			return true;
		}


		private static void SerializeString( string aString, StringBuilder builder )
		{
			builder.Append( "\"" );

			char[] charArray = aString.ToCharArray();
			foreach ( char c in charArray )
			{
				if ( c == '"' )
				{
					builder.Append( "\\\"" );
				}
				else if ( c == '\\' )
				{
					builder.Append( "\\\\" );
				}
				else if ( c == '\b' )
				{
					builder.Append( "\\b" );
				}
				else if ( c == '\f' )
				{
					builder.Append( "\\f" );
				}
				else if ( c == '\n' )
				{
					builder.Append( "\\n" );
				}
				else if ( c == '\r' )
				{
					builder.Append( "\\r" );
				}
				else if ( c == '\t' )
				{
					builder.Append( "\\t" );
				}
				else
				{
					//int codepoint = Convert.ToInt32( c );
					//if ( ( codepoint >= 32 ) && ( codepoint <= 126 ) )
					//{
					builder.Append( c.ToString( CultureInfo.InvariantCulture ) );
					//}
					//else
					//{
					//    builder.Append( "\\u" + Convert.ToString( codepoint, 16 ).PadLeft( 4, '0' ) );
					//}
				}
			}

			builder.Append( "\"" );
		}

		private static void SerializeNumber( float number, StringBuilder builder )
		{
			//builder.Append( Decimal.Parse( number.ToString( CultureInfo.InvariantCulture ), NumberStyles.Float ) );
			builder.Append( number.ToString( CultureInfo.InvariantCulture ) );
		}

		private static string Join( string s, int n )
		{
			StringBuilder sb = new StringBuilder();
			for ( int i = 0; i < n; i++ )
				sb.Append( s );
			return sb.ToString();
		}

		#endregion
	}

	public class DictionaryEntryComparer : IComparer
	{
		readonly CaseInsensitiveComparer _comparer = new CaseInsensitiveComparer();

		public int Compare( object x, object y )
		{
			DictionaryEntry e1 = ( DictionaryEntry )x;
			DictionaryEntry e2 = ( DictionaryEntry )y;
			return this._comparer.Compare( e1.Key, e2.Key );
		}
	}

	public class KeyValuePairComparer : IComparer<KeyValuePair<string, string>>
	{
		readonly CaseInsensitiveComparer _comparer = new CaseInsensitiveComparer();

		public int Compare( KeyValuePair<string, string> x, KeyValuePair<string, string> y )
		{
			return this._comparer.Compare( x.Key, y.Key );
		}
	}

	#region Extension methods

	public static class MiniJsonExtensions
	{
		public static string ToJson( this IDictionary obj, bool indent = false, bool sortKey = false )
		{
			return MiniJSON.JsonEncode( obj, indent, sortKey );
		}


		public static string ToJson( this Dictionary<string, string> obj, bool indent = false, bool sortKey = false )
		{
			return MiniJSON.JsonEncode( obj, indent, sortKey );
		}


		public static ArrayList ArrayListFromJson( this string json )
		{
			return MiniJSON.JsonDecode( json ) as ArrayList;
		}


		public static Hashtable HashtableFromJson( this string json )
		{
			return MiniJSON.JsonDecode( json ) as Hashtable;
		}
	}

	#endregion
}