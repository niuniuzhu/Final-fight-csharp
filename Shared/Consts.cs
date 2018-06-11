namespace Shared
{
	public enum EResult
	{
		Normal,
		CfgFailed,
		ParseProtoFailed,
	}

	public enum EServerNetState
	{
		eSNS_Closed = 0,
		eSNS_Connecting,
		eSNS_Free,
		eSNS_Busy,
		eSNS_Full,
	};

	public static class Consts
	{
		public const long c_tDefaultPingCDTick = 1000 * 160;
	}
}