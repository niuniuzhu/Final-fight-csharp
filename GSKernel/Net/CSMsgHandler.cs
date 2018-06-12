using Core;
using Core.Misc;
using Google.Protobuf;
using Shared;
using System;
using System.Collections.Generic;
using Shared.Net;

namespace GateServer.Net
{
	public class CSMsgHandler
	{
		private delegate EResult MsgHandler( byte[] data, int offset, int size );

		private readonly Dictionary<int, MsgHandler> _handlers = new Dictionary<int, MsgHandler>();

		public CSMsgHandler()
		{
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsAskPingRet] = this.OnMsgFromCS_AskPingRet;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsOrderOpenListen] = this.OnMsgFromCS_OrderOpenListen;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsOrderCloseListen] = this.OnMsgFromCS_OrderCloseListen;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsOrderKickoutGc] = this.OnMsgFromCS_OrderKickoutGC;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsUserConnectedSs] = this.OnMsgFromCS_UserConnectedToSS;
			this._handlers[( int )CSToGS.MsgID.EMsgToGsfromCsUserDisConnectedSs] = this.OnMsgFromCS_UserDisConnectedToSS;
		}

		private EResult OnMsgFromCS_AskPingRet( byte[] data, int offset, int size )
		{
			CSToGS.AskPing pingRet = new CSToGS.AskPing();
			pingRet.MergeFrom( data, offset, size );
			long curMilsec = TimeUtils.utcTime;
			long tickSpan = curMilsec - pingRet.Time;
			Logger.Info( $"Ping CS returned, tick span {tickSpan}." );

			return EResult.Normal;
		}

		private EResult OnMsgFromCS_OrderOpenListen( byte[] data, int offset, int size )
		{
			GSKernel.instance.netSessionMrg.CreateListener( GSKernel.instance.gsConfig.n32GCListenPort, 10240, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 0 );
			return EResult.Normal;
		}

		private EResult OnMsgFromCS_OrderCloseListen( byte[] data, int offset, int size )
		{
			GSKernel.instance.netSessionMrg.StopListener( 0 );
			return EResult.Normal;
		}

		private EResult OnMsgFromCS_OrderKickoutGC( byte[] data, int offset, int size )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromCS_UserConnectedToSS( byte[] data, int offset, int size )
		{
			throw new NotImplementedException();
		}

		private EResult OnMsgFromCS_UserDisConnectedToSS( byte[] data, int offset, int size )
		{
			throw new NotImplementedException();
		}

		public ErrorCode HandleUnhandledMsg( byte[] data, int offset, int size, int n32MsgID, int n32TransID, uint n32GcNetID )
		{
			if ( n32TransID < ( int )CSToGS.MsgID.EMsgToGsfromCsBegin || n32TransID >= ( int )CSToGS.MsgID.EMsgToGsfromCsEnd )
			{
				return ErrorCode.EC_InvalidMsgProtocalID;
			}
			if ( n32TransID == ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc )
			{
				if ( n32GcNetID == 0 )
					this.BroadcastToGameClient( data, offset, size, n32MsgID );
				else
					this.PostToGameClient( n32GcNetID, data, offset, size, n32MsgID );
			}
			else
			{
				if ( this._handlers.TryGetValue( n32TransID, out MsgHandler handler ) )
					handler( data, offset, size );
			}
			return ErrorCode.EC_NullMsgHandler;
		}

		private int BroadcastToGameClient( byte[] data, int offset, int size, int msgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ServerGS, uint.MaxValue, data, offset, size, msgID );
			return 0;
		}

		private int PostToGameClient( uint n32NetSessioNID, byte[] data, int offset, int size, int msgID )
		{
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ServerGS, n32NetSessioNID, data, offset, size, msgID );
			return 0;
		}
	}
}