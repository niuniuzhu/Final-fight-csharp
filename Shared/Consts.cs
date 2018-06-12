using System.Net.Sockets;

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
		SnsClosed = 0,
		SnsConnecting,
		SnsFree,
		SnsBusy,
		SnsFull,
	}

	public enum ErrorCode
	{
		EC_Begin = -0x00010000 * 2,
		EC_InvalidModelStatus, //非法的模块状态
		EC_InvalidModelID, //非法的模块ID
		EC_InvalidGSID, //非法的GSID
		EC_InvalidSSID, //非法的SSID
		EC_NullDataSource, //找不到数据源
		EC_NullObjMgr, //找不到ObjMgr
		EC_NullDataMgr, //找不到DataMgr
		EC_NullWorldMgr, //找不到WorldMgr
		EC_NullSceneServer, //找不到SceneServer
		EC_NullGateServer, //找不到GateServer
		EC_NullCentralServer, //找不到CentralServer
		EC_InvalidUserPwd, //非法的用户口令
		EC_InvalidPosition, //非法的位置
		EC_InvalidRegionID, //非法的区域ID
		EC_InvalidAreaID, //非法的区域ID
		EC_NetSessionCollide, //网络会话冲突
		EC_NullUser, //找不到用户
		EC_UserExist, //用户已经存在
		EC_GameObjectAlreadyExist, //游戏对象已经存在
		EC_NullGameUnit, //找不到游戏单元
		EC_LoadLibarayFail, //加载模块失败
		EC_OpenCfgFileFail, //找开配置文件失败
		EC_NoneNetListener, //没有网络监听器
		EC_NoneNetConnector, //没有网络连接器
		EC_OpenConnectorFail, //打开网络连接器失败
		EC_TooManyConnectorOpened, //打开了太多的连接
		EC_InitModelTimeOut, //初始化模块失败
		EC_StartModelTimeOut, //启动模块超时
		EC_NullMasterUser, //找不到主用户
		EC_InvalidAttackTarget, //非法的技能目标
		EC_NetConnectionClosed, //网络连接已经关闭
		EC_NetProtocalDataError, //网络协议数据错误
		EC_TooManySSNum, //SS数量过多
		EC_TooManyGSNum, //GS数量过多
		EC_InvalidNetState, //非法的网络状态
		EC_JustInBattle, //当前正在战役中
		EC_InvalidBattlePos, //非法的战役座位
		EC_TheBattleUserFull, //战役用户已满
		EC_UserDonotInTheBattle, //用户不在战役之中
		EC_JustNotInBattle, //当前不在战役之中
		EC_InvalidBattleState, //非法的战役状态
		EC_InvalidBattleID, //非法的战役ID
		EC_HeroExist, //英雄已经存在
		EC_InvalidUserNetInfo, //非法的用户网络信息
		EC_BattleExist, //战役已经存在
		EC_BattleDonotExist, //战役不存在
		EC_MapLoaded, //地图已经加载
		EC_BattleLocalObjIdxOverflow, //战役本地对象溢出
		EC_CannotFindoutTheObject, //找不到对象
		EC_NullHero, //找不到英雄
		EC_NotAllUserReady, //不是所有用户已经准备好
		EC_YouAreNotBattleManager, //你不是战役管理员
		EC_InvalidOrderCate, //非法的Order类别
		EC_InvalidGameObjectCamp, //非法的游戏对象阵营
		EC_CreateOrderFail, //创建Order失败
		EC_InvalidOrderState, //非法的Order状态
		EC_OrderNoMasterGO, //Order没有主对象
		EC_AbsentOrderPriority, //Order优先级不够
		EC_NullBattle, //找不到战役
		EC_MoveBlocked, //移动被阻挡
		EC_ReadCfgFileFail, //读取配置文件失败
		EC_TooManyNPCCfgNum, //NPC配置数量太多
		EC_TooManyHeroCfgNum, //英雄配置数量太多
		Ec_TooManMapDataCfgNum, //地图配置数量太多
		EC_InvalidHeroSeat, //非法的英雄座位
		EC_InvalidMapID, //非法的地图ID
		Ec_InvalidPos, //非法的位置
		EC_InvalidCellID, //非法的单元格ID
		EC_NullArea, //非法的区域
		EC_InvalidScriptParameters, //非法的脚本参数
		EC_InvalidObjTypeID, //非法的对象类型ID
		EC_InvalidPathNodeNum, //非法的路径节点数量
		EC_InvalidVector3D, //非法的3D向量
		EC_UserInfoUnComplete, //用户信息不全
		EC_GenerateGUIDFail, //创建GUID失败
		EC_NotEnemy, //没有敌人
		EC_NotNormalAttackSkill, //没有普通攻击技能
		EC_CanNotFindTheSkill, //找不到指定技能ID
		EC_NotOrderInfo, //没有Order信息
		EC_InvalidSkillID, //非法的技能ID
		EC_InvalidSkillState, //非法的技能状态
		EC_InvalidSkillTarget, //非法的技能目标
		EC_NullSkillData, //没有技能数据
		EC_InvalidGameObjectCate, //非法的游戏对象类型
		EC_HasChoosedHero, //已经选择了英雄
		EC_OthersHasChoosedHero, //
		EC_HasNoTheHero,
		EC_AbsentAttackDistance, //攻击距离不足
		EC_TargetIsDead, //目标已经死亡
		EC_InvalidTargetActionState, //目标行为状态非法
		EC_InvalidBattle, //非法的战役
		EC_TheBattlIsFull,  //战役已经满员
		EC_BattlePDWNotMatch,
		EC_NullGameObject,//找不到游戏对象
		EC_NULLNickName, //找不到昵称
		EC_TimeExpire, //时间超时
		EC_NoneAvailbleBuff, //没有有效BUFF
		EC_BuffOverlapUpperLimit, //超过BUFF上限
		EC_InvalidCurHPNum, //非法的当前HP
		EC_InvalidCurMPNum, //非法的当前MP
		Ec_NoPathNode, //找不到路径节点
		EC_InvalidBuffTypeID, //非法的缓冲类型ID
		EC_AbsentSkillDistance, //技能距离不足
		EC_BuildingCanNotMove, //建筑不能移动
		EC_AbsentCP, //CP不足
		EC_AbsentMP, //MP不足
		EC_AbsentHP, //HP不足
		EC_BuildingCanNotAddBuff, //建筑不能添加BUFF
		EC_InvalidActionState, //非法的行为状态
		EC_JustSkillAction,  //正在使用技能
		EC_AbsorbMonsterFailForLackLevel,
		EC_AbsorbMonsterFailForHasFullAbsorb,
		EC_AbsorbMonsterFailForHasSameSkillID,
		EC_AbsorbMonsterFailForLackCP,
		EC_AbsorbMonsterFailForMonsterDead,
		EC_AbsorbMonsterFailForMonsterCannotBeConstrol,
		EC_AbsorbMonsterFailForHeroDead,
		EC_AbsorbMonsterFailForNotMonster,
		EC_AbsorbMonsterFailForDiffNPC,
		EC_AbsorbMonsterFailForErrorState,
		EC_AbsorbMonsterFailForDizziness,
		EC_NULLNPC, //找不到NPC
		EC_NULLCfg, //找不到配置信息
		EC_InvaildSkillID, //非法的技能ID
		EC_RemoveAbsorbSkillFailed, //移除吸附技能失败
		EC_StateCanNotUseGas,
		Ec_ExistWildMonsterBornPos,
		Ec_InvalidControlNPCType,
		EC_GasExplosionNotFull,
		EC_MultiAbsortNotAllowed,
		EC_StaticBlock, //静态阻挡
		EC_DynamicBlock, //动态阻挡
		EC_NoDistanceToMove, //没有距离需要移动
		EC_CannotFindFullPathNode, //找不到完整的路径
		EC_HeroNotDead, //英雄没有死亡
		EC_NotEnoughGold, //金币不足
		EC_NoRebornTimes,
		EC_BattleIsPlaying,
		EC_RemoveBuffFailed,
		Ec_DeadAltar,
		EC_InvaildCampID,
		EC_NotInSameBattle,
		EC_AskBuyRunesFail,
		EC_AskComposeRunesFail,
		EC_AskUnUseRunesFail,
		EC_AskUseRunesFail,
		EC_AskMoveGoodsFail,
		EC_AskSellGoodsFail,
		EC_InvaildGridID,
		EC_AskUseGoodsFailForCoolDown,
		EC_rInvalidGoodsNum,
		EC_AskBuyGoodsFailForLackCP,
		EC_AskBuyGoodsFailForInvalidCPType,
		EC_AskBuyGoodsFailForHasSameTypeID,
		EC_AskBuyGoodsFailForHasFunType,
		EC_AskBuyGoodsFailForBagFull,
		EC_CannotCreateVoipServer,
		Ec_AttackOneObj,
		Ec_ExistObj,
		Ec_TheSkillEnd,
		Ec_ErrorSkillId,
		Ec_InvalidMastType,
		EC_NickNameCollision, //昵称冲突
		EC_ObjectAlreadyExist,
		EC_ForbitAbWMSolder, //不能吸附野怪兵
		EC_TargetCannotLooked,//目标不能锁定
		EC_ErrorAreaId,//错误的areaId
		Ec_NoWatchUser,//没有观察者
		Ec_MaxBornSolder,
		EC_AddBattleFailForLackOfGold,
		EC_CampNotBalance,
		EC_AskBuyGoodsFailForSole,

		EC_TimeOut,
		EC_AddEffectFailed,
		EC_EffectEnd,

		EC_UseSkillFailForSilenced,
		EC_UseSkillFailForDisarmed,
		EC_UseSkillFailForLackHP,
		EC_UseSkillFailForLackMP,
		EC_UseSkillFailForLackCP,
		EC_UseSkillFailForSkillCoolDown,
		EC_UseSkillFailForNULLTarget,
		EC_UseSkillFailForBuildingNullity,

		EC_UseGoodsFailForDizziness,
		EC_UseGoodsFailForBuildingNullity,
		EC_UseGoodsFailForNULLTarget,
		EC_UseGoodslFailForSilenced,
		EC_UseGoodslFailForErrorCamp,
		EC_UseSkillGasNotInRunState,
		EC_UseSkillGasHasInRunState,
		EC_NoAbsorbSkill,

		EC_UseSkillFailForDizziness,
		EC_AskBuyGoodsFailForLackTeamCP,
		EC_CanntAbsorb,
		EC_SkillPrepareFailed,
		EC_CancelSkillOrderFailed,

		EC_TheBattleObserverNotFull,
		EC_TheBattleObserverFull,
		EC_BeginBattleFailForNullPlayer,
		EC_AddBattleFailForAllFull,
		EC_AddBattleFailForUserFull,
		EC_WarningToSelectHero,
		EC_GuideNotOn,
		EC_HasCompGuideStep,
		Ec_InvalidStepId,
		Ec_DelAbsorbICOFailed,
		EC_AbsorbMonsterFail,
		EC_ZeroGUID,
		Ec_NoObjList,

		EC_JustInThatSeatPos,
		EC_NickNameNotAllowed,
		Ec_InvalidTarget,  //错误的对象:箭塔
		EC_GUDead,
		EC_TooManyUserInBattle,
		Ec_NoAtkObj,
		EC_InvalidMapInfo,
		Ec_InvalidMapId,
		Ec_NullLuaCfg,
		EC_NullMapCfg,
		EC_LoadFilterCfgFailed,
		EC_RemoveEffectFailed,
		EC_UserNotExist,
		EC_BattleFinished,

		EC_UserWasInFriendList,
		EC_UserWasInBlackList,
		EC_UserNotOnline,
		EC_MsgTooLarge,
		EC_UserRefuseReceiveYourMsg,
		EC_UserOfflineFull,

		EC_HaveBuySameGoods,
		EC_BuyGoodsFailedLackGold,
		EC_BuyGoodsFailedLackDiamond,
		Ec_ExistGuidCfg,
		EC_NullInfo,
		EC_UserRSExist,
		Ec_ErrorGuideStepId,
		Ec_invalidObjId,
		EC_NothingContent,
		Ec_DoneDBWrong,
		EC_AskTooFrequently,
		EC_UserOfflineMsgFull,
		Ec_InvalidAbsorbTar,
		EC_BeginAIFailed,
		EC_CanNotUseChinese,

		EC_TipsObjAppear,
		EC_TipsNPCBorn,
		EC_TipsSuperNPCBorn,
		Ec_ErrorType,
		EC_FriendsListFull,
		EC_BlackListFull,
		EC_JustInFriendsList,
		EC_JustInBlackList,
		EC_NullUserRSInfo,
		EC_UserBusy,
		EC_YouInOppositeBlackList,
		EC_CounterpartFriendListFull,
		EC_UserInYourBlackList,
		EC_AskHaveSend,
		Ec_existBattle,
		EC_CannotBuygoodsWhenDeath,
		Ec_TimeToSaveDB,

		Ec_91LoginFail,
		Ec_91InvalidAppID,
		Ec_91InvalidAct,
		Ec_91InvalidPara,
		Ec_91InvalidSign,
		Ec_91InvalidSessionID,

		Ec_UserNotHaveHero,
		EC_CannotSellgoodsWhenDeath,
		EC_PPUserNameRuleWrong,
		EC_PPUserNotExist,
		EC_PPInvalidAct,
		EC_PPUserExisted,
		EC_PPPwdCheckError,
		EC_PPUserProhibited,
		EC_PPDataError,
		EC_PPSessionTimeout,
		EC_PPUserHaveBinding,

		EC_TBInvalidToken,
		EC_TBInvalidFormat,

		Ec_FunClosed,
		EC_BattleClosing,  //用于重连
		EC_InvalidPwdLength,
		EC_PleaseEnterPwd,
		EC_InvalidUserNameLegth,

		EC_NullPointer,
		EC_InvalidMsgProtocalID,
		EC_NullMsgHandler,
		Ec_InvalidGUID,
		EC_NullMsg,
		EC_InvalidNSID,
		EC_GUIDCollision,
		EC_InvalidUserName,
		Ec_InvalidMailId,
		EC_UserOfflineOrNullUser,
		EC_UserWasPlaying,
		EC_RequestSended,
		EC_AddFriendSeccuse,
		EC_OppositeSideFriendFull,
		EC_ReEnterRoomFail,//重进房间失败
		EC_DiamondNotEnough,
		EC_ParseProtoError,//解析PB错误
		EC_UnKnownError,//未知错误//
		EC_ErrorTimes,
		EC_MatchLinkInvalid,//匹配链接已失效//
		EC_AddMatchTeamError,//加入匹配队伍失败
		EC_NotEnoughItem,      //item number not enough
		EC_DidNotHaveThisItem,
		EC_UserRefuseAddFriends,
		EC_MatchTeamateStoped,//等待匹配队友
		Ec_ExistGuideTaskId,   //存在的任务id
		EC_UnknowPlatform,
		EC_MsgAnalysisFail,
		EC_PostLoginMsgFail,
		EC_NickNameTooShort,
		EC_GuideUserForbit,
		EC_InvalidCDKey,
		EC_WashRuneFail,
		EC_GetCDKeyGiftSuccess,

		Ec_MailHasTimeOver, //邮件邮件过期
		Ec_MailHasRecv, //邮件已经领取 
		EC_HavedPerpetualHero,
		EC_InvalidPara,
	};

	public static class Consts
	{
		public const SocketType SOCKET_TYPE = SocketType.Stream;
		public const ProtocolType PROTOCOL_TYPE = ProtocolType.Tcp;
		public const int MAX_COUNT_LISTENER = 3;
		public const long C_T_DEFAULT_PING_CD_TICK = 1000 * 160;
		public const uint PP_INVALID = uint.MaxValue;
	}
}