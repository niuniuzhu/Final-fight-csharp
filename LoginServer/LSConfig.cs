using Core.Misc;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LoginServer
{
	public class LoginUserInfo
	{
		public string uin;
		public string sessionid;
		public uint plat;
	}

	public class OneBsInfo
	{
		public bool bs_isLost;
		public uint bs_nets;
		public string bs_IpExport; //bs 对外公布地址//
		public string bs_Ip;       //bs 对内验证地址//
		public int bs_Port;
		public uint bs_Id;
	}

	public class ServerAddr
	{
		public string str_name;
		public string str_addr;
		public int str_port;
	}

	public class LSConfig
	{
		public int bs_listen_port;
		public int client_listen_port;
		public readonly List<ServerAddr> gAllServerAddr = new List<ServerAddr>();

		public ErrorCode Load()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\LSCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load LSCfg failed for {e}" );
				return ErrorCode.CfgFailed;
			}

			Hashtable mainGC = json.GetMap( "MainGC" );
			Hashtable MainBS = json.GetMap( "MainBS" );

			this.client_listen_port = mainGC.GetInt( "ListernPortForClient" );
			this.bs_listen_port = MainBS.GetInt( "ListernPortForBS" );
			return this.LoadServerList();
		}

		private ErrorCode LoadServerList()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\SrvList.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load SrvList failed for {e}\n" );
				return ErrorCode.CfgFailed;
			}

			Hashtable mainList = json.GetMap( "MainList" );
			int listnum = mainList.GetInt( "ServerNum" );
			for ( uint i = 1; i <= listnum; ++i )
			{
				string server_name = mainList.GetString( $"Name{i}" );
				string server_addr = mainList.GetString( $"Addr{i}" );
				string[] pair = server_addr.Split( ':' );
				ServerAddr serveraddr = new ServerAddr
				{
					str_name = server_name,
					str_addr = pair[0],
					str_port = int.Parse( pair[1] )
				};
				this.gAllServerAddr.Add( serveraddr );
			}
			return ErrorCode.Success;
		}
	}
}