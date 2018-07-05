using Core.Misc;
using Core.Structure;
using Shared;
using System.Collections.Generic;
using System.Threading;

namespace LoginServer
{
	public class SDKAsynHandler
	{
		private class UserLoginData
		{
			public uint platFrom;
			public string sessionid;
			public string uin;
		}

		private readonly SwitchQueue<GBuffer> _sdkCallbackQueue = new SwitchQueue<GBuffer>();
		private readonly ThreadSafeObejctPool<GBuffer> _sdkCallbackQueuePool = new ThreadSafeObejctPool<GBuffer>();
		private readonly Dictionary<uint, UserLoginData> _userLoginDataMap = new Dictionary<uint, UserLoginData>();
		private readonly object _userLoginDataMapMutex = new object();
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
			this._sdkCallbackQueue.Switch();
			while ( !this._sdkCallbackQueue.isEmpty )
			{
				GBuffer gBuffer = this._sdkCallbackQueue.Pop();
				if ( gBuffer.data == ( int )GCToLS.MsgID.EMsgToLsfromGcAskLogin )
				{
					uint gcnetID = gBuffer.ReadUInt();
					UserPlatform eplat = ( UserPlatform )gBuffer.ReadInt();
					if ( IfTestPlatform( eplat ) )
						this.AsynHandleLoiginCheckMsg_PC( gcnetID );
					// 其他渠道暂不实现 todo
					//else
					//{
					//bool bState = IfPostVisit( platform );
					//string str = gBuffer.ReadUTF8E();
					//new_conn( gcNetID, gBuffer.data, str, this.gGlobalInfoInstance, platform, bState );
					//}
				}
				gBuffer.position = 0;
				this._sdkCallbackQueuePool.Push( gBuffer );
			}
		}

		public ErrorCode CheckLogin( GCToLS.AskLogin askLogin, int msgID, uint gcnetID )
		{
			UserPlatform platform = ( UserPlatform )askLogin.Platform;
			UserLoginData loginData = new UserLoginData
			{
				platFrom = askLogin.Platform,
				sessionid = askLogin.Sessionid,
				uin = askLogin.Uin
			};
			lock ( this._userLoginDataMapMutex )
			{
				if ( this._userLoginDataMap.ContainsKey( gcnetID ) )
				{
					Logger.Warn( $"client({askLogin.Uin}) login multiple times, but the server data has not been returned to the client" );
					return ErrorCode.Success;
				}
				this._userLoginDataMap[gcnetID] = loginData;
			}
			Logger.Log( $"GC Try To Login with uin:{askLogin.Uin}({gcnetID}), sessionid:{askLogin.Sessionid}, platform:{platform}" );

			string sendData = string.Empty;
			switch ( platform )
			{
				case UserPlatform.PC:
					sendData = "PCTest";
					break;

				default:
					this.PostToLoginFailQueue( ErrorCode.UnknowPlatform, gcnetID );
					break;
			}
			Logger.Log( $"{sendData}" );
			this.PostMsg( sendData, msgID, gcnetID, platform );
			return 0;
		}

		private void PostMsg( string msg, int msgID, uint gcnetID, UserPlatform eplat )
		{
			GBuffer pBuffer = this._sdkCallbackQueuePool.Pop();
			pBuffer.Write( gcnetID );
			pBuffer.Write( ( int )eplat );
			pBuffer.WriteUTF8E( msg );
			pBuffer.position = 0;
			pBuffer.data = msgID;
			this._sdkCallbackQueue.Push( pBuffer );
		}

		private void AsynHandleLoiginCheckMsg_PC( uint gcNetID )
		{
			UserLoginData userLoginData;
			lock ( this._userLoginDataMapMutex )
			{
				if ( !this._userLoginDataMap.TryGetValue( gcNetID, out userLoginData ) )
				{
					this.PostToLoginFailQueue( ErrorCode.UserNotExist, gcNetID );
					return;
				}
				this._userLoginDataMap.Remove( gcNetID );
			}

			string temp = string.Empty;
			if ( userLoginData.platFrom == ( uint )UserPlatform.PC )
			{
				temp += userLoginData.platFrom;
				temp += userLoginData.uin;
			}
			else
				temp = userLoginData.sessionid;

			LoginUserInfo tempInfo = new LoginUserInfo
			{
				plat = userLoginData.platFrom,
				sessionid = temp,
				uin = userLoginData.uin
			};
			LS.instance.sdkConnector.SendToInsertData( userLoginData.uin, tempInfo, gcNetID );
		}

		private void PostToLoginFailQueue( ErrorCode ErrorCode, uint gcNetID )
		{
			Logger.Warn( $"fail with gcNetID:{gcNetID}." );
			LS.instance.sdkConnector.SendToFailData( ErrorCode, gcNetID );
		}

		private static bool IfTestPlatform( UserPlatform platform )
		{
			switch ( platform )
			{
				case UserPlatform.PC:
				case UserPlatform.AndroidUC:
				case UserPlatform.OnlineGame:
					return true;
				default:
					return false;
			}
		}
	}
}