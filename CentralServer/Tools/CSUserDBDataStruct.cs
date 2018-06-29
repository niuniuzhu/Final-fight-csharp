using Shared;
using System;
using System.Collections.Generic;

namespace CentralServer.Tools
{
	public enum EUserDBDataType
	{
		eUserDBType_None = 0,//			un64ObjIdx; 
		eUserDBType_Channel,//		eUserPlatform;
		eUserDBType_Name,//		szUserName[c_n32DefaultNameLen];
		eUserDBType_Pwd,//			szUserPwd[c_n32DefaultUserPwdLen]; 
		eUserDBType_NickName,//				szNickName[c_n32DefaultNickNameLen+1];
		eUserDBType_HeaderId,//		un32HeaderID;
		eUserDBType_Sex,//				n8Sex; 
		eUserDBType_Diamond,//			n64Diamond;
		eUserDBType_Gold,//			n64Gold;;
		eUserDBType_RegisterTime,//			tRegisteUTCMillisec;	//注册时间(s)
		eUserDBType_Score,//				n64Score;
		eUserDBType_TotalGameInns,//			un32TotalGameInns;
		eUserDBType_TotalWinInns,//		un32TotalWinInns;
		eUserDBType_TotalHeroKills,//				un32TotalHeroKills;
		eUserDBType_TotalDestoryBuild,//				un32TotalDestoryBuildings;
		eUserDBType_TotalDeadTimes,//				un32TotalDeadTimes;

		eUserDBType_UserLv,//				un16UserLv;
		eUserDBType_UserLvExp,//				un32UserCurLvExp;
		eUserDBType_LastFirstWinTime,//			tLastFirstWinTime; 

		eUserDBType_CLDay,//				un16Cldays;
		eUserDBType_LastGetLoginReward,//			tLastGetLoginReward;

		eUserDBType_Friends,    //				szFriends[c_un32FriendsLen]; 

		eUserDBType_UpSSGuideSteps, //SUserGuideSteps		mGuideSteps;
		eUserDBType_ClearSSGuideSteps,  //SUserGuideSteps		mGuideSteps; 

		eUserDBType_UpCSGuideConSteps,  //CSUserGuideConSteps		mGuideSteps;
		eUserDBType_UpCSGuideNoConSteps,    //CSUserGuideNoConSteps		mGuideSteps;


		eUserDBType_HeroList,//		szHeroList[c_un32HeroListLen];
		eUserDBType_BuyGoods,////SUserBuyGoods		mSUserBuyGoods[c_TotalGoods];

		eUserDBType_VIPLevel,
		eUserDBType_VIPScore,

		eUserDBType_TotalAssist,
		eUserDBType_End
	}

	public class SUserHeroDBData
	{
		public readonly uint un32HeroID;
		public readonly long endTime; // time(NULL)
		public readonly long buyTime; //购买时间   time(NULL)

		public SUserHeroDBData( uint idx, long life, long btime )
		{
			this.un32HeroID = idx;
			this.endTime = life;
			this.buyTime = btime;
		}

		public static bool operator ==( SUserHeroDBData a, SUserHeroDBData b )
		{
			if ( a == b )
				return true;
			if ( a == null || b == null )
				return false;
			return a.un32HeroID == b.un32HeroID && a.endTime == b.endTime && a.buyTime == b.buyTime;
		}

		public static bool operator !=( SUserHeroDBData a, SUserHeroDBData b )
		{
			return !( a == b );
		}
		private bool Equals( SUserHeroDBData other )
		{
			return this.un32HeroID == other.un32HeroID && this.endTime == other.endTime && this.buyTime == other.buyTime;
		}

		public override bool Equals( object obj )
		{
			if ( ReferenceEquals( null, obj ) ) return false;
			if ( ReferenceEquals( this, obj ) ) return true;
			if ( obj.GetType() != this.GetType() ) return false;
			return this.Equals( ( SUserHeroDBData )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = ( int )this.un32HeroID;
				hashCode = ( hashCode * 397 ) ^ this.endTime.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.buyTime.GetHashCode();
				return hashCode;
			}
		}
	}

	public class SUserRelationshipInfo
	{
		public string stNickName;
		public ushort nHeadId;
		public ulong guididx;
		public uint viplv;
		public ERelationShip eRelationShip;
		public long tMilSec;

		//构造函数
		public SUserRelationshipInfo( string stName, int headId, ERelationShip ers, long tm, ulong GuidIdx, uint vipLv )
		{
			this.stNickName = stName.Length < Consts.DEFAULT_NICK_NAME_LEN ? stName : string.Empty;
		}
	}

	public class SUserItemInfo
	{
		public uint item_id;
		public bool ifusing;
		public int item_num;
		public ulong buy_time;
		public ulong end_time;

		public void Clear()
		{
			this.item_id = 0;
			this.end_time = 0;
			this.ifusing = false;
			this.item_id = 0;
			this.buy_time = 0;
		}
	}

	public class SUserGuideSteps
	{
		private const string GUIDE_SIGN = ",";

		public string szCSContinueGuide;                       //CS界面引导,记录的都是已经完成的引导(格式:1001,1002,....,ok),ok代表完成该引导
		public bool bSSGuideState;                         //战场引导是否完成(战场引导应完成一个CS的界面引导)
		public bool bIsChange;                             //是否该引导有改变(用于保存)

		public bool ifGuideEnd()
		{
			return -1 != this.szCSContinueGuide.IndexOf( "ok", StringComparison.Ordinal );
		}

		public bool CheckGuideTaskId( int taskId )
		{
			if ( this.ifGuideEnd() )
			{
				return true;
			}
			string szTask = string.Empty + taskId;
			return -1 != this.szCSContinueGuide.IndexOf( szTask, StringComparison.Ordinal );
		}

		public bool UpdateGuideInfo( int taskId, bool b )
		{
			if ( this.CheckGuideTaskId( taskId ) )
				return true;

			this.bIsChange = true;
			this.szCSContinueGuide += GUIDE_SIGN + taskId;
			if ( b ) this.szCSContinueGuide += GUIDE_SIGN + "ok";

			return false;
		}

		public void GetCurtGuideInfo( ref string input, ulong objID )
		{
			if ( !this.bIsChange )
				return;
			input += "update gameuser_guide set obj_cs_guide_com_steps = '" + this.szCSContinueGuide + "'" + " where obj_id = " + objID + ";";
			this.bIsChange = false;
		}
	}

	public class PODUsrDBData
	{
		public ulong un64ObjIdx;//玩家唯一标识
		public EUserPlatform eUserPlatform;
		public ushort un16HeaderID;//玩家头像ID在200内
		public short n16Sex;
		public long n64Diamond;
		public long n64Gold;
		public long tRegisteUTCMillisec; //注册时间(s)
		public long n64Score;
		public uint un32TotalGameInns;
		public uint un32TotalWinInns;
		public uint un32TotalHeroKills;
		public uint un32TotalDestoryBuildings;
		public uint un32TotalDeadTimes;
		public uint un32TotalAssist;

		public byte un8UserLv;//玩家等级，最高30级
		public uint un32UserCurLvExp;
		public ulong tLastFirstWinTime;

		public ushort un16Cldays;
		public uint un32LastGetLoginRewardDay;

		public short un16VipLv;
		public int vipScore;
		public volatile bool[] m_IfChangeArr = new bool[( int )EUserDBDataType.eUserDBType_End];

		public void Copy( PODUsrDBData other )
		{
			this.un64ObjIdx = other.un64ObjIdx;
			this.eUserPlatform = other.eUserPlatform;
			this.un16HeaderID = other.un16HeaderID;
			this.n16Sex = other.n16Sex;
			this.n64Diamond = other.n64Diamond;
			this.n64Gold = other.n64Gold;
			this.tRegisteUTCMillisec = other.tRegisteUTCMillisec;
			this.n64Score = other.n64Score;
			this.un32TotalGameInns = other.un32TotalGameInns;
			this.un32TotalWinInns = other.un32TotalWinInns;
			this.un32TotalHeroKills = other.un32TotalHeroKills;
			this.un32TotalDestoryBuildings = other.un32TotalDestoryBuildings;
			this.un32TotalDeadTimes = other.un32TotalDeadTimes;
			this.un32TotalAssist = other.un32TotalAssist;
			this.un8UserLv = other.un8UserLv;
			this.un32UserCurLvExp = other.un32UserCurLvExp;
			this.tLastFirstWinTime = other.tLastFirstWinTime;
			this.un16Cldays = other.un16Cldays;
			this.un32LastGetLoginRewardDay = other.un32LastGetLoginRewardDay;
			this.un16VipLv = other.un16VipLv;
			this.vipScore = other.vipScore;
		}

		public void Clear()
		{
			this.un64ObjIdx = 0;
			this.eUserPlatform = 0;
			this.un16HeaderID = 0;
			this.n16Sex = 0;
			this.n64Diamond = 0;
			this.n64Gold = 0;
			this.tRegisteUTCMillisec = 0;
			this.n64Score = 0;
			this.un32TotalGameInns = 0;
			this.un32TotalWinInns = 0;
			this.un32TotalHeroKills = 0;
			this.un32TotalDestoryBuildings = 0;
			this.un32TotalDeadTimes = 0;
			this.un32TotalAssist = 0;
			this.un8UserLv = 0;
			this.un32UserCurLvExp = 0;
			this.tLastFirstWinTime = 0;
			this.un16Cldays = 0;
			this.un32LastGetLoginRewardDay = 0;
			this.un16VipLv = 0;
			this.vipScore = 0;
			Array.Clear( this.m_IfChangeArr, 0, this.m_IfChangeArr.Length );
		}
	}

	public class SUserDBData
	{
		public readonly PODUsrDBData sPODUsrDBData = new PODUsrDBData();
		public string szNickName;
		public string szUserName;
		public string szUserPwd;
		public readonly SUserGuideSteps mGuideSteps = new SUserGuideSteps();
		public string szTaskData;
		public bool isTaskRush;
		public Dictionary<uint, SUserHeroDBData> heroListMap = new Dictionary<uint, SUserHeroDBData>();
		public Dictionary<uint, SUserRelationshipInfo> friendListMap = new Dictionary<uint, SUserRelationshipInfo>();
		public Dictionary<uint, SUserRelationshipInfo> blackListMap = new Dictionary<uint, SUserRelationshipInfo>();
		public Dictionary<uint, SUserItemInfo> item_Map = new Dictionary<uint, SUserItemInfo>();

		public SUserDBData()
		{
			this.Clear();
		}

		public void Clear()
		{
			this.sPODUsrDBData.Clear();
			this.isTaskRush = false;
			this.heroListMap.Clear();
			this.friendListMap.Clear();
			this.blackListMap.Clear();
			this.item_Map.Clear();
		}

		public void Copy( SUserDBData sData )
		{
			if ( this == sData )
				return;
			this.sPODUsrDBData.Copy( sData.sPODUsrDBData );
			this.szNickName = sData.szNickName;
			this.szUserName = sData.szUserName;
			this.szUserPwd = sData.szUserPwd;
			this.szTaskData = sData.szTaskData;
			this.isTaskRush = sData.isTaskRush;
			this.heroListMap = new Dictionary<uint, SUserHeroDBData>( sData.heroListMap );
			this.friendListMap = new Dictionary<uint, SUserRelationshipInfo>( sData.friendListMap );
			this.blackListMap = new Dictionary<uint, SUserRelationshipInfo>( sData.blackListMap );
			this.item_Map = new Dictionary<uint, SUserItemInfo>( sData.item_Map );
		}

		public void AddHero( SUserHeroDBData sSUserHeroDBData )
		{
			this.heroListMap.Add( sSUserHeroDBData.un32HeroID, sSUserHeroDBData );
		}

		public void ChangeUserDbData( EUserDBDataType dtype, long param )
		{
			switch ( dtype )
			{
				case EUserDBDataType.eUserDBType_Channel:
					this.sPODUsrDBData.eUserPlatform = ( EUserPlatform )param;
					break;
				case EUserDBDataType.eUserDBType_HeaderId:
					this.sPODUsrDBData.un16HeaderID = ( ushort )param;
					break;
				case EUserDBDataType.eUserDBType_Sex:
					this.sPODUsrDBData.n16Sex = ( short )param;
					break;
				case EUserDBDataType.eUserDBType_RegisterTime:
					this.sPODUsrDBData.tRegisteUTCMillisec = param;
					break;
				case EUserDBDataType.eUserDBType_Score:
					this.sPODUsrDBData.n64Score = param;
					break;
				case EUserDBDataType.eUserDBType_LastFirstWinTime:
					this.sPODUsrDBData.tLastFirstWinTime = ( ulong )param;
					break;
				case EUserDBDataType.eUserDBType_CLDay:
					this.sPODUsrDBData.un16Cldays = ( ushort )param;
					break;
				case EUserDBDataType.eUserDBType_Diamond:
					this.sPODUsrDBData.n64Diamond += param;
					break;
				case EUserDBDataType.eUserDBType_Gold:
					this.sPODUsrDBData.n64Gold += param;
					break;
				case EUserDBDataType.eUserDBType_TotalGameInns:
					this.sPODUsrDBData.un32TotalGameInns += ( uint )param;
					break;
				case EUserDBDataType.eUserDBType_TotalWinInns:
					this.sPODUsrDBData.un32TotalWinInns += ( uint )param;
					break;
				case EUserDBDataType.eUserDBType_TotalHeroKills:
					this.sPODUsrDBData.un32TotalHeroKills += ( uint )param;
					break;
				case EUserDBDataType.eUserDBType_TotalDestoryBuild:
					this.sPODUsrDBData.un32TotalDestoryBuildings += ( uint )param;
					break;
				case EUserDBDataType.eUserDBType_TotalDeadTimes:
					this.sPODUsrDBData.un32TotalDeadTimes += ( uint )param;
					break;
				case EUserDBDataType.eUserDBType_UserLv:
					this.sPODUsrDBData.un8UserLv += ( byte )param;
					break;
				case EUserDBDataType.eUserDBType_UserLvExp:
					this.sPODUsrDBData.un32UserCurLvExp += ( uint )param;
					break;
				case EUserDBDataType.eUserDBType_VIPLevel:
					this.sPODUsrDBData.un16VipLv = ( short )param;
					break;
				case EUserDBDataType.eUserDBType_VIPScore:
					this.sPODUsrDBData.vipScore = ( int )param;
					break;
				case EUserDBDataType.eUserDBType_TotalAssist:
					this.sPODUsrDBData.un32TotalAssist += ( uint )param;
					break;
				case EUserDBDataType.eUserDBType_LastGetLoginReward:
					this.sPODUsrDBData.un32LastGetLoginRewardDay = ( uint )param;
					break;
			}
			this.sPODUsrDBData.m_IfChangeArr[( int )dtype] = true;
		}

		public void ChangeUserDbData( EUserDBDataType dtype, string param )
		{
			switch ( dtype )
			{
				case EUserDBDataType.eUserDBType_Name:
					this.szUserName = param;
					break;
				case EUserDBDataType.eUserDBType_Pwd:
					this.szUserPwd = param;
					break;
				case EUserDBDataType.eUserDBType_NickName:
					this.szNickName = param;
					break;
			}
			this.sPODUsrDBData.m_IfChangeArr[( int )dtype] = true;
		}
	}
}