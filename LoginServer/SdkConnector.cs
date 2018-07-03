using Core.Misc;
using Core.Structure;
using Shared;
using System.Collections.Generic;

namespace LoginServer
{
	public class SdkConnector
	{
		private readonly Dictionary<string, LoginUserInfo> _allLoginUserInfo = new Dictionary<string, LoginUserInfo>();
		private readonly ThreadSafeObejctPool<SDKBuffer> _dbCallbackQueuePool = new ThreadSafeObejctPool<SDKBuffer>();
		private readonly SwitchQueue<SDKBuffer> _dbCallbackQueue = new SwitchQueue<SDKBuffer>();

		public LoginUserInfo GetLoginUserInfo( string sessionID )
		{
			this._allLoginUserInfo.TryGetValue( sessionID, out LoginUserInfo info );
			return info;
		}

		public void RemoveLoginUserInfo( string sessionID )
		{
			this._allLoginUserInfo.Remove( sessionID );
		}

		public void Update()
		{
			this._dbCallbackQueue.Switch();
			while ( !this._dbCallbackQueue.isEmpty )
			{
				SDKBuffer sdkBuffer = this._dbCallbackQueue.Pop();
				if ( sdkBuffer.data == 1 )
				{
					uint gcNetID = sdkBuffer.ReadUInt();
					uint platform = sdkBuffer.ReadUInt();
					string uid = sdkBuffer.ReadUTF8();
					string uin = sdkBuffer.ReadUTF8();
					string sessionID = sdkBuffer.ReadUTF8E();
					LoginUserInfo loginUserInfo = new LoginUserInfo
					{
						sessionid = sessionID,
						uin = uin,
						plat = platform
					};
					this._allLoginUserInfo[sessionID] = loginUserInfo;
					Logger.Log( $"add uid:{uid}, sessionID:{sessionID}" );

					this.PostMsgToGC_NotifyServerList( gcNetID );
				}
				else if ( sdkBuffer.data == 2 )
				{
					uint gcNetID = sdkBuffer.ReadUInt();
					int errorCode = sdkBuffer.ReadInt();
					Logger.Log( $"user login fail with netID:{gcNetID}, errorCode:{errorCode}." );
					this.PostMsgToGC_NotifyLoginFail( errorCode, gcNetID );
				}
				sdkBuffer.Clear();
				this._dbCallbackQueuePool.Push( sdkBuffer );
			}
		}

		/// <summary>
		/// 该方法为异步方法
		/// </summary>
		public void SendToInsertData( string uid, LoginUserInfo loginInfo, uint gcNetID )
		{
			SDKBuffer sdkBuffer = this._dbCallbackQueuePool.Pop();
			sdkBuffer.Write( gcNetID );
			sdkBuffer.Write( loginInfo.plat );
			sdkBuffer.WriteUTF8( uid );
			sdkBuffer.WriteUTF8( loginInfo.uin );
			sdkBuffer.WriteUTF8E( loginInfo.sessionid );
			sdkBuffer.position = 0;
			sdkBuffer.data = 1;
			this._dbCallbackQueue.Push( sdkBuffer );
		}

		/// <summary>
		/// 该方法为异步方法
		/// </summary>
		public void SendToFailData( ErrorCode errorCode, uint gcNetID )
		{
			SDKBuffer sdkBuffer = this._dbCallbackQueuePool.Pop();
			sdkBuffer.Write( gcNetID );
			sdkBuffer.Write( ( int )errorCode );
			sdkBuffer.position = 0;
			sdkBuffer.data = 2;
			Logger.Log( $"user login fail with netID:{gcNetID}, errorCode:{errorCode}." );
			this._dbCallbackQueue.Push( sdkBuffer );
		}

		private void PostMsgToGC_NotifyServerList( uint gcNetID )
		{
			//发送第2消息：登录成功，下发BS服务器列表
			LSToGC.ServerBSAddr serverList = new LSToGC.ServerBSAddr();
			List<ServerAddr> serverAddrs = LS.instance.lsConfig.gAllServerAddr;
			foreach ( ServerAddr serverAddr in serverAddrs )
			{
				LSToGC.ServerInfo info = new LSToGC.ServerInfo
				{
					ServerName = serverAddr.str_name,
					ServerAddr = serverAddr.str_addr,
					ServerPort = serverAddr.str_port
				};
				serverList.Serverinfo.Add( info );
			}
			Logger.Log( "Post Server List To User." );
			LS.instance.netSessionMgr.SendMsgToSession( gcNetID, serverList, ( int )LSToGC.MsgID.EMsgToGcfromLsNotifyServerBsaddr );
		}

		private void PostMsgToGC_NotifyLoginFail( int errorCode, uint gcNetID )
		{
			// 发送第1消息：登录失败
			LSToGC.LoginResult msg = new LSToGC.LoginResult { Result = errorCode };
			LS.instance.netSessionMgr.SendMsgToSession( gcNetID, msg, ( int )LSToGC.MsgID.EMsgToGcfromLsNotifyLoginResult );
		}
	}
}