using Core.Misc;
using GCToLS;
using Shared;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LoginServer
{
	public class SDKAsynHandler
	{
		private readonly ConcurrentQueue<SDKBuffer> m_SDKCallbackQueue = new ConcurrentQueue<SDKBuffer>();
		private readonly ThreadSafeObejctPool<SDKBuffer> m_SDKCallbackQueuePool = new ThreadSafeObejctPool<SDKBuffer>();
		private readonly Dictionary<uint, UserLoginData> m_UserLoginDataMap = new Dictionary<uint, UserLoginData>();
		private readonly object m_UserLoginDataMapMutex = new object();

		public EResult CheckLogin( AskLogin askLogin, int msgID, uint gcnetID )
		{
			EUserPlatform un32platform = ( EUserPlatform )askLogin.Platform;
			UserLoginData pData = new UserLoginData();
			pData.platFrom = askLogin.Platform;
			pData.sessionid = askLogin.Sessionid;
			pData.uin = askLogin.Uin;
			lock ( this.m_UserLoginDataMapMutex )
			{
				if ( this.m_UserLoginDataMap.ContainsKey( gcnetID ) )
				{
					Logger.Warn( $"玩家({askLogin.Uin})多次登录！！但服务器数据还没返回数据给客户端" );
					return EResult.Normal;
				}

				this.m_UserLoginDataMap[gcnetID] = pData;
			}

			Logger.Log( $"GC Try To Login with uin:{askLogin.Uin}({gcnetID}), sessionid:{askLogin.Sessionid}, platform:{un32platform}" );

			string sSendData = string.Empty;
			switch ( un32platform )
			{
				case EUserPlatform.ePlatform_PC:
					sSendData = "PCTest";
					break;

				default:
					this.PostToLoginFailQueue( ErrorCode.EC_UnknowPlatform, gcnetID );
					break;
			}
			Logger.Log( $"{sSendData}" );
			this.PostMsg( sSendData, msgID, gcnetID, un32platform );
			return 0;
		}

		private void PostMsg( string msg, int msgID, uint gcnetID, EUserPlatform eplat )
		{
			SDKBuffer pBuffer = this.m_SDKCallbackQueuePool.Pop();
			pBuffer.Write( gcnetID );
			pBuffer.Write( ( int )eplat );
			pBuffer.WriteUTF8( msg );
			pBuffer.data = msgID;
			this.m_SDKCallbackQueue.Enqueue( pBuffer );
		}

		public void Init()
		{
		}

		private void PostToLoginFailQueue( ErrorCode ErrorCode, uint gcnetID )
		{
			Logger.Warn( $"Fail with netid:{gcnetID}." );
			LS.instance.sdkConnector.SendToFailData( ErrorCode, gcnetID );
		}
	}
}