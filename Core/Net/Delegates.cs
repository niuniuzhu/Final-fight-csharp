namespace Core.Net
{
	public delegate byte[] PacketEncodeHandler( byte[] data, int offset, int size );
	public delegate int PacketDecodeHandler( byte[] buf, int offset, int size, out byte[] data );
	public delegate INetSession SessionCreateHandler();
}