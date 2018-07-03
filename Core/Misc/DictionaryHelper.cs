using System.Collections;

namespace Core.Misc
{
	public static class DictionaryHelper
	{
		public static void Copy( this IDictionary a, IDictionary b )
		{
			foreach ( DictionaryEntry de in a )
				b[de.Key] = de.Value;
		}
	}
}