using System;

namespace Core.FMath
{
	public struct FVec2
	{
		public Fix64 x, y;

		public Fix64 this[int index]
		{
			get
			{
				Fix64 result;
				if ( index != 0 )
				{
					if ( index != 1 )
					{
						throw new IndexOutOfRangeException( "Invalid Vector2 index!" );
					}
					result = this.y;
				}
				else
				{
					result = this.x;
				}
				return result;
			}
			set
			{
				if ( index != 0 )
				{
					if ( index != 1 )
					{
						throw new IndexOutOfRangeException( "Invalid Vector2 index!" );
					}
					this.y = value;
				}
				else
				{
					this.x = value;
				}
			}
		}

		private static readonly FVec2 ONE = new FVec2( 1 );
		private static readonly FVec2 MINUS_ONE = new FVec2( -1 );
		private static readonly FVec2 ZERO = new FVec2( 0 );
		private static readonly FVec2 RIGHT = new FVec2( 1, 0 );
		private static readonly FVec2 LEFT = new FVec2( -1, 0 );
		private static readonly FVec2 UP = new FVec2( 0, 1 );
		private static readonly FVec2 DOWN = new FVec2( 0, -1 );
		private static readonly FVec2 POSITIVE_INFINITY_VECTOR = new FVec2( float.PositiveInfinity, float.PositiveInfinity );
		private static readonly FVec2 NEGATIVE_INFINITY_VECTOR = new FVec2( float.NegativeInfinity, float.NegativeInfinity );

		public static FVec2 one => ONE;
		public static FVec2 minusOne => MINUS_ONE;
		public static FVec2 zero => ZERO;
		public static FVec2 right => RIGHT;
		public static FVec2 left => LEFT;
		public static FVec2 up => UP;
		public static FVec2 down => DOWN;
		public static FVec2 positiveInfinityVector => POSITIVE_INFINITY_VECTOR;
		public static FVec2 negativeInfinityVector => NEGATIVE_INFINITY_VECTOR;

		public FVec2( float value )
		{
			this.x = ( Fix64 )value;
			this.y = ( Fix64 )value;
		}

		public FVec2( float x, float y )
		{
			this.x = ( Fix64 )x;
			this.y = ( Fix64 )y;
		}

		public FVec2( Fix64 value )
		{
			this.x = value;
			this.y = value;
		}

		public FVec2( Fix64 x, Fix64 y )
		{
			this.x = x;
			this.y = y;
		}

		#region Operators

		public static FVec2 operator +( FVec2 p1, FVec2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static FVec2 operator +( FVec2 p1, Fix64 p2 )
		{
			p1.x += p2;
			p1.y += p2;
			return p1;
		}

		public static FVec2 operator +( Fix64 p1, FVec2 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			return p2;
		}

		public static FVec2 operator -( FVec2 p1, FVec2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static FVec2 operator -( FVec2 p1, Fix64 p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			return p1;
		}

		public static FVec2 operator -( Fix64 p1, FVec2 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			return p2;
		}

		public static FVec2 operator -( FVec2 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			return p2;
		}

		public static FVec2 operator *( FVec2 p1, FVec2 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			return p1;
		}

		public static FVec2 operator *( FVec2 p1, Fix64 p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			return p1;
		}

		public static FVec2 operator *( Fix64 p1, FVec2 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			return p2;
		}

		public static FVec2 operator *( FMat2 m, FVec2 v )
		{
			return new FVec2
			(
				v.x * m.x.x + v.y * m.y.x,
				v.x * m.x.y + v.y * m.y.y
			);
		}

		public static FVec2 operator /( FVec2 p1, FVec2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static FVec2 operator /( FVec2 p1, Fix64 p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			return p1;
		}

		public static FVec2 operator /( Fix64 p1, FVec2 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			return p2;
		}

		public static implicit operator FVec2( FVec3 v )
		{
			return new FVec2( v.x, v.y );
		}

		public static implicit operator FVec3( FVec2 v )
		{
			return new FVec3( v.x, v.y, Fix64.Zero );
		}

		public static bool operator ==( FVec2 p1, FVec2 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y;
		}

		public static bool operator !=( FVec2 p1, FVec2 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( FVec2 )obj == this;
		}

		public override string ToString()
		{
			return $"({this.x}, {this.y})";
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
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
			}
		}

		public Fix64 Magnitude()
		{
			return Fix64.Sqrt( this.x * this.x + this.y * this.y );
		}

		public Fix64 SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y;
		}

		public Fix64 Distance( FVec2 vector )
		{
			return ( vector - this ).Magnitude();
		}

		public Fix64 DistanceSquared( FVec2 vector )
		{
			return ( vector - this ).Dot();
		}

		public void Negate()
		{
			this.x = -this.x;
			this.y = -this.y;
		}

		public void Scale( FVec2 scale )
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}

		public Fix64 Dot()
		{
			return this.x * this.x + this.y * this.y;
		}

		public Fix64 Dot( FVec2 vector )
		{
			return this.x * vector.x + this.y * vector.y;
		}

		public void Normalize()
		{
			Fix64 f = Fix64.One / Fix64.Sqrt( this.x * this.x + this.y * this.y );
			this.x *= f;
			this.y *= f;
		}

		public FVec2 NormalizeSafe()
		{
			Fix64 dis = Fix64.Sqrt( this.x * this.x + this.y * this.y );
			if ( dis == Fix64.Zero ) return new FVec2();
			return this * ( Fix64.One / dis );
		}

		public FVec2 Transform( FMat2 matrix )
		{
			return matrix.x * this.x + matrix.y * this.y;
		}

		public bool AproxEqualsBox( FVec2 vector, Fix64 tolerance )
		{
			return
				( Fix64.Abs( this.x - vector.x ) <= tolerance ) &&
				( Fix64.Abs( this.y - vector.y ) <= tolerance );
		}

		public bool ApproxEquals( FVec2 vector, Fix64 tolerance )
		{
			return this.Distance( vector ) <= tolerance;
		}

		public static Fix64 Distance( FVec2 v0, FVec2 v1 )
		{
			return ( v1 - v0 ).Magnitude();
		}

		public static Fix64 DistanceSquared( FVec2 v0, FVec2 v1 )
		{
			return ( v1 - v0 ).Dot();
		}

		public static FVec2 ClampMagnitude( FVec2 v, Fix64 maxLength )
		{
			FVec2 nor = v;
			Fix64 sqrMagnitude = nor.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
				nor = nor * ( maxLength / Fix64.Sqrt( sqrMagnitude ) );
			return nor;
		}

		public static FVec2 Normalize( FVec2 v )
		{
			return v * ( Fix64.One / Fix64.Sqrt( v.x * v.x + v.y * v.y ) );
		}

		public static FVec2 NormalizeSafe( FVec2 v )
		{
			Fix64 dis = Fix64.Sqrt( v.x * v.x + v.y * v.y );
			if ( dis == Fix64.Zero )
				return new FVec2();
			return v * ( Fix64.One / dis );
		}

		public static Fix64 Dot( FVec2 v0, FVec2 v1 )
		{
			return v0.x * v1.x + v0.y * v1.y;
		}

		public static FVec2 LerpUnclamped( FVec2 from, FVec2 to, Fix64 t )
		{
			return new FVec2( from.x + ( to.x - from.x ) * t, from.y + ( to.y - from.y ) * t );
		}

		public static FVec2 Lerp( FVec2 from, FVec2 to, Fix64 t )
		{
			return t <= Fix64.Zero ? from : ( t >= Fix64.One ? to : LerpUnclamped( from, to, t ) );
		}

		#endregion
	}
}