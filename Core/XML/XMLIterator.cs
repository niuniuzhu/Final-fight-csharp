using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Core.XML
{
	public enum XMLTagType
	{
		Start,
		End,
		Void,
		CDATA,
		Comment,
		Instruction
	}

	public class XMLIterator
	{
		public static string tagName;
		public static XMLTagType tagType;
		public static string lastTagName;

		static string _source;
		static int _sourceLen;
		static int _parsePos;
		static int _tagPos;
		static int _tagLength;
		static int _lastTagEnd;
		static bool _attrParsed;
		static bool _lowerCaseName;
		static readonly StringBuilder BUFFER = new StringBuilder();
		static readonly Dictionary<string, string> ATTRIBUTES = new Dictionary<string, string>();

		const string CDATA_START = "<![CDATA[";
		const string CDATA_END = "]]>";
		const string COMMENT_START = "<!--";
		const string COMMENT_END = "-->";

		public static void Begin( string source, bool lowerCaseName = false )
		{
			_source = source;
			_lowerCaseName = lowerCaseName;
			_sourceLen = source.Length;
			_parsePos = 0;
			_lastTagEnd = 0;
			_tagPos = 0;
			_tagLength = 0;
			tagName = null;
		}

		public static bool NextTag()
		{
			int pos;
			tagType = XMLTagType.Start;
			BUFFER.Length = 0;
			_lastTagEnd = _parsePos;
			_attrParsed = false;
			lastTagName = tagName;

			while ( ( pos = _source.IndexOf( '<', _parsePos ) ) != -1 )
			{
				_parsePos = pos;
				pos++;

				if ( pos == _sourceLen )
					break;

				char c = _source[pos];
				if ( c == '!' )
				{
					if ( _sourceLen > pos + 7 && _source.Substring( pos - 1, 9 ) == CDATA_START )
					{
						pos = _source.IndexOf( CDATA_END, pos, StringComparison.Ordinal );
						tagType = XMLTagType.CDATA;
						tagName = string.Empty;
						_tagPos = _parsePos;
						if ( pos == -1 )
							_tagLength = _sourceLen - _parsePos;
						else
							_tagLength = pos + 3 - _parsePos;
						_parsePos += _tagLength;
						return true;
					}
					if ( _sourceLen > pos + 2 && _source.Substring( pos - 1, 4 ) == COMMENT_START )
					{
						pos = _source.IndexOf( COMMENT_END, pos, StringComparison.Ordinal );
						tagType = XMLTagType.Comment;
						tagName = string.Empty;
						_tagPos = _parsePos;
						if ( pos == -1 )
							_tagLength = _sourceLen - _parsePos;
						else
							_tagLength = pos + 3 - _parsePos;
						_parsePos += _tagLength;
						return true;
					}
					pos++;
					tagType = XMLTagType.Instruction;
				}
				else if ( c == '/' )
				{
					pos++;
					tagType = XMLTagType.End;
				}
				else if ( c == '?' )
				{
					pos++;
					tagType = XMLTagType.Instruction;
				}

				for ( ; pos < _sourceLen; pos++ )
				{
					c = _source[pos];
					if ( Char.IsWhiteSpace( c ) || c == '>' || c == '/' )
						break;
				}
				if ( pos == _sourceLen )
					break;

				BUFFER.Append( _source, _parsePos + 1, pos - _parsePos - 1 );
				if ( BUFFER.Length > 0 && BUFFER[0] == '/' )
					BUFFER.Remove( 0, 1 );

				bool singleQuoted = false, doubleQuoted = false;
				int possibleEnd = -1;
				for ( ; pos < _sourceLen; pos++ )
				{
					c = _source[pos];
					if ( c == '"' )
					{
						if ( !singleQuoted )
							doubleQuoted = !doubleQuoted;
					}
					else if ( c == '\'' )
					{
						if ( !doubleQuoted )
							singleQuoted = !singleQuoted;
					}

					if ( c == '>' )
					{
						if ( !( singleQuoted || doubleQuoted ) )
						{
							possibleEnd = -1;
							break;
						}

						possibleEnd = pos;
					}
					else if ( c == '<' )
						break;
				}
				if ( possibleEnd != -1 )
					pos = possibleEnd;

				if ( pos == _sourceLen )
					break;

				if ( _source[pos - 1] == '/' )
					tagType = XMLTagType.Void;

				tagName = BUFFER.ToString();
				if ( _lowerCaseName )
					tagName = tagName.ToLower();
				_tagPos = _parsePos;
				_tagLength = pos + 1 - _parsePos;
				_parsePos += _tagLength;

				return true;
			}

			_tagPos = _sourceLen;
			_tagLength = 0;
			tagName = null;
			return false;
		}

		public static string GetTagSource()
		{
			return _source.Substring( _tagPos, _tagLength );
		}

		public static string GetRawText( bool trim = false )
		{
			if ( _lastTagEnd == _tagPos )
				return string.Empty;
			if ( trim )
			{
				int i = _lastTagEnd;
				for ( ; i < _tagPos; i++ )
				{
					char c = _source[i];
					if ( !char.IsWhiteSpace( c ) )
						break;
				}

				if ( i == _tagPos )
					return string.Empty;
				return _source.Substring( i, _tagPos - i ).TrimEnd();
			}
			return _source.Substring( _lastTagEnd, _tagPos - _lastTagEnd );
		}

		public static string GetText( bool trim = false )
		{
			if ( _lastTagEnd == _tagPos )
				return string.Empty;
			if ( trim )
			{
				int i = _lastTagEnd;
				for ( ; i < _tagPos; i++ )
				{
					char c = _source[i];
					if ( !char.IsWhiteSpace( c ) )
						break;
				}

				if ( i == _tagPos )
					return string.Empty;
				return XMLUtils.DecodeString( _source.Substring( i, _tagPos - i ).TrimEnd() );
			}
			return XMLUtils.DecodeString( _source.Substring( _lastTagEnd, _tagPos - _lastTagEnd ) );
		}

		public static bool HasAttribute( string attrName )
		{
			if ( !_attrParsed )
			{
				ATTRIBUTES.Clear();
				ParseAttributes( ATTRIBUTES );
				_attrParsed = true;
			}

			return ATTRIBUTES.ContainsKey( attrName );
		}

		public static string GetAttribute( string attrName )
		{
			if ( !_attrParsed )
			{
				ATTRIBUTES.Clear();
				ParseAttributes( ATTRIBUTES );
				_attrParsed = true;
			}

			string value;
			if ( ATTRIBUTES.TryGetValue( attrName, out value ) )
				return value;
			return null;
		}

		public static string GetAttribute( string attrName, string defValue )
		{
			string ret = GetAttribute( attrName );
			if ( ret != null )
				return ret;
			return defValue;
		}

		public static int GetAttributeInt( string attrName )
		{
			return GetAttributeInt( attrName, 0 );
		}

		public static int GetAttributeInt( string attrName, int defValue )
		{
			string value = GetAttribute( attrName );
			if ( string.IsNullOrEmpty( value ) )
				return defValue;

			int ret;
			if ( int.TryParse( value, out ret ) )
				return ret;
			return defValue;
		}

		public static float GetAttributeFloat( string attrName )
		{
			return GetAttributeFloat( attrName, 0 );
		}

		public static float GetAttributeFloat( string attrName, float defValue )
		{
			string value = GetAttribute( attrName );
			if ( string.IsNullOrEmpty( value ) )
				return defValue;

			float ret;
			if ( float.TryParse( value, out ret ) )
				return ret;
			return defValue;
		}

		public static bool GetAttributeBool( string attrName )
		{
			return GetAttributeBool( attrName, false );
		}

		public static bool GetAttributeBool( string attrName, bool defValue )
		{
			string value = GetAttribute( attrName );
			if ( string.IsNullOrEmpty( value ) )
				return defValue;

			bool ret;
			if ( bool.TryParse( value, out ret ) )
				return ret;
			return defValue;
		}

		public static Dictionary<string, string> GetAttributes( Dictionary<string, string> result )
		{
			if ( result == null )
				result = new Dictionary<string, string>();

			if ( _attrParsed )
			{
				foreach ( KeyValuePair<string, string> kv in ATTRIBUTES )
					result[kv.Key] = kv.Value;
			}
			else //这里没有先ParseAttributes再赋值给result是为了节省复制的操作
				ParseAttributes( result );

			return result;
		}

		public static Hashtable GetAttributes( Hashtable result )
		{
			if ( result == null )
				result = new Hashtable();

			if ( _attrParsed )
			{
				foreach ( KeyValuePair<string, string> kv in ATTRIBUTES )
					result[kv.Key] = kv.Value;
			}
			else //这里没有先ParseAttributes再赋值给result是为了节省复制的操作
				ParseAttributes( result );

			return result;
		}

		static void ParseAttributes( IDictionary attrs )
		{
			string attrName;
			int valueStart;
			int valueEnd;
			bool waitValue = false;
			int quoted;
			BUFFER.Length = 0;
			int i = _tagPos;
			int attrEnd = _tagPos + _tagLength;

			if ( i < attrEnd && _source[i] == '<' )
			{
				for ( ; i < attrEnd; i++ )
				{
					char c = _source[i];
					if ( Char.IsWhiteSpace( c ) || c == '>' || c == '/' )
						break;
				}
			}

			for ( ; i < attrEnd; i++ )
			{
				char c = _source[i];
				if ( c == '=' )
				{
					valueStart = -1;
					valueEnd = -1;
					quoted = 0;
					for ( int j = i + 1; j < attrEnd; j++ )
					{
						char c2 = _source[j];
						if ( Char.IsWhiteSpace( c2 ) )
						{
							if ( valueStart != -1 && quoted == 0 )
							{
								valueEnd = j - 1;
								break;
							}
						}
						else if ( c2 == '>' )
						{
							if ( quoted == 0 )
							{
								valueEnd = j - 1;
								break;
							}
						}
						else if ( c2 == '"' )
						{
							if ( valueStart != -1 )
							{
								if ( quoted != 1 )
								{
									valueEnd = j - 1;
									break;
								}
							}
							else
							{
								quoted = 2;
								valueStart = j + 1;
							}
						}
						else if ( c2 == '\'' )
						{
							if ( valueStart != -1 )
							{
								if ( quoted != 2 )
								{
									valueEnd = j - 1;
									break;
								}
							}
							else
							{
								quoted = 1;
								valueStart = j + 1;
							}
						}
						else if ( valueStart == -1 )
						{
							valueStart = j;
						}
					}

					if ( valueStart != -1 && valueEnd != -1 )
					{
						attrName = BUFFER.ToString();
						if ( _lowerCaseName )
							attrName = attrName.ToLower();
						BUFFER.Length = 0;
						attrs[attrName] = XMLUtils.DecodeString( _source.Substring( valueStart, valueEnd - valueStart + 1 ) );
						i = valueEnd + 1;
					}
					else
						break;
				}
				else if ( !Char.IsWhiteSpace( c ) )
				{
					if ( waitValue || c == '/' || c == '>' )
					{
						if ( BUFFER.Length > 0 )
						{
							attrName = BUFFER.ToString();
							if ( _lowerCaseName )
								attrName = attrName.ToLower();
							attrs[attrName] = string.Empty;
							BUFFER.Length = 0;
						}

						waitValue = false;
					}

					if ( c != '/' && c != '>' )
						BUFFER.Append( c );
				}
				else
				{
					if ( BUFFER.Length > 0 )
						waitValue = true;
				}
			}
		}
	}
}
