using Core;
using Core.Misc;
using Google.Protobuf;
using Shared;
using Shared.Net;

namespace GateServer.Net
{
	public class M2SSession : CliSession
	{
		protected M2SSession( uint id ) : base( id )
		{
			this._handlerContainer.Register( ( int )SSToGS.MsgID.EMsgToGsfromSsAskRegisteRet, this.MsgInitHandler );
		}

		protected override void SendInitData()
		{
			GSSSInfo ssInfo = GSKernel.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.ssID}) Connected, try to register me." );
			ssInfo.nsID = this.id;
			GSToSS.AskRegiste askRegiste = new GSToSS.AskRegiste()
			{
				Gsid = GSKernel.instance.gsConfig.n32GSID,
				Pwd = GSKernel.instance.gsConfig.aszMyUserPwd
			};
			byte[] data = askRegiste.ToByteArray();
			this.owner.TranMsgToSession( ssInfo.nsID, data, 0, data.Length, ( int )GSToSS.MsgID.EMsgToSsfromGsAskRegiste, 0, 0 );
		}

		protected override void OnRealEstablish()
		{
			GSSSInfo ssInfo = GSKernel.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.ssID}) Connected and register ok." );
			ssInfo.nsID = this.id;
			ssInfo.lastConnMilsec = TimeUtils.utcTime;
			ssInfo.pingTickCounter = 0;
		}

		protected override void OnClose()
		{
			GSSSInfo ssInfo = GSKernel.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({ssInfo.ssID}) DisConnect." );
			//GSKernel.instance.OnEvent( EVT_ON_SS_DISCONNECT, pcSSInfo ); //todo
			ssInfo.nsID = 0;
		}

		#region msg handlers
		private bool MsgInitHandler( byte[] data, int offset, int size, int msgID )
		{
			// don't send any message until it init success.
			GSSSInfo ssInfo = GSKernel.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo == null || data == null )
			{
				Logger.Error( string.Empty );
				return false;
			}

			offset += 2 * sizeof( int );
			size -= 2 * sizeof( int );
			SSToGS.AskRegisteRet askRegisteRet = new SSToGS.AskRegisteRet();
			askRegisteRet.MergeFrom( data, offset, size );

			if ( ( int )EResult.Normal != askRegisteRet.State )
			{
				Logger.Warn( $"register to SS {ssInfo.ssID} Fail with error code {askRegisteRet.State}." );
				return false;
			}

			ssInfo.ssNetState = EServerNetState.SnsFree;
			Logger.Info( $"register to SS {ssInfo.ssID} success at session {ssInfo.nsID}." );
			this.SetInited( true, true );

			return true;
		}

		protected override bool HandleUnhandledMsg( byte[] data, int offset, int size, int msgID )
		{
			int realMsgID = 0;
			uint gcNetID = 0;
			offset += ByteUtils.Decode32i( data, offset, ref realMsgID );
			offset += ByteUtils.Decode32u( data, offset, ref gcNetID );
			size -= 2 * sizeof( int );
			GSSSInfo ssInfo = GSKernel.instance.gsStorage.GetSSInfo( this.logicID );
			if ( ssInfo != null )
				GSKernel.instance.ssMsgManager.HandleUnhandledMsg( ssInfo, data, offset, size, realMsgID, msgID, gcNetID );
			return true;
		}
		#endregion
	}
}