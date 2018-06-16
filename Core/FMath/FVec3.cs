using System;

namespace Core.FMath
{
	public struct FVec3
	{
		public Fix64 x, y, z;

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
					default:
						throw new IndexOutOfRangeException( "Invalid Vector3 index!" );
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
					default:
						throw new IndexOutOfRangeException( "Invalid Vector3 index!" );
				}
			}
		}

		private static readonly Fix64 OVER_SQRT2 = ( Fix64 )0.7071067811865475244008443621048490f;
		private static readonly FVec3 ONE = new FVec3( 1 );
		private static readonly FVec3 MINUS_ONE = new FVec3( -1 );
		private static readonly FVec3 ZERO = new FVec3( 0 );
		private static readonly FVec3 RIGHT = new FVec3( 1, 0, 0 );
		private static readonly FVec3 LEFT = new FVec3( -1, 0, 0 );
		private static readonly FVec3 UP = new FVec3( 0, 1, 0 );
		private static readonly FVec3 DOWN = new FVec3( 0, -1, 0 );
		private static readonly FVec3 FORWARD = new FVec3( 0, 0, 1 );
		private static readonly FVec3 BACKWARD = new FVec3( 0, 0, -1 );
		private static readonly FVec3 POSITIVE_INFINITY_VECTOR = new FVec3( float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity );
		private static readonly FVec3 NEGATIVE_INFINITY_VECTOR = new FVec3( float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity );
		public static FVec3 one => ONE;
		public static FVec3 minusOne => MINUS_ONE;
		public static FVec3 zero => ZERO;
		public static FVec3 right => RIGHT;
		public static FVec3 left => LEFT;
		public static FVec3 up => UP;
		public static FVec3 down => DOWN;
		public static FVec3 forward => FORWARD;
		public static FVec3 backward => BACKWARD;
		public static FVec3 positiveInfinityVector => POSITIVE_INFINITY_VECTOR;
		public static FVec3 negativeInfinityVector => NEGATIVE_INFINITY_VECTOR;

		public FVec3( float value )
		{
			this.x = ( Fix64 )value;
			this.y = ( Fix64 )value;
			this.z = ( Fix64 )value;
		}

		public FVec3( float x, float y, float z )
		{
			this.x = ( Fix64 )x;
			this.y = ( Fix64 )y;
			this.z = ( Fix64 )z;
		}

		public FVec3( Fix64 value )
		{
			this.x = value;
			this.y = value;
			this.z = value;
		}

		public FVec3( Fix64 x, Fix64 y, Fix64 z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		#region Operators

		public static FVec3 operator +( FVec3 p1, FVec3 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			return p1;
		}

		public static FVec3 operator +( FVec3 p1, FVec2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static FVec3 operator +( FVec2 p1, FVec3 p2 )
		{
			p2.x = p1.x + p2.x;
			p2.y = p1.y + p2.y;
			return p2;
		}

		public static FVec3 operator +( FVec3 p1, Fix64 p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			return p1;
		}

		public static FVec3 operator +( Fix64 p1, FVec3 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			return p2;
		}

		public static FVec3 operator -( FVec3 p1, FVec3 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			return p1;
		}

		public static FVec3 operator -( FVec3 p1, FVec2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static FVec3 operator -( FVec2 p1, FVec3 p2 )
		{
			p2.x = p1.x - p2.x;
			p2.y = p1.y - p2.y;
			return p2;
		}

		public static FVec3 operator -( FVec3 p1, Fix64 p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			return p1;
		}

		public static FVec3 operator -( Fix64 p1, FVec3 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			return p2;
		}

		public static FVec3 operator -( FVec3 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			return p2;
		}

		public static FVec3 operator *( FVec3 p1, FVec3 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			p1.z *= p2.z;
			return p1;
		}

		public static FVec3 operator *( FVec3 p1, FVec2 p2 )
		{
			p1.x *= p2.x;
			p1.y *= p2.y;
			return p1;
		}

		public static FVec3 operator *( FVec2 p1, FVec3 p2 )
		{
			p2.x = p1.x * p2.x;
			p2.y = p1.y * p2.y;
			return p2;
		}

		public static FVec3 operator *( FVec3 p1, Fix64 p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			return p1;
		}

		public static FVec3 operator *( Fix64 p1, FVec3 p2 )
		{
			p2.x *= p1;
			p2.y *= p1;
			p2.z *= p1;
			return p2;
		}

		public static FVec3 operator *( FMat3 m, FVec3 v )
		{
			return new FVec3
			(
				v.x * m.x.x + v.y * m.y.x + v.z * m.z.x,
				v.x * m.x.y + v.y * m.y.y + v.z * m.z.y,
				v.x * m.x.z + v.y * m.y.z + v.z * m.z.z
			);
		}

		public static FVec3 operator /( FVec3 p1, FVec3 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			return p1;
		}

		public static FVec3 operator /( FVec3 p1, FVec2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static FVec3 operator /( FVec2 p1, FVec3 p2 )
		{
			p2.x = p1.x / p2.x;
			p2.y = p1.y / p2.y;
			return p2;
		}

		public static FVec3 operator /( FVec3 p1, Fix64 p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			return p1;
		}

		public static FVec3 operator /( Fix64 p1, FVec3 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			return p2;
		}

		public static bool operator ==( FVec3 p1, FVec3 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
		}

		public static bool operator !=( FVec3 p1, FVec3 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( FVec3 )obj == this;
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
		public void Set( Fix64 x, Fix64 y, Fix64 z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public void ClampMagnitude( Fix64 maxLength )
		{
			Fix64 sqrMagnitude = this.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
			{
				Fix64 f = maxLength / Fix64.Sqrt( sqrMagnitude );
				this.x *= f;
				this.y *= f;
				this.z *= f;
			}
		}

		public Fix64 Magnitude()
		{
			return Fix64.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z );
		}

		public Fix64 SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y + this.z * this.z;
		}

		public Fix64 Distance( FVec3 vector )
		{
			return ( vector - this ).Magnitude();
		}

		public Fix64 DistanceSquared( FVec3 vector )
		{
			return ( vector - this ).Dot();
		}

		public void Negate()
		{
			this.x = -this.x;
			this.y = -this.y;
			this.z = -this.z;
		}

		public void Scale( FVec3 scale )
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		public Fix64 Dot()
		{
			return this.x * this.x + this.y * this.y + this.z * this.z;
		}

		public Fix64 Dot( FVec3 v )
		{
			return this.x * v.x + this.y * v.y + this.z * v.z;
		}

		public void Normalize()
		{
			Fix64 f = Fix64.One / Fix64.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z );
			this.x *= f;
			this.y *= f;
			this.z *= f;
		}

		public FVec3 NormalizeSafe()
		{
			Fix64 dis = Fix64.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z );
			if ( dis == Fix64.Zero ) return new FVec3();
			return this * ( Fix64.One / dis );
		}

		public FVec3 Cross( FVec3 v )
		{
			return new FVec3( this.y * v.z - this.z * v.y, this.z * v.x - this.x * v.z,
							 this.x * v.y - this.y * v.x );
		}

		public FVec3 TransformNormal( FMat4 matrix )
		{
			return new FVec3
				(
				this.x * matrix.x.x + this.y * matrix.y.x + this.z * matrix.z.x,
				this.x * matrix.x.y + this.y * matrix.y.y + this.z * matrix.z.y,
				this.x * matrix.x.z + this.y * matrix.y.z + this.z * matrix.z.z
				);
		}

		public FVec3 Transform( FMat3 matrix )
		{
			return matrix.x * this.x + matrix.y * this.y + matrix.z * this.z;
		}

		public FVec3 TransformTranspose( FMat3 matrix )
		{
			return new FVec3
				(
				this.x * matrix.x.x + this.y * matrix.x.y + this.z * matrix.x.z,
				this.x * matrix.y.x + this.y * matrix.y.y + this.z * matrix.y.z,
				this.x * matrix.z.x + this.y * matrix.z.y + this.z * matrix.z.z
				);
		}

		public FVec3 Transform( FQuat quaternion )
		{
			Fix64 x2 = quaternion.x + quaternion.x;
			Fix64 y2 = quaternion.y + quaternion.y;
			Fix64 z2 = quaternion.z + quaternion.z;
			Fix64 xx2 = quaternion.x * x2;
			Fix64 xy2 = quaternion.x * y2;
			Fix64 xz2 = quaternion.x * z2;
			Fix64 yy2 = quaternion.y * y2;
			Fix64 yz2 = quaternion.y * z2;
			Fix64 zz2 = quaternion.z * z2;
			Fix64 wx2 = quaternion.w * x2;
			Fix64 wy2 = quaternion.w * y2;
			Fix64 wz2 = quaternion.w * z2;

			return new FVec3
				(
				this.x * ( Fix64.One - yy2 - zz2 ) + this.y * ( xy2 - wz2 ) + this.z * ( xz2 + wy2 ),
				this.x * ( xy2 + wz2 ) + this.y * ( Fix64.One - xx2 - zz2 ) + this.z * ( yz2 - wx2 ),
				this.x * ( xz2 - wy2 ) + this.y * ( yz2 + wx2 ) + this.z * ( Fix64.One - xx2 - yy2 )
				);
		}

		public bool AproxEqualsBox( FVec3 vector, Fix64 tolerance )
		{
			return
				( Fix64.Abs( this.x - vector.x ) <= tolerance ) &&
				( Fix64.Abs( this.y - vector.y ) <= tolerance ) &&
				( Fix64.Abs( this.z - vector.z ) <= tolerance );
		}

		public bool ApproxEquals( FVec3 vector, Fix64 tolerance )
		{
			return this.Distance( vector ) <= tolerance;
		}

		public FVec3 RotateAround( Fix64 angle, FVec3 axis )
		{
			// rotate into world space
			FQuat quaternion = FQuat.AngleAxis( Fix64.Zero, axis );
			quaternion = quaternion.Conjugate();
			FVec3 worldSpaceVector = this.Transform( quaternion );

			// rotate back to vector space
			quaternion = FQuat.AngleAxis( angle, axis );
			worldSpaceVector = worldSpaceVector.Transform( quaternion );
			return worldSpaceVector;
		}

		public bool IntersectsTriangle( FVec3 p0, FVec3 p1, FVec3 p2 )
		{
			FVec3 v0 = p1 - p0;
			FVec3 v1 = p2 - p0;
			FVec3 v2 = this - p0;

			Fix64 dot00 = v0.Dot();
			Fix64 dot01 = v0.Dot( v1 );
			Fix64 dot02 = v0.Dot( v2 );
			Fix64 dot11 = v1.Dot();
			Fix64 dot12 = v1.Dot( v2 );

			Fix64 invDenom = Fix64.One / ( dot00 * dot11 - dot01 * dot01 );
			Fix64 u = ( dot11 * dot02 - dot01 * dot12 ) * invDenom;
			Fix64 v = ( dot00 * dot12 - dot01 * dot02 ) * invDenom;
			return ( u > Fix64.Zero ) && ( v > Fix64.Zero ) && ( u + v < Fix64.One );
		}

		public FVec3 Reflect( FVec3 planeNormal )
		{
			return this - planeNormal * this.Dot( planeNormal ) * ( Fix64 )2;
		}

		public FVec3 Refract( FVec3 normal, Fix64 refractionIndex )
		{
			//Fix64 refractionIndex = refractionIndexEnter / refractionIndexExit;
			Fix64 cosI = -normal.Dot( this );
			Fix64 sinT2 = refractionIndex * refractionIndex * ( Fix64.One - cosI * cosI );

			if ( sinT2 > Fix64.One ) return this;

			Fix64 cosT = Fix64.Sqrt( Fix64.One - sinT2 );
			return this * refractionIndex + normal * ( refractionIndex * cosI - cosT );
		}

		public FVec3 InersectNormal( FVec3 normal )
		{
			return normal * this.Dot( normal );
		}

		public FVec3 InersectRay( FVec3 rayOrigin, FVec3 rayDirection )
		{
			return rayDirection * ( this - rayOrigin ).Dot( rayDirection ) + rayOrigin;
		}

		public FVec3 InersectLine( FLine3 line )
		{
			FVec3 pointOffset = this - line.point1;
			FVec3 vector = Normalize( line.point2 - line.point1 );
			return vector * pointOffset.Dot( vector ) + line.point1;
		}

		public FVec3 InersectPlane( FVec3 planeNormal )
		{
			return this - planeNormal * this.Dot( planeNormal );
		}

		public FVec3 InersectPlane( FVec3 planeNormal, FVec3 planeLocation )
		{
			return this - planeNormal * ( this - planeLocation ).Dot( planeNormal );
		}

		public static Fix64 Distance( FVec3 v0, FVec3 v1 )
		{
			return ( v1 - v0 ).Magnitude();
		}

		public static Fix64 DistanceSquared( FVec3 v0, FVec3 v1 )
		{
			return ( v1 - v0 ).Dot();
		}

		public static FVec3 ClampMagnitude( FVec3 v, Fix64 maxLength )
		{
			FVec3 nor = v;
			Fix64 sqrMagnitude = nor.SqrMagnitude();
			if ( sqrMagnitude > ( maxLength * maxLength ) )
				nor = nor * ( maxLength / Fix64.Sqrt( sqrMagnitude ) );
			return nor;
		}

		public static FVec3 Normalize( FVec3 v )
		{
			return v * ( Fix64.One / Fix64.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z ) );
		}

		public static FVec3 NormalizeSafe( FVec3 v )
		{
			Fix64 dis = Fix64.Sqrt( v.x * v.x + v.y * v.y + v.z * v.z );
			if ( dis == Fix64.Zero )
				return new FVec3();
			return v * ( Fix64.One / dis );
		}

		public static Fix64 Dot( FVec3 v0, FVec3 v1 )
		{
			return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;
		}

		public static FVec3 Cross( FVec3 v0, FVec3 v1 )
		{
			return new FVec3( v0.y * v1.z - v0.z * v1.y, v0.z * v1.x - v0.x * v1.z,
							 v0.x * v1.y - v0.y * v1.x );
		}

		public static FVec3 OrthoNormalVector( FVec3 v )
		{
			FVec3 res = new FVec3();

			if ( Fix64.Abs( v.z ) > OVER_SQRT2 )
			{
				Fix64 a = v.y * v.y + v.z * v.z;
				Fix64 k = Fix64.One / Fix64.Sqrt( a );
				res.x = Fix64.Zero;
				res.y = -v.z * k;
				res.z = v.y * k;
			}
			else
			{
				Fix64 a = v.x * v.x + v.y * v.y;
				Fix64 k = Fix64.One / Fix64.Sqrt( a );
				res.x = -v.y * k;
				res.y = v.x * k;
				res.z = Fix64.Zero;
			}

			return res;
		}

		public static FVec3 SlerpUnclamped( FVec3 from, FVec3 to, Fix64 t )
		{
			Fix64 scale0, scale1;

			Fix64 len2 = to.Magnitude();
			Fix64 len1 = from.Magnitude();
			FVec3 v2 = to / len2;
			FVec3 v1 = from / len1;

			Fix64 len = ( len2 - len1 ) * t + len1;
			Fix64 cosom = Dot( v1, v2 );

			if ( cosom > ( Fix64 )( 1 - 1e-6 ) )
			{
				scale0 = Fix64.One - t;
				scale1 = t;
			}
			else if ( cosom < ( Fix64 )( -1 + 1e-6 ) )
			{
				FVec3 axis = OrthoNormalVector( from );
				FQuat q = FQuat.AngleAxis( ( Fix64 )180 * t, axis );
				FVec3 v = q * from * len;
				return v;
			}
			else
			{
				Fix64 omega = Fix64.Acos( cosom );
				Fix64 sinom = Fix64.Sin( omega );
				scale0 = Fix64.Sin( ( Fix64.One - t ) * omega ) / sinom;
				scale1 = Fix64.Sin( t * omega ) / sinom;
			}

			v2 = ( v2 * scale1 + v1 * scale0 ) * len;
			return v2;
		}

		public static FVec3 Slerp( FVec3 from, FVec3 to, Fix64 t )
		{
			return t <= Fix64.Zero ? from : ( t >= Fix64.One ? to : SlerpUnclamped( from, to, t ) );
		}

		public static FVec3 LerpUnclamped( FVec3 from, FVec3 to, Fix64 t )
		{
			return new FVec3( from.x + ( to.x - from.x ) * t, from.y + ( to.y - from.y ) * t, from.z + ( to.z - from.z ) * t );
		}

		public static FVec3 Lerp( FVec3 from, FVec3 to, Fix64 t )
		{
			return t <= Fix64.Zero ? from : ( t >= Fix64.One ? to : LerpUnclamped( from, to, t ) );
		}

		public static FVec3 SmoothDamp( FVec3 current, FVec3 target, ref FVec3 currentVelocity, Fix64 smoothTime, Fix64 maxSpeed,
									   Fix64 deltaTime )
		{
			smoothTime = Fix64.Max( Fix64.Epsilon, smoothTime );

			Fix64 num = ( Fix64 )2 / smoothTime;
			Fix64 num2 = num * deltaTime;
			Fix64 num3 = Fix64.One / ( Fix64.One + num2 + ( Fix64 )0.48 * num2 * num2 + ( Fix64 )0.235 * num2 * num2 * num2 );
			Fix64 maxLength = maxSpeed * smoothTime;
			FVec3 vector = current - target;

			vector.ClampMagnitude( maxLength );

			target = current - vector;
			FVec3 vec3 = ( currentVelocity + ( vector * num ) ) * deltaTime;
			currentVelocity = ( currentVelocity - ( vec3 * num ) ) * num3;

			var vector4 = target + ( vector + vec3 ) * num3;

			if ( Dot( target - current, vector4 - target ) > Fix64.Zero )
			{
				vector4 = target;
				currentVelocity.Set( Fix64.Zero, Fix64.Zero, Fix64.Zero );
			}
			return vector4;
		}

		public static FVec3 MoveTowards( FVec3 current, FVec3 target, Fix64 maxDistanceDelta )
		{
			FVec3 delta = target - current;

			Fix64 sqrDelta = delta.SqrMagnitude();
			Fix64 sqrDistance = maxDistanceDelta * maxDistanceDelta;

			if ( sqrDelta > sqrDistance )
			{
				Fix64 magnitude = Fix64.Sqrt( sqrDelta );
				if ( magnitude > Fix64.Epsilon )
				{
					delta = delta * maxDistanceDelta / magnitude + current;
					return delta;
				}
				return current;
			}
			return target;
		}

		private static Fix64 ClampedMove( Fix64 lhs, Fix64 rhs, Fix64 clampedDelta )
		{
			Fix64 delta = rhs - lhs;
			if ( delta > Fix64.Zero )
				return lhs + Fix64.Min( delta, clampedDelta );
			return lhs - Fix64.Min( -delta, clampedDelta );
		}

		public static FVec3 RotateTowards( FVec3 current, FVec3 target, Fix64 maxRadiansDelta, Fix64 maxMagnitudeDelta )
		{
			Fix64 len1 = current.Magnitude();
			Fix64 len2 = target.Magnitude();

			if ( len1 > Fix64.Epsilon && len2 > Fix64.Epsilon )
			{
				FVec3 from = current / len1;
				FVec3 to = target / len2;
				Fix64 cosom = Dot( from, to );

				if ( cosom > Fix64.One - Fix64.Epsilon )
					return MoveTowards( current, target, maxMagnitudeDelta );
				if ( cosom < -Fix64.One + Fix64.Epsilon )
				{
					FQuat q = FQuat.AngleAxis( maxRadiansDelta * Fix64.RAD_TO_DEG, OrthoNormalVector( from ) );
					return q * from * ClampedMove( len1, len2, maxMagnitudeDelta );
				}
				else
				{
					Fix64 angle = Fix64.Acos( cosom );
					FQuat q = FQuat.AngleAxis( Fix64.Min( maxRadiansDelta, angle ) * Fix64.RAD_TO_DEG,
											   Normalize( Cross( from, to ) ) );
					return q * from * ClampedMove( len1, len2, maxMagnitudeDelta );
				}
			}

			return MoveTowards( current, target, maxMagnitudeDelta );
		}

		public static void OrthoNormalize( ref FVec3 va, ref FVec3 vb, ref FVec3 vc )
		{
			va.Normalize();
			vb -= Project( vb, va );
			vb.Normalize();

			vc -= Project( vc, va );
			vc -= Project( vc, vb );
			vc.Normalize();
		}

		public static FVec3 Project( FVec3 vector, FVec3 onNormal )
		{
			Fix64 num = onNormal.SqrMagnitude();

			if ( num < Fix64.Epsilon )
				return zero;

			var num2 = Dot( vector, onNormal );
			var v3 = onNormal * ( num2 / num );
			return v3;
		}

		public static FVec3 ProjectOnPlane( FVec3 vector, FVec3 planeNormal )
		{
			return Project( vector, planeNormal ) * -Fix64.One + vector;
		}

		public static FVec3 Reflect( FVec3 inDirection, FVec3 inNormal )
		{
			var num = ( Fix64 )( -2 ) * Dot( inNormal, inDirection );
			inNormal = inNormal * num;
			inNormal += inDirection;
			return inNormal;
		}

		public static FVec3 Hermite( FVec3 value1, FVec3 tangent1, FVec3 value2, FVec3 tangent2, Fix64 t )
		{
			Fix64 weightSquared = t * t;
			Fix64 weightCubed = t * weightSquared;
			Fix64 value1Blend = ( Fix64 )2 * weightCubed - ( Fix64 )3 * weightSquared + Fix64.One;
			Fix64 tangent1Blend = weightCubed - ( Fix64 )2 * weightSquared + t;
			Fix64 value2Blend = ( Fix64 )( -2 ) * weightCubed + ( Fix64 )3 * weightSquared;
			Fix64 tangent2Blend = weightCubed - weightSquared;

			return new FVec3
				(
				value1.x * value1Blend + value2.x * value2Blend + tangent1.x * tangent1Blend + tangent2.x * tangent2Blend,
				value1.y * value1Blend + value2.y * value2Blend + tangent1.y * tangent1Blend + tangent2.y * tangent2Blend,
				value1.z * value1Blend + value2.z * value2Blend + tangent1.z * tangent1Blend + tangent2.z * tangent2Blend
				);
		}
		public static FVec3 Max( FVec3 v, Fix64 value )
		{
			return new FVec3( Fix64.Max( v.x, value ), Fix64.Max( v.y, value ), Fix64.Max( v.z, value ) );
		}

		public static FVec3 Max( FVec3 v, FVec3 v1 )
		{
			return new FVec3( Fix64.Max( v.x, v1.x ), Fix64.Max( v.y, v1.y ), Fix64.Max( v.z, v1.z ) );
		}

		public static FVec3 Min( FVec3 v, Fix64 v1 )
		{
			return new FVec3( Fix64.Min( v.x, v1 ), Fix64.Min( v.y, v1 ), Fix64.Min( v.z, v1 ) );
		}

		public static FVec3 Min( FVec3 v, FVec3 v1 )
		{
			return new FVec3( Fix64.Min( v.x, v1.x ), Fix64.Min( v.y, v1.y ), Fix64.Min( v.z, v1.z ) );
		}
		#endregion
	}
}