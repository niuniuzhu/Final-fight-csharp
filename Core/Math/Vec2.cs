namespace Core.Math
{
	public struct Vec2
	{
		public float x;
		public float y;

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
					default:
						result = this.y;
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
				}
			}
		}

		private static readonly Vec2 ONE = new Vec2( 1, 1 );
		private static readonly Vec2 MINUS_ONE = new Vec2( -1, -1 );
		private static readonly Vec2 ZERO = new Vec2( 0, 0 );
		private static readonly Vec2 RIGHT = new Vec2( 1, 0 );
		private static readonly Vec2 LEFT = new Vec2( -1, 0 );
		private static readonly Vec2 UP = new Vec2( 0, 1 );
		private static readonly Vec2 DOWN = new Vec2( 0, -1 );
		private static readonly Vec2 POSITIVE_INFINITY_VECTOR = new Vec2( float.PositiveInfinity, float.PositiveInfinity );
		private static readonly Vec2 NEGATIVE_INFINITY_VECTOR = new Vec2( float.NegativeInfinity, float.NegativeInfinity );

		public static Vec2 one => ONE;
		public static Vec2 minusOne => MINUS_ONE;
		public static Vec2 zero => ZERO;
		public static Vec2 right => RIGHT;
		public static Vec2 left => LEFT;
		public static Vec2 up => UP;
		public static Vec2 down => DOWN;
		public static Vec2 positiveInfinityVector => POSITIVE_INFINITY_VECTOR;
		public static Vec2 negativeInfinityVector => NEGATIVE_INFINITY_VECTOR;

		public Vec2( float x, float y )
		{
			this.x = x;
			this.y = y;
		}

		#region Operators

		public static Vec2 operator +( Vec2 p1, Vec2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static Vec2 operator +( Vec2 p1, float p2 )
		{
			p1.x += p2;
			p1.y += p2;
			return p1;
		}

		public static Vec2 operator +( float p1, Vec2 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			return p2;
		}

		public static Vec2 operator -( Vec2 p1, Vec2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static Vec2 operator -( Vec2 p1, float p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			return p1;
		}

		public static Vec2 operator -( float p1, Vec2 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			return p2;
		}

		public static Vec2 operator -( Vec2 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			return p2;
		}

		public static Vec2 operator *( Vec2 p1, Vec2 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			return p1;
		}

		public static Vec2 operator *( Vec2 p1, float p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			return p1;
		}

		public static Vec2 operator *( float p1, Vec2 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			return p2;
		}

		public static Vec2 operator *( Mat2 m, Vec2 v )
		{
			return new Vec2
			(
				v.x * m.x.x + v.y * m.y.x,
				v.x * m.x.y + v.y * m.y.y
			);
		}

		public static Vec2 operator /( Vec2 p1, Vec2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static Vec2 operator /( Vec2 p1, float p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			return p1;
		}

		public static Vec2 operator /( float p1, Vec2 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			return p2;
		}

		public static implicit operator Vec2( Vec3 v )
		{
			return new Vec2( v.x, v.y );
		}

		public static implicit operator Vec3( Vec2 v )
		{
			return new Vec3( v.x, v.y, 0 );
		}

		public static bool operator ==( Vec2 p1, Vec2 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y;
		}

		public static bool operator !=( Vec2 p1, Vec2 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( Vec2 )obj == this;
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

		public void ClampMagnitude( float maxLength )
		{
			float sqrMagnitude = this.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
			{
				float f = maxLength / MathUtils.Sqrt( sqrMagnitude );
				this.x *= f;
				this.y *= f;
			}
		}

		public float Magnitude()
		{
			return MathUtils.Sqrt( this.x * this.x + this.y * this.y );
		}

		public float SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y;
		}

		public float Distance( Vec2 vector )
		{
			return ( vector - this ).Magnitude();
		}

		public float DistanceSquared( Vec2 vector )
		{
			return ( vector - this ).SqrMagnitude();
		}

		public void Negate()
		{
			this.x = -this.x;
			this.y = -this.y;
		}

		public void Scale( Vec2 scale )
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}

		public float Dot()
		{
			return this.x * this.x + this.y * this.y;
		}

		public float Dot( Vec2 vector )
		{
			return this.x * vector.x + this.y * vector.y;
		}

		public void Normalize()
		{
			float f = 1 / MathUtils.Sqrt( this.x * this.x + this.y * this.y );
			this.x *= f;
			this.y *= f;
		}

		public void NormalizeSafe()
		{
			float f = MathUtils.Sqrt( this.x * this.x + this.y * this.y );
			if ( f == 0 )
				return;
			this.x *= f;
			this.y *= f;
		}

		public bool AproxEqualsBox( Vec2 vector, float tolerance )
		{
			return
				( MathUtils.Abs( this.x - vector.x ) <= tolerance ) &&
				( MathUtils.Abs( this.y - vector.y ) <= tolerance );
		}

		public bool ApproxEquals( Vec2 vector, float tolerance )
		{
			return this.Distance( vector ) <= tolerance;
		}

		public float Angle()
		{
			Vec2 vec = Normalize( this );
			float val = vec.x;
			val = val > 1 ? 1 : val;
			val = val < -1 ? -1 : val;
			return MathUtils.Acos( val );
		}

		public float Angle( Vec2 vector )
		{
			Vec2 vec = Normalize( this );
			float val = vec.Dot( Normalize( vector ) );
			val = val > 1 ? 1 : val;
			val = val < -1 ? -1 : val;
			return MathUtils.Acos( val );
		}

		public float Angle90()
		{
			Vec2 vec = Normalize( this );
			float val = MathUtils.Abs( vec.x );
			val = val > 1 ? 1 : val;
			return MathUtils.Acos( val );
		}

		public float Angle90( Vec2 vector )
		{
			Vec2 vec = Normalize( this );
			float val = MathUtils.Abs( vec.Dot( Normalize( vector ) ) );
			val = val > 1 ? 1 : val;
			return MathUtils.Acos( val );
		}

		public float Angle180()
		{
			Vec2 vec = Normalize( this );
			return MathUtils.Atan2( -vec.y, vec.x ) % MathUtils.PI2;
		}

		public float Angle180( Vec2 vector )
		{
			Vec2 vec = Normalize( this );
			vector = Normalize( vector );
			return MathUtils.Atan2( vec.x * vector.y - vec.y * vector.x, vec.x * vector.x + vec.y * vector.y ) %
				   MathUtils.PI2;
		}

		public float Angle360()
		{
			Vec2 vec = Normalize( this );
			float value = MathUtils.Atan2( -vec.y, vec.x ) % MathUtils.PI2;
			return value < 0 ? MathUtils.PI + value + MathUtils.PI : value;
		}

		public float Angle360( Vec2 vector )
		{
			Vec2 vec = Normalize( this );
			vector = Normalize( vector );
			float value = MathUtils.Atan2( vec.x * vector.y - vec.y * vector.x, vec.x * vector.x + vec.y * vector.y ) %
						  MathUtils.PI2;
			return value < 0 ? MathUtils.PI + value + MathUtils.PI : value;
		}

		public static float Distance( Vec2 v0, Vec2 v1 )
		{
			return ( v1 - v0 ).Magnitude();
		}

		public static float DistanceSquared( Vec2 v0, Vec2 v1 )
		{
			return ( v1 - v0 ).SqrMagnitude();
		}

		public static Vec2 ClampMagnitude( Vec2 v, float maxLength )
		{
			Vec2 nor = v;
			float sqrMagnitude = nor.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
				nor = nor * ( maxLength / MathUtils.Sqrt( sqrMagnitude ) );
			return nor;
		}

		public static Vec2 Normalize( Vec2 v )
		{
			return v * ( 1 / MathUtils.Sqrt( v.x * v.x + v.y * v.y ) );
		}

		public static Vec2 NormalizeSafe( Vec2 v )
		{
			float dis = MathUtils.Sqrt( v.x * v.x + v.y * v.y );
			if ( dis == 0f )
				return new Vec2();
			return v * ( 1 / dis );
		}

		public static Vec2 LerpUnclamped( Vec2 from, Vec2 to, float t )
		{
			return new Vec2( from.x + ( to.x - from.x ) * t, from.y + ( to.y - from.y ) * t );
		}

		public static Vec2 Lerp( Vec2 from, Vec2 to, float t )
		{
			return t <= 0 ? @from : ( t >= 1 ? to : LerpUnclamped( @from, to, t ) );
		}
		public static float SlopeXy( Vec2 v )
		{
			return v.x / v.y;
		}

		public static float SlopeYx( Vec2 v )
		{
			return v.y / v.x;
		}

		public static Vec2 DegToRad( Vec2 v )
		{
			return new Vec2( MathUtils.DegToRad( v.x ), MathUtils.DegToRad( v.y ) );
		}

		public static Vec2 RadToDeg( Vec2 v )
		{
			return new Vec2( MathUtils.RadToDeg( v.x ), MathUtils.RadToDeg( v.y ) );
		}

		public static Vec2 Max( Vec2 v, float value )
		{
			return new Vec2( MathUtils.Max( v.x, value ), MathUtils.Max( v.y, value ) );
		}

		public static Vec2 Max( Vec2 v, Vec2 values )
		{
			return new Vec2( MathUtils.Max( v.x, values.x ), MathUtils.Max( v.y, values.y ) );
		}

		public static Vec2 Min( Vec2 v, float value )
		{
			return new Vec2( MathUtils.Min( v.x, value ), MathUtils.Min( v.y, value ) );
		}

		public static Vec2 Min( Vec2 v, Vec2 values )
		{
			return new Vec2( MathUtils.Min( v.x, values.x ), MathUtils.Min( v.y, values.y ) );
		}

		public static Vec2 Abs( Vec2 v )
		{
			return new Vec2( MathUtils.Abs( v.x ), MathUtils.Abs( v.y ) );
		}

		public static Vec2 Pow( Vec2 v, float value )
		{
			return new Vec2( MathUtils.Pow( v.x, value ), MathUtils.Pow( v.y, value ) );
		}

		public static Vec2 Floor( Vec2 v )
		{
			return new Vec2( MathUtils.Floor( v.x ), MathUtils.Floor( v.y ) );
		}

		public static Vec2 Round( Vec2 v )
		{
			return new Vec2( MathUtils.Round( v.x ), MathUtils.Round( v.y ) );
		}

		#endregion
	}
}