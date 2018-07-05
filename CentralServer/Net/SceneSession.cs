using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace CentralServer.Net
{
	public class SceneSession : SrvCliSession
	{
		protected SceneSession( uint id ) : base( id )
		{
			this.msgCenter.Register( ( int )SSToCS.MsgID.EMsgToCsfromSsAskRegiste, this.MsgHandleInit );
		}

		protected override void SendInitData()
		{
			long tCurMilSec = TimeUtils.utcTime;
			CSToSS.AskRegisteRet aAskRegisteRet = new CSToSS.AskRegisteRet();
			aAskRegisteRet.Ret = ( int )ErrorCode.Success;
			aAskRegisteRet.Time = tCurMilSec;
			aAskRegisteRet.Basegsid = ( int )CS.instance.csKernelCfg.un32GSBaseIdx;
			for ( uint i = 0; i < CS.instance.csKernelCfg.un32MaxGSNum; i++ )
			{
				CSToSS.AskRegisteRet.Types.GSInfo pGSInfo =
					new CSToSS.AskRegisteRet.Types.GSInfo
					{
						Gsid = CS.instance.csKernelCfg.gsInfoList[i].m_n32GSID,
						Userpwd = CS.instance.csKernelCfg.gsInfoList[i].m_szUserPwd
					};
				aAskRegisteRet.Gsinfo.Add( pGSInfo );
			}
			this.owner.SendMsgToSession( this.id, aAskRegisteRet, ( int )CSToSS.MsgId.EMsgToSsfromCsAskRegisteRet );

			//////////////////////////////////////////////////////////////////////////
			CSSSInfo pcSSInfo = CS.instance.GetSSInfoByNSID( this.id );
			CSToGS.OneSSConnected sOneSSConnected = new CSToGS.OneSSConnected
			{
				State = ( int )ErrorCode.Success,
				Time = tCurMilSec,
				Ssid = pcSSInfo.m_n32SSID,
				Ip = pcSSInfo.m_sListenIP,
				Port = pcSSInfo.m_n32ListenPort,
				Netstate = ( int )pcSSInfo.m_eSSNetState,
				Basessid = ( int )CS.instance.csKernelCfg.un32SSBaseIdx
			};

			CS.instance.BroadCastMsgToGS( sOneSSConnected, ( int )CSToGS.MsgID.EMsgToGsfromCsOneSsconnected );
		}

		protected override void OnRealEstablish()
		{
			CSSSInfo pcSSInfo = CS.instance.GetSSInfoByNSID( this.id );
			if ( pcSSInfo != null )
				Logger.Info( $"SS({ pcSSInfo.m_n32SSID}) Connected" );
		}

		protected override void OnClose()
		{
			CSSSInfo pcSSInfo = CS.instance.GetSSInfoByNSID( this.id );
			if ( pcSSInfo != null )
			{
				Logger.Info( $"SS({pcSSInfo.m_n32SSID}) DisConnected" );
				int pos = pcSSInfo.m_n32SSID - ( int )CS.instance.csKernelCfg.un32SSBaseIdx;
				pcSSInfo.m_eSSNetState = ServerNetState.Closed;
				pcSSInfo.m_n32NSID = 0;
				pcSSInfo.m_tLastConnMilsec = 0;
				pcSSInfo.m_un32ConnTimes = 0;
				CS.instance.ssNetInfoList[pos].tConnMilsec = 0;
				CS.instance.ssNetInfoList[pos].pcSSInfo = null;
			}
		}

		private ErrorCode MsgHandleInit( byte[] data, int offset, int size, int msgid )
		{
			SSToCS.AskRegiste aAskRegiste = new SSToCS.AskRegiste();
			aAskRegiste.MergeFrom( data, offset, size );

			uint ssPos = ( uint )aAskRegiste.Ssid - CS.instance.csKernelCfg.un32SSBaseIdx;
			CSSSInfo pcSSInfo = CS.instance.GetSSInfoBySSID( ( uint )aAskRegiste.Ssid );
			if ( null == pcSSInfo )
				return ErrorCode.InvalidSSID;

			if ( pcSSInfo.m_szUserPwd != aAskRegiste.Userpwd )
			{
				this.Close();
				return ErrorCode.InvalidUserPwd;
			}

			// 加入SS
			pcSSInfo.m_eSSNetState = ServerNetState.Connecting;
			pcSSInfo.m_n32NSID = ( int )this.id;
			this.logicID = pcSSInfo.m_n32SSID;
			pcSSInfo.m_tLastConnMilsec = TimeUtils.utcTime;
			pcSSInfo.m_sListenIP = aAskRegiste.Ip;
			pcSSInfo.m_n32ListenPort = aAskRegiste.Port;
			CS.instance.ssNetInfoList[ssPos].pcSSInfo = pcSSInfo;
			pcSSInfo.m_un32ConnTimes++;
			pcSSInfo.m_tLastConnMilsec = TimeUtils.utcTime;
			pcSSInfo.m_tLastPingMilSec = pcSSInfo.m_tLastConnMilsec;

			this.SetInited( true, true );

			return ErrorCode.Success;
		}
	}
}