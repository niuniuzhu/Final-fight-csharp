using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;
using System.Collections.Generic;

namespace GateServer.Net
{
	public class GSSSInfo
	{
		//property from config file.
		public int ssID;
		public string userPwd;
		//property from scene server.
		public string listenIp;
		public int listenPort;
		//property from local.
		public EServerNetState ssNetState;
		public uint nsID;
		public uint connTimes;
		public long lastConnMilsec;
		public long pingTickCounter;

		public long msgReceived;
		public long msgSent;
		public long dataReceived;
		public long dataSent;
	}

	public struct SUserToken
	{
		public string userName;
		public string userToken;
		public long overTime;
		public uint reconnectCount;
		public uint netSessionID;
	}

	public class GSStorage
	{
		public uint csNetSessionId;
		public long csTimeError;
		public uint ssBaseIdx;
		public int ssConnectNum;

		#region user to ssInfo
		private readonly Dictionary<uint, GSSSInfo> _user2SSInfoMap = new Dictionary<uint, GSSSInfo>();
		private readonly List<uint> _userToDelete = new List<uint>();

		/// <summary>
		/// 添加场景服务器信息
		/// </summary>
		public void AddUserSSInfo( uint gcNetID, GSSSInfo gsssInfo ) => this._user2SSInfoMap[gcNetID] = gsssInfo;

		/// <summary>
		/// 移除指定场景服务器信息
		/// </summary>
		public bool RemoveUserSSInfo( uint gcNetID ) => this._user2SSInfoMap.Remove( gcNetID );

		/// <summary>
		/// 获取本地场景服务器信息
		/// </summary>

		public GSSSInfo GetUserSSInfo( uint gcNetID )
		{
			this._user2SSInfoMap.TryGetValue( gcNetID, out GSSSInfo ssInfo );
			return ssInfo;
		}

		/// <summary>
		/// 是否包含指定场景服务器id的信息
		/// </summary>
		public bool ContainsUserSSInfo( uint gcNetID ) => this._user2SSInfoMap.ContainsKey( gcNetID );

		public void OnSSClosed( GSSSInfo ssInfo )
		{
			foreach ( KeyValuePair<uint, GSSSInfo> kv in this._user2SSInfoMap )
			{
				if ( ssInfo != kv.Value )
					continue;
				GSKernel.instance.PostGameClientDisconnect( kv.Key );
				this._userToDelete.Add( kv.Key );
			}
			int count = this._userToDelete.Count;
			if ( count <= 0 )
				return;
			for ( int i = 0; i < count; i++ )
				this._user2SSInfoMap.Remove( this._userToDelete[i] );
			this._userToDelete.Clear();
		}
		#endregion

		#region ssInfo
		private readonly Dictionary<int, GSSSInfo> _GSSSInfoMap = new Dictionary<int, GSSSInfo>();

		/// <summary>
		/// 添加场景服务器信息
		/// </summary>
		public void AddSSInfo( int ssID, GSSSInfo gsssInfo ) => this._GSSSInfoMap[ssID] = gsssInfo;

		/// <summary>
		/// 移除指定场景服务器信息
		/// </summary>
		public bool RemoveSSInfo( int ssID ) => this._GSSSInfoMap.Remove( ssID );

		/// <summary>
		/// 获取本地场景服务器信息
		/// </summary>
		public GSSSInfo GetSSInfo( int ssID )
		{
			this._GSSSInfoMap.TryGetValue( ssID, out GSSSInfo ssInfo );
			return ssInfo;
		}

		/// <summary>
		/// 是否包含指定场景服务器id的信息
		/// </summary>
		public bool ContainsSSInfo( int ssID ) => this._GSSSInfoMap.ContainsKey( ssID );

		/// <summary>
		/// 向每个场景服务器发送ping
		/// </summary>
		public EResult PingSS( long utcTime )
		{
			if ( this._GSSSInfoMap.Count == 0 )
				return EResult.Normal;

			foreach ( KeyValuePair<int, GSSSInfo> kv in this._GSSSInfoMap )
			{
				GSSSInfo ssInfo = kv.Value;
				if ( 0 == ssInfo.ssID || 0 == ssInfo.nsID )
					continue;

				if ( utcTime - ssInfo.pingTickCounter < Consts.DEFAULT_PING_CD_TICK )
					continue;

				GSToSS.AskPing sMsg = new GSToSS.AskPing { Time = utcTime };
				byte[] data = sMsg.ToByteArray();
				GSKernel.instance.TranMsgToSession( ssInfo.nsID, data, 0, data.Length, ( int )GSToSS.MsgID.EMsgToSsfromGsAskPing, 0, 0 );
				ssInfo.pingTickCounter = utcTime;
			}
			return EResult.Normal;
		}
		#endregion

		#region usertoken
		private readonly Dictionary<string, SUserToken> _userTokenList = new Dictionary<string, SUserToken>();
		private readonly Dictionary<uint, SUserToken> _userTokenListByNsId = new Dictionary<uint, SUserToken>();
		private readonly List<string> _tokensToDelete = new List<string>();
		private long _lastReprot;
		private long _nextReportTime;

		public void AddUserToken( string userName, string token )
		{
			if ( !this._userTokenList.TryGetValue( userName, out SUserToken userToken ) )
				userToken = new SUserToken();
			userToken.userName = userName;
			userToken.userToken = token;
			userToken.overTime = TimeUtils.utcTime + 60000;
			if ( userToken.netSessionID > 0 )
			{
				this._userTokenListByNsId.Remove( userToken.netSessionID );
				userToken.netSessionID = 0;
			}
			userToken.reconnectCount = 0;
			this._userTokenList[userName] = userToken;
		}

		public void ClearAllUserToken()
		{
			this._userTokenList.Clear();
			this._userTokenListByNsId.Clear();
		}

		public bool IsUserCanLogin( string userName, string token, uint nsId )
		{
			if ( GSKernel.instance.gsConfig.n32SkipBalance > 0 )
				return true;

			if ( !this._userTokenList.TryGetValue( userName, out SUserToken userToken ) )
				return false;

			if ( userToken.userToken == token )
			{
				++userToken.reconnectCount;
				if ( userToken.reconnectCount > int.MaxValue )
					this._userTokenList.Remove( userName ); //最大重练次数限制
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
			this._userTokenListByNsId.Remove( nsId );

			GSToCS.UserOffLine sUserOffLineToCs = new GSToCS.UserOffLine();
			sUserOffLineToCs.Usernetid = ( int )nsId;
			byte[] data = sUserOffLineToCs.ToByteArray();
			GSKernel.instance.TranMsgToSession( SessionType.ClientG2C, data, 0, data.Length, ( int )GSToCS.MsgID.EMsgToCsfromGsUserOffLine, 0, 0 );
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
					this._tokensToDelete.Add( kv.Key );
				}
			}

			int count = this._tokensToDelete.Count;
			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
					this._userTokenList.Remove( this._tokensToDelete[i] );
				this._tokensToDelete.Clear();
			}
			return EResult.Normal;
		}

		/// <summary>
		/// 向负载均衡服务器报告网关服务器的状态
		/// </summary>
		public EResult ReportGsInfo( long time )
		{
			if ( this._nextReportTime > time )
				return EResult.Normal;
			this._nextReportTime = time + 2000;

			GSToBS.ReportAllClientInf sMsg = new GSToBS.ReportAllClientInf();
			sMsg.TokenlistSize = ( uint )this._userTokenList.Count;
			GSKernel.instance.SendMsgToSession( SessionType.ClientG2B, sMsg, ( int )GSToBS.MsgID.EMsgToBsfromGsReportAllClientInfo );
			return EResult.Normal;
		}
		#endregion
	}
}