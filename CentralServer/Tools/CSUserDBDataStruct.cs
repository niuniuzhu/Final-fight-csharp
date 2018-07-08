using Core.Misc;
using ProtoBuf;
using Shared;
using System;
using System.Collections.Generic;

namespace CentralServer.Tools
{
	public enum UserDBDataType
	{
		None = 0,//			un64ObjIdx; 
		Channel,//		userPlatform;
		Name,//		szUserName[c_n32DefaultNameLen];
		Pwd,//			szUserPwd[c_n32DefaultUserPwdLen]; 
		NickName,//				szNickName[c_n32DefaultNickNameLen+1];
		HeaderId,//		un32HeaderID;
		Sex,//				n8Sex; 
		Diamond,//			n64Diamond;
		Gold,//			n64Gold;;
		RegisterTime,//			tRegisteUTCMillisec;	//注册时间(s)
		Score,//				n64Score;
		TotalGameInns,//			un32TotalGameInns;
		TotalWinInns,//		un32TotalWinInns;
		TotalHeroKills,//				un32TotalHeroKills;
		TotalDestoryBuild,//				un32TotalDestoryBuildings;
		TotalDeadTimes,//				un32TotalDeadTimes;

		UserLv,//				un16UserLv;
		UserLvExp,//				un32UserCurLvExp;
		LastFirstWinTime,//			tLastFirstWinTime; 

		CLDay,//				un16Cldays;
		LastGetLoginReward,//			tLastGetLoginReward;

		Friends,    //				szFriends[c_un32FriendsLen]; 

		UpSSGuideSteps, //UserGuideSteps		guideSteps;
		ClearSSGuideSteps,  //UserGuideSteps		guideSteps; 

		UpCSGuideConSteps,  //CSUserGuideConSteps		guideSteps;
		UpCSGuideNoConSteps,    //CSUserGuideNoConSteps		guideSteps;

		HeroList,//		szHeroList[c_un32HeroListLen];
		BuyGoods,////SUserBuyGoods		mSUserBuyGoods[c_TotalGoods];

		VIPLevel,
		VIPScore,

		TotalAssist,
		End
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

		public UserRelationshipInfo()
		{
		}

		public UserRelationshipInfo( string stName, int headId, RelationShip ers, long tm, ulong GuidIdx, uint vipLv ) =>
			this.stNickName = stName.Length < Consts.DEFAULT_NICK_NAME_LEN ? stName : string.Empty;
	}

	[ProtoContract]
	public class UserItemInfo
	{
		[ProtoMember( 1 )]
		public uint itemID;
		[ProtoMember( 2 )]
		public bool ifusing;
		[ProtoMember( 3 )]
		public int itemNum;
		[ProtoMember( 4 )]
		public ulong buyTime;
		[ProtoMember( 5 )]
		public ulong endTime;

		public void Clear()
		{
			this.itemID = 0;
			this.endTime = 0;
			this.ifusing = false;
			this.itemID = 0;
			this.buyTime = 0;
		}
	}

	[ProtoContract]
	public class UserGuideSteps
	{
		private const string GUIDE_SIGN = ",";

		[ProtoMember( 1 )]
		public string szCSContinueGuide;    //CS界面引导,记录的都是已经完成的引导(格式:1001,1002,....,ok),ok代表完成该引导
		[ProtoMember( 2 )]
		public bool bSSGuideState;          //战场引导是否完成(战场引导应完成一个CS的界面引导)
		[ProtoMember( 3 )]
		public bool bIsChange;              //是否该引导有改变(用于保存)

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
		public volatile bool[] _ifChangeArr = new bool[( int )UserDBDataType.End];

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

		public void ChangeUserDbData( UserDBDataType type, object param )
		{
			switch ( type )
			{
				case UserDBDataType.Name:
					this.szUserName = ( string )param;
					break;
				case UserDBDataType.Pwd:
					this.szUserPwd = ( string )param;
					break;
				case UserDBDataType.NickName:
					this.szNickName = ( string )param;
					break;
				case UserDBDataType.Channel:
					this.usrDBData.userPlatform = ( UserPlatform )param;
					break;
				case UserDBDataType.HeaderId:
					this.usrDBData.un16HeaderID = Convert.ToUInt16( param );
					break;
				case UserDBDataType.Sex:
					this.usrDBData.n16Sex = Convert.ToInt16( param );
					break;
				case UserDBDataType.RegisterTime:
					this.usrDBData.tRegisteUTCMillisec = ( long )param;
					break;
				case UserDBDataType.Score:
					this.usrDBData.n64Score = ( long )param;
					break;
				case UserDBDataType.LastFirstWinTime:
					this.usrDBData.tLastFirstWinTime = ( ulong )param;
					break;
				case UserDBDataType.CLDay:
					this.usrDBData.un16Cldays = Convert.ToUInt16( param );
					break;
				case UserDBDataType.Diamond:
					this.usrDBData.n64Diamond += ( long )param;
					break;
				case UserDBDataType.Gold:
					this.usrDBData.n64Gold += ( long )param;
					break;
				case UserDBDataType.TotalGameInns:
					this.usrDBData.un32TotalGameInns += ( uint )param;
					break;
				case UserDBDataType.TotalWinInns:
					this.usrDBData.un32TotalWinInns += ( uint )param;
					break;
				case UserDBDataType.TotalHeroKills:
					this.usrDBData.un32TotalHeroKills += ( uint )param;
					break;
				case UserDBDataType.TotalDestoryBuild:
					this.usrDBData.un32TotalDestoryBuildings += ( uint )param;
					break;
				case UserDBDataType.TotalDeadTimes:
					this.usrDBData.un32TotalDeadTimes += ( uint )param;
					break;
				case UserDBDataType.UserLv:
					this.usrDBData.un8UserLv += ( byte )param;
					break;
				case UserDBDataType.UserLvExp:
					this.usrDBData.un32UserCurLvExp += ( uint )param;
					break;
				case UserDBDataType.VIPLevel:
					this.usrDBData.un16VipLv = ( short )param;
					break;
				case UserDBDataType.VIPScore:
					this.usrDBData.vipScore = ( int )param;
					break;
				case UserDBDataType.TotalAssist:
					this.usrDBData.un32TotalAssist += ( uint )param;
					break;
				case UserDBDataType.LastGetLoginReward:
					this.usrDBData.un32LastGetLoginRewardDay = ( int )param;
					break;
			}
			this.usrDBData._ifChangeArr[( int )type] = true;
		}
	}

	public struct ConsumeStruct
	{
		public readonly ConsumeType type;
		public readonly int price;

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
		public readonly List<ConsumeStruct> consumeList = new List<ConsumeStruct>();
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