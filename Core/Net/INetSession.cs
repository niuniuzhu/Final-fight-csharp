namespace Core.Net
{
	public interface INetSession
	{
		uint id { get; }
		IConnection connection { get; }
		void Dispose();
		void Close();
		void OnEstablish();
		void OnTerminate();
		void OnError( string error );
		void OnRecv( byte[] data, int offset, int size );
		void OnSend();
	}
}