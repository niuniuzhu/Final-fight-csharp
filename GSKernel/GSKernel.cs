using System;
using System.IO;
using Core;
using Core.XML;
using Net;

namespace GateServer
{
	public class GSKernel
	{
		public delegate int PFCSMsgHandler( string pMsg, int n32MsgLength );
		public delegate int PFSSMsgHandler( CGSSSInfo piSSInfo, string pMsg, int n32MsgLength );
		public delegate int PFGCMsgHandler( int n32NSID, string pMsg, int n32MsgLength );

		struct SUserToken
		{
			public string UserName;
			public string UserToken;
			public long OverTime;
			public uint ReconnectCount;
			public uint NetSessionID;
		}

		struct SGSConfig
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
		}

		struct SSSNetInfo
		{
			public long tConnMilsec;
			public CGSSSInfo pcSSInfo;
			public ulong un64MsgSent;
			public ulong un64MsgRecved;
			public ulong un64DataSent;
			public ulong un64DataRecved;
		}

		private PFCSMsgHandler[] m_asCSMsgHandler;//中心服务器消息
		private PFSSMsgHandler[] m_asSSMsgHandler;//场景服务器消息
		private SGSConfig m_sGSConfig;
		private long m_tStartTime;
		private int m_n32HeartBeatTimes;
		private long m_tHeartBeatTicks;
		private int m_n32MaxSSNum;

		private static GSKernel _instance;
		public static GSKernel instance => _instance ?? ( _instance = new GSKernel() );

		private GSKernel()
		{
			this.m_asCSMsgHandler = new PFCSMsgHandler[100];
			this.m_asSSMsgHandler = new PFSSMsgHandler[100];
		}

		public EResult Initialize()
		{
			this.m_asCSMsgHandler[CSToGS.MsgID.EMsgToGsfromCsAskPingRet - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_AskPingRet;
			this.m_asCSMsgHandler[CSToGS.MsgID.EMsgToGsfromCsOrderOpenListen - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_OrderOpenListen;
			this.m_asCSMsgHandler[CSToGS.MsgID.EMsgToGsfromCsOrderCloseListen - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_OrderCloseListen;
			this.m_asCSMsgHandler[CSToGS.MsgID.EMsgToGsfromCsOrderKickoutGc - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_OrderKickoutGC;
			this.m_asCSMsgHandler[CSToGS.MsgID.EMsgToGsfromCsUserConnectedSs - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_UserConnectedToSS;
			this.m_asCSMsgHandler[CSToGS.MsgID.EMsgToGsfromCsUserDisConnectedSs - CSToGS.MsgID.EMsgToGsfromCsBegin] = this.OnMsgFromCS_UserDisConnectedToSS;

			this.m_asSSMsgHandler[SSToGS.MsgID.EMsgToGsfromSsAskPingRet - SSToGS.MsgID.EMsgToGsfromSsBegin] = this.OnMsgFromSS_AskPingRet;
			this.m_asSSMsgHandler[SSToGS.MsgID.EMsgToGsfromSsOrderKickoutGc - SSToGS.MsgID.EMsgToGsfromSsBegin] = this.OnMsgFromSS_OrderKickoutGC;

			EResult eResult = this.LoadConfig();

			if ( EResult.Normal == eResult )
			{
				Logger.Info( "CGSKernel Initialize success" );
			}
			return eResult;
		}

		private EResult LoadConfig()
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

			this.m_sGSConfig.sCSIP = doc.GetNode( "IP" ).text;
			this.m_sGSConfig.n32CSPort = int.Parse( doc.GetNode( "Port" ).text );
			this.m_sGSConfig.n32CSMaxMsgSize = int.Parse( doc.GetNode( "MsgMaxSize" ).text );
			this.m_sGSConfig.aszMyUserPwd = doc.GetNode( "PWD" ).text;
			this.m_sGSConfig.n32GSID = int.Parse( doc.GetNode( "GSID" ).text );
			this.m_sGSConfig.sGCListenIP = doc.GetNode( "ListenIP" ).text;
			this.m_sGSConfig.n32GCListenPort = int.Parse( doc.GetNode( "ListenPort" ).text );
			this.m_sGSConfig.n32GCMaxMsgSize = int.Parse( doc.GetNode( "MsgMaxSize" ).text );
			this.m_sGSConfig.n32MaxGCNum = int.Parse( doc.GetNode( "MaxGCNum" ).text );
			this.m_sGSConfig.sBSListenIP = doc.GetNode( "BSIP" ).text;
			this.m_sGSConfig.n32BSListenPort = int.Parse( doc.GetNode( "BSPort" ).text );
			this.m_sGSConfig.n32SkipBalance = int.Parse( doc.GetNode( "IfSkipBS" ).text );

			return EResult.Normal;
		}

		public EResult Start()
		{
			NetSessionMgr netSession = new NetSessionMgr();
			// 允许客户端连接
			netSession.CreateListener( NetworkProtoType.TCP, m_sGSConfig.n32GCListenPort, 102400, out _ );
			// 连接中心服务器
			netSession.CreateConnector( NetworkProtoType.TCP, SessionType.ClientG2C, m_sGSConfig.sCSIP, m_sGSConfig.n32CSPort, 10240000, 0 );
			// 连接DBServer
			netSession.CreateConnector( NetworkProtoType.TCP, SessionType.ClientG2B, m_sGSConfig.sBSListenIP, m_sGSConfig.n32BSListenPort, 1024000, 0 );

			//reset member variables.
			m_tHeartBeatTicks = 0;
			this.MainLoop();

			return EResult.Normal;
		}

		private EResult Stop()
		{
			Logger.Info( "GSKernel Stop success!" );
			return EResult.Normal;
		}

		private void MainLoop()
		{
		}

		private int OnMsgFromCS_AskPingRet( string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_OrderOpenListen( string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_OrderCloseListen( string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_OrderKickoutGC( string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_UserConnectedToSS( string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromCS_UserDisConnectedToSS( string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromSS_AskPingRet( CGSSSInfo pissinfo, string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}

		private int OnMsgFromSS_OrderKickoutGC( CGSSSInfo pissinfo, string pmsg, int n32msglength )
		{
			throw new NotImplementedException();
		}
	}
}