using Shared;
using System.Collections.Generic;
using Core.Misc;
using Google.Protobuf;

namespace CentralServer.Net
{
	public class GSMsgManager
	{
		private delegate ErrorCode DGCMsgHandler( CSGSInfo cpiGSInfo, int gcnsID, byte[] data, int offset, int size );
		private delegate ErrorCode DGSMsgHandler( CSGSInfo cpiGSInfo, byte[] data, int offset, int size );

		private readonly Dictionary<int, DGCMsgHandler> _gcHandlers = new Dictionary<int, DGCMsgHandler>();
		private readonly Dictionary<int, DGSMsgHandler> _gshandlers = new Dictionary<int, DGSMsgHandler>();

		public GSMsgManager()
		{
			#region 注册消息处理函数
			this._gshandlers[( int )GSToCS.MsgID.EMsgToCsfromGsAskRegiste] = this.OnMsgFromGSAskRegiste;
			this._gshandlers[( int )GSToCS.MsgID.EMsgToCsfromGsAskPing] = this.OnMsgFromGSAskPing;
			//this._gcHandlers[( int )GSToCS.MsgID.EMsgToCsfromGsReportGcmsg] = this.OnMsgFromGSReportGCMsg;
			#endregion
		}

		private ErrorCode OnMsgFromGSAskRegiste( CSGSInfo csgsInfo, byte[] data, int offset, int size )
		{
			GSToCS.AskRegiste sMsg = new GSToCS.AskRegiste();
			sMsg.MergeFrom( data, offset, size );

			// 找到位置号
			int gsPos = -1;
			CSGSInfo pcGSInfo = null;
			for ( int i = 0; i < CS.instance.m_sCSKernelCfg.un32MaxGSNum; i++ )
			{
				if ( sMsg.Gsid != CS.instance.m_pcGSInfoList[i].m_n32GSID )
					continue;
				pcGSInfo = CS.instance.m_pcGSInfoList[i];
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
				pcGSInfo.m_eGSNetState = EServerNetState.SnsFree;
				pcGSInfo.m_n32NSID = csgsInfo.m_n32NSID;
				pcGSInfo.m_tLastConnMilsec = tCurUTCMilsec;
				pcGSInfo.m_sListenIP = sMsg.Ip;
				pcGSInfo.m_n32ListenPort = sMsg.Port;
				CS.instance.m_psGSNetInfoList[gsPos].pcGSInfo = pcGSInfo;

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
				sAskRegisteRet.Ssbaseid = CS.instance.m_sCSKernelCfg.un32SSBaseIdx;
				for ( int i = 0; i < CS.instance.m_sCSKernelCfg.un32MaxSSNum; i++ )
				{
					CSSSInfo csssInfo = CS.instance.m_pcSSInfoList[i];
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

			CS.instance.netSessionMgr.TranMsgToSession( csgsInfo.m_n32NSID, sAskRegisteRet, ( int )CSToGS.MsgID.EMsgToGsfromCsAskRegisteRet, 0, 0 );

			if ( ErrorCode.Success != n32Registe )
				CS.instance.netSessionMgr.DisconnectOne( csgsInfo.m_n32NSID );

			return ErrorCode.Success;
		}

		private ErrorCode OnMsgFromGSAskPing( CSGSInfo cpigsinfo, byte[] data, int offset, int size )
		{
			throw new System.NotImplementedException();
		}

		private ErrorCode OnMsgFromGSReportGCMsg( CSGSInfo cpigsinfo, byte[] data, int offset, int size )
		{
			throw new System.NotImplementedException();
		}

		public void HandleUnhandledMsg( CSGSInfo csgsInfo, byte[] data, int offset, int size, int realMsgID, int msgID, uint gcNetID )
		{
			throw new System.NotImplementedException();
		}
	}
}