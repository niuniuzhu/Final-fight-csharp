namespace Core.Misc
{
	public class BitFlags
	{
		public uint value;

		public BitFlags()
		{
		}

		public BitFlags( uint value )
		{
			this.value = value;
		}

		public void Max()
		{
			this.value = uint.MaxValue;
		}

		public void Clear()
		{
			this.value = 0;
		}

		public void SetFlags( uint value, bool setting = true )
		{
			if ( setting )
				this.value |= value;
			else
				this.value &= ~value;
		}

		public void ClearFlags( uint value )
		{
			this.value &= ~value;
		}

		public void ClearBit( int bit )
		{
			this.value &= ( ~( 1u << bit ) );
		}

		public void SetBit( int bit, bool setting = true )
		{
			if ( setting )
				this.value |= ( 1u << bit );
			else
				this.value &= ( ~( 1u << bit ) );
		}

		public bool IsEmpty()
		{
			return this.value == 0;
		}

		public bool TestBit( int bit )
		{
			uint result = this.value & ( 1u << bit );
			return result > 0;
		}

		public bool TestFlags( uint flag )
		{
			uint result = this.value & flag;
			return result == flag;
		}

		public bool TestAny( uint flag )
		{
			uint result = this.value & flag;
			return result > 0;
		}

		public bool IsEqual( uint value )
		{
			return this.value == value;
		}

		public uint HighestBit()
		{
			uint i = 0;
			uint j = 1;
			for ( ; j <= this.value && i < 32; j <<= 1 )
				i++;
			return i - 1;
		}


		public uint LowestBit()
		{
			uint i = 0;
			uint j = 1;
			for ( ; ( j & this.value ) == 0 && i < 32; j <<= 1 )
				i++;
			return i;
		}


		public BitFlags Clone()
		{
			BitFlags newFlag = new BitFlags();
			newFlag.value = this.value;
			return newFlag;
		}
	}
}