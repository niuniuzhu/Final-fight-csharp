namespace Core.Math
{
	public struct Mat3
	{
		public Vec3 x;
		public Vec3 y;
		public Vec3 z;

		public Mat3( Vec3 x, Vec3 y, Vec3 z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static Mat3 FromScale( float scale )
		{
			return FromScale( scale, scale, scale );
		}

		public static Mat3 FromScale( Vec3 scale )
		{
			return FromScale( scale.x, scale.y, scale.z );
		}

		public static Mat3 FromScale( float scaleX, float scaleY, float scaleZ )
		{
			return new Mat3
			(
				new Vec3( scaleX, 0, 0 ),
				new Vec3( 0, scaleY, 0 ),
				new Vec3( 0, 0, scaleZ )
			);
		}

		public static Mat3 FromOuterProduct( Vec3 vector1, Vec3 vector2 )
		{
			return new Mat3
				(
				new Vec3( vector1.x * vector2.x, vector1.x * vector2.y, vector1.x * vector2.z ),
				new Vec3( vector1.y * vector2.x, vector1.y * vector2.y, vector1.y * vector2.z ),
				new Vec3( vector1.z * vector2.x, vector1.z * vector2.y, vector1.z * vector2.z )
				);
		}

		public static Mat3 FromEuler( Vec3 euler )
		{
			return FromEuler( euler.x, euler.y, euler.z );
		}

		public static Mat3 FromEuler( float eulerX, float eulerY, float eulerZ )
		{
			eulerX = MathUtils.DegToRad( eulerX );
			eulerY = MathUtils.DegToRad( eulerY );
			eulerZ = MathUtils.DegToRad( eulerZ );

			float cx = MathUtils.Cos( eulerX );
			float sx = MathUtils.Sin( eulerX );
			float cy = MathUtils.Cos( eulerY );
			float sy = MathUtils.Sin( eulerY );
			float cz = MathUtils.Cos( eulerZ );
			float sz = MathUtils.Sin( eulerZ );

			return new Mat3
			(
				new Vec3( cy * cz,
						  cy * sz,
						  -sy ),
				new Vec3( cz * sx * sy - cx * sz,
						  cx * cz + sx * sy * sz,
						  cy * sx ),
				new Vec3( cx * cz * sy + sx * sz,
						  -cz * sx + cx * sy * sz,
						  cx * cy
				)
			);
		}

		public static Mat3 FromQuaternion( Quat quaternion )
		{
			Vec4 squared = new Vec4( quaternion.x * quaternion.x, quaternion.y * quaternion.y, quaternion.z * quaternion.z,
									 quaternion.w * quaternion.w );
			float invSqLength = 1 / ( squared.x + squared.y + squared.z + squared.w );

			float temp1 = quaternion.x * quaternion.y;
			float temp2 = quaternion.z * quaternion.w;
			float temp3 = quaternion.x * quaternion.z;
			float temp4 = quaternion.y * quaternion.w;
			float temp5 = quaternion.y * quaternion.z;
			float temp6 = quaternion.x * quaternion.w;

			return new Mat3
			(
				new Vec3( ( squared.x - squared.y - squared.z + squared.w ) * invSqLength,
						  2 * ( temp1 + temp2 ) * invSqLength,
						  2 * ( temp3 - temp4 ) * invSqLength ),
				new Vec3( 2 * ( temp1 - temp2 ) * invSqLength,
						  ( -squared.x + squared.y - squared.z + squared.w ) * invSqLength,
						  2 * ( temp5 + temp6 ) * invSqLength ),
				new Vec3( 2 * ( temp3 + temp4 ) * invSqLength,
						  2 * ( temp5 - temp6 ) * invSqLength,
						  ( -squared.x - squared.y + squared.z + squared.w ) * invSqLength )
			);
		}

		public static Mat3 FromRotationAxis( float angle, Vec3 axis )
		{
			Quat quaternion = Quat.AngleAxis( angle, axis );
			return FromQuaternion( quaternion );
		}

		public static Mat3 LookAt( Vec3 forward, Vec3 up )
		{
			Vec3 z = Vec3.Normalize( forward );
			Vec3 x = Vec3.Normalize( up.Cross( z ) );
			Vec3 y = z.Cross( x );

			return new Mat3( x, y, z );
		}

		public static Mat3 FromCross( Vec3 vector )
		{
			Mat3 result;
			result.x.x = 0;
			result.x.y = -vector.z;
			result.x.z = vector.y;

			result.y.x = vector.z;
			result.y.y = 0;
			result.y.z = -vector.x;

			result.z.x = -vector.y;
			result.z.y = vector.x;
			result.z.z = 0;
			return result;
		}

		public static Mat3 NonhomogeneousInvert( Mat3 m )
		{
			Mat3 m1 = m;
			m1.NonhomogeneousInvert();
			return m1;
		}

		public static Mat3 Invert( Mat3 m )
		{
			float determinant = 1 / m.Determinant();

			return new Mat3
			(
				new Vec3
				(
					( m.y.y * m.z.z - m.y.z * m.z.y ) * determinant,
					( m.x.z * m.z.y - m.z.z * m.x.y ) * determinant,
					( m.x.y * m.y.z - m.y.y * m.x.z ) * determinant
				),
				new Vec3
				(
					( m.y.z * m.z.x - m.y.x * m.z.z ) * determinant,
					( m.x.x * m.z.z - m.x.z * m.z.x ) * determinant,
					( m.x.z * m.y.x - m.x.x * m.y.z ) * determinant
				),
				new Vec3
				(
					( m.y.x * m.z.y - m.y.y * m.z.x ) * determinant,
					( m.x.y * m.z.x - m.x.x * m.z.y ) * determinant,
					( m.x.x * m.y.y - m.x.y * m.y.x ) * determinant
				)
			);
		}

		public static Mat3 Transpose( Mat3 m )
		{
			return new Mat3
			(
				new Vec3( m.x.x, m.y.x, m.z.x ),
				new Vec3( m.x.y, m.y.y, m.z.y ),
				new Vec3( m.x.z, m.y.z, m.z.z )
			);
		}

		public static Mat3 Abs( Mat3 m )
		{
			return new Mat3( Vec3.Abs( m.x ), Vec3.Abs( m.y ), Vec3.Abs( m.z ) );
		}

		public static readonly Mat3 IDENTITY = new Mat3
			(
			new Vec3( 1, 0, 0 ),
			new Vec3( 0, 1, 0 ),
			new Vec3( 0, 0, 1 )
			);

		public static Mat3 operator +( Mat3 p1, Mat3 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			return p1;
		}

		public static Mat3 operator +( Mat3 p1, float p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			return p1;
		}

		public static Mat3 operator +( float p1, Mat3 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			return p2;
		}

		public static Mat3 operator -( Mat3 p1, Mat3 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			return p1;
		}

		public static Mat3 operator -( Mat3 p1, float p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			return p1;
		}

		public static Mat3 operator -( float p1, Mat3 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			return p2;
		}

		public static Mat3 operator -( Mat3 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			return p2;
		}

		public static Mat3 operator *( Mat3 m1, Mat3 m2 )
		{
			return new Mat3
			(
				new Vec3( m1.x.x * m2.x.x + m1.x.y * m2.y.x + m1.x.z * m2.z.x,
						  m1.x.x * m2.x.y + m1.x.y * m2.y.y + m1.x.z * m2.z.y,
						  m1.x.x * m2.x.z + m1.x.y * m2.y.z + m1.x.z * m2.z.z ),
				new Vec3( m1.y.x * m2.x.x + m1.y.y * m2.y.x + m1.y.z * m2.z.x,
						  m1.y.x * m2.x.y + m1.y.y * m2.y.y + m1.y.z * m2.z.y,
						  m1.y.x * m2.x.z + m1.y.y * m2.y.z + m1.y.z * m2.z.z ),
				new Vec3( m1.z.x * m2.x.x + m1.z.y * m2.y.x + m1.z.z * m2.z.x,
						  m1.z.x * m2.x.y + m1.z.y * m2.y.y + m1.z.z * m2.z.y,
						  m1.z.x * m2.x.z + m1.z.y * m2.y.z + m1.z.z * m2.z.z )
			);
		}

		public static Vec3 operator *( Vec3 v, Mat3 m )
		{
			return m.Transform( v );
		}

		public static Mat3 operator *( Mat3 p1, float p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			return p1;
		}

		public static Mat3 operator *( float p1, Mat3 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			p2.z = p1 * p2.z;
			return p2;
		}

		public static Mat3 operator /( Mat3 p1, Mat3 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			return p1;
		}

		public static Mat3 operator /( Mat3 p1, float p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			return p1;
		}

		public static Mat3 operator /( float p1, Mat3 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			return p2;
		}

		public static bool operator ==( Mat3 p1, Mat3 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
		}

		public static bool operator !=( Mat3 p1, Mat3 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z;
		}

		public static implicit operator Mat3( Mat2 v )
		{
			Mat3 o = IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		public static implicit operator Mat2( Mat3 v )
		{
			Mat2 o = Mat2.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( Mat3 )obj == this;
		}

		public override string ToString()
		{
			return $"(x:{this.x}\ny:{this.y}\nz:{this.z})";
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Vec3 Transform( Vec3 v )
		{
			return new Vec3
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z
			);
		}

		public Vec2 TransformPoint( Vec2 v )
		{
			return new Vec2
			(
				v.x * this.x.x + v.y * this.y.x + this.z.x,
				v.x * this.x.y + v.y * this.y.y + this.z.y
			);
		}

		public Vec2 TransformVector( Vec2 v )
		{
			return new Vec2
			(
				v.x * this.x.x + v.y * this.y.x,
				v.x * this.x.y + v.y * this.y.y
			);
		}

		public void Identity()
		{
			this.x.x = 1;
			this.x.y = 0;
			this.x.z = 0;
			this.y.x = 0;
			this.y.y = 1;
			this.y.z = 0;
			this.z.x = 0;
			this.z.y = 0;
			this.z.z = 1;
		}

		public Vec3 Euler()
		{
			if ( this.z.x < 1 )
			{
				if ( this.z.x > -1 )
				{
					return new Vec3( MathUtils.Atan2( this.z.y, this.z.z ), MathUtils.Asin( -this.z.x ),
									 MathUtils.Atan2( this.y.x, this.x.x ) );
				}
				return new Vec3( 0, MathUtils.PI_HALF, -MathUtils.Atan2( this.y.z, this.y.y ) );
			}
			return new Vec3( 0, -MathUtils.PI_HALF, MathUtils.Atan2( -this.y.z, this.y.y ) );
		}

		public void Transpose()
		{
			float m00 = this.x.x;
			float m01 = this.y.x;
			float m02 = this.z.x;
			float m10 = this.x.y;
			float m11 = this.y.y;
			float m12 = this.z.y;
			float m20 = this.x.z;
			float m21 = this.y.z;
			float m22 = this.z.z;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public Mat3 MultiplyTransposed( Mat3 matrix )
		{
			return new Mat3
			(
				new Vec3
				(
					this.x.x * matrix.x.x + this.y.x * matrix.y.x + this.z.x * matrix.z.x,
					this.x.x * matrix.x.y + this.y.x * matrix.y.y + this.z.x * matrix.z.y,
					this.x.x * matrix.x.z + this.y.x * matrix.y.z + this.z.x * matrix.z.z
				),
				new Vec3
				(
					this.x.y * matrix.x.x + this.y.y * matrix.y.x + this.z.y * matrix.z.x,
					this.x.y * matrix.x.y + this.y.y * matrix.y.y + this.z.y * matrix.z.y,
					this.x.y * matrix.x.z + this.y.y * matrix.y.z + this.z.y * matrix.z.z
				),
				new Vec3
				(
					this.x.z * matrix.x.x + this.y.z * matrix.y.x + this.z.z * matrix.z.x,
					this.x.z * matrix.x.y + this.y.z * matrix.y.y + this.z.z * matrix.z.y,
					this.x.z * matrix.x.z + this.y.z * matrix.y.z + this.z.z * matrix.z.z
				)
			);
		}

		public float Determinant()
		{
			return this.x.x * this.y.y * this.z.z + this.x.y * this.y.z * this.z.x + this.x.z * this.y.x * this.z.y -
				   this.z.x * this.y.y * this.x.z - this.z.y * this.y.z * this.x.x - this.z.z * this.y.x * this.x.y;
		}

		public void NonhomogeneousInvert()
		{
			Mat2 m3 = this;
			m3.Invert();
			this.x = m3.x;
			this.y = m3.y;
			Vec2 v = m3.Transform( this.z );
			this.z.x = -v.x;
			this.z.y = -v.y;
		}

		public void Invert()
		{
			float determinant = 1 / this.Determinant();

			float m00 = ( this.y.y * this.z.z - this.y.z * this.z.y ) * determinant;
			float m01 = ( this.x.z * this.z.y - this.z.z * this.x.y ) * determinant;
			float m02 = ( this.x.y * this.y.z - this.y.y * this.x.z ) * determinant;
			float m10 = ( this.y.z * this.z.x - this.y.x * this.z.z ) * determinant;
			float m11 = ( this.x.x * this.z.z - this.x.z * this.z.x ) * determinant;
			float m12 = ( this.x.z * this.y.x - this.x.x * this.y.z ) * determinant;
			float m20 = ( this.y.x * this.z.y - this.y.y * this.z.x ) * determinant;
			float m21 = ( this.x.y * this.z.x - this.x.x * this.z.y ) * determinant;
			float m22 = ( this.x.x * this.y.y - this.x.y * this.y.x ) * determinant;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public void RotateAroundAxisX( float angle )
		{
			angle = MathUtils.DegToRad( angle );
			float tCos = MathUtils.Cos( angle ), tSin = MathUtils.Sin( angle );
			float m00 = this.x.x;
			float m01 = this.y.x * tCos - this.z.x * tSin;
			float m02 = this.y.x * tSin + this.z.x * tCos;
			float m10 = this.x.y;
			float m11 = this.y.y * tCos - this.z.y * tSin;
			float m12 = this.y.y * tSin + this.z.y * tCos;
			float m20 = this.x.z;
			float m21 = this.y.z * tCos - this.z.z * tSin;
			float m22 = this.y.z * tSin + this.z.z * tCos;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public void RotateAroundAxisY( float angle )
		{
			angle = MathUtils.DegToRad( angle );
			float tCos = MathUtils.Cos( angle ), tSin = MathUtils.Sin( angle );
			float m00 = this.z.x * tSin + this.x.x * tCos;
			float m01 = this.y.x;
			float m02 = this.z.x * tCos - this.x.x * tSin;
			float m10 = this.z.y * tSin + this.x.y * tCos;
			float m11 = this.y.y;
			float m12 = this.z.y * tCos - this.x.y * tSin;
			float m20 = this.z.z * tSin + this.x.z * tCos;
			float m21 = this.y.z;
			float m22 = this.z.z * tCos - this.x.z * tSin;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public void RotateAroundAxisZ( float angle )
		{
			angle = MathUtils.DegToRad( angle );
			float tCos = MathUtils.Cos( angle ), tSin = MathUtils.Sin( angle );
			float m00 = this.x.x * tCos - this.y.x * tSin;
			float m01 = this.x.x * tSin + this.y.x * tCos;
			float m02 = this.z.x;
			float m10 = this.x.y * tCos - this.y.y * tSin;
			float m11 = this.x.y * tSin + this.y.y * tCos;
			float m12 = this.z.y;
			float m20 = this.x.z * tCos - this.y.z * tSin;
			float m21 = this.x.z * tSin + this.y.z * tCos;
			float m22 = this.z.z;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public void RotateAroundWorldAxisX( float angle )
		{
			angle = MathUtils.DegToRad( angle );
			angle = -angle;
			float tCos = MathUtils.Cos( angle ), tSin = MathUtils.Sin( angle );
			float m00 = this.x.x;
			float m01 = this.y.x;
			float m02 = this.z.x;
			float m10 = this.x.y * tCos - this.x.z * tSin;
			float m11 = this.y.y * tCos - this.y.z * tSin;
			float m12 = this.z.y * tCos - this.z.z * tSin;
			float m20 = this.x.y * tSin + this.x.z * tCos;
			float m21 = this.y.y * tSin + this.y.z * tCos;
			float m22 = this.z.y * tSin + this.z.z * tCos;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public void RotateAroundWorldAxisY( float angle )
		{
			angle = MathUtils.DegToRad( angle );
			angle = -angle;
			float tCos = MathUtils.Cos( angle ), tSin = MathUtils.Sin( angle );
			float m00 = this.x.z * tSin + this.x.x * tCos;
			float m01 = this.y.z * tSin + this.y.x * tCos;
			float m02 = this.z.z * tSin + this.z.x * tCos;
			float m10 = this.x.y;
			float m11 = this.y.y;
			float m12 = this.z.y;
			float m20 = this.x.z * tCos - this.x.x * tSin;
			float m21 = this.y.z * tCos - this.y.x * tSin;
			float m22 = this.z.z * tCos - this.z.x * tSin;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public void RotateAroundWorldAxisZ( float angle )
		{
			angle = MathUtils.DegToRad( angle );
			angle = -angle;
			float tCos = MathUtils.Cos( angle ), tSin = MathUtils.Sin( angle );
			float m00 = this.x.x * tCos - this.x.y * tSin;
			float m01 = this.y.x * tCos - this.y.y * tSin;
			float m02 = this.z.x * tCos - this.z.y * tSin;
			float m10 = this.x.x * tSin + this.x.y * tCos;
			float m11 = this.y.x * tSin + this.y.y * tCos;
			float m12 = this.z.x * tSin + this.z.y * tCos;
			float m20 = this.x.z;
			float m21 = this.y.z;
			float m22 = this.z.z;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
		}

		public Mat3 RotateAround( float angle, Vec3 axis )
		{
			// rotate into world space
			Quat quaternion = Quat.AngleAxis( 0, axis ).Conjugate();
			Mat3 worldSpaceMatrix = FromQuaternion( quaternion ) * this;

			// rotate back to matrix space
			quaternion = Quat.AngleAxis( angle, axis );
			Mat3 qMat = FromQuaternion( quaternion );
			worldSpaceMatrix = qMat * worldSpaceMatrix;
			return worldSpaceMatrix;
		}
	}
}