using Core;
using Core.Misc;
using CSToGS;
using Google.Protobuf;
using GSToCS;
using Shared;
using Shared.Net;
using System;

namespace GateServer
{
	public partial class GSKernel
	{
		private long _lastPingCSTickCounter;

		public void ResetlastPingCSTickCounter()
		{
			this._lastPingCSTickCounter = 0;
		}

		private EResult OnHeartBeat( UpdateContext context )
		{
			this.CheckCSConnect( context.utcTime );
			this.CheckSSConnect( context.utcTime );
			this.ChechUserToken( context.utcTime );
			this.ReportGsInfo( context.utcTime );
			return EResult.Normal;
		}

		private EResult CheckCSConnect( long utcTime )
		{
			if ( utcTime - this._lastPingCSTickCounter >= Consts.c_tDefaultPingCDTick )
			{
				Asking sPing = new Asking { Time = utcTime };
				ByteString bs = sPing.ToByteString();
				this.TransToCS( bs, ( int )GSToCS.MsgID.EMsgToCsfromGsAskPing, 0, 0 );
				this._lastPingCSTickCounter = utcTime;
			}
			return EResult.Normal;
		}

		private EResult CheckSSConnect( long utcTime )
		{
			//if ( _GSSSInfoMap.empty() )
			//{
			//	return EResult.Normal;
			//}

			//TIME_MILSEC tCurUTCMilsec = GetMiliSecond();
			//for ( CGSSSInfoMap::iterator iter = _GSSSInfoMap.begin(); iter != _GSSSInfoMap.end(); ++iter )
			//{
			//	GSSSInfo pSSInfo = iter.second;
			//	if ( NULL != pSSInfo && 0 != pSSInfo.GetSSID() && 0 != pSSInfo.GetNSID() )
			//	{
			//		if ( tCurTime - pSSInfo.m_tPingTickCounter >= c_tDefaultPingCDTick )
			//		{
			//			GSToSS::AskPing sMsg;
			//			sMsg.set_time( tCurUTCMilsec );
			//			string sData = sMsg.SerializeAsString();
			//			this.TransToSS( pSSInfo, sData.c_str(), sData.size(), sMsg.mgsid(), 0, 0 );
			//			pSSInfo.m_tPingTickCounter = tCurTime;
			//		}
			//	}
			//}

			return EResult.Normal;
		}

		private EResult ChechUserToken( long utcTime )
		{
			//static TIME_TICK lastReprot = tUICMilsec;
			//if ( lastReprot > tUICMilsec ) return eNormal;
			//lastReprot = tUICMilsec + 2000;

			//map<string, SUserToken*>::iterator it = m_UserTokenList.begin();
			//for ( ; it != m_UserTokenList.end(); )
			//{
			//	SUserToken* userToken = it.second;
			//	if ( userToken.NetSessionID == 0 &&
			//		 tUICMilsec > userToken.OverTime )//最大重练超时限制//
			//	{
			//		for ( auto it2 = m_UserTokenListByNsId.begin(); it2 != m_UserTokenListByNsId.end(); ++it2 )
			//		{
			//			Assert( it2.second != userToken );//检查没有对象引用它
			//		}
			//		it = m_UserTokenList.erase( it );
			//		delete userToken;
			//	}
			//	else
			//	{
			//		++it;
			//	}
			//}
			return EResult.Normal;
		}

		private void ReportGsInfo( long utcTime )
		{
		}

		private bool ParseProtoMsg<T>( byte[] data, int n32DataLength, ref T sMsg ) where T : IMessage
		{
			using ( CodedInputStream inputStream = new CodedInputStream( data ) )
			{
				inputStream.ReadMessage( sMsg );
			}
			return true;
		}

		private EResult OnMsgFromCS_AskPingRet( byte[] data, int len )
		{
			AskPing pingRet = new AskPing();
			if ( !this.ParseProtoMsg( data, len, ref pingRet ) )
				return EResult.ParseProtoFailed;
			long curMilsec = TimeUtils.utcTime;
			long tickSpan = curMilsec - pingRet.Time;
			Logger.Info( $"Ping CS returned, tick span {tickSpan}." );

			return EResult.Normal;
		}

		private EResult OnMsgFromCS_OrderOpenListen( byte[] data, int len )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromCS_OrderCloseListen( byte[] data, int len )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromCS_OrderKickoutGC( byte[] data, int len )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromCS_UserConnectedToSS( byte[] data, int len )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromCS_UserDisConnectedToSS( byte[] data, int len )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromSS_AskPingRet( GSSSInfo pissinfo, byte[] data, int len )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromSS_OrderKickoutGC( GSSSInfo pissinfo, byte[] data, int len )
		{
			throw new NotImplementedException();
		}
	}
}