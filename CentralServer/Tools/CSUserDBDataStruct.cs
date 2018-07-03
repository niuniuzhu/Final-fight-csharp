using Core.Misc;
using ProtoBuf;
using Shared;
using System;
using System.Collections.Generic;

namespace CentralServer.Tools
{
	public enum UserDBDataType
	{
		UserDBType_None = 0,//			un64ObjIdx; 
		UserDBType_Channel,//		userPlatform;
		UserDBType_Name,//		szUserName[c_n32DefaultNameLen];
		UserDBType_Pwd,//			szUserPwd[c_n32DefaultUserPwdLen]; 
		UserDBType_NickName,//				szNickName[c_n32DefaultNickNameLen+1];
		UserDBType_HeaderId,//		un32HeaderID;
		UserDBType_Sex,//				n8Sex; 
		UserDBType_Diamond,//			n64Diamond;
		UserDBType_Gold,//			n64Gold;;
		UserDBType_RegisterTime,//			tRegisteUTCMillisec;	//注册时间(s)
		UserDBType_Score,//				n64Score;
		UserDBType_TotalGameInns,//			un32TotalGameInns;
		UserDBType_TotalWinInns,//		un32TotalWinInns;
		UserDBType_TotalHeroKills,//				un32TotalHeroKills;
		UserDBType_TotalDestoryBuild,//				un32TotalDestoryBuildings;
		UserDBType_TotalDeadTimes,//				un32TotalDeadTimes;

		UserDBType_UserLv,//				un16UserLv;
		UserDBType_UserLvExp,//				un32UserCurLvExp;
		UserDBType_LastFirstWinTime,//			tLastFirstWinTime; 

		UserDBType_CLDay,//				un16Cldays;
		UserDBType_LastGetLoginReward,//			tLastGetLoginReward;

		UserDBType_Friends,    //				szFriends[c_un32FriendsLen]; 

		UserDBType_UpSSGuideSteps, //UserGuideSteps		guideSteps;
		UserDBType_ClearSSGuideSteps,  //UserGuideSteps		guideSteps; 

		UserDBType_UpCSGuideConSteps,  //CSUserGuideConSteps		guideSteps;
		UserDBType_UpCSGuideNoConSteps,    //CSUserGuideNoConSteps		guideSteps;


		UserDBType_HeroList,//		szHeroList[c_un32HeroListLen];
		UserDBType_BuyGoods,////SUserBuyGoods		mSUserBuyGoods[c_TotalGoods];

		UserDBType_VIPLevel,
		UserDBType_VIPScore,

		UserDBType_TotalAssist,
		UserDBType_End
	}

	public class UserDbSaveConfig
	{
		public int dbSaveTimeSpace;
		public int delayDelFromCacheTime;

		public bool dbCSLogSwitch;
		public bool dbSSLogSwitch;
		public int logRefreshTime;

		public bool sSGMCmdSwitch;
		public bool ssSolderBornSwitch;
		public bool ssMonsterBornSwitch;
		public bool ssAltarSolderSwitch;

		public int m_MaxUserMailN;
		public bool m_IfDelAtLooked;
		public bool m_ifSortAtLooked;

		void Clear()
		{
			this.dbSaveTimeSpace = 0;
			this.delayDelFromCacheTime = 0;
			this.dbCSLogSwitch = false;
			this.dbSSLogSwitch = false;
			this.logRefreshTime = 0;
			this.sSGMCmdSwitch = false;
			this.ssSolderBornSwitch = false;
			this.ssMonsterBornSwitch = false;
			this.ssAltarSolderSwitch = false;
			this.m_MaxUserMailN = 0;
			this.m_IfDelAtLooked = false;
			this.m_ifSortAtLooked = false;
		}
	}

	[ProtoContract]
	public class UserHeroDBData
	{
		[ProtoMember( 1 )]
		public readonly uint un32HeroID;
		[ProtoMember( 2 )]
		public readonly long endTime;
		[ProtoMember( 3 )]
		public readonly long buyTime;

		public UserHeroDBData( uint idx, long life, long btime )
		{
			this.un32HeroID = idx;
			this.endTime = life;
			this.buyTime = btime;
		}

		public static bool operator ==( UserHeroDBData a, UserHeroDBData b )
		{
			if ( a == b )
				return true;
			if ( a == null || b == null )
				return false;
			return a.un32HeroID == b.un32HeroID && a.endTime == b.endTime && a.buyTime == b.buyTime;
		}

		public static bool operator !=( UserHeroDBData a, UserHeroDBData b )
		{
			return !( a == b );
		}
		private bool Equals( UserHeroDBData other )
		{
			return this.un32HeroID == other.un32HeroID && this.endTime == other.endTime && this.buyTime == other.buyTime;
		}

		public override bool Equals( object obj )
		{
			if ( ReferenceEquals( null, obj ) ) return false;
			if ( ReferenceEquals( this, obj ) ) return true;
			if ( obj.GetType() != this.GetType() ) return false;
			return this.Equals( ( UserHeroDBData )obj );
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

	[ProtoContract]
	public class UserRelationshipInfo
	{
		[ProtoMember( 1 )]
		public string stNickName;
		[ProtoMember( 2 )]
		public ushort nHeadId;
		[ProtoMember( 3 )]
		public ulong guididx;
		[ProtoMember( 4 )]
		public uint viplv;
		[ProtoMember( 5 )]
		public RelationShip relationShip;
		[ProtoMember( 6 )]
		public long tMilSec;

		//构造函数
		public UserRelationshipInfo( string stName, int headId, RelationShip ers, long tm, ulong GuidIdx, uint vipLv )
		{
			this.stNickName = stName.Length < Consts.DEFAULT_NICK_NAME_LEN ? stName : string.Empty;
		}
	}

	[ProtoContract]
	public class UserItemInfo
	{
		[ProtoMember( 1 )]
		public uint item_id;
		[ProtoMember( 2 )]
		public bool ifusing;
		[ProtoMember( 3 )]
		public int item_num;
		[ProtoMember( 4 )]
		public ulong buy_time;
		[ProtoMember( 5 )]
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

	[ProtoContract]
	public class UserGuideSteps
	{
		private const string GUIDE_SIGN = ",";

		[ProtoMember( 1 )]
		public string szCSContinueGuide;                       //CS界面引导,记录的都是已经完成的引导(格式:1001,1002,....,ok),ok代表完成该引导
		[ProtoMember( 2 )]
		public bool bSSGuideState;                         //战场引导是否完成(战场引导应完成一个CS的界面引导)
		[ProtoMember( 3 )]
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

	[ProtoContract]
	public class PODUsrDBData
	{
		[ProtoMember( 1 )]
		public ulong un64ObjIdx;//玩家唯一标识
		[ProtoMember( 2 )]
		public UserPlatform userPlatform;
		[ProtoMember( 3 )]
		public ushort un16HeaderID;//玩家头像ID在200内
		[ProtoMember( 4 )]
		public short n16Sex;
		[ProtoMember( 5 )]
		public long n64Diamond;
		[ProtoMember( 6 )]
		public long n64Gold;
		[ProtoMember( 7 )]
		public long tRegisteUTCMillisec; //注册时间(s)
		[ProtoMember( 8 )]
		public long n64Score;
		[ProtoMember( 9 )]
		public uint un32TotalGameInns;
		[ProtoMember( 10 )]
		public uint un32TotalWinInns;
		[ProtoMember( 11 )]
		public uint un32TotalHeroKills;
		[ProtoMember( 12 )]
		public uint un32TotalDestoryBuildings;
		[ProtoMember( 13 )]
		public uint un32TotalDeadTimes;
		[ProtoMember( 14 )]
		public uint un32TotalAssist;

		[ProtoMember( 15 )]
		public byte un8UserLv;//玩家等级，最高30级
		[ProtoMember( 16 )]
		public uint un32UserCurLvExp;
		[ProtoMember( 17 )]
		public ulong tLastFirstWinTime;

		[ProtoMember( 18 )]
		public ushort un16Cldays;
		[ProtoMember( 19 )]
		public int un32LastGetLoginRewardDay;

		[ProtoMember( 20 )]
		public short un16VipLv;
		[ProtoMember( 21 )]
		public int vipScore;
		[ProtoMember( 22 )]
		public volatile bool[] _ifChangeArr = new bool[( int )UserDBDataType.UserDBType_End];

		public void Copy( PODUsrDBData other )
		{
			this.un64ObjIdx = other.un64ObjIdx;
			this.userPlatform = other.userPlatform;
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
			this.userPlatform = 0;
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
			Array.Clear( this._ifChangeArr, 0, this._ifChangeArr.Length );
		}
	}

	[ProtoContract]
	public class UserDBData
	{
		[ProtoMember( 1 )]
		public string szNickName = string.Empty;
		[ProtoMember( 2 )]
		public string szUserName = string.Empty;
		[ProtoMember( 3 )]
		public string szUserPwd = string.Empty;
		[ProtoMember( 4 )]
		public string szTaskData = string.Empty;
		[ProtoMember( 5 )]
		public bool isTaskRush;
		[ProtoMember( 6 )]
		public readonly PODUsrDBData usrDBData = new PODUsrDBData();
		[ProtoMember( 7 )]
		public readonly UserGuideSteps guideSteps = new UserGuideSteps();
		[ProtoMember( 8 )]
		public readonly Dictionary<uint, UserHeroDBData> heroListMap = new Dictionary<uint, UserHeroDBData>();
		[ProtoMember( 9 )]
		public readonly Dictionary<ulong, UserRelationshipInfo> friendListMap = new Dictionary<ulong, UserRelationshipInfo>();
		[ProtoMember( 10 )]
		public readonly Dictionary<ulong, UserRelationshipInfo> blackListMap = new Dictionary<ulong, UserRelationshipInfo>();
		[ProtoMember( 11 )]
		public readonly Dictionary<uint, UserItemInfo> item_Map = new Dictionary<uint, UserItemInfo>();

		public UserDBData()
		{
			this.Clear();
		}

		private void Clear()
		{
			this.usrDBData.Clear();
			this.isTaskRush = false;
			this.heroListMap.Clear();
			this.friendListMap.Clear();
			this.blackListMap.Clear();
			this.item_Map.Clear();
		}

		public UserDBData Clone()
		{
			UserDBData cloned = new UserDBData();
			cloned.Copy( this );
			return cloned;
		}

		private void Copy( UserDBData data )
		{
			if ( this == data )
				return;
			this.usrDBData.Copy( data.usrDBData );
			this.szNickName = data.szNickName;
			this.szUserName = data.szUserName;
			this.szUserPwd = data.szUserPwd;
			this.szTaskData = data.szTaskData;
			this.isTaskRush = data.isTaskRush;
			this.heroListMap.Copy( data.heroListMap );
			this.friendListMap.Copy( data.friendListMap );
			this.blackListMap.Copy( data.blackListMap );
			this.item_Map.Copy( data.item_Map );
		}

		public void AddHero( UserHeroDBData userHeroDbData )
		{
			this.heroListMap.Add( userHeroDbData.un32HeroID, userHeroDbData );
		}

		public void ChangeUserDbData( UserDBDataType dtype, long param )
		{
			switch ( dtype )
			{
				case UserDBDataType.UserDBType_Channel:
					this.usrDBData.userPlatform = ( UserPlatform )param;
					break;
				case UserDBDataType.UserDBType_HeaderId:
					this.usrDBData.un16HeaderID = ( ushort )param;
					break;
				case UserDBDataType.UserDBType_Sex:
					this.usrDBData.n16Sex = ( short )param;
					break;
				case UserDBDataType.UserDBType_RegisterTime:
					this.usrDBData.tRegisteUTCMillisec = param;
					break;
				case UserDBDataType.UserDBType_Score:
					this.usrDBData.n64Score = param;
					break;
				case UserDBDataType.UserDBType_LastFirstWinTime:
					this.usrDBData.tLastFirstWinTime = ( ulong )param;
					break;
				case UserDBDataType.UserDBType_CLDay:
					this.usrDBData.un16Cldays = ( ushort )param;
					break;
				case UserDBDataType.UserDBType_Diamond:
					this.usrDBData.n64Diamond += param;
					break;
				case UserDBDataType.UserDBType_Gold:
					this.usrDBData.n64Gold += param;
					break;
				case UserDBDataType.UserDBType_TotalGameInns:
					this.usrDBData.un32TotalGameInns += ( uint )param;
					break;
				case UserDBDataType.UserDBType_TotalWinInns:
					this.usrDBData.un32TotalWinInns += ( uint )param;
					break;
				case UserDBDataType.UserDBType_TotalHeroKills:
					this.usrDBData.un32TotalHeroKills += ( uint )param;
					break;
				case UserDBDataType.UserDBType_TotalDestoryBuild:
					this.usrDBData.un32TotalDestoryBuildings += ( uint )param;
					break;
				case UserDBDataType.UserDBType_TotalDeadTimes:
					this.usrDBData.un32TotalDeadTimes += ( uint )param;
					break;
				case UserDBDataType.UserDBType_UserLv:
					this.usrDBData.un8UserLv += ( byte )param;
					break;
				case UserDBDataType.UserDBType_UserLvExp:
					this.usrDBData.un32UserCurLvExp += ( uint )param;
					break;
				case UserDBDataType.UserDBType_VIPLevel:
					this.usrDBData.un16VipLv = ( short )param;
					break;
				case UserDBDataType.UserDBType_VIPScore:
					this.usrDBData.vipScore = ( int )param;
					break;
				case UserDBDataType.UserDBType_TotalAssist:
					this.usrDBData.un32TotalAssist += ( uint )param;
					break;
				case UserDBDataType.UserDBType_LastGetLoginReward:
					this.usrDBData.un32LastGetLoginRewardDay = ( int )param;
					break;
			}
			this.usrDBData._ifChangeArr[( int )dtype] = true;
		}

		public void ChangeUserDbData( UserDBDataType dtype, string param )
		{
			switch ( dtype )
			{
				case UserDBDataType.UserDBType_Name:
					this.szUserName = param;
					break;
				case UserDBDataType.UserDBType_Pwd:
					this.szUserPwd = param;
					break;
				case UserDBDataType.UserDBType_NickName:
					this.szNickName = param;
					break;
			}
			this.usrDBData._ifChangeArr[( int )dtype] = true;
		}
	}

	public struct ConsumeStruct
	{
		public ConsumeType type;
		public int price;

		public ConsumeStruct( ConsumeType type, int price )
		{
			this.type = type;
			this.price = price;
		}
	}

	public class RunesCfg
	{
		public ObjectType eOT;
		public byte un8Level;
		public byte un8EffectID;
		public float fEffectVal;
		public float fEffectPer;
		public bool bIsCanComposed;
		public int n32ComposedSubID;
		public List<ConsumeStruct> sConsumeList = new List<ConsumeStruct>();
		public bool bIfShowInShop;
	}

	public class DiscountCfg
	{
		public uint un32CommdityID;
		public GoodsType eGoodsType;
		public uint un32GoodsID;
		public List<ConsumeStruct> sConsumeList = new List<ConsumeStruct>();
	}

	public class LoginReward
	{
		public const int MAX_REWARD = 5;
		public int n32Days;
		public RewardType[] eRewardType;
		public LoginRewardItemType[] eItemType;
		public uint[] un32num;

		public LoginReward()
		{
			this.eRewardType = new RewardType[MAX_REWARD];
			this.eItemType = new LoginRewardItemType[MAX_REWARD];
			this.un32num = new uint[MAX_REWARD];
		}
	}

	public class HeroBuyCfg
	{
		public uint un32CommondityID;
		public uint un32HeroID;
		public readonly HeroKind[] eHeroKind = new HeroKind[3];
		public bool bIsShowHot;
		public SkinType eDefaultSkin;
		public SkinType eOnSaleSkins;
		public long useTimeSpan;
		public readonly List<ConsumeStruct> sConsumeList = new List<ConsumeStruct>();
		public bool bIfShowInShop;
	}

	public class HeroListStruct
	{
		public uint heroid;
		public long expiredTime;
		public bool ifFree;

		public HeroListStruct( uint heroid, long expiredTime, bool ifFree )
		{
			this.heroid = heroid;
			this.expiredTime = expiredTime;
			this.ifFree = ifFree;
		}
	}
}