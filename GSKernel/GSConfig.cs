using Core;
using Core.XML;
using Shared;
using System;
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
			XML doc;
			try
			{
				string content = File.ReadAllText( @".\Config\GSCfg.xml" );
				doc = new XML( content );
			}
			catch ( Exception e )
			{
				Logger.Error( $"load GSCfg.xml failed for {e}\n" );
				return EResult.CfgFailed;
			}

			this.sCSIP = doc.GetNode( "IP" ).text;
			this.n32CSPort = int.Parse( doc.GetNode( "Port" ).text );
			this.n32CSMaxMsgSize = int.Parse( doc.GetNode( "MsgMaxSize" ).text );
			this.aszMyUserPwd = doc.GetNode( "PWD" ).text;
			this.n32GSID = int.Parse( doc.GetNode( "GSID" ).text );
			this.sGCListenIP = doc.GetNode( "ListenIP" ).text;
			this.n32GCListenPort = int.Parse( doc.GetNode( "ListenPort" ).text );
			this.n32GCMaxMsgSize = int.Parse( doc.GetNode( "MsgMaxSize" ).text );
			this.n32MaxGCNum = int.Parse( doc.GetNode( "MaxGCNum" ).text );
			this.sBSListenIP = doc.GetNode( "BSIP" ).text;
			this.n32BSListenPort = int.Parse( doc.GetNode( "BSPort" ).text );
			this.n32SkipBalance = int.Parse( doc.GetNode( "IfSkipBS" ).text );

			return EResult.Normal;
		}
	}
}