using System;

namespace Core.FMath
{
	public struct FQuat : IEquatable<FQuat>
	{
		public FVec3 xyz
		{
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
			}
			get => new FVec3( this.x, this.y, this.z );
		}

		/// <summary>
		///     <para>X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public Fix64 x;

		/// <summary>
		///     <para>Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public Fix64 y;

		/// <summary>
		///     <para>Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public Fix64 z;

		/// <summary>
		///     <para>W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public Fix64 w;

		public Fix64 this[int index]
		{
			get
			{
				switch ( index )
				{
					case 0:
						return this.x;
					case 1:
						return this.y;
					case 2:
						return this.z;
					case 3:
						return this.w;
					default:
						throw new IndexOutOfRangeException( "Invalid Quaternion index: " + index + ", can use only 0,1,2,3" );
				}
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
						throw new IndexOutOfRangeException( "Invalid Quaternion index: " + index + ", can use only 0,1,2,3" );
				}
			}
		}

		/// <summary>
		///     <para>The identity rotation (RO).</para>
		/// </summary>
		public static FQuat identity => new FQuat( Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One );

		/// <summary>
		///     <para>Returns the euler angle representation of the rotation.</para>
		/// </summary>
		public FVec3 eulerAngles
		{
			get => ToEulerRad( this ) * Fix64.RAD_TO_DEG;
			set => this = FromEulerRad( value * Fix64.DEG_TO_RAD );
		}

		public Fix64 length => Fix64.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );

		public Fix64 lengthSquared => this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;

		public FQuat( float x, float y, float z, float w )
		{
			this.x = ( Fix64 )x;
			this.y = ( Fix64 )y;
			this.z = ( Fix64 )z;
			this.w = ( Fix64 )w;
		}

		public FQuat( Fix64 x, Fix64 y, Fix64 z, Fix64 w )
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public FQuat( FVec3 v, Fix64 w )
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = w;
		}

		public void Set( FVec3 v, Fix64 w )
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = w;
		}

		/// <summary>
		///     <para>Set x, y, z and w components of an existing FQuat.</para>
		/// </summary>
		/// <param name="newX"></param>
		/// <param name="newY"></param>
		/// <param name="newZ"></param>
		/// <param name="newW"></param>
		public void Set( Fix64 newX, Fix64 newY, Fix64 newZ, Fix64 newW )
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
			this.w = newW;
		}

		/// <summary>
		///     Scales the FQuat to unit length.
		/// </summary>
		public void Normalize()
		{
			if ( this.length == Fix64.Zero )
			{
				this.x = Fix64.Zero;
				this.y = Fix64.Zero;
				this.z = Fix64.Zero;
				this.w = Fix64.Zero;
				return;
			}
			Fix64 scale = Fix64.One / this.length;
			this.x *= scale;
			this.y *= scale;
			this.z *= scale;
			this.w *= scale;
		}

		/// <summary>
		///     Scale the given quaternion to unit length
		/// </summary>
		/// <param name="q">The quaternion to normalize</param>
		/// <returns>The normalized quaternion</returns>
		public static FQuat Normalize( FQuat q )
		{
			FQuat result = q;
			result.Normalize();
			return result;
		}

		/// <summary>
		///     <para>The dot product between two rotations.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Fix64 Dot( FQuat a, FQuat b )
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		/// <summary>
		///     <para>Creates a rotation which rotates /angle/ degrees around /axis/.</para>
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="axis"></param>
		public static FQuat AngleAxis( Fix64 angle, FVec3 axis )
		{
			FQuat result = identity;
			Fix64 thetaOver2 = angle * Fix64.DEG_TO_RAD * Fix64.Half;
			Fix64 sinThetaOver2 = Fix64.Sin( thetaOver2 );
			result.x = axis.x * sinThetaOver2;
			result.y = axis.y * sinThetaOver2;
			result.z = axis.z * sinThetaOver2;
			result.w = Fix64.Cos( thetaOver2 );
			return result;
		}

		public void ToAngleAxis( out Fix64 angle, out FVec3 axis )
		{
			ToAxisAngleRad( this, out axis, out angle );
			angle *= Fix64.RAD_TO_DEG;
		}

		/// <summary>
		///     <para>Creates a rotation which rotates from /from/ to /to/.</para>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public static FQuat FromToRotation( FVec3 @from, FVec3 to )
		{
			FQuat result = identity;
			Fix64 dot = @from.Dot( to );
			if ( dot < Fix64.NegativeOne + Fix64.Epsilon )
			{
				result.Set( FVec3.Normalize( Orthogonal( @from ) ), Fix64.Zero );
			}
			else if ( dot > Fix64.One - Fix64.Epsilon )
			{
				result.Set( Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One );
			}
			else
			{
				Fix64 angle = Fix64.Acos( dot );
				FVec3 axis = FVec3.Normalize( @from.Cross( to ) );
				Fix64 thetaOver2 = angle * Fix64.Half;
				Fix64 sinThetaOver2 = Fix64.Sin( thetaOver2 );
				result.x = axis.x * sinThetaOver2;
				result.y = axis.y * sinThetaOver2;
				result.z = axis.z * sinThetaOver2;
				result.w = Fix64.Cos( thetaOver2 );
			}
			return result;
		}

		public static FVec3 Orthogonal( FVec3 v )
		{
			Fix64 x = Fix64.Abs( v.x );
			Fix64 y = Fix64.Abs( v.y );
			Fix64 z = Fix64.Abs( v.z );

			FVec3 other = x < y ? ( x < z ? FVec3.right : FVec3.forward ) : ( y < z ? FVec3.up : FVec3.forward );
			return FVec3.Cross( v, other );
		}

		public static FQuat LookRotation( FVec3 forward )
		{
			return LookRotation( forward, FVec3.up );
		}

		/// <summary>
		///     <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
		/// </summary>
		/// <param name="forward">The direction to look in.</param>
		/// <param name="upwards">The vector that defines in which direction up is.</param>
		public static FQuat LookRotation( FVec3 forward, FVec3 upwards )
		{
			forward.Normalize();
			FVec3 right = FVec3.Normalize( upwards.Cross( forward ) );
			upwards = forward.Cross( right );
			Fix64 m00 = right.x;
			Fix64 m01 = right.y;
			Fix64 m02 = right.z;
			Fix64 m10 = upwards.x;
			Fix64 m11 = upwards.y;
			Fix64 m12 = upwards.z;
			Fix64 m20 = forward.x;
			Fix64 m21 = forward.y;
			Fix64 m22 = forward.z;

			Fix64 num8 = m00 + m11 + m22;
			FQuat quaternion = new FQuat();
			if ( num8 > Fix64.Zero )
			{
				Fix64 num = Fix64.Sqrt( num8 + Fix64.One );
				quaternion.w = num * Fix64.Half;
				num = Fix64.Half / num;
				quaternion.x = ( m12 - m21 ) * num;
				quaternion.y = ( m20 - m02 ) * num;
				quaternion.z = ( m01 - m10 ) * num;
				return quaternion;
			}
			if ( ( m00 >= m11 ) &&
				 ( m00 >= m22 ) )
			{
				Fix64 num7 = Fix64.Sqrt( Fix64.One + m00 - m11 - m22 );
				Fix64 num4 = Fix64.Half / num7;
				quaternion.x = Fix64.Half * num7;
				quaternion.y = ( m01 + m10 ) * num4;
				quaternion.z = ( m02 + m20 ) * num4;
				quaternion.w = ( m12 - m21 ) * num4;
				return quaternion;
			}
			if ( m11 > m22 )
			{
				Fix64 num6 = Fix64.Sqrt( Fix64.One + m11 - m00 - m22 );
				Fix64 num3 = Fix64.Half / num6;
				quaternion.x = ( m10 + m01 ) * num3;
				quaternion.y = Fix64.Half * num6;
				quaternion.z = ( m21 + m12 ) * num3;
				quaternion.w = ( m20 - m02 ) * num3;
				return quaternion;
			}
			Fix64 num5 = Fix64.Sqrt( Fix64.One + m22 - m00 - m11 );
			Fix64 num2 = Fix64.Half / num5;
			quaternion.x = ( m20 + m02 ) * num2;
			quaternion.y = ( m21 + m12 ) * num2;
			quaternion.z = Fix64.Half * num5;
			quaternion.w = ( m01 - m10 ) * num2;
			return quaternion;
		}

		public void SetLookRotation( FVec3 view )
		{
			FVec3 up = FVec3.up;
			this.SetLookRotation( view, up );
		}

		/// <summary>
		///     <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
		/// </summary>
		/// <param name="view">The direction to look in.</param>
		/// <param name="up">The vector that defines in which direction up is.</param>
		public void SetLookRotation( FVec3 view, FVec3 up )
		{
			this = LookRotation( view, up );
		}

		public FQuat Conjugate()
		{
			return new FQuat( -this.x, -this.y, -this.z, this.w );
		}

		/// <summary>
		///     <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is clamped to the range [0, 1].</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static FQuat Slerp( FQuat a, FQuat b, Fix64 t )
		{
			t = Fix64.Clamp( t, Fix64.Zero, Fix64.One );
			return SlerpUnclamped( a, b, t );
		}

		/// <summary>
		///     <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is not clamped.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static FQuat SlerpUnclamped( FQuat a, FQuat b, Fix64 t )
		{
			// if either input is zero, return the other.
			if ( a.lengthSquared == Fix64.Zero )
			{
				if ( b.lengthSquared == Fix64.Zero )
					return identity;
				return b;
			}
			if ( b.lengthSquared == Fix64.Zero )
				return a;


			Fix64 cosHalfAngle = a.w * b.w + a.xyz.Dot( b.xyz );

			if ( cosHalfAngle >= Fix64.One ||
				 cosHalfAngle <= -Fix64.One )
			{
				// angle = Fix64.Zero, so just return one input.
				return a;
			}
			if ( cosHalfAngle < Fix64.Zero )
			{
				b.xyz = -b.xyz;
				b.w = -b.w;
				cosHalfAngle = -cosHalfAngle;
			}

			Fix64 blendA;
			Fix64 blendB;
			if ( cosHalfAngle < Fix64.One - Fix64.Epsilon )
			{
				// do proper slerp for big angles
				Fix64 halfAngle = Fix64.Acos( cosHalfAngle );
				Fix64 sinHalfAngle = Fix64.Sin( halfAngle );
				Fix64 oneOverSinHalfAngle = Fix64.One / sinHalfAngle;
				blendA = Fix64.Sin( halfAngle * ( Fix64.One - t ) ) * oneOverSinHalfAngle;
				blendB = Fix64.Sin( halfAngle * t ) * oneOverSinHalfAngle;
			}
			else
			{
				// do lerp if angle is really small.
				blendA = Fix64.One - t;
				blendB = t;
			}

			FQuat result = new FQuat( blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w );
			if ( result.lengthSquared > Fix64.Zero )
				return Normalize( result );
			return identity;
		}

		/// <summary>
		///     <para>
		///         Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is clamped to
		///         the range [0, 1].
		///     </para>
		/// </summary>
		/// <param name="q1"></param>
		/// <param name="q2"></param>
		/// <param name="t"></param>
		public static FQuat Lerp( FQuat q1, FQuat q2, Fix64 t )
		{
			t = Fix64.Clamp( t, Fix64.Zero, Fix64.One );
			return LerpUnclamped( q1, q2, t );
		}

		/// <summary>
		///     <para>
		///         Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is not
		///         clamped.
		///     </para>
		/// </summary>
		public static FQuat LerpUnclamped( FQuat q1, FQuat q2, Fix64 t )
		{
			FQuat q = identity;
			if ( Dot( q1, q2 ) < Fix64.Zero )
			{
				q.x = q1.x + t * ( -q2.x - q1.x );
				q.y = q1.y + t * ( -q2.y - q1.y );
				q.z = q1.z + t * ( -q2.z - q1.z );
				q.w = q1.w + t * ( -q2.w - q1.w );
			}
			else
			{
				q.x = q1.x + ( q2.x - q1.x ) * t;
				q.y = q1.y + ( q2.y - q1.y ) * t;
				q.z = q1.z + ( q2.z - q1.z ) * t;
				q.w = q1.w + ( q2.w - q1.w ) * t;
			}
			q.Normalize();
			return q;
		}

		/// <summary>
		///     <para>Rotates a rotation /from/ towards /to/.</para>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="maxDegreesDelta"></param>
		public static FQuat RotateTowards( FQuat from, FQuat to, Fix64 maxDegreesDelta )
		{
			Fix64 num = Angle( from, to );
			if ( num == Fix64.Zero )
				return to;
			Fix64 t = Fix64.Min( Fix64.One, maxDegreesDelta / num );
			return SlerpUnclamped( from, to, t );
		}

		/// <summary>
		///     <para>Returns the Inverse of /rotation/.</para>
		/// </summary>
		/// <param name="rotation"></param>
		public static FQuat Inverse( FQuat rotation )
		{
			Fix64 lengthSq = rotation.lengthSquared;
			if ( lengthSq != Fix64.Zero )
			{
				Fix64 i = Fix64.One / lengthSq;
				return new FQuat( rotation.xyz * -i, rotation.w * i );
			}
			return rotation;
		}

		public void Inverse()
		{
			Fix64 lengthSq = this.lengthSquared;
			if ( lengthSq != Fix64.Zero )
			{
				Fix64 i = Fix64.One / lengthSq;
				this.xyz *= -i;
				this.w *= i;
			}
		}

		public override string ToString()
		{
			return $"({this.x}, {this.y}, {this.z}, {this.w})";
		}

		/// <summary>
		///     <para>Returns the angle in degrees between two rotations /a/ and /b/.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Fix64 Angle( FQuat a, FQuat b )
		{
			Fix64 f = Dot( a, b );
			return Fix64.Acos( Fix64.Min( Fix64.Abs( f ), Fix64.One ) ) * Fix64.Two * Fix64.RAD_TO_DEG;
		}

		/// <summary>
		///     <para>
		///         Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees
		///         around the y axis (in that order).
		///     </para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public static FQuat Euler( Fix64 x, Fix64 y, Fix64 z )
		{
			return FromEulerRad( new FVec3( x, y, z ) * Fix64.DEG_TO_RAD );
		}

		/// <summary>
		///     <para>
		///         Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees
		///         around the y axis (in that order).
		///     </para>
		/// </summary>
		/// <param name="euler"></param>
		public static FQuat Euler( FVec3 euler )
		{
			return FromEulerRad( euler * Fix64.DEG_TO_RAD );
		}

		private static FVec3 ToEulerRad( FQuat rotation )
		{
			Fix64 sqw = rotation.w * rotation.w;
			Fix64 sqx = rotation.x * rotation.x;
			Fix64 sqy = rotation.y * rotation.y;
			Fix64 sqz = rotation.z * rotation.z;
			Fix64 unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
			Fix64 test = rotation.x * rotation.w - rotation.y * rotation.z;
			FVec3 v;

			if ( test > ( Fix64.Half - Fix64.Epsilon ) * unit )
			{
				// singularity at north pole
				v.y = Fix64.Two * Fix64.Atan2( rotation.y, rotation.x );
				v.x = Fix64.PiOver2;
				v.z = Fix64.Zero;
				return NormalizeAngles( v * Fix64.RAD_TO_DEG );
			}
			if ( test < -( Fix64.Half - Fix64.Epsilon ) * unit )
			{
				// singularity at south pole
				v.y = -Fix64.Two * Fix64.Atan2( rotation.y, rotation.x );
				v.x = -Fix64.PiOver2;
				v.z = Fix64.Zero;
				return NormalizeAngles( v * Fix64.RAD_TO_DEG );
			}
			FQuat q = new FQuat( rotation.w, rotation.z, rotation.x, rotation.y );
			v.y = Fix64.Atan2( Fix64.Two * q.x * q.w + Fix64.Two * q.y * q.z, Fix64.One - Fix64.Two * ( q.z * q.z + q.w * q.w ) ); // Yaw
			v.x = Fix64.Asin( Fix64.Two * ( q.x * q.z - q.w * q.y ) ); // Pitch
			v.z = Fix64.Atan2( Fix64.Two * q.x * q.y + Fix64.Two * q.z * q.w, Fix64.One - Fix64.Two * ( q.y * q.y + q.z * q.z ) ); // Roll
			return NormalizeAngles( v * Fix64.RAD_TO_DEG );
		}

		private static FVec3 NormalizeAngles( FVec3 angles )
		{
			angles.x = NormalizeAngle( angles.x );
			angles.y = NormalizeAngle( angles.y );
			angles.z = NormalizeAngle( angles.z );
			return angles;
		}

		private static Fix64 NormalizeAngle( Fix64 angle )
		{
			while ( angle > ( Fix64 )360 )
				angle -= ( Fix64 )360;
			while ( angle < Fix64.Zero )
				angle += ( Fix64 )360;
			return angle;
		}

		private static FQuat FromEulerRad( FVec3 euler )
		{
			Fix64 yaw = euler.x;
			Fix64 pitch = euler.y;
			Fix64 roll = euler.z;
			Fix64 yawOver2 = yaw * Fix64.Half;
			Fix64 cosYawOver2 = Fix64.Cos( yawOver2 );
			Fix64 sinYawOver2 = Fix64.Sin( yawOver2 );
			Fix64 pitchOver2 = pitch * Fix64.Half;
			Fix64 cosPitchOver2 = Fix64.Cos( pitchOver2 );
			Fix64 sinPitchOver2 = Fix64.Sin( pitchOver2 );
			Fix64 rollOver2 = roll * Fix64.Half;
			Fix64 cosRollOver2 = Fix64.Cos( rollOver2 );
			Fix64 sinRollOver2 = Fix64.Sin( rollOver2 );
			FQuat result;
			result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
			result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
			result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
			result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

			return result;
		}

		private static void ToAxisAngleRad( FQuat q, out FVec3 axis, out Fix64 angle )
		{
			if ( Fix64.Abs( q.w ) > Fix64.One )
				q.Normalize();
			angle = Fix64.Two * Fix64.Acos( q.w ); // angle
			Fix64 den = Fix64.Sqrt( Fix64.One - q.w * q.w );
			if ( den > Fix64.Epsilon )
				axis = q.xyz / den;
			else
			{
				// This occurs when the angle is zero. 
				// Not a problem: just set an arbitrary normalized axis.
				axis = new FVec3( 1, 0, 0 );
			}
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
		}

		public override bool Equals( object other )
		{
			if ( !( other is FQuat ) )
				return false;
			FQuat quaternion = ( FQuat )other;
			return this.x.Equals( quaternion.x ) && this.y.Equals( quaternion.y ) && this.z.Equals( quaternion.z ) &&
				   this.w.Equals( quaternion.w );
		}

		public bool Equals( FQuat other )
		{
			return this.x.Equals( other.x ) && this.y.Equals( other.y ) && this.z.Equals( other.z ) && this.w.Equals( other.w );
		}

		public static FQuat operator *( FQuat lhs, FQuat rhs )
		{
			return new FQuat( lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
							 lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
							 lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
							 lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z );
		}

		public static FVec3 operator *( FQuat rotation, FVec3 point )
		{
			Fix64 num = rotation.x * Fix64.Two;
			Fix64 num2 = rotation.y * Fix64.Two;
			Fix64 num3 = rotation.z * Fix64.Two;
			Fix64 num4 = rotation.x * num;
			Fix64 num5 = rotation.y * num2;
			Fix64 num6 = rotation.z * num3;
			Fix64 num7 = rotation.x * num2;
			Fix64 num8 = rotation.x * num3;
			Fix64 num9 = rotation.y * num3;
			Fix64 num10 = rotation.w * num;
			Fix64 num11 = rotation.w * num2;
			Fix64 num12 = rotation.w * num3;
			FVec3 result;
			result.x = ( Fix64.One - ( num5 + num6 ) ) * point.x + ( num7 - num12 ) * point.y + ( num8 + num11 ) * point.z;
			result.y = ( num7 + num12 ) * point.x + ( Fix64.One - ( num4 + num6 ) ) * point.y + ( num9 - num10 ) * point.z;
			result.z = ( num8 - num11 ) * point.x + ( num9 + num10 ) * point.y + ( Fix64.One - ( num4 + num5 ) ) * point.z;
			return result;
		}

		public static bool operator ==( FQuat lhs, FQuat rhs )
		{
			return Dot( lhs, rhs ) > Fix64.One - Fix64.Epsilon;
		}

		public static bool operator !=( FQuat lhs, FQuat rhs )
		{
			return Dot( lhs, rhs ) <= Fix64.One - Fix64.Epsilon;
		}
	}
}