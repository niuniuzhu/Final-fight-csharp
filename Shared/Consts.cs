using System.Net.Sockets;

namespace Shared
{
	public enum ServerNetState
	{
		Closed = 0,
		Connecting,
		Free,
		Busy,
		Full
	}

	public enum ErrorCode
	{
		Success = 0,
		Begin = -0x00010000 * 2,
		InvalidModelStatus, //非法的模块状态
		InvalidModelID, //非法的模块ID
		InvalidGSID, //非法的GSID
		InvalidSSID, //非法的SSID
		NullDataSource, //找不到数据源
		NullObjMgr, //找不到ObjMgr
		NullDataMgr, //找不到DataMgr
		NullWorldMgr, //找不到WorldMgr
		NullSceneServer, //找不到SceneServer
		NullGateServer, //找不到GateServer
		NullCentralServer, //找不到CentralServer
		InvalidUserPwd, //非法的用户口令
		InvalidPosition, //非法的位置
		InvalidRegionID, //非法的区域ID
		InvalidAreaID, //非法的区域ID
		NetSessionCollide, //网络会话冲突
		NullUser, //找不到用户
		UserExist, //用户已经存在
		GameObjectAlreadyExist, //游戏对象已经存在
		NullGameUnit, //找不到游戏单元
		LoadLibarayFail, //加载模块失败
		OpenCfgFileFail, //找开配置文件失败
		NoneNetListener, //没有网络监听器
		NoneNetConnector, //没有网络连接器
		OpenConnectorFail, //打开网络连接器失败
		TooManyConnectorOpened, //打开了太多的连接
		InitModelTimeOut, //初始化模块失败
		StartModelTimeOut, //启动模块超时
		NullMasterUser, //找不到主用户
		InvalidAttackTarget, //非法的技能目标
		NetConnectionClosed, //网络连接已经关闭
		NetProtocalDataError, //网络协议数据错误
		TooManySSNum, //SS数量过多
		TooManyGSNum, //GS数量过多
		InvalidNetState, //非法的网络状态
		JustInBattle, //当前正在战役中
		InvalidBattlePos, //非法的战役座位
		TheBattleUserFull, //战役用户已满
		UserDonotInTheBattle, //用户不在战役之中
		JustNotInBattle, //当前不在战役之中
		InvalidBattleState, //非法的战役状态
		InvalidBattleID, //非法的战役ID
		HeroExist, //英雄已经存在
		InvalidUserNetInfo, //非法的用户网络信息
		BattleExist, //战役已经存在
		BattleDonotExist, //战役不存在
		MapLoaded, //地图已经加载
		BattleLocalObjIdxOverflow, //战役本地对象溢出
		CannotFindoutTheObject, //找不到对象
		NullHero, //找不到英雄
		NotAllUserReady, //不是所有用户已经准备好
		YouAreNotBattleManager, //你不是战役管理员
		InvalidOrderCate, //非法的Order类别
		InvalidGameObjectCamp, //非法的游戏对象阵营
		CreateOrderFail, //创建Order失败
		InvalidOrderState, //非法的Order状态
		OrderNoMasterGO, //Order没有主对象
		AbsentOrderPriority, //Order优先级不够
		NullBattle, //找不到战役
		MoveBlocked, //移动被阻挡
		ReadCfgFileFail, //读取配置文件失败
		TooManyNPCCfgNum, //NPC配置数量太多
		TooManyHeroCfgNum, //英雄配置数量太多
		TooManMapDataCfgNum, //地图配置数量太多
		InvalidHeroSeat, //非法的英雄座位
		InvalidMapID, //非法的地图ID
		InvalidPos, //非法的位置
		InvalidCellID, //非法的单元格ID
		NullArea, //非法的区域
		InvalidScriptParameters, //非法的脚本参数
		InvalidObjTypeID, //非法的对象类型ID
		InvalidPathNodeNum, //非法的路径节点数量
		InvalidVector3D, //非法的3D向量
		UserInfoUnComplete, //用户信息不全
		GenerateGUIDFail, //创建GUID失败
		NotEnemy, //没有敌人
		NotNormalAttackSkill, //没有普通攻击技能
		CanNotFindTheSkill, //找不到指定技能ID
		NotOrderInfo, //没有Order信息
		InvalidSkillID, //非法的技能ID
		InvalidSkillState, //非法的技能状态
		InvalidSkillTarget, //非法的技能目标
		NullSkillData, //没有技能数据
		InvalidGameObjectCate, //非法的游戏对象类型
		HasChoosedHero, //已经选择了英雄
		OthersHasChoosedHero, //
		HasNoTheHero,
		AbsentAttackDistance, //攻击距离不足
		TargetIsDead, //目标已经死亡
		InvalidTargetActionState, //目标行为状态非法
		InvalidBattle, //非法的战役
		TheBattlIsFull,  //战役已经满员
		BattlePDWNotMatch,
		NullGameObject,//找不到游戏对象
		NULLNickName, //找不到昵称
		TimeExpire, //时间超时
		NoneAvailbleBuff, //没有有效BUFF
		BuffOverlapUpperLimit, //超过BUFF上限
		InvalidCurHPNum, //非法的当前HP
		InvalidCurMPNum, //非法的当前MP
		NoPathNode, //找不到路径节点
		InvalidBuffTypeID, //非法的缓冲类型ID
		AbsentSkillDistance, //技能距离不足
		BuildingCanNotMove, //建筑不能移动
		AbsentCP, //CP不足
		AbsentMP, //MP不足
		AbsentHP, //HP不足
		BuildingCanNotAddBuff, //建筑不能添加BUFF
		InvalidActionState, //非法的行为状态
		JustSkillAction,  //正在使用技能
		AbsorbMonsterFailForLackLevel,
		AbsorbMonsterFailForHasFullAbsorb,
		AbsorbMonsterFailForHasSameSkillID,
		AbsorbMonsterFailForLackCP,
		AbsorbMonsterFailForMonsterDead,
		AbsorbMonsterFailForMonsterCannotBeConstrol,
		AbsorbMonsterFailForHeroDead,
		AbsorbMonsterFailForNotMonster,
		AbsorbMonsterFailForDiffNPC,
		AbsorbMonsterFailForErrorState,
		AbsorbMonsterFailForDizziness,
		NULLNPC, //找不到NPC
		NULLCfg, //找不到配置信息
		InvaildSkillID, //非法的技能ID
		RemoveAbsorbSkillFailed, //移除吸附技能失败
		StateCanNotUseGas,
		ExistWildMonsterBornPos,
		InvalidControlNPCType,
		GasExplosionNotFull,
		MultiAbsortNotAllowed,
		StaticBlock, //静态阻挡
		DynamicBlock, //动态阻挡
		NoDistanceToMove, //没有距离需要移动
		CannotFindFullPathNode, //找不到完整的路径
		HeroNotDead, //英雄没有死亡
		NotEnoughGold, //金币不足
		NoRebornTimes,
		BattleIsPlaying,
		RemoveBuffFailed,
		DeadAltar,
		InvaildCampID,
		NotInSameBattle,
		AskBuyRunesFail,
		AskComposeRunesFail,
		AskUnUseRunesFail,
		AskUseRunesFail,
		AskMoveGoodsFail,
		AskSellGoodsFail,
		InvaildGridID,
		AskUseGoodsFailForCoolDown,
		rInvalidGoodsNum,
		AskBuyGoodsFailForLackCP,
		AskBuyGoodsFailForInvalidCPType,
		AskBuyGoodsFailForHasSameTypeID,
		AskBuyGoodsFailForHasFunType,
		AskBuyGoodsFailForBagFull,
		CannotCreateVoipServer,
		AttackOneObj,
		ExistObj,
		TheSkillEnd,
		ErrorSkillId,
		InvalidMastType,
		NickNameCollision, //昵称冲突
		ObjectAlreadyExist,
		ForbitAbWMSolder, //不能吸附野怪兵
		TargetCannotLooked,//目标不能锁定
		ErrorAreaId,//错误的areaId
		NoWatchUser,//没有观察者
		MaxBornSolder,
		AddBattleFailForLackOfGold,
		CampNotBalance,
		AskBuyGoodsFailForSole,

		TimeOut,
		AddEffectFailed,
		EffectEnd,

		UseSkillFailForSilenced,
		UseSkillFailForDisarmed,
		UseSkillFailForLackHP,
		UseSkillFailForLackMP,
		UseSkillFailForLackCP,
		UseSkillFailForSkillCoolDown,
		UseSkillFailForNULLTarget,
		UseSkillFailForBuildingNullity,

		UseGoodsFailForDizziness,
		UseGoodsFailForBuildingNullity,
		UseGoodsFailForNULLTarget,
		UseGoodslFailForSilenced,
		UseGoodslFailForErrorCamp,
		UseSkillGasNotInRunState,
		UseSkillGasHasInRunState,
		NoAbsorbSkill,

		UseSkillFailForDizziness,
		AskBuyGoodsFailForLackTeamCP,
		CanntAbsorb,
		SkillPrepareFailed,
		CancelSkillOrderFailed,

		TheBattleObserverNotFull,
		TheBattleObserverFull,
		BeginBattleFailForNullPlayer,
		AddBattleFailForAllFull,
		AddBattleFailForUserFull,
		WarningToSelectHero,
		GuideNotOn,
		HasCompGuideStep,
		InvalidStepId,
		DelAbsorbICOFailed,
		AbsorbMonsterFail,
		ZeroGUID,
		NoObjList,

		JustInThatSeatPos,
		NickNameNotAllowed,
		InvalidTarget,  //错误的对象:箭塔
		GUDead,
		TooManyUserInBattle,
		NoAtkObj,
		InvalidMapInfo,
		InvalidMapId,
		NullLuaCfg,
		NullMapCfg,
		LoadFilterCfgFailed,
		RemoveEffectFailed,
		UserNotExist,
		BattleFinished,

		UserWasInFriendList,
		UserWasInBlackList,
		UserNotOnline,
		MsgTooLarge,
		UserRefuseReceiveYourMsg,
		UserOfflineFull,

		HaveBuySameGoods,
		BuyGoodsFailedLackGold,
		BuyGoodsFailedLackDiamond,
		ExistGuidCfg,
		NullInfo,
		UserRSExist,
		ErrorGuideStepId,
		invalidObjId,
		NothingContent,
		DoneDBWrong,
		AskTooFrequently,
		UserOfflineMsgFull,
		InvalidAbsorbTar,
		BeginAIFailed,
		CanNotUseChinese,

		TipsObjAppear,
		TipsNPCBorn,
		TipsSuperNPCBorn,
		ErrorType,
		FriendsListFull,
		BlackListFull,
		JustInFriendsList,
		JustInBlackList,
		NullUserRSInfo,
		UserBusy,
		YouInOppositeBlackList,
		CounterpartFriendListFull,
		UserInYourBlackList,
		AskHaveSend,
		existBattle,
		CannotBuygoodsWhenDeath,
		TimeToSaveDB,

		E91LoginFail,
		E91InvalidAppID,
		E91InvalidAct,
		E91InvalidPara,
		E91InvalidSign,
		E91InvalidSessionID,

		UserNotHaveHero,
		CannotSellgoodsWhenDeath,
		PPUserNameRuleWrong,
		PPUserNotExist,
		PPInvalidAct,
		PPUserExisted,
		PPPwdCheckError,
		PPUserProhibited,
		PPDataError,
		PPSessionTimeout,
		PPUserHaveBinding,

		TBInvalidToken,
		TBInvalidFormat,

		FunClosed,
		BattleClosing,  //用于重连
		InvalidPwdLength,
		PleaseEnterPwd,
		InvalidUserNameLegth,

		NullPointer,
		InvalidMsgProtocalID,
		NullMsgHandler,
		InvalidGUID,
		NullMsg,
		InvalidNSID,
		GUIDCollision,
		InvalidUserName,
		AddUserNameFailed,
		InvalidMailId,
		UserOfflineOrNullUser,
		UserWasPlaying,
		RequestSended,
		AddFriendSeccuse,
		OppositeSideFriendFull,
		ReEnterRoomFail,//重进房间失败
		DiamondNotEnough,
		ParseProtoError,//解析PB错误
		UnKnownError,//未知错误//
		ErrorTimes,
		MatchLinkInvalid,//匹配链接已失效//
		AddMatchTeamError,//加入匹配队伍失败
		NotEnoughItem,      //item number not enough
		DidNotHaveThisItem,
		UserRefuseAddFriends,
		MatchTeamateStoped,//等待匹配队友
		ExistGuideTaskId,   //存在的任务id
		UnknowPlatform,
		MsgAnalysisFail,
		PostLoginMsgFail,
		NickNameTooShort,
		GuideUserForbit,
		InvalidCDKey,
		WashRuneFail,
		GetCDKeyGiftSuccess,

		MailHasTimeOver, //邮件邮件过期
		MailHasRecv, //邮件已经领取 
		HavedPerpetualHero,
		InvalidPara,

		CfgFailed,
		InvaildLogicID,
		GSNotFound,
		SSNotFound,
		RedisReplyNil,
		InvalidDatabase,
		SqlExecError,
		EncodeMsgToBufferFailed,
		UserDataNotFound
	}

	public enum UserPlatform
	{
		//ios
		PC = 0,
		IOS_91 = 1,
		IOS_TB = 2,
		IOS_PP = 3,
		IOS_CMGE = 4,
		IOS_UC = 5,
		IOS_iTools = 6,
		OnlineGame = 7,
		IOS_As = 8,
		IOS_XY = 9,
		IOS_CMGE_ZB = 10,
		//android
		AndroidCMGE = 104,
		AndroidUC = 105,
		//其他
		PlatformiOS_CMGEInfo = 304,
		//RC use
		All = int.MaxValue
	}

	public enum RelationShip
	{
		None,
		Friends,
		Detestation
	}

	public enum DBOperation
	{
		None,
		Add,
		Del,
		Upd
	}

	public enum UserPlayingStatus
	{
		OffLine,
		Playing
	}

	public enum PayType
	{
		None,
		Gold,
		Diamond
	}

	public enum GoodsType
	{
		None,
		Hero,
		Skin,
		Rune
	}

	public enum HeroKind
	{
		None,
		Dps,
		Magic,
		Ass,
		Tank
	}

	public enum SkinType
	{
		None
	}

	public enum ConsumeType
	{
		Free,
		Gold,
		Diamond
	}

	public enum DBType
	{
		None,
		Game,
		Cdkey,
		Log,
		Mail
	}

	public enum ObjectType
	{
		None,
		Guild,
		User,
		HeroBegin = Consts.OBJ_TYPE_SPACE,
		NPCBegin = Consts.OBJ_TYPE_SPACE * 2,
		GoodsBegin = Consts.OBJ_TYPE_SPACE * 3,
		AiRobotBegin = Consts.OBJ_TYPE_SPACE * 4
	}

	public enum LobbyType
	{
		Null = -1,
		Ele, //初级场
		Mid, //中级场
		Hey, //高级场
		TaW,
		Pve1,
		TowerDef,
		Pvp007,
		Guid1,
		Guid2,
		Guid3,
		Guid4
	}

	public enum RewardType
	{
		None,   //未设
		Nomal,  //普通
		Extend //额外
	}

	public enum LoginRewardItemType
	{
		None,           //未设
		Gold,           //金币
		Diamond,        //钻石
		GoodsIDBegin   //商品
	}

	public class DBCfg
	{
		public int un32DBHostPort;
		public string aszDBHostIP;
		public string aszDBUserName;
		public string aszDBUserPwd;
		public string aszDBName;
		public string aszUpgradeScriptDir;
	}

	public enum MailCurtState
	{
		None,
		New,
		LookedButNotGotGift,
		Look,
		Del
	}

	public enum MailType
	{
		None,
		T1,
		T2,
		T3,
		T4
	}

	public class MailDBData
	{
		public long objIdx;
		public int mailId;
		public int channelId;
		public MailCurtState curtState;
		public MailType mailType; //邮件类型 
		public long n64CreateTime;//邮件发送时间(mCreateTime创建时间)
		public long n64EndTime; //邮件过期时间 

		public string mailTitle;
		public string mailContent;
		public string mailGift;    //邮件礼包type:key:value eg:1:1:1000;2:2:1000;3:goodsid:1;3:goodsid:2;
		public string szSender;
		public string mCreateTime;
		public string mEndTime;

		public bool bIfPerDel;
		public bool bIfNewMail;
	}

	public struct UserNetInfo
	{
		public int gsID { get; private set; }
		public uint gcNetID { get; private set; }

		public UserNetInfo( int gsID, uint gcNetID )
		{
			this.gsID = gsID;
			this.gcNetID = gcNetID;
		}

		public void Clear()
		{
			this.gcNetID = 0;
			this.gsID = 0;
		}

		public bool IsValid()
		{
			return this.gcNetID > 0 && this.gsID > 0;
		}
	}

	public enum NoticeFlag
	{
		eFlag_None = 0,
		eFlag_OnSale = 1,
		eFlag_Notice = 2,
		eFlag_Services = 3
	}

	public enum NoticeState
	{
		eState_None = 0,
		eState_Hot = 1,
		eState_New = 2
	}

	public class Notice
	{
		//字段、属性、方法、事件
		public long noticeID;//数据库主键
		public uint id;
		public UserPlatform platform;
		public string title;
		public NoticeFlag flag;
		public NoticeState state;
		public uint priority;
		public string msg;
		public long star_time;
		public long end_time;
	}

	public static class Consts
	{
		public const SocketType SOCKET_TYPE = SocketType.Stream;
		public const ProtocolType PROTOCOL_TYPE = ProtocolType.Tcp;

		/// <summary>
		/// 心跳间隔
		/// </summary>
		public const long HEART_PACK = 100;

		/// <summary>
		/// 最大监听器数
		/// </summary>
		public const int MAX_COUNT_LISTENER = 3;

		/// <summary>
		/// Ping的时间间隔
		/// </summary>
		public const long DEFAULT_PING_CD_TICK = 1000 * 160;

		/// <summary>
		/// 重连检测的时间间隔
		/// </summary>
		public const long RECONN_DETECT_INTERVAL = 10000;

		public const int MAX_BATTLE_IN_SS = 200;

		public const int DEFAULT_NAME_LEN = 30;
		public const int DEFAULT_NICK_NAME_LEN = 32;
		public const int DEFAULT_DEVICE_KEY_LEN = DEFAULT_NAME_LEN * 5;
		public const int PROJECTILE_MAX_CHILD_PROJECTILE_TYPE = 3;
		public const int DEFAULT_REMOVE_CONSOLE_KEY_LEN = 65;
		public const int OBJ_TYPE_SPACE = 10000;
		public const int PERSIST_TIME_ALWAYS = -1;
	}
}