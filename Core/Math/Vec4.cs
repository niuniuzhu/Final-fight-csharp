namespace Core.Math
{
	public struct Vec4
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public float this[int index]
		{
			get
			{
				float result;
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
					default:
						result = this.w;
						break;
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
				}
			}
		}

		private static readonly Vec4 ONE = new Vec4( 1, 1, 1, 1 );
		private static readonly Vec4 MINUS_ONE = new Vec4( -1, -1, -1, -1 );
		private static readonly Vec4 ZERO = new Vec4( 0, 0, 0, 0 );
		private static readonly Vec4 RIGHT = new Vec4( 1, 0, 0, 0 );
		private static readonly Vec4 LEFT = new Vec4( -1, 0, 0, 0 );
		private static readonly Vec4 UP = new Vec4( 0, 1, 0, 0 );
		private static readonly Vec4 DOWN = new Vec4( 0, -1, 0, 0 );
		private static readonly Vec4 FORWARD = new Vec4( 0, 0, 1, 0 );
		private static readonly Vec4 BACKWARD = new Vec4( 0, 0, -1, 0 );
		private static readonly Vec4 HIGH = new Vec4( 0, 0, 0, 1 );
		private static readonly Vec4 LOW = new Vec4( 0, 0, 0, -1 );
		private static readonly Vec4 POSITIVE_INFINITY_VECTOR = new Vec4( float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity );
		private static readonly Vec4 NEGATIVE_INFINITY_VECTOR = new Vec4( float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity );

		public static Vec4 one => ONE;
		public static Vec4 minusOne => MINUS_ONE;
		public static Vec4 zero => ZERO;
		public static Vec4 right => RIGHT;
		public static Vec4 left => LEFT;
		public static Vec4 up => UP;
		public static Vec4 down => DOWN;
		public static Vec4 forward => FORWARD;
		public static Vec4 backward => BACKWARD;
		public static Vec4 high => HIGH;
		public static Vec4 low => LOW;
		public static Vec4 positiveInfinityVector => POSITIVE_INFINITY_VECTOR;
		public static Vec4 negativeInfinityVector => NEGATIVE_INFINITY_VECTOR;

		public Vec4( float x, float y, float z, float w )
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		#region Operators

		public static Vec4 operator +( Vec4 p1, Vec4 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			p1.w += p2.w;
			return p1;
		}

		public static Vec4 operator +( Vec4 p1, Vec3 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			return p1;
		}

		public static Vec4 operator +( Vec3 p1, Vec4 p2 )
		{
			p2.x = p1.x + p2.x;
			p2.y = p1.y + p2.y;
			p2.z = p1.z + p2.z;
			return p2;
		}

		public static Vec4 operator +( Vec4 p1, Vec2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static Vec4 operator +( Vec2 p1, Vec4 p2 )
		{
			p2.x = p1.x + p2.x;
			p2.y = p1.y + p2.y;
			return p2;
		}

		public static Vec4 operator +( Vec4 p1, float p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			p1.w += p2;
			return p1;
		}

		public static Vec4 operator +( float p1, Vec4 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			p2.w = p1 + p2.w;
			return p2;
		}

		public static Vec4 operator -( Vec4 p1, Vec4 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			p1.w -= p2.w;
			return p1;
		}

		public static Vec4 operator -( Vec4 p1, Vec3 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			return p1;
		}

		public static Vec4 operator -( Vec3 p1, Vec4 p2 )
		{
			p2.x = p1.x - p2.x;
			p2.y = p1.y - p2.y;
			p2.z = p1.z - p2.z;
			return p2;
		}

		public static Vec4 operator -( Vec4 p1, Vec2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static Vec4 operator -( Vec2 p1, Vec4 p2 )
		{
			p2.x = p1.x - p2.x;
			p2.y = p1.y - p2.y;
			return p2;
		}

		public static Vec4 operator -( Vec4 p1, float p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			p1.w -= p2;
			return p1;
		}

		public static Vec4 operator -( float p1, Vec4 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			p2.w = p1 - p2.w;
			return p2;
		}

		public static Vec4 operator -( Vec4 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			p2.w = -p2.w;
			return p2;
		}

		public static Vec4 operator *( Vec4 p1, Vec4 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			p1.z *= p2.z;
			p1.w *= p2.w;
			return p1;
		}

		public static Vec4 operator *( Vec4 p1, Vec3 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			p1.z *= p2.z;
			return p1;
		}

		public static Vec4 operator *( Vec3 p1, Vec4 p2 )
		{
			p2.x = p1.x * p2.x;
			p2.y = p1.y * p2.y;
			p2.z = p1.z * p2.z;
			return p2;
		}

		public static Vec4 operator *( Vec4 p1, Vec2 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			return p1;
		}

		public static Vec4 operator *( Vec2 p1, Vec4 p2 )
		{
			p2.x = p1.x * p2.x;
			p2.y = p1.y * p2.y;
			return p2;
		}

		public static Vec4 operator *( Vec4 p1, float p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			p1.w *= p2;
			return p1;
		}

		public static Vec4 operator *( float p1, Vec4 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			p2.z = p1 * p2.z;
			p2.w = p1 * p2.w;
			return p2;
		}

		public static Vec4 operator *( Mat4 m, Vec4 v )
		{
			return new Vec4
			(
				v.x * m.x.x + v.y * m.y.x + v.z * m.z.x + v.w * m.w.x,
				v.x * m.x.y + v.y * m.y.y + v.z * m.z.y + v.w * m.w.y,
				v.x * m.x.z + v.y * m.y.z + v.z * m.z.z + v.w * m.w.z,
				v.x * m.x.w + v.y * m.y.w + v.z * m.z.w + v.w * m.w.w
			);
		}

		public static Vec4 operator /( Vec4 p1, Vec4 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			p1.w /= p2.w;
			return p1;
		}

		public static Vec4 operator /( Vec4 p1, Vec3 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			return p1;
		}

		public static Vec4 operator /( Vec3 p1, Vec4 p2 )
		{
			p2.x = p1.x / p2.x;
			p2.y = p1.y / p2.y;
			p2.z = p1.z / p2.z;
			return p2;
		}

		public static Vec4 operator /( Vec4 p1, Vec2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static Vec4 operator /( Vec2 p1, Vec4 p2 )
		{
			p2.x = p1.x / p2.x;
			p2.y = p1.y / p2.y;
			return p2;
		}

		public static Vec4 operator /( Vec4 p1, float p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			p1.w /= p2;
			return p1;
		}

		public static Vec4 operator /( float p1, Vec4 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			p2.w = p1 / p2.w;
			return p2;
		}

		public static implicit operator Vec4( Vec3 v )
		{
			return new Vec4( v.x, v.y, v.z, 0 );
		}

		public static implicit operator Vec3( Vec4 v )
		{
			return new Vec3( v.x, v.y, v.z );
		}

		public static implicit operator Vec4( Vec2 v )
		{
			return new Vec4( v.x, v.y, 0, 0 );
		}

		public static implicit operator Vec2( Vec4 v )
		{
			return new Vec2( v.x, v.y );
		}

		public static bool operator ==( Vec4 p1, Vec4 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z && p1.w == p2.w;
		}

		public static bool operator !=( Vec4 p1, Vec4 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z || p1.w != p2.w;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( Vec4 )obj == this;
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

		public void ClampMagnitude( float maxLength )
		{
			float sqrMagnitude = this.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
			{
				float f = maxLength / MathUtils.Sqrt( sqrMagnitude );
				this.x *= f;
				this.y *= f;
				this.z *= f;
				this.w *= f;
			}
		}

		public float Magnitude()
		{
			return MathUtils.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );
		}

		public float SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
		}

		public float Distance( Vec4 vector )
		{
			return ( vector - this ).Magnitude();
		}

		public float DistanceSquared( Vec4 vector )
		{
			return ( vector - this ).SqrMagnitude();
		}

		public void Negate()
		{
			this.x = -this.x;
			this.y = -this.y;
			this.z = -this.z;
			this.w = -this.w;
		}

		public void Scale( Vec4 scale )
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
			this.w *= scale.w;
		}

		public float Dot( Vec4 vector )
		{
			return this.x * vector.x + this.y * vector.y + this.z * vector.z + this.w * vector.w;
		}

		public void Normalize()
		{
			float f = 1 / MathUtils.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );
			this.x *= f;
			this.y *= f;
			this.z *= f;
			this.w *= f;
		}

		public void NormalizeSafe()
		{
			float f = MathUtils.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );
			if ( f == 0 )
				return;
			this.x *= f;
			this.y *= f;
			this.z *= f;
			this.w *= f;
		}

		public bool AproxEqualsBox( Vec4 vector, float tolerance )
		{
			return
				( MathUtils.Abs( this.x - vector.x ) <= tolerance ) &&
				( MathUtils.Abs( this.y - vector.y ) <= tolerance ) &&
				( MathUtils.Abs( this.z - vector.z ) <= tolerance ) &&
				( MathUtils.Abs( this.w - vector.w ) <= tolerance );
		}

		public bool ApproxEquals( Vec4 vector, float tolerance )
		{
			return this.Distance( vector ) <= tolerance;
		}

		public static float Distance( Vec4 v0, Vec4 v1 )
		{
			return ( v1 - v0 ).Magnitude();
		}

		public static float DistanceSquared( Vec4 v0, Vec4 v1 )
		{
			return ( v1 - v0 ).SqrMagnitude();
		}

		public static Vec4 ClampMagnitude( Vec4 v, float maxLength )
		{
			Vec4 nor = v;
			float sqrMagnitude = nor.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
				nor = nor * ( maxLength / MathUtils.Sqrt( sqrMagnitude ) );
			return nor;
		}

		public static Vec4 Normalize( Vec4 v )
		{
			return v * ( 1 / MathUtils.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w ) );
		}

		public static Vec4 NormalizeSafe( Vec4 v )
		{
			float dis = MathUtils.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w );
			if ( dis == 0f )
				return new Vec4();
			return v * ( 1 / dis );
		}

		public static float Dot( Vec4 v0, Vec4 v1 )
		{
			return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z + v0.w * v1.w;
		}

		public static Vec4 LerpUnclamped( Vec4 from, Vec4 to, float t )
		{
			return new Vec4( from.x + ( to.x - from.x ) * t, from.y + ( to.y - from.y ) * t, from.z + ( to.z - from.z ) * t, from.w + ( to.w - from.w ) * t );
		}

		public static Vec4 Lerp( Vec4 from, Vec4 to, float t )
		{
			return t <= 0 ? @from : ( t >= 1 ? to : LerpUnclamped( @from, to, t ) );
		}

		public static Vec4 Max( Vec4 v, float value )
		{
			return new Vec4( MathUtils.Max( v.x, value ), MathUtils.Max( v.y, value ), MathUtils.Max( v.z, value ),
							 MathUtils.Max( v.w, value ) );
		}

		public static Vec4 Max( Vec4 v, Vec4 values )
		{
			return new Vec4( MathUtils.Max( v.x, values.x ), MathUtils.Max( v.y, values.y ), MathUtils.Max( v.z, values.z ),
							 MathUtils.Max( v.w, values.w ) );
		}

		public static Vec4 Min( Vec4 v, float value )
		{
			return new Vec4( MathUtils.Min( v.x, value ), MathUtils.Min( v.y, value ), MathUtils.Min( v.z, value ),
							 MathUtils.Min( v.w, value ) );
		}

		public static Vec4 Min( Vec4 v, Vec4 values )
		{
			return new Vec4( MathUtils.Min( v.x, values.x ), MathUtils.Min( v.y, values.y ), MathUtils.Min( v.z, values.z ),
							 MathUtils.Min( v.w, values.w ) );
		}

		public static Vec4 Abs( Vec4 v )
		{
			return new Vec4( MathUtils.Abs( v.x ), MathUtils.Abs( v.y ), MathUtils.Abs( v.z ), MathUtils.Abs( v.w ) );
		}

		public static Vec4 Pow( Vec4 v, float power )
		{
			return new Vec4( MathUtils.Pow( v.x, power ), MathUtils.Pow( v.y, power ),
							 MathUtils.Pow( v.z, power ), MathUtils.Pow( v.w, power ) );
		}

		public static Vec4 Floor( Vec4 v )
		{
			return new Vec4( MathUtils.Floor( v.x ), MathUtils.Floor( v.y ), MathUtils.Floor( v.z ),
							 MathUtils.Floor( v.w ) );
		}

		public static Vec4 Round( Vec4 v )
		{
			return new Vec4( MathUtils.Round( v.x ), MathUtils.Round( v.y ), MathUtils.Round( v.z ),
							 MathUtils.Round( v.w ) );
		}

		#endregion
	}
}