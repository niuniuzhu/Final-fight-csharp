using Core.Misc;
using Shared;
using System;
using System.Collections;
using System.IO;

namespace GateServer
{
	public class GSConfig
	{
		public int n32GSID;//网关分配号
		public string sCSIP;//中心服务器地址
		public int n32CSPort;//中心服务器端口
		public string aszMyUserPwd;//很奇怪,该密码发给了中心服务器和场景服务器
		public int n32CSMaxMsgSize;//中心服务器最大消息长度
		public int n32WorkingThreadNum;//似乎没用这个
		public string sGCListenIP;//网关监听地址
		public int n32GCListenPort;//网关监听端口
		public int n32GCMaxMsgSize;//网关最大消息长度
		public int n32MaxGCNum;//网关最大连接数
		public string sBSListenIP;//负载监听地址
		public int n32BSListenPort;//负载监听端口	
		public int n32SkipBalance;//是否跳过BS认证

		public EResult Load()
		{
			Hashtable json;
			try
			{
				string content = File.ReadAllText( @".\Config\GSCfg.json" );
				json = ( Hashtable )MiniJSON.JsonDecode( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load GSCfg.xml failed for {e}\n" );
				return EResult.CfgFailed;
			}

			this.sCSIP = json.GetString( "IP" );
			this.n32CSPort = json.GetInt( "Port" );
			this.n32CSMaxMsgSize = json.GetInt( "MsgMaxSize" );
			this.aszMyUserPwd = json.GetString( "PWD" );
			this.n32GSID = json.GetInt( "GSID" );
			this.sGCListenIP = json.GetString( "ListenIP" );
			this.n32GCListenPort = json.GetInt( "ListenPort" );
			this.n32GCMaxMsgSize = json.GetInt( "MsgMaxSize" );
			this.n32MaxGCNum = json.GetInt( "MaxGCNum" );
			this.sBSListenIP = json.GetString( "BSIP" );
			this.n32BSListenPort = json.GetInt( "BSPort" );
			this.n32SkipBalance = json.GetInt( "IfSkipBS" );

			return EResult.Normal;
		}
	}
}