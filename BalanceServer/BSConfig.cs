using Core.Misc;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.Net;

namespace BalanceServer
{
	public class OneGsInfo
	{
		public bool gs_isLost;
		public uint gs_nets;
		public uint gs_gc_count;//gs 当前连接数//
		public string gs_IpExport;//gs 对外公布地址//
		public string gs_Ip;//gs 对内验证地址//
		public int gs_Port;
		public int gs_Id;
	}

	public class BSConfig
	{
		public int gs_listen_port;
		public int gs_full_count;
		public int gs_base_index;
		public int gs_max_count;
		public int client_listen_port;
		public string ls_ip;
		public int ls_port;
		public readonly List<string> gs_ip_list = new List<string>();
		public readonly Dictionary<int, OneGsInfo> allGsInfo = new Dictionary<int, OneGsInfo>();

		public ErrorCode Load()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\BSCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load GSCfg failed for {e}" );
				return ErrorCode.CfgFailed;
			}

			Hashtable mainGate = json.GetMap( "MainGate" );
			Hashtable mainClient = json.GetMap( "MainClient" );
			Hashtable mainLogin = json.GetMap( "MainLogin" );

			this.client_listen_port = mainClient.GetInt( "ListernPortForClient" );
			this.ls_ip = mainLogin.GetString( "LSIP" );
			this.ls_port = mainLogin.GetInt( "LSPort" );
			this.gs_listen_port = mainGate.GetInt( "ListernPortForGate" );
			this.gs_base_index = mainGate.GetInt( "GateBaseIndex" );
			this.gs_max_count = mainGate.GetInt( "GateMaxCount" );
			this.gs_full_count = mainGate.GetInt( "GateFullCount" );

			for ( int i = 1; i <= this.gs_max_count; ++i )
			{
				string server_address = mainGate.GetString( $"GateServer{i}" );
				string server_address_ex = mainGate.GetString( $"GateServer{i}Export" );
				this.gs_ip_list.Add( server_address );
				int key = this.gs_base_index + i - 1;
				OneGsInfo oneGsInfo = new OneGsInfo
				{
					gs_Id = key,
					gs_nets = 0,
					gs_isLost = true,
					gs_gc_count = 0
				};
				this.allGsInfo[key] = oneGsInfo;
				{
					string[] pair = server_address.Split( ':' );
					oneGsInfo.gs_Ip = pair[0];
					oneGsInfo.gs_Port = int.Parse( pair[1] );
				}
				{
					string[] pair = server_address_ex.Split( ':' );
					oneGsInfo.gs_IpExport = pair[0];
					int exPos = int.Parse( pair[1] );
					if ( exPos > 0 )
						Tools.GetNetIP( ref oneGsInfo.gs_IpExport, exPos );
				}
			}
			return ErrorCode.Success;
		}
	}
}