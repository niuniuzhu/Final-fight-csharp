using Core.Misc;
using Core.Structure;
using Shared;
using System.Collections.Generic;
using System.Threading;

namespace LoginServer
{
	public class SDKAsynHandler
	{
		public class UserLoginData
		{
			public uint platFrom;
			public string sessionid;
			public string uin;
		}

		private readonly SwitchQueue<SDKBuffer> m_SDKCallbackQueue = new SwitchQueue<SDKBuffer>();
		private readonly ThreadSafeObejctPool<SDKBuffer> m_SDKCallbackQueuePool = new ThreadSafeObejctPool<SDKBuffer>();
		private readonly Dictionary<uint, UserLoginData> m_UserLoginDataMap = new Dictionary<uint, UserLoginData>();
		private readonly object m_UserLoginDataMapMutex = new object();
		private int _checkTime = 100;
		private Timer _timer;

		public void Dispose()
		{
			this._timer.Dispose();
		}

		public void Init()
		{
			this._timer = new Timer( this.OnTimer, null, 0, this._checkTime );
		}

		private void OnTimer( object state )
		{
			this.m_SDKCallbackQueue.Switch();
			while ( !this.m_SDKCallbackQueue.isEmpty )
			{
				SDKBuffer sdkBuffer = this.m_SDKCallbackQueue.Pop();
				if ( sdkBuffer.data == ( int )GCToLS.MsgID.EMsgToLsfromGcAskLogin )
				{
					uint gcnetID = sdkBuffer.ReadUInt();
					EUserPlatform eplat = ( EUserPlatform )sdkBuffer.ReadInt();
					if ( IfTestPlatform( eplat ) )
						this.AsynHandleLoiginCheckMsg_PC( gcnetID );
					// 其他渠道暂不实现 todo
					//else
					//{
					//bool bState = IfPostVisit( eplat );
					//string str = sdkBuffer.ReadUTF8E();
					//new_conn( gcnetID, sdkBuffer.data, str, this.gGlobalInfoInstance, eplat, bState );
					//}
				}
				sdkBuffer.position = 0;
				this.m_SDKCallbackQueuePool.Push( sdkBuffer );
			}
		}

		public EResult CheckLogin( GCToLS.AskLogin askLogin, int msgID, uint gcnetID )
		{
			EUserPlatform platform = ( EUserPlatform )askLogin.Platform;
			UserLoginData loginData = new UserLoginData
			{
				platFrom = askLogin.Platform,
				sessionid = askLogin.Sessionid,
				uin = askLogin.Uin
			};
			lock ( this.m_UserLoginDataMapMutex )
			{
				if ( this.m_UserLoginDataMap.ContainsKey( gcnetID ) )
				{
					Logger.Warn( $"客户端({askLogin.Uin})多次登录！！但服务器数据还没返回数据给客户端" );
					return EResult.Normal;
				}
				this.m_UserLoginDataMap[gcnetID] = loginData;
			}
			Logger.Log( $"GC Try To Login with uin:{askLogin.Uin}({gcnetID}), sessionid:{askLogin.Sessionid}, platform:{platform}" );

			string sendData = string.Empty;
			switch ( platform )
			{
				case EUserPlatform.ePlatform_PC:
					sendData = "PCTest";
					break;

				default:
					this.PostToLoginFailQueue( ErrorCode.EC_UnknowPlatform, gcnetID );
					break;
			}
			Logger.Log( $"{sendData}" );
			this.PostMsg( sendData, msgID, gcnetID, platform );
			return 0;
		}

		private void PostMsg( string msg, int msgID, uint gcnetID, EUserPlatform eplat )
		{
			SDKBuffer pBuffer = this.m_SDKCallbackQueuePool.Pop();
			pBuffer.Write( gcnetID );
			pBuffer.Write( ( int )eplat );
			pBuffer.WriteUTF8E( msg );
			pBuffer.position = 0;
			pBuffer.data = msgID;
			this.m_SDKCallbackQueue.Push( pBuffer );
		}

		private void AsynHandleLoiginCheckMsg_PC( uint gcnetID )
		{
			UserLoginData sUserData;
			lock ( this.m_UserLoginDataMapMutex )
			{
				if ( !this.m_UserLoginDataMap.TryGetValue( gcnetID, out sUserData ) )
				{
					this.PostToLoginFailQueue( ErrorCode.EC_UserNotExist, gcnetID );
					return;
				}
				this.m_UserLoginDataMap.Remove( gcnetID );
			}

			string temp = string.Empty;
			if ( sUserData.platFrom == ( uint )EUserPlatform.ePlatform_PC )
			{
				temp += sUserData.platFrom;
				temp += sUserData.uin;
			}
			else
				temp = sUserData.sessionid;

			LoginUserInfo sTempInfo = new LoginUserInfo
			{
				plat = sUserData.platFrom,
				sessionid = temp,
				uin = sUserData.uin
			};
			LS.instance.sdkConnector.SendToInsertData( sUserData.uin, sTempInfo, gcnetID );
		}

		private void PostToLoginFailQueue( ErrorCode ErrorCode, uint gcnetID )
		{
			Logger.Warn( $"Fail with netid:{gcnetID}." );
			LS.instance.sdkConnector.SendToFailData( ErrorCode, gcnetID );
		}

		private static bool IfTestPlatform( EUserPlatform eplat )
		{
			switch ( eplat )
			{
				case EUserPlatform.ePlatform_PC:
				case EUserPlatform.ePlatformAndroid_UC:
				case EUserPlatform.ePlatformiOS_OnlineGame:
					return true;
				default:
					return false;
			}
		}
	}
}