using Core.Misc;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BalanceServer
{
	public class sOneGsInfo
	{
		public bool gs_isLost;
		public uint gs_nets;
		public uint gs_gc_count;//gs 当前连接数//
		public string gs_IpExport;//gs 对外公布地址//
		public string gs_Ip;//gs 对内验证地址//
		public int gs_Port;
		public uint gs_Id;
	}

	public class BSConfig
	{
		public int gs_listen_port;
		public int gs_full_count;
		public int gs_base_index;
		public int gs_max_count;
		public List<string> gs_ip_list;
		public int client_listen_port;
		public string ls_ip;
		public int ls_port;
		public readonly Dictionary<uint, sOneGsInfo> gAllGsInfo = new Dictionary<uint, sOneGsInfo>();

		public EResult Load()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\BSCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load GSCfg.xml failed for {e}\n" );
				return EResult.CfgFailed;
			}

			this.client_listen_port = json.GetInt( "ListernPortForClient" );
			this.ls_ip = json.GetString( "LSIP" );
			this.ls_port = json.GetInt( "LSPort" );
			this.gs_listen_port = json.GetInt( "ListernPortForGate" );
			this.gs_base_index = json.GetInt( "GateBaseIndex" );
			this.gs_max_count = json.GetInt( "GateMaxCount" );
			this.gs_full_count = json.GetInt( "GateFullCount" );

			for ( int i = 1; i <= this.gs_max_count; ++i )
			{
				string server_address = json.GetString( $"GateServer{i}" );
				string server_address_ex = json.GetString( $"GateServer{i}Export" );
				this.gs_ip_list.Add( server_address );
				uint key = ( uint )( this.gs_base_index + i - 1 );
				sOneGsInfo oneGsInfo = new sOneGsInfo();
				oneGsInfo.gs_Id = key;
				oneGsInfo.gs_nets = 0;
				oneGsInfo.gs_isLost = true;
				oneGsInfo.gs_gc_count = 0;
				this.gAllGsInfo[key] = oneGsInfo;
				{
					string[] pair = server_address.Split( ':' );
					oneGsInfo.gs_Ip = pair[0];
					oneGsInfo.gs_Port = int.Parse( pair[1] );
				}
				{
					string[] pair = server_address_ex.Split( ':' );
					oneGsInfo.gs_IpExport = pair[0];
					int exPos = int.Parse( pair[1] );
					//todo
					//if ( exPos > 0 )
					//	gNetSessionMgr.getnetip( oneGsInfo.gs_IpExport, exPos - 1 );
				}
			}
			return EResult.Normal;
		}
	}
}