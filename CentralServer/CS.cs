using CentralServer.Net;
using CentralServer.Tools;
using CentralServer.UserModule;
using Core.Misc;
using Core.Net;
using Google.Protobuf;
using Shared;
using Shared.Net;
using StackExchange.Redis;
using System;

namespace CentralServer
{
	public delegate void HeartbeatCallback( long curTime, long tickSpan );

	public class CS
	{
		private static CS _instance;
		public static CS instance => _instance ?? new CS();

		public CSKernelCfg csKernelCfg { get; private set; }
		public CSCfgMgr csCfg { get; private set; }
		public CSNetSessionMgr netSessionMgr { get; private set; }
		public CSUserMgr csUserMgr { get; private set; }
		public BattleTimer battleTimer { get; private set; }
		public SSNetInfo[] ssNetInfoList { get; private set; }
		public GSNetInfo[] gsNetInfoList { get; private set; }
		public RCNetInfo[] rcNetInfoList { get; private set; }
		public uint csID => this.csKernelCfg.unCSId;

		private ConnectionMultiplexer _userDBredisAsyncContext;

		private CS()
		{
			_instance = this;
		}

		public void Dispose()
		{
			this.netSessionMgr.Dispose();
			NetSessionPool.instance.Dispose();
		}

		public ErrorCode Initialize()
		{
			Console.Title = "CS";

			this.csCfg = new CSCfgMgr();
			ErrorCode eResult = this.csCfg.Load();
			if ( ErrorCode.Success != eResult )
			{
				Logger.Info( $"CS initialize failed with error code:{eResult}" );
				return eResult;
			}

			this.csKernelCfg = new CSKernelCfg();
			eResult = this.csKernelCfg.Load();
			if ( ErrorCode.Success != eResult )
			{
				Logger.Info( $"CS initialize failed with error code:{eResult}" );
				return eResult;
			}

			this.ssNetInfoList = new SSNetInfo[this.csKernelCfg.un32MaxSSNum];
			for ( int i = 0; i < this.csKernelCfg.un32MaxSSNum; i++ )
				this.ssNetInfoList[i] = new SSNetInfo();

			this.gsNetInfoList = new GSNetInfo[this.csKernelCfg.un32MaxGSNum];
			for ( int i = 0; i < this.csKernelCfg.un32MaxGSNum; i++ )
				this.gsNetInfoList[i] = new GSNetInfo();

			this.rcNetInfoList = new RCNetInfo[10];

			this.netSessionMgr = new CSNetSessionMgr();
			this.csUserMgr = new CSUserMgr();
			this.battleTimer = new BattleTimer();

			return eResult;
		}

		#region load config
		#endregion

		public ErrorCode Start()
		{
			this.netSessionMgr.CreateListener( this.csKernelCfg.n32GSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateGateSession );
			this.netSessionMgr.CreateListener( this.csKernelCfg.n32SSNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateSceneSession );
			this.netSessionMgr.CreateListener( this.csKernelCfg.n32RCNetListenerPort, 1024000, Consts.SOCKET_TYPE,
											   Consts.PROTOCOL_TYPE, 0, this.netSessionMgr.CreateRemoteConsoleSession );
			this.netSessionMgr.CreateConnector( SessionType.ClientC2Log, this.csKernelCfg.LogAddress,
												this.csKernelCfg.LogPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE, 102400,
												0 );

			ConfigurationOptions config = new ConfigurationOptions
			{
				EndPoints =
				{
					{ this.csKernelCfg.redisAddress, this.csKernelCfg.redisPort }
				},
				KeepAlive = 180,
				AbortOnConnectFail = false,
				Password = this.csKernelCfg.redisPwd
			};
			this._userDBredisAsyncContext = ConnectionMultiplexer.Connect( config );
			if ( !this._userDBredisAsyncContext.IsConnected )
				Logger.Error( "connect to redis fail." );

			//if ( this.csKernelCfg.redisLogicAddress != "0" )
			//{
			//	this.netSessionMgr.CreateConnector( SessionType.ClientC2LogicRedis, this.csKernelCfg.redisLogicAddress,
			//										this.csKernelCfg.redisLogicPort, Consts.SOCKET_TYPE, Consts.PROTOCOL_TYPE,
			//										102400, 0 );
			//}

			return ErrorCode.Success;
		}

		public void Update( long elapsed, long dt )
		{
			this.netSessionMgr.Update();
			this.csUserMgr.OnHeartBeatImmediately();
			//todo
		}

		/// <summary>
		/// 获取指定位置号的GS信息(配置文件定义的序号)
		/// </summary>
		public CSGSInfo GetGSInfoByGSID( uint gsID )
		{
			gsID -= this.csKernelCfg.un32GSBaseIdx;
			return gsID < this.csKernelCfg.un32MaxGSNum ? this.csKernelCfg.gsInfoList[gsID] : null;
		}

		/// <summary>
		/// 获取指定逻辑ID的GS信息
		/// </summary>
		public CSGSInfo GetGSInfoByNSID( uint nsID )
		{
			for ( int i = 0; i < this.csKernelCfg.un32MaxGSNum; ++i )
			{
				CSGSInfo csgsInfo = this.gsNetInfoList[i].pcGSInfo;
				if ( null == csgsInfo )
					continue;
				if ( csgsInfo.m_n32NSID == nsID )
					return csgsInfo;
			}
			return null;
		}

		/// <summary>
		/// 获取指定位置号的SS信息(配置文件定义的序号)
		/// </summary>
		public CSSSInfo GetSSInfoBySSID( uint ssID )
		{
			ssID -= this.csKernelCfg.un32SSBaseIdx;
			return ssID < this.csKernelCfg.un32MaxGSNum ? this.csKernelCfg.ssInfoList[ssID] : null;
		}

		/// <summary>
		/// 获取指定逻辑ID的SS信息
		/// </summary>
		public CSSSInfo GetSSInfoByNSID( uint nsID )
		{
			for ( int i = 0; i < this.csKernelCfg.un32MaxSSNum; ++i )
			{
				CSSSInfo pcInfo = this.ssNetInfoList[i].pcSSInfo;
				if ( null == pcInfo )
					continue;
				if ( pcInfo.m_n32NSID == nsID )
					return pcInfo;
			}
			return null;
		}

		public ErrorCode InvokeGCMsg( CSGSInfo csgsInfo, int msgID, uint gcNetID, byte[] data, int offset, int size ) =>
			this.csUserMgr.Invoke( csgsInfo, msgID, gcNetID, data, offset, size );

		public ConnectionMultiplexer GetUserDBCacheRedisHandler() => this._userDBredisAsyncContext;

		public long AddTimer( HeartbeatCallback pHeartbeatCallback, long interval, bool ifPersist ) =>
			this.battleTimer.AddTimer( pHeartbeatCallback, interval, ifPersist );

		public void RemoveTimer( long timerID ) => this.battleTimer.RemoveTimer( timerID );

		public ErrorCode PostMsgToGS_KickoutGC( CSGSInfo csgsInfo, uint gcNetID )
		{
			CSToGS.OrderKickoutGC sMsg = new CSToGS.OrderKickoutGC();
			sMsg.Gcnid = ( int )gcNetID;
			return this.PostMsgToGS( csgsInfo, sMsg, ( int )CSToGS.MsgID.EMsgToGsfromCsOrderKickoutGc, 0 );
		}

		public ErrorCode PostMsgToGS( CSGSInfo csgsInfo, IMessage msg, int msgID, uint gcNetID )
		{
			if ( csgsInfo == null )
				return ErrorCode.NullMsg;
			this.netSessionMgr.TranMsgToSession( csgsInfo.m_n32NSID, msg, msgID, gcNetID == 0 ? 0 : ( int )CSToGS.MsgID.EMsgToGsfromCsOrderPostToGc, gcNetID );
			return ErrorCode.Success;
		}

		public ErrorCode BroadCastMsgToGS( IMessage msg, int msgID )
		{
			this.netSessionMgr.TranMsgToSession( SessionType.ServerCsOnlyGS, msg, msgID, 0, 0, false );
			return ErrorCode.Success;
		}

		public ErrorCode PostMsgToGC( UserNetInfo userNetInfo, IMessage msg, int msgID )
		{
			if ( userNetInfo.gcNetID == 0 )
				return ErrorCode.InvalidUserNetInfo;
			this.PostMsgToGS( this.GetGSInfoByGSID( ( uint )userNetInfo.gsID ), msg, msgID, userNetInfo.gcNetID );
			return ErrorCode.Success;
		}

		public ErrorCode PostMsgToLogServer( IMessage msg, int msgID )
		{
			this.netSessionMgr.SendMsgToSession( SessionType.ClientC2Log, msg, msgID );
			return ErrorCode.Success;
		}
	}
}