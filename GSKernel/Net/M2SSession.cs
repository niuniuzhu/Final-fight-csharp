using Core;
using Google.Protobuf;
using Shared.Net;

namespace GateServer.Net
{
	public class M2SSession : CliSession
	{
		protected M2SSession( int id ) : base( id )
		{
		}

		protected override void SendInitData()
		{
			GSSSInfo pcSSInfo = GSKernel.instance.GetGSSSInfoBySSID( this.logicID );
			if ( pcSSInfo == null )
			{
				Logger.Error( string.Empty );
				return;
			}
			Logger.Info( $"SS({pcSSInfo.m_n32SSID}) Connected, try to register me." );
			pcSSInfo.m_n32NSID = this.id;
			GSToSS.AskRegiste askRegiste = new GSToSS.AskRegiste()
			{
				Gsid = GSKernel.instance.gsConfig.n32GSID,
				Pwd = GSKernel.instance.gsConfig.aszMyUserPwd
			};
			ByteString bs = askRegiste.ToByteString();
			GSKernel.instance.TransToSS( pcSSInfo, bs, ( int )GSToSS.MsgID.EMsgToSsfromGsAskRegiste, 0, 0 );
		}
	}
}