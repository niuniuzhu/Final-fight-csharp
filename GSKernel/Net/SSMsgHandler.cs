using Shared;
using Shared.Net;
using System;
using System.Collections.Generic;

namespace GateServer.Net
{
	public class SSMsgHandler
	{
		private delegate EResult MsgHandler( byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public SSMsgHandler()
		{
			this._handlers[( int )SSToGS.MsgID.EMsgToGsfromSsAskPingRet] = this.OnMsgFromSS_AskPingRet;
			this._handlers[( int )SSToGS.MsgID.EMsgToGsfromSsOrderKickoutGc] = this.OnMsgFromSS_OrderKickoutGC;
		}

		private EResult OnMsgFromSS_AskPingRet( byte[] bytes, int offset, int len )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromSS_OrderKickoutGC( byte[] bytes, int offset, int len )
		{
			throw new NotImplementedException();
		}

		public ErrorCode HandleUnhandledMsg( GSSSInfo pcSsInfo, byte[] data, int offset, int size, int n32MsgID, int n32TransID, uint n32GcNetID )
		{
			if ( n32TransID < ( int )CSToGS.MsgID.EMsgToGsfromCsBegin || n32TransID >= ( int )CSToGS.MsgID.EMsgToGsfromCsEnd )
			{
				return ErrorCode.EC_InvalidMsgProtocalID;
			}
			if ( n32TransID == ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc )
			{
				if ( n32GcNetID == 0 )
				{
					this.BroadcastToGameClient( data, offset, size, n32MsgID );
				}
				else
				{
					if ( n32MsgID == ( int )GSToGC.MsgID.EMsgToGcfromGsNotifyBattleBaseInfo )
					{
						//m_User2SSInfoMap[n32GcNetID] = piSSInfo; todo
					}
					this.PostToGameClient( n32GcNetID, data, offset, size, n32MsgID );
				}
			}
			else
			{
				if ( this._handlers.TryGetValue( n32TransID, out MsgHandler handler ) )
					handler( data, offset, size );
			}
			return ErrorCode.EC_NullMsgHandler;
		}

		private int BroadcastToGameClient( byte[] data, int offset, int size, int n32MsgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ServerGS, uint.MaxValue, data, offset, size, n32MsgID );
			return 0;
		}

		private int PostToGameClient( uint n32NetSessioNID, byte[] data, int offset, int size, int n32MsgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ServerGS, n32NetSessioNID, data, offset, size, n32MsgID );
			return 0;
		}
	}
}