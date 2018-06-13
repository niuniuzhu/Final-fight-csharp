using Core.Misc;
using Google.Protobuf;
using Shared;
using System.Collections.Generic;
using Shared.Net;

namespace GateServer.Net
{
	public struct SUserToken
	{
		public string userName;
		public string userToken;
		public long overTime;
		public uint reconnectCount;
		public uint netSessionID;
	};

	public class UserTokenMgr
	{
		private readonly Dictionary<string, SUserToken> _userTokenList = new Dictionary<string, SUserToken>();
		private readonly Dictionary<uint, SUserToken> _userTokenListByNsId = new Dictionary<uint, SUserToken>();
		private readonly List<string> _tobeDeletes = new List<string>();
		private long _lastReprot;
		private long _nextReportTime;

		public void AddUserToken( string sUserName, string sToken )
		{
			if ( !this._userTokenList.TryGetValue( sUserName, out SUserToken userToken ) )
				userToken = new SUserToken();
			userToken.userName = sUserName;
			userToken.userToken = sToken;
			userToken.overTime = TimeUtils.utcTime + 60000;
			if ( userToken.netSessionID > 0 )
			{
				this._userTokenListByNsId.Remove( userToken.netSessionID );
				userToken.netSessionID = 0;
			}
			userToken.reconnectCount = 0;
		}

		public void ClearAllUserToken()
		{
			this._userTokenList.Clear();
			this._userTokenListByNsId.Clear();
		}

		public bool IsUserCanLogin( string sUserName, string sToken, uint nsId )
		{
			if ( GSKernel.instance.gsConfig.n32SkipBalance > 0 )
				return true;

			if ( !this._userTokenList.TryGetValue( sUserName, out SUserToken userToken ) )
				return false;

			if ( userToken.userToken == sToken )
			{
				++userToken.reconnectCount;
				if ( userToken.reconnectCount > int.MaxValue )
					this._userTokenList.Remove( sUserName ); //最大重练次数限制
				else
				{
					if ( userToken.netSessionID > 0 )
						this._userTokenListByNsId.Remove( userToken.netSessionID );
					userToken.overTime = 0;
					userToken.netSessionID = nsId;
					this._userTokenListByNsId[nsId] = userToken;
				}

				return true;
			}
			return false;
		}

		public void OnUserLost( uint nsId )
		{
			if ( this._userTokenListByNsId.TryGetValue( nsId, out SUserToken userToken ) )
			{
				userToken.netSessionID = 0;
				userToken.overTime = TimeUtils.utcTime + 60000;
				this._userTokenListByNsId.Remove( nsId );
			}

			GSToCS.UserOffLine sUserOffLineToCs = new GSToCS.UserOffLine();
			sUserOffLineToCs.Usernetid = ( int )nsId;
			byte[] data = sUserOffLineToCs.ToByteArray();
			GSKernel.instance.netSessionMrg.TranMsgToSession( SessionType.ClientG2C, data, 0, data.Length, ( int )GSToCS.MsgID.EMsgToCsfromGsUserOffLine, 0, 0 );
		}

		public EResult ChechUserToken( long time )
		{
			if ( time < this._lastReprot )
				return EResult.Normal;

			this._lastReprot = time + 2000;
			foreach ( KeyValuePair<string, SUserToken> kv in this._userTokenList )
			{
				SUserToken userToken = kv.Value;
				if ( userToken.netSessionID == 0 &&
					 time > userToken.overTime )//最大重练超时限制
				{
					this._tobeDeletes.Add( kv.Key );
				}
			}

			int count = this._tobeDeletes.Count;
			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
					this._userTokenList.Remove( this._tobeDeletes[i] );
				this._tobeDeletes.Clear();
			}
			return EResult.Normal;
		}

		public EResult ReportGsInfo( long time )
		{
			if ( this._nextReportTime > time )
				return EResult.Normal;
			this._nextReportTime = time + 2000;

			GSToBS.ReportAllClientInf sMsg = new GSToBS.ReportAllClientInf();
			sMsg.TokenlistSize = ( uint )this._userTokenList.Count;
			GSKernel.instance.netSessionMrg.SendMsgToSession( SessionType.ClientG2B, sMsg, ( int )GSToBS.MsgID.EMsgToBsfromGsReportAllClientInfo );
			return EResult.Normal;
		}
	}
}