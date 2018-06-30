using Core.Misc;
using Google.Protobuf;
using Shared;

namespace CentralServer.User
{
	public partial class CSUser
	{
		public ErrorCode SynUser_UserBaseInfo()
		{
			GSToGC.UserBaseInfo sUserBaseInfo = new GSToGC.UserBaseInfo();
			sUserBaseInfo.Nickname = this.userDbData.szNickName;
			sUserBaseInfo.Name = this.userDbData.szUserName;
			sUserBaseInfo.Sex = this.userDbData.sPODUsrDBData.n16Sex;
			sUserBaseInfo.Curscore = this.userDbData.sPODUsrDBData.n64Score;
			sUserBaseInfo.Curgold = this.userDbData.sPODUsrDBData.n64Gold;
			sUserBaseInfo.Curdiamoand = this.userDbData.sPODUsrDBData.n64Diamond;
			sUserBaseInfo.Guid = this.userDbData.sPODUsrDBData.un64ObjIdx;
			//todo
			//sUserBaseInfo.Mapid = GetBattleMgrInstance().GetBattleMapID( m_sUserBattleInfoEx.GetBattleID() );
			//sUserBaseInfo.Battleid = m_sUserBattleInfoEx.GetBattleID();
			sUserBaseInfo.Ifreconnect = false;
			sUserBaseInfo.Level = this.userDbData.sPODUsrDBData.un8UserLv;
			sUserBaseInfo.Headid = this.userDbData.sPODUsrDBData.un16HeaderID;
			sUserBaseInfo.VipLevel = this.userDbData.sPODUsrDBData.un16VipLv;
			sUserBaseInfo.VipScore = this.userDbData.sPODUsrDBData.vipScore;
			sUserBaseInfo.CurExp = ( int )this.userDbData.sPODUsrDBData.un32UserCurLvExp;
			return this.PostMsgToGC( sUserBaseInfo, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyUserBaseInfo );
		}

		private ErrorCode PostMsgToGC( IMessage sMsg, int msgID )
		{
			CSGSInfo cpiGSInfo = CS.instance.GetGSInfoByGSID( this.userNetInfo.n32GSID );
			if ( null == cpiGSInfo )
				return ErrorCode.NullGateServer;

			CS.instance.netSessionMgr.TranMsgToSession( cpiGSInfo.m_n32NSID, sMsg, msgID,
														this.userNetInfo.n32GCNSID == 0
															? 0
															: ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc,
														this.userNetInfo.n32GCNSID );
			return ErrorCode.Success;
		}

		public void PostCSNotice()
		{
			GSToGC.GameNotice pMsg = new GSToGC.GameNotice();
			CS.instance.csUserMgr.ForeachNotice( tempNotice =>
			{
				if ( tempNotice.msg.Length < 1 )
					return;

				//平台判断
				if ( tempNotice.platform != UserPlatform.Platform_All )
					if ( tempNotice.platform != this.userDbData.sPODUsrDBData.userPlatform )
						return;

				//过期判断
				long temp_date = TimeUtils.utcTime;
				long temp_end = tempNotice.end_time - temp_date;
				if ( temp_end < 0 )
					return;

				//是否到发送时间
				long temp_star = tempNotice.star_time - temp_date;
				if ( temp_star > 0 )
					return;

				GSToGC.GameNotice.Types.Notice notice = new GSToGC.GameNotice.Types.Notice
				{
					Title = tempNotice.title,
					Flag = ( uint )tempNotice.flag,
					Status = ( uint )tempNotice.state,
					Priority = tempNotice.priority,
					Notice_ = tempNotice.msg
				};
				pMsg.Notice.Add( notice );
			} );
			this.PostMsgToGC( pMsg, ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyNotice );
		}
	}
}