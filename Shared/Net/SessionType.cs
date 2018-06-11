namespace Shared.Net
{
	public enum SessionType
	{
		None,
		ServerCsOnlySs,
		ServerCsOnlyGS,
		ServerSs,
		ServerGS,
		ServerLog,
		ServerBSOnlyGS,
		ServerBSOnlyGc,
		ServerLsOnlyBS,
		ServerLsOnlyGc,
		ClientB2L,
		ClientG2C,
		ClientG2S,
		ClientG2B,
		ClientS2C,//as client to link gs
		ServerCsOnlyRc,
		ClientC2Lg,
		ClientS2Lg,
		ClientC2L,// link login server
		ClientC2B,//link balance server
		ClientC2G,//link gate server
		ClientC2R,
		ClientS2Log,
		ClientC2Log,
		ClientC2LogicRedis,
	};
}