using System.IO;
using System.Text;

namespace Core.Net
{
	public class StreamBuffer
	{
		private readonly MemoryStream _ms;
		private readonly BinaryWriter _bw;
		private readonly BinaryReader _br;

		public MemoryStream ms => this._ms;
		public BinaryWriter bw => this._bw;
		public BinaryReader br => this._br;

		/// <summary>
		/// 获取当前位置
		/// </summary>
		public int position
		{
			get => ( int )this._ms.Position;
			set => this._ms.Position = value;
		}

		/// <summary>
		/// 获取当前数据长度
		/// </summary>
		public long length
		{
			get => this._ms.Length;
			set => this._ms.SetLength( value );
		}

		/// <summary>
		/// 当前是否还有数据可以读取
		/// </summary>
		public bool readable => this._ms.Length > this._ms.Position;

		/// <summary>
		/// 可读数据长度
		/// </summary>
		public long bytesAvailable => this._ms.Length - this._ms.Position;

		public StreamBuffer()
		{
			this._ms = new MemoryStream();
			this._bw = new BinaryWriter( this._ms, Encoding.UTF8 );
			this._br = new BinaryReader( this._ms, Encoding.UTF8 );
		}

		public StreamBuffer( byte[] buff )
		{
			this._ms = new MemoryStream( buff );
			this._bw = new BinaryWriter( this._ms );
			this._br = new BinaryReader( this._ms );
		}

		public void Close()
		{
			this._bw.Close();
			this._br.Close();
			this._ms.Close();
		}

		public void Clear()
		{
			this._ms.SetLength( 0 );
		}

		public void Strip( int pos, int length )
		{
			byte[] bytes = this.ReadBytes( pos, length );
			this.Clear();
			this.Write( bytes );
		}

		public void Write( StreamBuffer streamBuffer )
		{
			this._bw.Write( streamBuffer.GetBuffer() );
		}

		public void Write( int value )
		{
			this._bw.Write( value );
		}

		public void Write( byte value )
		{
			this._bw.Write( value );
		}

		public void Write( bool value )
		{
			this._bw.Write( value );
		}

		public void Write( string value )
		{
			this._bw.Write( value );
		}

		public void Write( byte[] value )
		{
			this._bw.Write( value );
		}

		public void Write( byte[] value, int index, int count )
		{
			this._bw.Write( value, index, count );
		}

		public void Write( char[] chars, int index, int count )
		{
			this._bw.Write( chars, index, count );
		}

		public void Write( char[] chars )
		{
			this._bw.Write( chars );
		}

		public void Write( double value )
		{
			this._bw.Write( value );
		}

		public void Write( float value )
		{
			this._bw.Write( value );
		}

		public void Write( long value )
		{
			this._bw.Write( value );
		}

		public void Write( ushort value )
		{
			this._bw.Write( value );
		}

		public void Write( uint value )
		{
			this._bw.Write( value );
		}

		public void Write( ulong value )
		{
			this._bw.Write( value );
		}

		public void WriteUTF8( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
			{
				this.Write( 0 );
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes( value );
			this.Write( bytes.Length );
			this.Write( bytes );
		}

		public void WriteUTF8E( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return;
			byte[] bytes = Encoding.UTF8.GetBytes( value );
			this.Write( bytes );
		}

		public int ReadInt()
		{
			return this._br.ReadInt32();
		}

		public byte ReadByte()
		{
			return this._br.ReadByte();
		}

		public bool ReadBool()
		{
			return this._br.ReadBoolean();
		}

		public string ReadString()
		{
			return this._br.ReadString();
		}

		public string ReadUTF8()
		{
			int len = this.ReadInt();
			if ( len == 0 )
				return string.Empty;
			byte[] bytes = this.ReadBytes( len );
			return Encoding.UTF8.GetString( bytes );
		}

		public string ReadUTF8E()
		{
			if ( this.bytesAvailable <= 0 )
				return string.Empty;
			byte[] bytes = this.ReadBytes( ( int )this.bytesAvailable );
			return Encoding.UTF8.GetString( bytes );
		}

		public byte[] ReadBytes( int length )
		{
			return this._br.ReadBytes( length );
		}

		public double ReadDouble()
		{
			return this._br.ReadDouble();
		}

		public float ReadFloat()
		{
			return this._br.ReadSingle();
		}

		public long ReadLong()
		{
			return this._br.ReadInt64();
		}

		public short ReadShort()
		{
			return this._br.ReadInt16();
		}

		public ushort ReadUShort()
		{
			return this._br.ReadUInt16();
		}

		public uint ReadUInt()
		{
			return this._br.ReadUInt32();
		}

		public ulong ReadULong()
		{
			return this._br.ReadUInt64();
		}

		public void Write( int pos, int value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, byte value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, bool value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, string value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, byte[] value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, byte[] value, int index, int count )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value, index, count );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, char[] chars, int index, int count )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( chars, index, count );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, char[] chars )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( chars );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, double value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, float value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, long value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, ushort value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, uint value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void Write( int pos, ulong value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			this._bw.Write( value );
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public void WriteUTF8( int pos, string value )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			if ( string.IsNullOrEmpty( value ) )
				this.Write( 0 );
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes( value );
				this.Write( bytes.Length );
				this.Write( bytes );
			}
			this._bw.Seek( p, SeekOrigin.Begin );
		}

		public int ReadInt( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			int value = this._br.ReadInt32();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public byte ReadByte( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			byte value = this._br.ReadByte();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public bool ReadBool( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			bool value = this._br.ReadBoolean();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public string ReadString( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			string value = this._br.ReadString();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public string ReadUTF8( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			int len = this.ReadInt();
			string str;
			if ( len == 0 )
				str = string.Empty;
			else
			{
				byte[] bytes = this.ReadBytes( len );
				str = Encoding.UTF8.GetString( bytes );
			}
			this._bw.Seek( p, SeekOrigin.Begin );
			return str;
		}

		public byte[] ReadBytes( int pos, int length )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			byte[] value = this._br.ReadBytes( length );
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public double ReadDouble( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			double value = this._br.ReadDouble();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public float ReadFloat( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			float value = this._br.ReadSingle();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public long ReadLong( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			long value = this._br.ReadInt64();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public short ReadShort( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			short value = this._br.ReadInt16();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public ushort ReadUShort( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			ushort value = this._br.ReadUInt16();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public uint ReadUInt( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			uint value = this._br.ReadUInt32();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public ulong ReadULong( int pos )
		{
			int p = this.position;
			this._bw.Seek( pos, SeekOrigin.Begin );
			ulong value = this._br.ReadUInt64();
			this._bw.Seek( p, SeekOrigin.Begin );
			return value;
		}

		public byte[] GetBuffer()
		{
			return this._ms.GetBuffer();
		}

		public byte[] ToArray()
		{
			return this._ms.ToArray();
		}
	}
}
