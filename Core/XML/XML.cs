using System;
using System.Collections.Generic;

namespace Core.XML
{
	public class XML
	{
		public string name { get; private set; }
		public string text { get; private set; }

		Dictionary<string, string> _attributes;
		XMLList _children;

		public XML( string text )
		{
			this.Parse( text );
		}

		private XML()
		{
		}

		public bool HasAttribute( string attrName )
		{
			if ( this._attributes == null )
				return false;

			return this._attributes.ContainsKey( attrName );
		}

		public string GetAttribute( string attrName )
		{
			return this.GetAttribute( attrName, null );
		}

		public string GetAttribute( string attrName, string defValue )
		{
			if ( this._attributes == null )
				return defValue;

			string ret;
			if ( this._attributes.TryGetValue( attrName, out ret ) )
				return ret;
			return defValue;
		}

		public int GetAttributeInt( string attrName )
		{
			return this.GetAttributeInt( attrName, 0 );
		}

		public int GetAttributeInt( string attrName, int defValue )
		{
			string value = this.GetAttribute( attrName );
			if ( string.IsNullOrEmpty( value ) )
				return defValue;

			int ret;
			if ( int.TryParse( value, out ret ) )
				return ret;
			return defValue;
		}

		public float GetAttributeFloat( string attrName )
		{
			return this.GetAttributeFloat( attrName, 0 );
		}

		public float GetAttributeFloat( string attrName, float defValue )
		{
			string value = this.GetAttribute( attrName );
			if ( string.IsNullOrEmpty( value ) )
				return defValue;

			float ret;
			if ( float.TryParse( value, out ret ) )
				return ret;
			return defValue;
		}

		public bool GetAttributeBool( string attrName, bool defValue = false )
		{
			string value = this.GetAttribute( attrName );
			if ( string.IsNullOrEmpty( value ) )
				return defValue;

			bool ret;
			if ( bool.TryParse( value, out ret ) )
				return ret;
			return defValue;
		}

		public string[] GetAttributeArray( string attrName )
		{
			string value = this.GetAttribute( attrName );
			if ( value != null )
			{
				if ( value.Length == 0 )
					return new string[] { };
				return value.Split( ',' );
			}
			return null;
		}

		public string[] GetAttributeArray( string attrName, char seperator )
		{
			string value = this.GetAttribute( attrName );
			if ( value != null )
			{
				if ( value.Length == 0 )
					return new string[] { };
				return value.Split( seperator );
			}
			return null;
		}

		public void SetAttribute( string attrName, string attrValue )
		{
			if ( this._attributes == null )
				this._attributes = new Dictionary<string, string>();

			this._attributes[attrName] = attrValue;
		}

		public XML GetNode( string selector )
		{
		    return this._children?.Find( selector );
		}

		public XMLList Elements()
		{
			return this._children ?? ( this._children = new XMLList() );
		}

		public XMLList Elements( string selector )
		{
			if ( this._children == null )
				this._children = new XMLList();
			return this._children.Filter( selector );
		}

		static readonly Stack<XML> S_NODE_STACK = new Stack<XML>();
		void Parse( string aSource )
		{
			XML lastOpenNode = null;
			S_NODE_STACK.Clear();

			XMLIterator.Begin( aSource );
			while ( XMLIterator.NextTag() )
			{
				if ( XMLIterator.tagType == XMLTagType.Start || XMLIterator.tagType == XMLTagType.Void )
				{
					XML childNode;
					if ( lastOpenNode != null )
						childNode = new XML();
					else
					{
						if ( this.name != null )
						{
							this.Cleanup();
							throw new Exception( "Invalid xml format - no root node." );
						}
						childNode = this;
					}

					childNode.name = XMLIterator.tagName;
					childNode._attributes = XMLIterator.GetAttributes( childNode._attributes );

					if ( lastOpenNode != null )
					{
						if ( XMLIterator.tagType != XMLTagType.Void )
							S_NODE_STACK.Push( lastOpenNode );
						if ( lastOpenNode._children == null )
							lastOpenNode._children = new XMLList();
						lastOpenNode._children.Add( childNode );
					}
					if ( XMLIterator.tagType != XMLTagType.Void )
						lastOpenNode = childNode;
				}
				else if ( XMLIterator.tagType == XMLTagType.End )
				{
					if ( lastOpenNode == null || lastOpenNode.name != XMLIterator.tagName )
					{
						this.Cleanup();
						throw new Exception( "Invalid xml format - <" + XMLIterator.tagName + "> dismatched." );
					}

					if ( lastOpenNode._children == null || lastOpenNode._children.count == 0 )
					{
						lastOpenNode.text = XMLIterator.GetText();
					}

					lastOpenNode = S_NODE_STACK.Count > 0 ? S_NODE_STACK.Pop() : null;
				}
			}
		}

		void Cleanup()
		{
			this.name = null;
		    this._attributes?.Clear();
		    this._children?.Clear();
		    this.text = null;
		}
	}
}