using Core.Misc;
using Google.Protobuf;
using Shared;
using System.Collections.Generic;

namespace CentralServer.UserModule
{
	public partial class UserMsgMgr
	{
		private delegate ErrorCode GCMsgHandler( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size );

		private readonly Dictionary<int, GCMsgHandler> _gcMsgHandlers = new Dictionary<int, GCMsgHandler>();

		public UserMsgMgr()
		{
			this._gcMsgHandlers[( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin] = this.OnMsgToGstoCsfromGcAskLogin;
		}

		public ErrorCode Invoke( CSGSInfo csgsInfo, int msgID, uint gcNetID, byte[] data, int offset, int size )
		{
			if ( this._gcMsgHandlers.TryGetValue( msgID, out GCMsgHandler handler ) )
				return handler.Invoke( csgsInfo, gcNetID, data, offset, size );
			Logger.Warn( $"invalid msg:{msgID}." );
			return ErrorCode.InvalidMsgProtocalID;
		}

		private static void PostMsgToGCAskReturn( CSGSInfo csgsInfo, uint gcNetID, int askProtocalID, ErrorCode errorCode )
		{
			GSToGC.AskRet msg = new GSToGC.AskRet
			{
				Askid = askProtocalID,
				Errorcode = ( int )errorCode
			};
			CS.instance.netSessionMgr.TranMsgToSession( csgsInfo.m_n32NSID, msg,
														( int )GSToGC.MsgID.EMsgToGcfromGsGcaskRet,
														gcNetID == 0 ? 0 : ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc,
														gcNetID );
		}

		private ErrorCode OnMsgToGstoCsfromGcAskLogin( CSGSInfo csgsInfo, uint gcNetID, byte[] data, int offset, int size )
		{
			GCToCS.Login login = new GCToCS.Login();
			login.MergeFrom( data, offset, size );
			Logger.Log( $"--new login({login.Name})--" );
			ErrorCode errorCode = this.UserAskLogin( csgsInfo, gcNetID, login );
			if ( ErrorCode.Success != errorCode )
				PostMsgToGCAskReturn( csgsInfo, gcNetID, ( int )GCToCS.MsgNum.EMsgToGstoCsfromGcAskLogin, errorCode );
			return ErrorCode.Success;
		}
	}
}