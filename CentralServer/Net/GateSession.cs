using CentralServer.User;
using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace CentralServer.Net
{
	public class GateSession : SrvCliSession
	{
		protected GateSession( uint id ) : base( id )
		{
			this.msgCenter.Register( ( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste, this.MsgHandleInit );
			// this._msgCenter.Register( ( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste, this.OnMsgFromGSAskRegiste );
			this.msgCenter.Register( ( int )GSToCS.MsgID.EMsgToCsfromGsAskPing, this.OnMsgFromGSAskPing );
			this.msgCenter.Register( ( int )GSToCS.MsgID.EMsgToCsfromGsUserOffLine, this.OnMsgToCsfromGsUserOffLine );
			this.msgCenter.Register( ( int )GSToCS.MsgID.EMsgToCsfromGsReportGcmsg, this.OnMsgFromGSReportGCMsg );
		}

		protected override void SendInitData()
		{
			long time = TimeUtils.utcTime;
			CSToGS.AskRegisteRet msg = new CSToGS.AskRegisteRet
			{
				Registe = 0,
				Curtime = time,
				Ssbaseid = CS.instance.csKernelCfg.un32SSBaseIdx
			};

			//for ( uint i = 0; i < CS.instance.csKernelCfg.un32MaxSSNum; i++ )
			//{
			//	CSToGS.AskRegisteRet.Types.SSInfo pSsinfo =
			//		new CSToGS.AskRegisteRet.Types.SSInfo
			//		{
			//			Ssid = CS.instance.ssInfoList[i].m_n32SSID,
			//			Ip = CS.instance.ssInfoList[i].m_sListenIP,
			//			Port = CS.instance.ssInfoList[i].m_n32ListenPort,
			//			Netstate = ( int )CS.instance.ssInfoList[i].m_eSSNetState
			//		};
			//}

			byte[] data = msg.ToByteArray();
			this.owner.TranMsgToSession( this.id, data, 0, data.Length, ( int )CSToGS.MsgID.EMsgToGsfromCsAskRegisteRet, 0, 0 );
		}

		protected override void OnRealEstablish()
		{
			CSGSInfo csgsInfo = CS.instance.GetGSInfoByNSID( this.id );
			if ( csgsInfo != null )
				Logger.Info( $"GS({csgsInfo.m_n32GSID}) Connected" );
		}

		protected override void OnClose()
		{
			CSGSInfo csgsInfo = CS.instance.GetGSInfoByNSID( this.id );
			if ( csgsInfo == null )
				return;
			Logger.Info( $"GS({csgsInfo.m_n32GSID}) DisConnected" );
			int pos = ( int )( csgsInfo.m_n32GSID - CS.instance.csKernelCfg.un32GSBaseIdx );
			csgsInfo.m_eGSNetState = ServerNetState.SnsClosed;
			csgsInfo.m_n32NSID = 0;
			csgsInfo.m_tLastConnMilsec = 0;
			CS.instance.gsNetInfoList[pos].tConnMilsec = 0;
			CS.instance.gsNetInfoList[pos].pcGSInfo = null;
		}

		private ErrorCode MsgHandleInit( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			GSToCS.AskRegiste msg = new GSToCS.AskRegiste();
			msg.MergeFrom( data, offset, size );

			//找到位置号
			int gsPos = -1;
			CSGSInfo csgsInfo = null;
			for ( int i = 0; i < CS.instance.csKernelCfg.un32MaxGSNum; i++ )
			{
				if ( msg.Gsid != CS.instance.gsInfoList[i].m_n32GSID )
					continue;
				csgsInfo = CS.instance.gsInfoList[i];
				gsPos = i;
				break;
			}

			if ( null == csgsInfo )
			{
				this.Close();
				return ErrorCode.InvalidGSID;
			}

			if ( csgsInfo.m_szUserPwd != msg.Usepwd )
			{
				this.Close();
				return ErrorCode.InvalidUserPwd;
			}

			// 加入GS
			long time = TimeUtils.utcTime;
			csgsInfo.m_eGSNetState = ServerNetState.SnsFree;
			csgsInfo.m_n32NSID = this.id;
			this.logicID = csgsInfo.m_n32GSID;
			csgsInfo.m_tLastConnMilsec = time;
			csgsInfo.m_sListenIP = msg.Ip;
			csgsInfo.m_n32ListenPort = msg.Port;
			CS.instance.gsNetInfoList[gsPos].pcGSInfo = csgsInfo;
			csgsInfo.m_n64MsgReceived++;
			csgsInfo.m_n64DataReceived += msg.CalculateSize();
			csgsInfo.m_un32ConnTimes++;
			csgsInfo.m_tLastConnMilsec = time;
			csgsInfo.m_tLastPingMilSec = time;

			this.SetInited( true, true );
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgFromGSAskRegiste( CSGSInfo csgsInfo, byte[] data, int offset, int size )
		{
			GSToCS.AskRegiste sMsg = new GSToCS.AskRegiste();
			sMsg.MergeFrom( data, offset, size );

			// 找到位置号
			int gsPos = -1;
			CSGSInfo pcGSInfo = null;
			for ( int i = 0; i < CS.instance.csKernelCfg.un32MaxGSNum; i++ )
			{
				if ( sMsg.Gsid != CS.instance.gsInfoList[i].m_n32GSID )
					continue;
				pcGSInfo = CS.instance.gsInfoList[i];
				gsPos = i;
				break;
			}

			ErrorCode n32Registe = ErrorCode.Success;
			if ( null == pcGSInfo )
			{
				n32Registe = ErrorCode.InvalidGSID;
			}

			if ( ErrorCode.Success == n32Registe )
			{
				if ( pcGSInfo.m_szUserPwd != sMsg.Usepwd )
					n32Registe = ErrorCode.InvalidUserPwd;
			}

			long tCurUTCMilsec = TimeUtils.utcTime;
			if ( ErrorCode.Success == n32Registe )
			{
				pcGSInfo.m_eGSNetState = ServerNetState.SnsFree;
				pcGSInfo.m_n32NSID = csgsInfo.m_n32NSID;
				pcGSInfo.m_tLastConnMilsec = tCurUTCMilsec;
				pcGSInfo.m_sListenIP = sMsg.Ip;
				pcGSInfo.m_n32ListenPort = sMsg.Port;
				CS.instance.gsNetInfoList[gsPos].pcGSInfo = pcGSInfo;

				pcGSInfo.m_n64MsgReceived++;
				pcGSInfo.m_n64DataReceived += sMsg.CalculateSize();
				pcGSInfo.m_un32ConnTimes++;
				pcGSInfo.m_tLastConnMilsec = tCurUTCMilsec;
				pcGSInfo.m_tLastPingMilSec = tCurUTCMilsec;
				Logger.Info( $"Gate Server with GSID {pcGSInfo.m_n32GSID} registed at net session {gsPos}, total conn times {pcGSInfo.m_un32ConnTimes}" );
			}

			CSToGS.AskRegisteRet sAskRegisteRet = new CSToGS.AskRegisteRet();
			sAskRegisteRet.Registe = ( int )n32Registe;
			sAskRegisteRet.Curtime = tCurUTCMilsec;
			if ( ErrorCode.Success == n32Registe )
			{
				sAskRegisteRet.Ssbaseid = CS.instance.csKernelCfg.un32SSBaseIdx;
				for ( int i = 0; i < CS.instance.csKernelCfg.un32MaxSSNum; i++ )
				{
					CSSSInfo csssInfo = CS.instance.ssInfoList[i];
					CSToGS.AskRegisteRet.Types.SSInfo pSSInfo =
						new CSToGS.AskRegisteRet.Types.SSInfo
						{
							Ssid = csssInfo.m_n32SSID,
							Ip = csssInfo.m_sListenIP,
							Port = csssInfo.m_n32ListenPort,
							Netstate = ( int )csssInfo.m_eSSNetState
						};
					sAskRegisteRet.Ssinfo.Add( pSSInfo );
				}
			}

			CS.instance.PostMsgToGS( csgsInfo, sAskRegisteRet, ( int )CSToGS.MsgID.EMsgToGsfromCsAskRegisteRet, 0 );

			if ( ErrorCode.Success != n32Registe )
				CS.instance.netSessionMgr.DisconnectOne( csgsInfo.m_n32NSID );

			return ErrorCode.Success;
		}

		private ErrorCode OnMsgFromGSAskPing( byte[] data, int offset, int size, int transID, int msgID, uint gcNetIDID )
		{
			GSToCS.Asking askPing = new GSToCS.Asking();
			askPing.MergeFrom( data, offset, size );

			CSToGS.AskPing msg = new CSToGS.AskPing { Time = askPing.Time };

			CSGSInfo csgsInfo = CS.instance.GetGSInfoByNSID( this.id );
			if ( csgsInfo == null )
				return ErrorCode.GSNotFound;
			this.owner.TranMsgToSession( csgsInfo.m_n32NSID, msg, ( int )CSToGS.MsgID.EMsgToGsfromCsAskPingRet, 0, 0 );

			return ErrorCode.Success;
		}

		private ErrorCode OnMsgToCsfromGsUserOffLine( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			GSToCS.UserOffLine sMsp = new GSToCS.UserOffLine();
			sMsp.MergeFrom( data, offset, size );

			CSGSInfo csgsInfo = CS.instance.GetGSInfoByNSID( this.id );
			CSUser pUser = CS.instance.csUserMgr.GetUser( csgsInfo, ( uint )sMsp.Usernetid );
			if ( null != pUser )
			{
				//todo
				//if ( pUser.GetUserBattleInfoEx().GetBattleState() == eBattleState_Free )
				CS.instance.csUserMgr.RemoveUser( pUser );
				pUser.OnOffline();
			}
			return ErrorCode.Success;
		}

		private ErrorCode OnMsgFromGSReportGCMsg( byte[] data, int offset, int size, int transID, int msgID, uint gcNetID )
		{
			GSToCS.ReportGCMsg reportGCMsg = new GSToCS.ReportGCMsg();
			reportGCMsg.MergeFrom( data, offset, size );

			CSGSInfo csgsInfo = CS.instance.GetGSInfoByNSID( this.id );
			return CS.instance.InvokeGCMsg( csgsInfo, msgID, gcNetID, data, offset, size );
		}
	}
}