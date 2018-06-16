using System;

namespace Core.FMath
{
	public struct FVec4
	{
		public Fix64 x, y, z, w;

		public Fix64 this[int index]
		{
			get
			{
				Fix64 result;
				switch ( index )
				{
					case 0:
						result = this.x;
						break;
					case 1:
						result = this.y;
						break;
					case 2:
						result = this.z;
						break;
					case 3:
						result = this.w;
						break;
					default:
						throw new IndexOutOfRangeException( "Invalid Vector4 index!" );
				}
				return result;
			}
			set
			{
				switch ( index )
				{
					case 0:
						this.x = value;
						break;
					case 1:
						this.y = value;
						break;
					case 2:
						this.z = value;
						break;
					case 3:
						this.w = value;
						break;
					default:
						throw new IndexOutOfRangeException( "Invalid Vector4 index!" );
				}
			}
		}

		private static readonly FVec4 ONE = new FVec4( 1 );
		private static readonly FVec4 MINUS_ONE = new FVec4( -1 );
		private static readonly FVec4 ZERO = new FVec4( 0 );
		private static readonly FVec4 RIGHT = new FVec4( 1, 0, 0, 0 );
		private static readonly FVec4 LEFT = new FVec4( -1, 0, 0, 0 );
		private static readonly FVec4 UP = new FVec4( 0, 1, 0, 0 );
		private static readonly FVec4 DOWN = new FVec4( 0, -1, 0, 0 );
		private static readonly FVec4 FORWARD = new FVec4( 0, 0, 1, 0 );
		private static readonly FVec4 BACKWARD = new FVec4( 0, 0, -1, 0 );
		private static readonly FVec4 HIGH = new FVec4( 0, 0, 0, 1 );
		private static readonly FVec4 LOW = new FVec4( 0, 0, 0, -1 );
		private static readonly FVec4 POSITIVE_INFINITY_VECTOR = new FVec4( float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity );
		private static readonly FVec4 NEGATIVE_INFINITY_VECTOR = new FVec4( float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity );

		public static FVec4 one => ONE;
		public static FVec4 minusOne => MINUS_ONE;
		public static FVec4 zero => ZERO;
		public static FVec4 right => RIGHT;
		public static FVec4 left => LEFT;
		public static FVec4 up => UP;
		public static FVec4 down => DOWN;
		public static FVec4 forward => FORWARD;
		public static FVec4 backward => BACKWARD;
		public static FVec4 high => HIGH;
		public static FVec4 low => LOW;
		public static FVec4 positiveInfinityVector => POSITIVE_INFINITY_VECTOR;
		public static FVec4 negativeInfinityVector => NEGATIVE_INFINITY_VECTOR;

		public FVec4( float value )
		{
			this.x = ( Fix64 )value;
			this.y = ( Fix64 )value;
			this.z = ( Fix64 )value;
			this.w = ( Fix64 )value;
		}

		public FVec4( float x, float y, float z, float w )
		{
			this.x = ( Fix64 )x;
			this.y = ( Fix64 )y;
			this.z = ( Fix64 )z;
			this.w = ( Fix64 )w;
		}

		public FVec4( Fix64 value )
		{
			this.x = value;
			this.y = value;
			this.z = value;
			this.w = value;
		}

		public FVec4( Fix64 x, Fix64 y, Fix64 z, Fix64 w )
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		#region Operators

		public static FVec4 operator +( FVec4 p1, FVec4 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			p1.w += p2.w;
			return p1;
		}

		public static FVec4 operator +( FVec4 p1, FVec3 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			return p1;
		}

		public static FVec4 operator +( FVec3 p1, FVec4 p2 )
		{
			p2.x = p1.x + p2.x;
			p2.y = p1.y + p2.y;
			p2.z = p1.z + p2.z;
			return p2;
		}

		public static FVec4 operator +( FVec4 p1, FVec2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static FVec4 operator +( FVec2 p1, FVec4 p2 )
		{
			p2.x = p1.x + p2.x;
			p2.y = p1.y + p2.y;
			return p2;
		}

		public static FVec4 operator +( FVec4 p1, Fix64 p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			p1.w += p2;
			return p1;
		}

		public static FVec4 operator +( Fix64 p1, FVec4 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			p2.w = p1 + p2.w;
			return p2;
		}

		public static FVec4 operator -( FVec4 p1, FVec4 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			p1.w -= p2.w;
			return p1;
		}

		public static FVec4 operator -( FVec4 p1, FVec3 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			return p1;
		}

		public static FVec4 operator -( FVec3 p1, FVec4 p2 )
		{
			p2.x = p1.x - p2.x;
			p2.y = p1.y - p2.y;
			p2.z = p1.z - p2.z;
			return p2;
		}

		public static FVec4 operator -( FVec4 p1, FVec2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static FVec4 operator -( FVec2 p1, FVec4 p2 )
		{
			p2.x = p1.x - p2.x;
			p2.y = p1.y - p2.y;
			return p2;
		}

		public static FVec4 operator -( FVec4 p1, Fix64 p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			p1.w -= p2;
			return p1;
		}

		public static FVec4 operator -( Fix64 p1, FVec4 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			p2.w = p1 - p2.w;
			return p2;
		}

		public static FVec4 operator -( FVec4 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			p2.w = -p2.w;
			return p2;
		}

		public static FVec4 operator *( FVec4 p1, FVec4 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			p1.z *= p2.z;
			p1.w *= p2.w;
			return p1;
		}

		public static FVec4 operator *( FVec4 p1, FVec3 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			p1.z *= p2.z;
			return p1;
		}

		public static FVec4 operator *( FVec3 p1, FVec4 p2 )
		{
			p2.x = p1.x * p2.x;
			p2.y = p1.y * p2.y;
			p2.z = p1.z * p2.z;
			return p2;
		}

		public static FVec4 operator *( FVec4 p1, FVec2 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			return p1;
		}

		public static FVec4 operator *( FVec2 p1, FVec4 p2 )
		{
			p2.x = p1.x * p2.x;
			p2.y = p1.y * p2.y;
			return p2;
		}

		public static FVec4 operator *( FVec4 p1, Fix64 p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			p1.w *= p2;
			return p1;
		}

		public static FVec4 operator *( Fix64 p1, FVec4 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			p2.z = p1 * p2.z;
			p2.w = p1 * p2.w;
			return p2;
		}

		public static FVec4 operator *( FMat4 m, FVec4 v )
		{
			return new FVec4
			(
				v.x * m.x.x + v.y * m.y.x + v.z * m.z.x + v.w * m.w.x,
				v.x * m.x.y + v.y * m.y.y + v.z * m.z.y + v.w * m.w.y,
				v.x * m.x.z + v.y * m.y.z + v.z * m.z.z + v.w * m.w.z,
				v.x * m.x.w + v.y * m.y.w + v.z * m.z.w + v.w * m.w.w
			);
		}

		public static FVec4 operator /( FVec4 p1, FVec4 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			p1.w /= p2.w;
			return p1;
		}

		public static FVec4 operator /( FVec4 p1, FVec3 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			return p1;
		}

		public static FVec4 operator /( FVec3 p1, FVec4 p2 )
		{
			p2.x = p1.x / p2.x;
			p2.y = p1.y / p2.y;
			p2.z = p1.z / p2.z;
			return p2;
		}

		public static FVec4 operator /( FVec4 p1, FVec2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static FVec4 operator /( FVec2 p1, FVec4 p2 )
		{
			p2.x = p1.x / p2.x;
			p2.y = p1.y / p2.y;
			return p2;
		}

		public static FVec4 operator /( FVec4 p1, Fix64 p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			p1.w /= p2;
			return p1;
		}

		public static FVec4 operator /( Fix64 p1, FVec4 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			p2.w = p1 / p2.w;
			return p2;
		}

		public static implicit operator FVec4( FVec3 v )
		{
			return new FVec4( v.x, v.y, v.z, Fix64.Zero );
		}

		public static implicit operator FVec3( FVec4 v )
		{
			return new FVec3( v.x, v.y, v.z );
		}

		public static implicit operator FVec4( FVec2 v )
		{
			return new FVec4( v.x, v.y, Fix64.Zero, Fix64.Zero );
		}

		public static implicit operator FVec2( FVec4 v )
		{
			return new FVec2( v.x, v.y );
		}

		public static bool operator ==( FVec4 p1, FVec4 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z && p1.w == p2.w;
		}

		public static bool operator !=( FVec4 p1, FVec4 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z || p1.w != p2.w;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( FVec4 )obj == this;
		}

		public override string ToString()
		{
			return $"({this.x}, {this.y}, {this.z}, {this.w})";
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode() ^ this.w.GetHashCode();
		}

		#endregion

		#region Methods

		public void ClampMagnitude( Fix64 maxLength )
		{
			Fix64 sqrMagnitude = this.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
			{
				Fix64 f = maxLength / Fix64.Sqrt( sqrMagnitude );
				this.x *= f;
				this.y *= f;
				this.z *= f;
				this.w *= f;
			}
		}

		public Fix64 Magnitude()
		{
			return Fix64.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );
		}

		public Fix64 SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
		}

		public Fix64 Distance( FVec4 vector )
		{
			return ( vector - this ).Magnitude();
		}

		public Fix64 DistanceSquared( FVec4 vector )
		{
			return ( vector - this ).Dot();
		}

		public void Negate()
		{
			this.x = -this.x;
			this.y = -this.y;
			this.z = -this.z;
			this.w = -this.w;
		}

		public void Scale( FVec4 scale )
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
			this.w *= scale.w;
		}

		public Fix64 Dot()
		{
			return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
		}

		public Fix64 Dot( FVec4 vector )
		{
			return this.x * vector.x + this.y * vector.y + this.z * vector.z + this.w * vector.w;
		}

		public void Normalize()
		{
			Fix64 f = Fix64.One / Fix64.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );
			this.x *= f;
			this.y *= f;
			this.z *= f;
			this.w *= f;
		}

		public FVec4 NormalizeSafe()
		{
			Fix64 dis = Fix64.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );
			if ( dis == Fix64.Zero )
				return new FVec4();
			return this * ( Fix64.One / dis );
		}

		public FVec4 Transform( FMat4 matrix )
		{
			return matrix.x * this.x + matrix.y * this.y + matrix.z * this.z + matrix.w * this.w;
		}

		public FVec4 Multiply( FMat4 matrix )
		{
			return new FVec4
				(
				matrix.x.x * this.x + matrix.x.y * this.y + matrix.x.z * this.z + matrix.x.w * this.w,
				matrix.y.x * this.x + matrix.y.y * this.y + matrix.y.z * this.z + matrix.y.w * this.w,
				matrix.z.x * this.x + matrix.z.y * this.y + matrix.z.z * this.z + matrix.z.w * this.w,
				matrix.w.x * this.x + matrix.w.y * this.y + matrix.w.z * this.z + matrix.w.w * this.w
				);
		}

		public bool AproxEqualsBox( FVec4 vector, Fix64 tolerance )
		{
			return
				( Fix64.Abs( this.x - vector.x ) <= tolerance ) &&
				( Fix64.Abs( this.y - vector.y ) <= tolerance ) &&
				( Fix64.Abs( this.z - vector.z ) <= tolerance ) &&
				( Fix64.Abs( this.w - vector.w ) <= tolerance );
		}

		public bool ApproxEquals( FVec4 vector, Fix64 tolerance )
		{
			return this.Distance( vector ) <= tolerance;
		}

		public static Fix64 Distance( FVec4 v0, FVec4 v1 )
		{
			return ( v1 - v0 ).Magnitude();
		}

		public static Fix64 DistanceSquared( FVec4 v0, FVec4 v1 )
		{
			return ( v1 - v0 ).Dot();
		}

		public static FVec4 ClampMagnitude( FVec4 v, Fix64 maxLength )
		{
			FVec4 nor = v;
			Fix64 sqrMagnitude = nor.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
				nor = nor * ( maxLength / Fix64.Sqrt( sqrMagnitude ) );
			return nor;
		}

		public static FVec4 Normalize( FVec4 v )
		{
			return v * ( Fix64.One / Fix64.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w ) );
		}

		public static FVec4 NormalizeSafe( FVec4 v )
		{
			Fix64 dis = Fix64.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w );
			if ( dis == Fix64.Zero )
				return new FVec4();
			return v * ( Fix64.One / dis );
		}

		public static Fix64 Dot( FVec4 v0, FVec4 v1 )
		{
			return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z + v0.w * v1.w;
		}

		public static FVec4 LerpUnclamped( FVec4 from, FVec4 to, Fix64 t )
		{
			return new FVec4( from.x + ( to.x - from.x ) * t, from.y + ( to.y - from.y ) * t, from.z + ( to.z - from.z ) * t, from.w + ( to.w - from.w ) * t );
		}

		public static FVec4 Lerp( FVec4 from, FVec4 to, Fix64 t )
		{
			return t <= Fix64.Zero ? @from : ( t >= Fix64.One ? to : LerpUnclamped( @from, to, t ) );
		}

		public FVec4 Project( FMat4 projectionMatrix, FMat4 viewMatrix, Fix64 viewX, Fix64 viewY, Fix64 viewWidth, Fix64 viewHeight )
		{
			FVec4 result = this;
			result = result.Multiply( viewMatrix );
			result = result.Multiply( projectionMatrix );

			Fix64 wDelta = Fix64.One / result.w;
			result.x *= wDelta;
			result.y *= wDelta;
			result.z *= wDelta;

			result.x = result.x * Fix64.Half + Fix64.Half;
			result.y = result.y * Fix64.Half + Fix64.Half;
			result.z = result.z * Fix64.Half + Fix64.Half;

			result.x = result.x * viewWidth + viewX;
			result.y = result.y * viewHeight + viewY;

			return result;
		}

		public FVec4 UnProject( FMat4 viewProjInverse, Fix64 viewX, Fix64 viewY, Fix64 viewWidth, Fix64 viewHeight )
		{
			FVec4 result = this;
			result.x = ( result.x - viewX ) / viewWidth;
			result.y = ( result.y - viewY ) / viewHeight;
			result = result * Fix64.Two - Fix64.One;

			result = result.Transform( viewProjInverse );
			Fix64 wDelta = Fix64.One / result.w;
			result.x *= wDelta;
			result.y *= wDelta;
			result.z *= wDelta;

			return result;
		}

		public FVec4 UnProject( FMat4 projectionMatrix, FMat4 viewMatrix, Fix64 viewX, Fix64 viewY, Fix64 viewWidth, Fix64 viewHeight )
		{
			FMat4 viewProjInverse = FMat4.Invert( projectionMatrix * viewMatrix );

			FVec4 result = this;
			result.x = ( result.x - viewX ) / viewWidth;
			result.y = ( result.y - viewY ) / viewHeight;
			result = result * Fix64.Two - Fix64.One;

			result = result.Transform( viewProjInverse );
			Fix64 wDelta = Fix64.One / result.w;
			result.x *= wDelta;
			result.y *= wDelta;
			result.z *= wDelta;

			return result;
		}

		public static FVec4 Max( FVec4 v, Fix64 value )
		{
			return new FVec4( Fix64.Max( v.x, value ), Fix64.Max( v.y, value ), Fix64.Max( v.z, value ),
							 Fix64.Max( v.w, value ) );
		}

		public static FVec4 Max( FVec4 v, FVec4 values )
		{
			return new FVec4( Fix64.Max( v.x, values.x ), Fix64.Max( v.y, values.y ), Fix64.Max( v.z, values.z ),
							 Fix64.Max( v.w, values.w ) );
		}

		public static FVec4 Min( FVec4 v, Fix64 value )
		{
			return new FVec4( Fix64.Min( v.x, value ), Fix64.Min( v.y, value ), Fix64.Min( v.z, value ),
							 Fix64.Min( v.w, value ) );
		}

		public static FVec4 Min( FVec4 v, FVec4 values )
		{
			return new FVec4( Fix64.Min( v.x, values.x ), Fix64.Min( v.y, values.y ), Fix64.Min( v.z, values.z ),
							 Fix64.Min( v.w, values.w ) );
		}

		public static FVec4 Abs( FVec4 v )
		{
			return new FVec4( Fix64.Abs( v.x ), Fix64.Abs( v.y ), Fix64.Abs( v.z ), Fix64.Abs( v.w ) );
		}

		public static FVec4 Pow( FVec4 v, Fix64 power )
		{
			return new FVec4( Fix64.Pow( v.x, power ), Fix64.Pow( v.y, power ),
							 Fix64.Pow( v.z, power ), Fix64.Pow( v.w, power ) );
		}

		public static FVec4 Floor( FVec4 v )
		{
			return new FVec4( Fix64.Floor( v.x ), Fix64.Floor( v.y ), Fix64.Floor( v.z ),
							 Fix64.Floor( v.w ) );
		}

		public static FVec4 Round( FVec4 v )
		{
			return new FVec4( Fix64.Round( v.x ), Fix64.Round( v.y ), Fix64.Round( v.z ),
							 Fix64.Round( v.w ) );
		}

		#endregion
	}
}