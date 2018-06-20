using Core.Misc;
using Core.Net;
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
		public int bs_base_index;
		public int bs_max_count;
		public int client_listen_port;
		public readonly Dictionary<uint, OneBsInfo> allBsInfo = new Dictionary<uint, OneBsInfo>();
		public readonly List<ServerAddr> gAllServerAddr = new List<ServerAddr>();

		public EResult Load()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\LSCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load LSCfg failed for {e}\n" );
				return EResult.CfgFailed;
			}

			Hashtable mainGC = json.GetMap( "MainGC" );
			Hashtable MainBS = json.GetMap( "MainBS" );

			this.client_listen_port = mainGC.GetInt( "ListernPortForClient" );
			this.bs_listen_port = MainBS.GetInt( "ListernPortForBS" );
			this.bs_base_index = MainBS.GetInt( "BSBaseIndex" );
			this.bs_max_count = MainBS.GetInt( "BSMaxCount" );

			for ( int i = 1; i <= this.bs_max_count; ++i )
			{
				string server_address = MainBS.GetString( $"BS{i}" );
				string server_address_ex = MainBS.GetString( $"BS{i}Export" );
				uint key = ( uint )( this.bs_base_index + i - 1 );
				OneBsInfo oneBsInfo = new OneBsInfo
				{
					bs_Id = key,
					bs_nets = 0,
					bs_isLost = true
				};
				this.allBsInfo[key] = oneBsInfo;
				{
					string[] pair = server_address.Split( ':' );
					oneBsInfo.bs_Ip = pair[0];
					oneBsInfo.bs_Port = int.Parse( pair[1] );
				}
				{
					string[] pair = server_address_ex.Split( ':' );
					oneBsInfo.bs_IpExport = pair[0];
					int exPos = int.Parse( pair[1] );
					if ( exPos > 0 )
						Tools.GetNetIP( ref oneBsInfo.bs_IpExport, exPos );
				}
			}
			return this.LoadServerList();
		}

		private EResult LoadServerList()
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
				return EResult.CfgFailed;
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
			return EResult.Normal;
		}
	}
}