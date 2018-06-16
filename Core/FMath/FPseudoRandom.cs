using Core.Misc;
using System;

namespace Core.FMath
{
	public class FPseudoRandom : Random
	{
		private static uint _idx;

		public FVec2 onUnitCircle
		{
			get
			{
				Fix64 angle = this.NextFix64( Fix64.Zero, Fix64.PiTimes2 );
				return new FVec2( Fix64.Cos( angle ), Fix64.Sin( angle ) );
			}
		}

		public FVec2 insideUnitCircle
		{
			get
			{
				Fix64 radius = this.NextFix64( Fix64.Zero, Fix64.One );
				Fix64 angle = this.NextFix64( Fix64.Zero, Fix64.PiTimes2 );
				return new FVec2( radius * Fix64.Cos( angle ), radius * Fix64.Sin( angle ) );
			}
		}

		public FVec3 onUnitSphere
		{
			get
			{
				Fix64 theta = this.NextFix64( Fix64.Zero, Fix64.PiTimes2 );
				Fix64 phi = Fix64.Acos( Fix64.Two * this.NextFix64( Fix64.Zero, Fix64.One ) - Fix64.One );
				return new FVec3( Fix64.Cos( theta ) * Fix64.Sin( phi ), Fix64.Sin( theta ) * Fix64.Sin( phi ), Fix64.Cos( phi ) );
			}
		}

		public FVec3 insideUnitSphere
		{
			get
			{
				Fix64 theta = this.NextFix64( Fix64.Zero, Fix64.PiTimes2 );
				Fix64 phi = Fix64.Acos( Fix64.Two * this.NextFix64( Fix64.Zero, Fix64.One ) - Fix64.One );
				Fix64 r = Fix64.Pow( this.NextFix64( Fix64.Zero, Fix64.One ), ( Fix64 )( 1f / 3f ) );
				return new FVec3( r * Fix64.Cos( theta ) * Fix64.Sin( phi ), r * Fix64.Sin( theta ) * Fix64.Sin( phi ), r * Fix64.Cos( phi ) );
			}
		}

		public FQuat rotation
		{
			get
			{
				Fix64 theta = this.NextFix64( Fix64.Zero, Fix64.PiTimes2 );
				Fix64 phi = this.NextFix64( -Fix64.PiOver2, Fix64.PiOver2 );
				FVec3 v = new FVec3( Fix64.Sin( phi ) * Fix64.Sin( theta ), Fix64.Cos( phi ) * Fix64.Sin( theta ), Fix64.Cos( theta ) );
				return FQuat.FromToRotation( FVec3.forward, v );
			}
		}

		public FQuat rotationUniform => FQuat.FromToRotation( FVec3.forward, this.onUnitSphere );

		public FPseudoRandom( int seed )
			: base( seed )
		{
		}

		public float NextFloat( float min, float max )
		{
			return ( float )this.NextFix64() * ( max - min ) + min;
		}

		public float NextFloat()
		{
			return ( float )this.NextFix64();
		}

		public long NextLong( long min = long.MinValue, long max = long.MaxValue )
		{
			if ( max <= min )
				max = min;

			ulong uRange = ( ulong )( max - min );
			ulong ulongRand;
			do
			{
				byte[] buf = new byte[8];
				this.NextBytes( buf );
				ulongRand = ( ulong )BitConverter.ToInt64( buf, 0 );
			} while ( ulongRand > ulong.MaxValue - ( ( ulong.MaxValue % uRange ) + 1 ) % uRange );

			return ( long )( ulongRand % uRange ) + min;
		}

		public Fix64 NextFix64()
		{
			return ( Fix64 )this.NextDouble();
		}

		public Fix64 NextFix64( Fix64 min, Fix64 max )
		{
			return new Fix64( this.NextLong( min.RawValue, max.RawValue ) );
		}

		public string IdHash( string id )
		{
			byte[] bytes = new byte[8];
			int offset = ByteUtils.Encode32u( bytes, 0, _idx++ );
			ByteUtils.Encode32u( bytes, offset, ( uint )this.Next() );
			return id + "@" + Convert.ToBase64String( bytes, 0 );
		}
	}
}