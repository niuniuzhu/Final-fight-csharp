namespace Core.Math
{
	public struct Quat
	{
		public Vec3 xyz
		{
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
			}
			get => new Vec3( this.x, this.y, this.z );
		}

		/// <summary>
		///     <para>X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public float x;

		/// <summary>
		///     <para>Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public float y;

		/// <summary>
		///     <para>Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public float z;

		/// <summary>
		///     <para>W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.</para>
		/// </summary>
		public float w;

		public float this[int index]
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
					default:
						return this.w;
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
				}
			}
		}

		/// <summary>
		///     <para>The identity rotation (RO).</para>
		/// </summary>
		public static Quat identity => new Quat( 0f, 0f, 0f, 1f );

		/// <summary>
		///     <para>Returns the euler angle representation of the rotation.</para>
		/// </summary>
		public Vec3 eulerAngles
		{
			get => ToEulerRad( this ) * MathUtils.RAD_TO_DEG;
			set
			{
				Quat q = FromEulerRad( value * MathUtils.DEG_TO_RAD );
				this.x = q.x;
				this.y = q.y;
				this.z = q.z;
				this.w = q.w;
			}
		}

		/// <summary>
		///     Gets the length (magnitude) of the quaternion.
		/// </summary>
		/// <seealso cref="lengthSquared" />
		public float length => MathUtils.Sqrt( this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w );

		/// <summary>
		///     Gets the square of the quaternion length (magnitude).
		/// </summary>
		public float lengthSquared => this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;

		/// <summary>
		///     <para>Constructs new Quat with given x,y,z,w components.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		public Quat( float x, float y, float z, float w )
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		/// <summary>
		///     Construct a new Quat from vector and w components
		/// </summary>
		/// <param name="v">The vector part</param>
		/// <param name="w">The w part</param>
		public Quat( Vec3 v, float w )
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = w;
		}

		public void Set( Vec3 v, float w )
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = w;
		}

		/// <summary>
		///     <para>Set x, y, z and w components of an existing Quat.</para>
		/// </summary>
		/// <param name="newX"></param>
		/// <param name="newY"></param>
		/// <param name="newZ"></param>
		/// <param name="newW"></param>
		public void Set( float newX, float newY, float newZ, float newW )
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
			this.w = newW;
		}

		/// <summary>
		///     Scales the Quat to unit length.
		/// </summary>
		public void Normalize()
		{
			float l = this.length;
			if ( l == 0 )
			{
				this.x = 0f;
				this.y = 0f;
				this.z = 0f;
				this.w = 0f;
				return;
			}
			float scale = 1.0f / l;
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
		public static Quat Normalize( Quat q )
		{
			float scale = 1.0f / q.length;
			Quat result = new Quat( q.xyz * scale, q.w * scale );
			return result;
		}

		/// <summary>
		///     <para>The dot product between two rotations.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Dot( Quat a, Quat b )
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public Vec3 Transform( Vec3 v )
		{
			float x2 = this.x + this.x;
			float y2 = this.y + this.y;
			float z2 = this.z + this.z;
			float xx2 = this.x * x2;
			float xy2 = this.x * y2;
			float xz2 = this.x * z2;
			float yy2 = this.y * y2;
			float yz2 = this.y * z2;
			float zz2 = this.z * z2;
			float wx2 = this.w * x2;
			float wy2 = this.w * y2;
			float wz2 = this.w * z2;

			return new Vec3
			(
				v.x * ( 1f - yy2 - zz2 ) + v.y * ( xy2 - wz2 ) + v.z * ( xz2 + wy2 ),
				v.x * ( xy2 + wz2 ) + v.y * ( 1f - xx2 - zz2 ) + v.z * ( yz2 - wx2 ),
				v.x * ( xz2 - wy2 ) + v.y * ( yz2 + wx2 ) + v.z * ( 1f - xx2 - yy2 )
			);
		}

		/// <summary>
		///     <para>Creates a rotation which rotates /angle/ degrees around /axis/.</para>
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="axis"></param>
		public static Quat AngleAxis( float angle, Vec3 axis )
		{
			Quat result = identity;
			float thetaOver2 = angle * MathUtils.DEG_TO_RAD * 0.5f;
			float sinThetaOver2 = MathUtils.Sin( thetaOver2 );
			result.x = axis.x * sinThetaOver2;
			result.y = axis.y * sinThetaOver2;
			result.z = axis.z * sinThetaOver2;
			result.w = MathUtils.Cos( thetaOver2 );
			return result;
		}

		public void ToAngleAxis( out float angle, out Vec3 axis )
		{
			ToAxisAngleRad( this, out axis, out angle );
			angle *= MathUtils.RAD_TO_DEG;
		}

		/// <summary>
		///     <para>Creates a rotation which rotates from /from/ to /to/.</para>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public static Quat FromToRotation( Vec3 @from, Vec3 to )
		{
			Quat result = identity;
			float dot = @from.Dot( to );
			if ( dot < -1 + 1e-6 )
			{
				result.Set( Vec3.Normalize( Orthogonal( @from ) ), 0 );
			}
			else if ( dot > 1 - 1e-6 )
			{
				result.Set( 0, 0, 0, 1 );
			}
			else
			{
				float angle = MathUtils.Acos( dot );
				Vec3 axis = Vec3.Normalize( @from.Cross( to ) );
				float thetaOver2 = angle * 0.5f;
				float sinThetaOver2 = MathUtils.Sin( thetaOver2 );
				result.x = axis.x * sinThetaOver2;
				result.y = axis.y * sinThetaOver2;
				result.z = axis.z * sinThetaOver2;
				result.w = MathUtils.Cos( thetaOver2 );
			}
			return result;
		}

		public static Vec3 Orthogonal( Vec3 v )
		{
			float x = MathUtils.Abs( v.x );
			float y = MathUtils.Abs( v.y );
			float z = MathUtils.Abs( v.z );

			Vec3 other = x < y ? ( x < z ? Vec3.right : Vec3.forward ) : ( y < z ? Vec3.up : Vec3.forward );
			return Vec3.Cross( v, other );
		}

		public static Quat LookRotation( Vec3 forward )
		{
			return LookRotation( forward, Vec3.up );
		}

		/// <summary>
		///     <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
		/// </summary>
		/// <param name="forward">The direction to look in.</param>
		/// <param name="upwards">The vector that defines in which direction up is.</param>
		public static Quat LookRotation( Vec3 forward, Vec3 upwards )
		{
			forward.Normalize();
			Vec3 right = Vec3.Normalize( upwards.Cross( forward ) );
			upwards = forward.Cross( right );
			float m00 = right.x;
			float m01 = right.y;
			float m02 = right.z;
			float m10 = upwards.x;
			float m11 = upwards.y;
			float m12 = upwards.z;
			float m20 = forward.x;
			float m21 = forward.y;
			float m22 = forward.z;

			float num8 = m00 + m11 + m22;
			Quat quaternion = new Quat();
			if ( num8 > 0f )
			{
				float num = MathUtils.Sqrt( num8 + 1f );
				quaternion.w = num * 0.5f;
				num = 0.5f / num;
				quaternion.x = ( m12 - m21 ) * num;
				quaternion.y = ( m20 - m02 ) * num;
				quaternion.z = ( m01 - m10 ) * num;
				return quaternion;
			}
			if ( ( m00 >= m11 ) &&
				 ( m00 >= m22 ) )
			{
				float num7 = MathUtils.Sqrt( 1f + m00 - m11 - m22 );
				float num4 = 0.5f / num7;
				quaternion.x = 0.5f * num7;
				quaternion.y = ( m01 + m10 ) * num4;
				quaternion.z = ( m02 + m20 ) * num4;
				quaternion.w = ( m12 - m21 ) * num4;
				return quaternion;
			}
			if ( m11 > m22 )
			{
				float num6 = MathUtils.Sqrt( 1f + m11 - m00 - m22 );
				float num3 = 0.5f / num6;
				quaternion.x = ( m10 + m01 ) * num3;
				quaternion.y = 0.5f * num6;
				quaternion.z = ( m21 + m12 ) * num3;
				quaternion.w = ( m20 - m02 ) * num3;
				return quaternion;
			}
			float num5 = MathUtils.Sqrt( 1f + m22 - m00 - m11 );
			float num2 = 0.5f / num5;
			quaternion.x = ( m20 + m02 ) * num2;
			quaternion.y = ( m21 + m12 ) * num2;
			quaternion.z = 0.5f * num5;
			quaternion.w = ( m01 - m10 ) * num2;
			return quaternion;
		}

		public void SetLookRotation( Vec3 view )
		{
			Vec3 up = Vec3.up;
			this.SetLookRotation( view, up );
		}

		/// <summary>
		///     <para>Creates a rotation with the specified /forward/ and /upwards/ directions.</para>
		/// </summary>
		/// <param name="view">The direction to look in.</param>
		/// <param name="up">The vector that defines in which direction up is.</param>
		public void SetLookRotation( Vec3 view, Vec3 up )
		{
			Quat rot = LookRotation( view, up );
			this.Set( rot.x, rot.y, rot.z, rot.w );
		}

		public Quat Conjugate()
		{
			return new Quat( -this.x, -this.y, -this.z, this.w );
		}

		/// <summary>
		///     <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is clamped to the range [0, 1].</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Quat Slerp( Quat a, Quat b, float t )
		{
			t = MathUtils.Clamp( t, 0, 1 );
			return SlerpUnclamped( a, b, t );
		}

		/// <summary>
		///     <para>Spherically interpolates between /a/ and /b/ by t. The parameter /t/ is not clamped.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Quat SlerpUnclamped( Quat a, Quat b, float t )
		{
			// if either input is zero, return the other.
			if ( a.lengthSquared == 0.0f )
			{
				if ( b.lengthSquared == 0.0f )
					return identity;
				return b;
			}
			if ( b.lengthSquared == 0.0f )
				return a;

			float cosHalfAngle = a.w * b.w + a.xyz.Dot( b.xyz );

			if ( cosHalfAngle >= 1.0f ||
				 cosHalfAngle <= -1.0f )
			{
				// angle = 0.0f, so just return one input.
				return a;
			}
			if ( cosHalfAngle < 0.0f )
			{
				b.xyz = -b.xyz;
				b.w = -b.w;
				cosHalfAngle = -cosHalfAngle;
			}

			float blendA;
			float blendB;
			if ( cosHalfAngle < 0.99f )
			{
				// do proper slerp for big angles
				float halfAngle = MathUtils.Acos( cosHalfAngle );
				float sinHalfAngle = MathUtils.Sin( halfAngle );
				float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
				blendA = MathUtils.Sin( halfAngle * ( 1.0f - t ) ) * oneOverSinHalfAngle;
				blendB = MathUtils.Sin( halfAngle * t ) * oneOverSinHalfAngle;
			}
			else
			{
				// do lerp if angle is really small.
				blendA = 1.0f - t;
				blendB = t;
			}

			Quat result = new Quat( blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w );
			if ( result.lengthSquared > 0.0f )
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
		public static Quat Lerp( Quat q1, Quat q2, float t )
		{
			t = MathUtils.Clamp( t, 0, 1 );
			return LerpUnclamped( q1, q2, t );
		}

		/// <summary>
		///     <para>
		///         Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is not
		///         clamped.
		///     </para>
		/// </summary>
		public static Quat LerpUnclamped( Quat q1, Quat q2, float t )
		{
			Quat q = identity;
			if ( Dot( q1, q2 ) < 0f )
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
		public static Quat RotateTowards( Quat from, Quat to, float maxDegreesDelta )
		{
			float num = Angle( from, to );
			if ( num == 0f )
				return to;
			float t = MathUtils.Min( 1f, maxDegreesDelta / num );
			return SlerpUnclamped( from, to, t );
		}

		/// <summary>
		///     <para>Returns the Inverse of /rotation/.</para>
		/// </summary>
		/// <param name="rotation"></param>
		public static Quat Inverse( Quat rotation )
		{
			float lengthSq = rotation.lengthSquared;
			if ( lengthSq != 0.0f )
			{
				float i = 1.0f / lengthSq;
				return new Quat( rotation.xyz * -i, rotation.w * i );
			}
			return rotation;
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
		public static float Angle( Quat a, Quat b )
		{
			float f = Dot( a, b );
			return MathUtils.Acos( MathUtils.Min( MathUtils.Abs( f ), 1f ) ) * 2f * MathUtils.RAD_TO_DEG;
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
		public static Quat Euler( float x, float y, float z )
		{
			return FromEulerRad( new Vec3( x, y, z ) * MathUtils.DEG_TO_RAD );
		}

		/// <summary>
		///     <para>
		///         Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees
		///         around the y axis (in that order).
		///     </para>
		/// </summary>
		/// <param name="euler"></param>
		public static Quat Euler( Vec3 euler )
		{
			return FromEulerRad( euler * MathUtils.DEG_TO_RAD );
		}

		private static Vec3 ToEulerRad( Quat rotation )
		{
			float sqw = rotation.w * rotation.w;
			float sqx = rotation.x * rotation.x;
			float sqy = rotation.y * rotation.y;
			float sqz = rotation.z * rotation.z;
			float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
			float test = rotation.x * rotation.w - rotation.y * rotation.z;
			Vec3 v;

			if ( test > 0.4995f * unit )
			{
				// singularity at north pole
				v.y = 2f * MathUtils.Atan2( rotation.y, rotation.x );
				v.x = MathUtils.PI / 2;
				v.z = 0;
				return NormalizeAngles( v * MathUtils.RAD_TO_DEG );
			}
			if ( test < -0.4995f * unit )
			{
				// singularity at south pole
				v.y = -2f * MathUtils.Atan2( rotation.y, rotation.x );
				v.x = -MathUtils.PI / 2;
				v.z = 0;
				return NormalizeAngles( v * MathUtils.RAD_TO_DEG );
			}
			Quat q = new Quat( rotation.w, rotation.z, rotation.x, rotation.y );
			v.y = MathUtils.Atan2( 2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * ( q.z * q.z + q.w * q.w ) ); // Yaw
			v.x = MathUtils.Asin( 2f * ( q.x * q.z - q.w * q.y ) ); // Pitch
			v.z = MathUtils.Atan2( 2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * ( q.y * q.y + q.z * q.z ) ); // Roll
			return NormalizeAngles( v * MathUtils.RAD_TO_DEG );
		}

		private static Vec3 NormalizeAngles( Vec3 angles )
		{
			angles.x = NormalizeAngle( angles.x );
			angles.y = NormalizeAngle( angles.y );
			angles.z = NormalizeAngle( angles.z );
			return angles;
		}

		private static float NormalizeAngle( float angle )
		{
			while ( angle > 360 )
				angle -= 360;
			while ( angle < 0 )
				angle += 360;
			return angle;
		}

		private static Quat FromEulerRad( Vec3 euler )
		{
			float yaw = euler.x;
			float pitch = euler.y;
			float roll = euler.z;
			float yawOver2 = yaw * 0.5f;
			float cosYawOver2 = MathUtils.Cos( yawOver2 );
			float sinYawOver2 = MathUtils.Sin( yawOver2 );
			float pitchOver2 = pitch * 0.5f;
			float cosPitchOver2 = MathUtils.Cos( pitchOver2 );
			float sinPitchOver2 = MathUtils.Sin( pitchOver2 );
			float rollOver2 = roll * 0.5f;
			float cosRollOver2 = MathUtils.Cos( rollOver2 );
			float sinRollOver2 = MathUtils.Sin( rollOver2 );
			Quat result;
			result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
			result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
			result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
			result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

			return result;
		}

		private static void ToAxisAngleRad( Quat q, out Vec3 axis, out float angle )
		{
			if ( MathUtils.Abs( q.w ) > 1.0f )
				q.Normalize();
			angle = 2.0f * MathUtils.Acos( q.w ); // angle
			float den = MathUtils.Sqrt( 1.0f - q.w * q.w );
			if ( den > 0.0001f )
				axis = q.xyz / den;
			else
			{
				// This occurs when the angle is zero. 
				// Not a problem: just set an arbitrary normalized axis.
				axis = new Vec3( 1, 0, 0 );
			}
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
		}

		public override bool Equals( object other )
		{
			if ( !( other is Quat ) )
				return false;
			Quat quaternion = ( Quat )other;
			return this.x.Equals( quaternion.x ) && this.y.Equals( quaternion.y ) && this.z.Equals( quaternion.z ) &&
				   this.w.Equals( quaternion.w );
		}

		public bool Equals( Quat other )
		{
			return this.x.Equals( other.x ) && this.y.Equals( other.y ) && this.z.Equals( other.z ) && this.w.Equals( other.w );
		}

		public static Quat operator *( Quat lhs, Quat rhs )
		{
			return new Quat( lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
							 lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
							 lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
							 lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z );
		}

		public static Vec3 operator *( Quat rotation, Vec3 point )
		{
			float num = rotation.x * 2f;
			float num2 = rotation.y * 2f;
			float num3 = rotation.z * 2f;
			float num4 = rotation.x * num;
			float num5 = rotation.y * num2;
			float num6 = rotation.z * num3;
			float num7 = rotation.x * num2;
			float num8 = rotation.x * num3;
			float num9 = rotation.y * num3;
			float num10 = rotation.w * num;
			float num11 = rotation.w * num2;
			float num12 = rotation.w * num3;
			Vec3 result;
			result.x = ( 1f - ( num5 + num6 ) ) * point.x + ( num7 - num12 ) * point.y + ( num8 + num11 ) * point.z;
			result.y = ( num7 + num12 ) * point.x + ( 1f - ( num4 + num6 ) ) * point.y + ( num9 - num10 ) * point.z;
			result.z = ( num8 - num11 ) * point.x + ( num9 + num10 ) * point.y + ( 1f - ( num4 + num5 ) ) * point.z;
			return result;
		}

		public static bool operator ==( Quat lhs, Quat rhs )
		{
			return Dot( lhs, rhs ) > 0.999999f;
		}

		public static bool operator !=( Quat lhs, Quat rhs )
		{
			return Dot( lhs, rhs ) <= 0.999999f;
		}
	}
}