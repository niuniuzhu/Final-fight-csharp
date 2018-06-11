using Shared;

namespace GateServer
{
	public class GSSSInfo
	{
		//property from config file.
		public int m_n32SSID;

		public string m_szUserPwd;
		//property  from scene server.
		public string m_sListenIP;
		public int m_n32ListenPort;
		//property from local.
		public EServerNetState m_eSSNetState;
		public int m_n32NSID;
		public uint m_un32ConnTimes;
		public long m_tLastConnMilsec;
		public long m_tPingTickCounter;

		public long m_n64MsgReceived;
		public long m_n64MsgSent;
		public long m_n64DataReceived;
		public long m_n64DataSent;
	}
}