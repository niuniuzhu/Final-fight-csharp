namespace Core.Math
{
	public struct Vec3
	{
		public float x;
		public float y;
		public float z;

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
					default:
						result = this.z;
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
				}
			}
		}

		private const float OVER_SQRT2 = 0.7071067811865475244008443621048490f;
		private static readonly Vec3 ONE = new Vec3( 1, 1, 1 );
		private static readonly Vec3 MINUS_ONE = new Vec3( -1, -1, -1 );
		private static readonly Vec3 ZERO = new Vec3( 0, 0, 0 );
		private static readonly Vec3 RIGHT = new Vec3( 1, 0, 0 );
		private static readonly Vec3 LEFT = new Vec3( -1, 0, 0 );
		private static readonly Vec3 UP = new Vec3( 0, 1, 0 );
		private static readonly Vec3 DOWN = new Vec3( 0, -1, 0 );
		private static readonly Vec3 FORWARD = new Vec3( 0, 0, 1 );
		private static readonly Vec3 BACKWARD = new Vec3( 0, 0, -1 );
		private static readonly Vec3 POSITIVE_INFINITY_VECTOR = new Vec3( float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity );
		private static readonly Vec3 NEGATIVE_INFINITY_VECTOR = new Vec3( float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity );
		public static Vec3 one => ONE;
		public static Vec3 minusOne => MINUS_ONE;
		public static Vec3 zero => ZERO;
		public static Vec3 right => RIGHT;
		public static Vec3 left => LEFT;
		public static Vec3 up => UP;
		public static Vec3 down => DOWN;
		public static Vec3 forward => FORWARD;
		public static Vec3 backward => BACKWARD;
		public static Vec3 positiveInfinityVector => POSITIVE_INFINITY_VECTOR;
		public static Vec3 negativeInfinityVector => NEGATIVE_INFINITY_VECTOR;

		public Vec3( float x, float y, float z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		#region Operators

		public static Vec3 operator +( Vec3 p1, Vec3 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			return p1;
		}

		public static Vec3 operator +( Vec3 p1, Vec2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static Vec3 operator +( Vec2 p1, Vec3 p2 )
		{
			p2.x = p1.x + p2.x;
			p2.y = p1.y + p2.y;
			return p2;
		}

		public static Vec3 operator +( Vec3 p1, float p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			return p1;
		}

		public static Vec3 operator +( float p1, Vec3 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			return p2;
		}

		public static Vec3 operator -( Vec3 p1, Vec3 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			return p1;
		}

		public static Vec3 operator -( Vec3 p1, Vec2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static Vec3 operator -( Vec2 p1, Vec3 p2 )
		{
			p2.x = p1.x - p2.x;
			p2.y = p1.y - p2.y;
			return p2;
		}

		public static Vec3 operator -( Vec3 p1, float p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			return p1;
		}

		public static Vec3 operator -( float p1, Vec3 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			return p2;
		}

		public static Vec3 operator -( Vec3 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			return p2;
		}

		public static Vec3 operator *( Vec3 p1, Vec3 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			p1.z *= p2.z;
			return p1;
		}

		public static Vec3 operator *( Vec3 p1, Vec2 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			return p1;
		}

		public static Vec3 operator *( Vec2 p1, Vec3 p2 )
		{
			p2.x = p1.x * p2.x;
			p2.y = p1.y * p2.y;
			return p2;
		}

		public static Vec3 operator *( Vec3 p1, float p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			return p1;
		}

		public static Vec3 operator *( float p1, Vec3 p2 )
		{
			p2.x *= p1;
			p2.y *= p1;
			p2.z *= p1;
			return p2;
		}

		public static Vec3 operator /( Vec3 p1, Vec3 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			return p1;
		}

		public static Vec3 operator /( Vec3 p1, Vec2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static Vec3 operator /( Vec2 p1, Vec3 p2 )
		{
			p2.x = p1.x / p2.x;
			p2.y = p1.y / p2.y;
			return p2;
		}

		public static Vec3 operator /( Vec3 p1, float p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			return p1;
		}

		public static Vec3 operator /( float p1, Vec3 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			return p2;
		}

		public static bool operator ==( Vec3 p1, Vec3 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
		}

		public static bool operator !=( Vec3 p1, Vec3 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( Vec3 )obj == this;
		}

		public override string ToString()
		{
			return $"({this.x}, {this.y}, {this.z})";
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
		}
		#endregion

		#region Methods
		public void Set( float x, float y, float z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public void ClampMagnitude( float maxLength )
		{
			float sqrMagnitude = this.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
			{
				float f = maxLength / MathUtils.Sqrt( sqrMagnitude );
				this.x *= f;
				this.y *= f;
				this.z *= f;
			}
		}

		public float Magnitude()
		{
			return MathUtils.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z );
		}

		public float SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y + this.z * this.z;
		}

		public float Distance( Vec3 vector )
		{
			return ( vector - this ).Magnitude();
		}

		public float DistanceSquared( Vec3 vector )
		{
			return ( vector - this ).SqrMagnitude();
		}

		public void Negate()
		{
			this.x = -this.x;
			this.y = -this.y;
			this.z = -this.z;
		}

		public void Scale( Vec3 scale )
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		public float Dot( Vec3 v )
		{
			return this.x * v.x + this.y * v.y + this.z * v.z;
		}

		public void Normalize()
		{
			float f = 1 / MathUtils.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z );
			this.x *= f;
			this.y *= f;
			this.z *= f;
		}

		public void NormalizeSafe()
		{
			float f = MathUtils.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z );
			if ( f == 0 )
				return;
			this.x *= f;
			this.y *= f;
			this.z *= f;
		}

		public Vec3 Cross( Vec3 v )
		{
			return new Vec3( this.y * v.z - this.z * v.y, this.z * v.x - this.x * v.z,
							 this.x * v.y - this.y * v.x );
		}

		public bool AproxEqualsBox( Vec3 vector, float tolerance )
		{
			return
				( MathUtils.Abs( this.x - vector.x ) <= tolerance ) &&
				( MathUtils.Abs( this.y - vector.y ) <= tolerance ) &&
				( MathUtils.Abs( this.z - vector.z ) <= tolerance );
		}

		public bool ApproxEquals( Vec3 vector, float tolerance )
		{
			return this.Distance( vector ) <= tolerance;
		}

		public Vec3 RotateAround( float angle, Vec3 axis )
		{
			// rotate into world space
			Quat quaternion = Quat.AngleAxis( 0, axis );
			quaternion = quaternion.Conjugate();
			Vec3 worldSpaceVector = quaternion.Transform( this );

			// rotate back to vector space
			quaternion = Quat.AngleAxis( angle, axis );
			worldSpaceVector = quaternion.Transform( worldSpaceVector );
			return worldSpaceVector;
		}

		public bool IntersectsTriangle( Vec3 p0, Vec3 p1, Vec3 p2 )
		{
			Vec3 v0 = p1 - p0;
			Vec3 v1 = p2 - p0;
			Vec3 v2 = this - p0;

			float dot00 = v0.SqrMagnitude();
			float dot01 = v0.Dot( v1 );
			float dot02 = v0.Dot( v2 );
			float dot11 = v1.SqrMagnitude();
			float dot12 = v1.Dot( v2 );

			float invDenom = 1 / ( dot00 * dot11 - dot01 * dot01 );
			float u = ( dot11 * dot02 - dot01 * dot12 ) * invDenom;
			float v = ( dot00 * dot12 - dot01 * dot02 ) * invDenom;
			return ( u > 0 ) && ( v > 0 ) && ( u + v < 1 );
		}

		public Vec3 Reflect( Vec3 planeNormal )
		{
			return this - planeNormal * this.Dot( planeNormal ) * 2;
		}

		public Vec3 Refract( Vec3 normal, float refractionIndex )
		{
			//float refractionIndex = refractionIndexEnter / refractionIndexExit;
			float cosI = -normal.Dot( this );
			float sinT2 = refractionIndex * refractionIndex * ( 1.0f - cosI * cosI );

			if ( sinT2 > 1.0f ) return this;

			float cosT = MathUtils.Sqrt( 1.0f - sinT2 );
			return this * refractionIndex + normal * ( refractionIndex * cosI - cosT );
		}

		public Vec3 InersectNormal( Vec3 normal )
		{
			return normal * this.Dot( normal );
		}

		public Vec3 InersectRay( Vec3 rayOrigin, Vec3 rayDirection )
		{
			return rayDirection * ( this - rayOrigin ).Dot( rayDirection ) + rayOrigin;
		}

		public Vec3 InersectLine( Line3 line )
		{
			Vec3 pointOffset = this - line.point1;
			Vec3 vector = Normalize( line.point2 - line.point1 );
			return vector * pointOffset.Dot( vector ) + line.point1;
		}

		public Vec3 InersectPlane( Vec3 planeNormal )
		{
			return this - planeNormal * this.Dot( planeNormal );
		}

		public Vec3 InersectPlane( Vec3 planeNormal, Vec3 planeLocation )
		{
			return this - planeNormal * ( this - planeLocation ).Dot( planeNormal );
		}

		public static float Distance( Vec3 v0, Vec3 v1 )
		{
			return ( v1 - v0 ).Magnitude();
		}

		public static float DistanceSquared( Vec3 v0, Vec3 v1 )
		{
			return ( v1 - v0 ).SqrMagnitude();
		}

		public static float Angle( Vec3 from, Vec3 to )
		{
			return MathUtils.Acos( MathUtils.Clamp( Dot( Normalize( @from ), Normalize( to ) ), -1, 1 ) ) * MathUtils.RAD_TO_DEG;
		}

		public static Vec3 ClampMagnitude( Vec3 v, float maxLength )
		{
			Vec3 nor = v;
			float sqrMagnitude = nor.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
				nor = nor * ( maxLength / MathUtils.Sqrt( sqrMagnitude ) );
			return nor;
		}

		public static Vec3 Normalize( Vec3 v )
		{
			return v * ( 1 / MathUtils.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z ) );
		}

		public static Vec3 NormalizeSafe( Vec3 v )
		{
			float dis = MathUtils.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z );
			if ( dis == 0f )
				return new Vec3();
			return v * ( 1 / dis );
		}

		public static float Dot( Vec3 v0, Vec3 v1 )
		{
			return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;
		}

		public static Vec3 Cross( Vec3 v0, Vec3 v1 )
		{
			return new Vec3( v0.y * v1.z - v0.z * v1.y, v0.z * v1.x - v0.x * v1.z,
							 v0.x * v1.y - v0.y * v1.x );
		}

		public static Vec3 OrthoNormalVector( Vec3 v )
		{
			Vec3 res = new Vec3();

			if ( MathUtils.Abs( v.z ) > OVER_SQRT2 )
			{
				float a = v.y * v.y + v.z * v.z;
				float k = 1f / MathUtils.Sqrt( a );
				res.x = 0;
				res.y = -v.z * k;
				res.z = v.y * k;
			}
			else
			{
				float a = v.x * v.x + v.y * v.y;
				float k = 1f / MathUtils.Sqrt( a );
				res.x = -v.y * k;
				res.y = v.x * k;
				res.z = 0;
			}

			return res;
		}

		public static Vec3 SlerpUnclamped( Vec3 from, Vec3 to, float t )
		{
			float scale0, scale1;

			float len2 = to.Magnitude();
			float len1 = from.Magnitude();
			Vec3 v2 = to / len2;
			Vec3 v1 = from / len1;

			float len = ( len2 - len1 ) * t + len1;
			float cosom = Dot( v1, v2 );

			if ( cosom > 1 - 1e-6 )
			{
				scale0 = 1 - t;
				scale1 = t;
			}
			else if ( cosom < -1 + 1e-6 )
			{
				Vec3 axis = OrthoNormalVector( from );
				Quat q = Quat.AngleAxis( 180.0f * t, axis );
				Vec3 v = q * from * len;
				return v;
			}
			else
			{
				float omega = MathUtils.Acos( cosom );
				float sinom = MathUtils.Sin( omega );
				scale0 = MathUtils.Sin( ( 1 - t ) * omega ) / sinom;
				scale1 = MathUtils.Sin( t * omega ) / sinom;
			}

			v2 = ( v2 * scale1 + v1 * scale0 ) * len;
			return v2;
		}

		public static Vec3 Slerp( Vec3 from, Vec3 to, float t )
		{
			return t <= 0 ? @from : ( t >= 1 ? to : SlerpUnclamped( @from, to, t ) );
		}

		public static Vec3 LerpUnclamped( Vec3 from, Vec3 to, float t )
		{
			return new Vec3( from.x + ( to.x - from.x ) * t, from.y + ( to.y - from.y ) * t, from.z + ( to.z - from.z ) * t );
		}

		public static Vec3 Lerp( Vec3 from, Vec3 to, float t )
		{
			return t <= 0 ? @from : ( t >= 1 ? to : LerpUnclamped( @from, to, t ) );
		}

		public static Vec3 SmoothDamp( Vec3 current, Vec3 target, ref Vec3 currentVelocity, float smoothTime, float maxSpeed,
									   float deltaTime )
		{
			smoothTime = MathUtils.Max( 0.0001f, smoothTime );

			float num = 2 / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / ( 1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2 );
			float maxLength = maxSpeed * smoothTime;
			Vec3 vector = current - target;

			vector.ClampMagnitude( maxLength );

			target = current - vector;
			Vec3 vec3 = ( currentVelocity + ( vector * num ) ) * deltaTime;
			currentVelocity = ( currentVelocity - ( vec3 * num ) ) * num3;

			Vec3 vector4 = target + ( vector + vec3 ) * num3;

			if ( Dot( target - current, vector4 - target ) > 0 )
			{
				vector4 = target;
				currentVelocity.Set( 0, 0, 0 );
			}
			return vector4;
		}

		public static Vec3 MoveTowards( Vec3 current, Vec3 target, float maxDistanceDelta )
		{
			Vec3 delta = target - current;

			float sqrDelta = delta.SqrMagnitude();
			float sqrDistance = maxDistanceDelta * maxDistanceDelta;

			if ( sqrDelta > sqrDistance )
			{
				float magnitude = MathUtils.Sqrt( sqrDelta );
				if ( magnitude > 1e-6 )
				{
					delta = delta * maxDistanceDelta / magnitude + current;
					return delta;
				}
				return current;
			}
			return target;
		}

		private static float ClampedMove( float lhs, float rhs, float clampedDelta )
		{
			float delta = rhs - lhs;
			if ( delta > 0 )
				return lhs + MathUtils.Min( delta, clampedDelta );
			return lhs - MathUtils.Min( -delta, clampedDelta );
		}

		public static Vec3 RotateTowards( Vec3 current, Vec3 target, float maxRadiansDelta, float maxMagnitudeDelta )
		{
			float len1 = current.Magnitude();
			float len2 = target.Magnitude();

			if ( len1 > 1e-6 && len2 > 1e-6 )
			{
				Vec3 from = current / len1;
				Vec3 to = target / len2;
				float cosom = Dot( from, to );

				if ( cosom > 1 - 1e-6 )
					return MoveTowards( current, target, maxMagnitudeDelta );
				if ( cosom < -1 + 1e-6 )
				{
					Quat q = Quat.AngleAxis( maxRadiansDelta * MathUtils.RAD_TO_DEG, OrthoNormalVector( from ) );
					return q * from * ClampedMove( len1, len2, maxMagnitudeDelta );
				}
				else
				{
					float angle = MathUtils.Acos( cosom );
					Quat q = Quat.AngleAxis( MathUtils.Min( maxRadiansDelta, angle ) * MathUtils.RAD_TO_DEG,
											 Normalize( Cross( from, to ) ) );
					return q * from * ClampedMove( len1, len2, maxMagnitudeDelta );
				}
			}

			return MoveTowards( current, target, maxMagnitudeDelta );
		}

		public static void OrthoNormalize( ref Vec3 va, ref Vec3 vb, ref Vec3 vc )
		{
			va.Normalize();
			vb -= Project( vb, va );
			vb.Normalize();

			vc -= Project( vc, va );
			vc -= Project( vc, vb );
			vc.Normalize();
		}

		public static Vec3 Project( Vec3 vector, Vec3 onNormal )
		{
			float num = onNormal.SqrMagnitude();

			if ( num < 1.175494e-38 )
				return zero;

			float num2 = Dot( vector, onNormal );
			Vec3 v3 = onNormal * ( num2 / num );
			return v3;
		}

		public static Vec3 ProjectOnPlane( Vec3 vector, Vec3 planeNormal )
		{
			return Project( vector, planeNormal ) * -1 + vector;
		}

		public static Vec3 Reflect( Vec3 inDirection, Vec3 inNormal )
		{
			float num = -2 * Dot( inNormal, inDirection );
			inNormal = inNormal * num;
			inNormal += inDirection;
			return inNormal;
		}

		public static Vec3 Hermite( Vec3 value1, Vec3 tangent1, Vec3 value2, Vec3 tangent2, float t )
		{
			float weightSquared = t * t;
			float weightCubed = t * weightSquared;
			float value1Blend = 2 * weightCubed - 3 * weightSquared + 1;
			float tangent1Blend = weightCubed - 2 * weightSquared + t;
			float value2Blend = -2 * weightCubed + 3 * weightSquared;
			float tangent2Blend = weightCubed - weightSquared;

			return new Vec3
				(
				value1.x * value1Blend + value2.x * value2Blend + tangent1.x * tangent1Blend + tangent2.x * tangent2Blend,
				value1.y * value1Blend + value2.y * value2Blend + tangent1.y * tangent1Blend + tangent2.y * tangent2Blend,
				value1.z * value1Blend + value2.z * value2Blend + tangent1.z * tangent1Blend + tangent2.z * tangent2Blend
				);
		}

		public static Vec3 DegToRad( Vec3 v )
		{
			return new Vec3( MathUtils.DegToRad( v.x ), MathUtils.DegToRad( v.y ), MathUtils.DegToRad( v.z ) );
		}

		public static Vec3 RadToDeg( Vec3 v )
		{
			return new Vec3( MathUtils.RadToDeg( v.x ), MathUtils.RadToDeg( v.y ), MathUtils.RadToDeg( v.z ) );
		}

		public static Vec3 Max( Vec3 v, float value )
		{
			return new Vec3( MathUtils.Max( v.x, value ), MathUtils.Max( v.y, value ), MathUtils.Max( v.z, value ) );
		}

		public static Vec3 Max( Vec3 v, Vec3 v1 )
		{
			return new Vec3( MathUtils.Max( v.x, v1.x ), MathUtils.Max( v.y, v1.y ), MathUtils.Max( v.z, v1.z ) );
		}

		public static Vec3 Min( Vec3 v, float v1 )
		{
			return new Vec3( MathUtils.Min( v.x, v1 ), MathUtils.Min( v.y, v1 ), MathUtils.Min( v.z, v1 ) );
		}

		public static Vec3 Min( Vec3 v, Vec3 v1 )
		{
			return new Vec3( MathUtils.Min( v.x, v1.x ), MathUtils.Min( v.y, v1.y ), MathUtils.Min( v.z, v1.z ) );
		}

		public static Vec3 Abs( Vec3 v )
		{
			return new Vec3( MathUtils.Abs( v.x ), MathUtils.Abs( v.y ), MathUtils.Abs( v.z ) );
		}

		public static Vec3 Pow( Vec3 v, float value )
		{
			return new Vec3( MathUtils.Pow( v.x, value ), MathUtils.Pow( v.y, value ),
							 MathUtils.Pow( v.z, value ) );
		}

		public static Vec3 Floor( Vec3 v )
		{
			return new Vec3( MathUtils.Floor( v.x ), MathUtils.Floor( v.y ), MathUtils.Floor( v.z ) );
		}

		public static Vec3 Round( Vec3 v )
		{
			return new Vec3( MathUtils.Round( v.x ), MathUtils.Round( v.y ), MathUtils.Round( v.z ) );
		}

		#endregion
	}
}