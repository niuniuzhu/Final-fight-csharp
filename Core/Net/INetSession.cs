namespace Core.Net
{
	public interface INetSession
	{
		IConnection connection { get; }
		void Dispose();
		void Release();
		void OnEstablish();
		void OnTerminate();
		void OnError( int moduleErr, int sysErr );
		void OnRecv( byte[] buf, int size );
	}
}