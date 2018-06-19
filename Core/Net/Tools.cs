using System;
using System.Net;
using Core.Misc;

namespace Core.Net
{
	public static class Tools
	{
		public static bool GetNetIP( ref string ipaddr, int pos )
		{
			string host_name;
			try
			{
				host_name = Dns.GetHostName();
			}
			catch ( Exception e )
			{
				Logger.Error( e );
				return false;
			}

			IPAddress[] ipAddresses = Dns.GetHostAddresses( host_name );
			ipaddr = ipAddresses[pos].ToString();
			return true;
		}
	}
}