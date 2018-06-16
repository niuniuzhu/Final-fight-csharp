using Core.Misc;
using System;

namespace Core.Math
{
	public class PseudoRandom : Random
	{
		private static uint _idx;

		public Vec2 onUnitCircle
		{
			get
			{
				float angle = this.NextFloat( 0, MathUtils.PI2 );
				return new Vec2( MathUtils.Cos( angle ), MathUtils.Sin( angle ) );
			}
		}

		public Vec2 insideUnitCircle
		{
			get
			{
				float radius = this.NextFloat( 0, 1 );
				float angle = this.NextFloat( 0, MathUtils.PI2 );
				return new Vec2( radius * MathUtils.Cos( angle ), radius * MathUtils.Sin( angle ) );
			}
		}

		public Vec3 onUnitSphere
		{
			get
			{
				float theta = this.NextFloat( 0, MathUtils.PI2 );
				float phi = MathUtils.Acos( 2 * this.NextFloat( 0, 1 ) - 1 );
				return new Vec3( MathUtils.Cos( theta ) * MathUtils.Sin( phi ), MathUtils.Sin( theta ) * MathUtils.Sin( phi ), MathUtils.Cos( phi ) );
			}
		}

		public Vec3 insideUnitSphere
		{
			get
			{
				float theta = this.NextFloat( 0, MathUtils.PI2 );
				float phi = MathUtils.Acos( 2 * this.NextFloat( 0, 1 ) - 1 );
				float r = MathUtils.Pow( this.NextFloat( 0, 1 ), 1f / 3f );
				return new Vec3( r * MathUtils.Cos( theta ) * MathUtils.Sin( phi ), r * MathUtils.Sin( theta ) * MathUtils.Sin( phi ), r * MathUtils.Cos( phi ) );
			}
		}

		public Quat rotation
		{
			get
			{
				float theta = this.NextFloat( 0, MathUtils.PI2 );
				float phi = this.NextFloat( -MathUtils.PI_HALF, MathUtils.PI_HALF );
				Vec3 v = new Vec3( MathUtils.Sin( phi ) * MathUtils.Sin( theta ), MathUtils.Cos( phi ) * MathUtils.Sin( theta ), MathUtils.Cos( theta ) );
				return Quat.FromToRotation( Vec3.forward, v );
			}
		}

		public Quat rotationUniform => Quat.FromToRotation( Vec3.forward, this.onUnitSphere );

		public PseudoRandom( int seed )
			: base( seed )
		{
		}

		public float NextFloat( float min, float max )
		{
			return this.NextFloat() * ( max - min ) + min;
		}

		public float NextFloat()
		{
			return ( float )this.NextDouble();
		}

		public int NextInt( int min = int.MinValue, int max = int.MaxValue )
		{
			return this.Next( min, max );
		}

		public Color4 ColorHSV( float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax, float alphaMin, float alphaMax )
		{
			float h = MathUtils.Lerp( hueMin, hueMax, this.NextFloat() );
			float s = MathUtils.Lerp( saturationMin, saturationMax, this.NextFloat() );
			float v = MathUtils.Lerp( valueMin, valueMax, this.NextFloat() );
			Color4 result = Color4.HsvtoRgb( h, s, v, true );
			result.a = MathUtils.Lerp( alphaMin, alphaMax, this.NextFloat() );
			return result;
		}

		public string IdHash()
		{
			byte[] bytes = new byte[5];
			int offset = ByteUtils.Encode32u( bytes, 0, _idx++ );
			ByteUtils.Encode32u( bytes, offset, ( uint )this.Next() );
			return Convert.ToBase64String( bytes, 0 );
		}
	}
}