using Shared;

namespace GateServer
{
	public class GSSSInfo
	{
		//property from config file.
		public int ssID;
		public string userPwd;
		//property from scene server.
		public string listenIp;
		public int listenPort;
		//property from local.
		public EServerNetState ssNetState;
		public uint nsID;
		public uint connTimes;
		public long lastConnMilsec;
		public long pingTickCounter;

		public long msgReceived;
		public long msgSent;
		public long dataReceived;
		public long dataSent;
	}
}