using System;
using System.Collections.Generic;

namespace Core.XML
{
	public class XMLList : IEnumerable<XML>
	{
		readonly List<XML> _list;

		internal XMLList()
		{
		    this._list = new List<XML>();
		}

		internal XMLList( List<XML> list )
		{
		    this._list = list;
		}

		public int count
		{
			get { return this._list.Count; }
		}

		public XML this[int index]
		{
			get { return this._list[index]; }
		}

		public IEnumerator<XML> GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		internal void Add( XML xml )
		{
		    this._list.Add( xml );
		}

		internal void Clear()
		{
		    this._list.Clear();
		}

		static List<XML> _tmpList = new List<XML>();
		internal XMLList Filter( string selector )
		{
			bool allFit = true;
			_tmpList.Clear();
			foreach ( XML xml in this._list )
			{
				if ( xml.name != selector )
					allFit = false;
				else
					_tmpList.Add( xml );
			}

			if ( allFit )
				return this;
			XMLList ret = new XMLList( _tmpList );
			_tmpList = new List<XML>();
			return ret;
		}

		internal XML Find( string selector )
		{
			foreach ( XML xml in this._list )
			{
				if ( xml.name == selector )
					return xml;
			}
			return null;
		}
	}
}
