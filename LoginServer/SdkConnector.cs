using Core.Misc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Shared;

namespace LoginServer
{
	public class SdkConnector
	{
		private readonly Dictionary<string, LoginUserInfo> _allLoginUserInfo = new Dictionary<string, LoginUserInfo>();
		private readonly ThreadSafeObejctPool<SDKBuffer> _dbCallbackQueuePool = new ThreadSafeObejctPool<SDKBuffer>();
		private readonly ConcurrentQueue<SDKBuffer> _dbCallbackQueue = new ConcurrentQueue<SDKBuffer>();

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
			while ( !this._dbCallbackQueue.IsEmpty )
			{
				this._dbCallbackQueue.TryDequeue( out SDKBuffer pBuffer );
				if ( pBuffer.data == 1 )
				{
					int gcnetid = pBuffer.ReadInt();
					string uid = pBuffer.ReadUTF8();
					string uin = pBuffer.ReadUTF8();
					string sessionid = pBuffer.ReadUTF8();
					uint platform = pBuffer.ReadUInt();

					LoginUserInfo pLoginUserInfo = new LoginUserInfo();
					pLoginUserInfo.sessionid = sessionid;
					pLoginUserInfo.uin = uin;
					pLoginUserInfo.plat = platform;

					this._allLoginUserInfo[sessionid] = pLoginUserInfo;
					Logger.Log( $"Add uid:{uid}, sessionid{sessionid}" );

					this.PostMsgToGC_NotifyServerList( ( uint )gcnetid );
				}
				else if ( pBuffer.data == 2 )
				{
					int gcnetid = pBuffer.ReadInt();
					int errorcode = pBuffer.ReadInt();
					Logger.Log( $"User Login Fail with netid:{gcnetid}, errorcode:{errorcode}." );
					this.PostMsgToGC_NotifyLoginFail( errorcode, ( uint )gcnetid );
				}

				this._dbCallbackQueuePool.Push( pBuffer );
			}
		}

		public void SendToInsertData( string uid, LoginUserInfo loginInfo, int gcnetid )
		{
			SDKBuffer pBuffer = this._dbCallbackQueuePool.Pop();
			pBuffer.Write( gcnetid );
			pBuffer.WriteUTF8( uid );
			pBuffer.WriteUTF8( loginInfo.uin );
			pBuffer.WriteUTF8( loginInfo.sessionid );
			pBuffer.Write( loginInfo.plat );
			pBuffer.data = 1;
			this._dbCallbackQueue.Enqueue( pBuffer );
		}

		public void SendToFailData( ErrorCode errorcode, uint gcnetID )
		{
			SDKBuffer pBuffer = this._dbCallbackQueuePool.Pop();
			pBuffer.Write( gcnetID );
			pBuffer.Write( ( int )errorcode );
			pBuffer.data = 2;
			Logger.Log( $"User Login Fail with netid:{gcnetID}, errorcode:{errorcode}." );
			this._dbCallbackQueue.Enqueue( pBuffer );
		}

		private void PostMsgToGC_NotifyServerList( uint gcnetid )
		{
			// 发送第2消息：登录成功，bs服务器列表
			LSToGC.ServerBSAddr ServerList = new LSToGC.ServerBSAddr();
			Dictionary<uint, ServerAddr> serverAddr = LS.instance.lsConfig.gAllServerAddr;
			foreach ( KeyValuePair<uint, ServerAddr> kv in serverAddr )
			{
				LSToGC.ServerInfo pInfo = new LSToGC.ServerInfo();
				pInfo.ServerName = kv.Value.str_name;
				pInfo.ServerAddr = kv.Value.str_addr;
				pInfo.ServerPort = kv.Value.str_port;
				ServerList.Serverinfo.Add( pInfo );
			}
			Logger.Log( "Post Server List To User." );
			LS.instance.netSessionMgr.SendMsgToSession( gcnetid, ServerList, ( int )LSToGC.MsgID.EMsgToGcfromLsNotifyServerBsaddr );
		}

		private void PostMsgToGC_NotifyLoginFail( int errorcode, uint gcnetid )
		{
			// 发送第1消息：登录失败
			LSToGC.LoginResult sMsg = new LSToGC.LoginResult();
			sMsg.Result = errorcode;
			LS.instance.netSessionMgr.SendMsgToSession( gcnetid, sMsg, ( int )LSToGC.MsgID.EMsgToGcfromLsNotifyLoginResult );
		}
	}
}