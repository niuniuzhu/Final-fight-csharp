namespace Core.FMath
{
	public struct FMat3
	{
		public FVec3 x, y, z;

		public FMat3( FVec3 x, FVec3 y, FVec3 z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static FMat3 FromScale( Fix64 scale )
		{
			return FromScale( scale, scale, scale );
		}

		public static FMat3 FromScale( FVec3 scale )
		{
			return FromScale( scale.x, scale.y, scale.z );
		}

		public static FMat3 FromScale( Fix64 scaleX, Fix64 scaleY, Fix64 scaleZ )
		{
			return new FMat3
			(
				new FVec3( scaleX, Fix64.Zero, Fix64.Zero ),
				new FVec3( Fix64.Zero, scaleY, Fix64.Zero ),
				new FVec3( Fix64.Zero, Fix64.Zero, scaleZ )
			);
		}

		public static FMat3 FromOuterProduct( FVec3 vector1, FVec3 vector2 )
		{
			return new FMat3
				(
				new FVec3( vector1.x * vector2.x, vector1.x * vector2.y, vector1.x * vector2.z ),
				new FVec3( vector1.y * vector2.x, vector1.y * vector2.y, vector1.y * vector2.z ),
				new FVec3( vector1.z * vector2.x, vector1.z * vector2.y, vector1.z * vector2.z )
				);
		}

		public static FMat3 FromEuler( FVec3 euler )
		{
			return FromEuler( euler.x, euler.x, euler.z );
		}

		public static FMat3 FromEuler( Fix64 eulerX, Fix64 eulerY, Fix64 eulerZ )
		{
			eulerX = Fix64.DegToRad( eulerX );
			eulerY = Fix64.DegToRad( eulerY );
			eulerZ = Fix64.DegToRad( eulerZ );

			Fix64 cx = Fix64.Cos( eulerX );
			Fix64 sx = Fix64.Sin( eulerX );
			Fix64 cy = Fix64.Cos( eulerY );
			Fix64 sy = Fix64.Sin( eulerY );
			Fix64 cz = Fix64.Cos( eulerZ );
			Fix64 sz = Fix64.Sin( eulerZ );

			return new FMat3
			(
				new FVec3( cy * cz,
						  cy * sz,
						  -sy ),
				new FVec3( cz * sx * sy - cx * sz,
						  cx * cz + sx * sy * sz,
						  cy * sx ),
				new FVec3( cx * cz * sy + sx * sz,
						  -cz * sx + cx * sy * sz,
						  cx * cy
				)
			);
		}

		public static FMat3 FromQuaternion( FQuat quaternion )
		{
			FVec4 squared = new FVec4( quaternion.x * quaternion.x, quaternion.y * quaternion.y, quaternion.z * quaternion.z,
									 quaternion.w * quaternion.w );
			Fix64 invSqLength = Fix64.One / ( squared.x + squared.y + squared.z + squared.w );

			Fix64 temp1 = quaternion.x * quaternion.y;
			Fix64 temp2 = quaternion.z * quaternion.w;
			Fix64 temp3 = quaternion.x * quaternion.z;
			Fix64 temp4 = quaternion.y * quaternion.w;
			Fix64 temp5 = quaternion.y * quaternion.z;
			Fix64 temp6 = quaternion.x * quaternion.w;

			return new FMat3
			(
				new FVec3( ( squared.x - squared.y - squared.z + squared.w ) * invSqLength,
						  Fix64.Two * ( temp1 + temp2 ) * invSqLength,
						   Fix64.Two * ( temp3 - temp4 ) * invSqLength ),
				new FVec3( Fix64.Two * ( temp1 - temp2 ) * invSqLength,
						  ( -squared.x + squared.y - squared.z + squared.w ) * invSqLength,
						   Fix64.Two * ( temp5 + temp6 ) * invSqLength ),
				new FVec3( Fix64.Two * ( temp3 + temp4 ) * invSqLength,
						   Fix64.Two * ( temp5 - temp6 ) * invSqLength,
						  ( -squared.x - squared.y + squared.z + squared.w ) * invSqLength )
			);
		}

		public static FMat3 FromRotationAxis( Fix64 angle, FVec3 axis )
		{
			FQuat quaternion = FQuat.AngleAxis( angle, axis );
			return FromQuaternion( quaternion );
		}

		public static FMat3 LookAt( FVec3 forward, FVec3 up )
		{
			FVec3 z = FVec3.Normalize( forward );
			FVec3 x = FVec3.Normalize( up.Cross( z ) );
			FVec3 y = z.Cross( x );

			return new FMat3( x, y, z );
		}

		public static FMat3 FromCross( FVec3 vector )
		{
			FMat3 result;
			result.x.x = Fix64.Zero;
			result.x.y = -vector.z;
			result.x.z = vector.y;

			result.y.x = vector.z;
			result.y.y = Fix64.Zero;
			result.y.z = -vector.x;

			result.z.x = -vector.y;
			result.z.y = vector.x;
			result.z.z = Fix64.Zero;
			return result;
		}

		public static FMat3 NonhomogeneousInverse( FMat3 m )
		{
			FMat2 m2 = m;
			m2.Invert();
			FMat3 o = m2;
			FVec3 v = m2 * m.z;
			o.z.x = -v.x;
			o.z.y = -v.y;
			return o;
		}

		public static FMat3 Invert( FMat3 m )
		{
			Fix64 determinant = Fix64.One / m.Determinant();

			return new FMat3
			(
				new FVec3
				(
					( m.y.y * m.z.z - m.y.z * m.z.y ) * determinant,
					( m.x.z * m.z.y - m.z.z * m.x.y ) * determinant,
					( m.x.y * m.y.z - m.y.y * m.x.z ) * determinant
				),
				new FVec3
				(
					( m.y.z * m.z.x - m.y.x * m.z.z ) * determinant,
					( m.x.x * m.z.z - m.x.z * m.z.x ) * determinant,
					( m.x.z * m.y.x - m.x.x * m.y.z ) * determinant
				),
				new FVec3
				(
					( m.y.x * m.z.y - m.y.y * m.z.x ) * determinant,
					( m.x.y * m.z.x - m.x.x * m.z.y ) * determinant,
					( m.x.x * m.y.y - m.x.y * m.y.x ) * determinant
				)
			);
		}

		public static FMat3 Transpose( FMat3 m )
		{
			return new FMat3
			(
				new FVec3( m.x.x, m.y.x, m.z.x ),
				new FVec3( m.x.y, m.y.y, m.z.y ),
				new FVec3( m.x.z, m.y.z, m.z.z )
			);
		}


		public static readonly FMat3 IDENTITY = new FMat3
			(
			new FVec3( 1, 0, 0 ),
			new FVec3( 0, 1, 0 ),
			new FVec3( 0, 0, 1 )
			);

		public static FMat3 operator +( FMat3 p1, FMat3 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			return p1;
		}

		public static FMat3 operator +( FMat3 p1, Fix64 p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			return p1;
		}

		public static FMat3 operator +( Fix64 p1, FMat3 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			return p2;
		}

		public static FMat3 operator -( FMat3 p1, FMat3 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			return p1;
		}

		public static FMat3 operator -( FMat3 p1, Fix64 p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			return p1;
		}

		public static FMat3 operator -( Fix64 p1, FMat3 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			return p2;
		}

		public static FMat3 operator -( FMat3 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			return p2;
		}

		public static FMat3 operator *( FMat3 m1, FMat3 m2 )
		{
			return new FMat3
			(
				new FVec3( m1.x.x * m2.x.x + m1.x.y * m2.y.x + m1.x.z * m2.z.x,
						   m1.x.x * m2.x.y + m1.x.y * m2.y.y + m1.x.z * m2.z.y,
						   m1.x.x * m2.x.z + m1.x.y * m2.y.z + m1.x.z * m2.z.z ),
				new FVec3( m1.y.x * m2.x.x + m1.y.y * m2.y.x + m1.y.z * m2.z.x,
						   m1.y.x * m2.x.y + m1.y.y * m2.y.y + m1.y.z * m2.z.y,
						   m1.y.x * m2.x.z + m1.y.y * m2.y.z + m1.y.z * m2.z.z ),
				new FVec3( m1.z.x * m2.x.x + m1.z.y * m2.y.x + m1.z.z * m2.z.x,
						   m1.z.x * m2.x.y + m1.z.y * m2.y.y + m1.z.z * m2.z.y,
						   m1.z.x * m2.x.z + m1.z.y * m2.y.z + m1.z.z * m2.z.z )
			);
		}

		public static FVec3 operator *( FVec3 v, FMat3 m )
		{
			return m.Transform( v );
		}

		public static FMat3 operator *( FMat3 p1, Fix64 p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			return p1;
		}

		public static FMat3 operator *( Fix64 p1, FMat3 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			p2.z = p1 * p2.z;
			return p2;
		}

		public static FMat3 operator /( FMat3 p1, FMat3 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			return p1;
		}

		public static FMat3 operator /( FMat3 p1, Fix64 p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			return p1;
		}

		public static FMat3 operator /( Fix64 p1, FMat3 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			return p2;
		}

		public static bool operator ==( FMat3 p1, FMat3 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
		}

		public static bool operator !=( FMat3 p1, FMat3 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z;
		}

		public override bool Equals( object obj )
		{
			return obj != null && ( FMat3 )obj == this;
		}

		public override string ToString()
		{
			return $"({this.x} : {this.y} : {this.z})";
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public FVec3 Transform( FVec3 v )
		{
			return new FVec3
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z
			);
		}

		public FVec2 TransformPoint( FVec2 v )
		{
			return new FVec2
			(
				v.x * this.x.x + v.y * this.y.x + this.z.x,
				v.x * this.x.y + v.y * this.y.y + this.z.y
			);
		}

		public FVec2 TransformVector( FVec2 v )
		{
			return new FVec2
			(
				v.x * this.x.x + v.y * this.y.x,
				v.x * this.x.y + v.y * this.y.y
			);
		}

		public void Identity()
		{
			this.x.x = Fix64.One;
			this.x.y = Fix64.Zero;
			this.x.z = Fix64.Zero;
			this.y.x = Fix64.Zero;
			this.y.y = Fix64.One;
			this.y.z = Fix64.Zero;
			this.z.x = Fix64.Zero;
			this.z.y = Fix64.Zero;
			this.z.z = Fix64.One;
		}

		public FVec3 Euler()
		{
			if ( this.z.x < Fix64.One )
			{
				if ( this.z.x > Fix64.NegativeOne )
				{
					return new FVec3( Fix64.Atan2( this.z.y, this.z.z ), Fix64.Asin( -this.z.x ),
									 Fix64.Atan2( this.y.x, this.x.x ) );
				}
				return new FVec3( Fix64.Zero, Fix64.PiOver2, -Fix64.Atan2( this.y.z, this.y.y ) );
			}
			return new FVec3( Fix64.Zero, -Fix64.PiOver2, Fix64.Atan2( -this.y.z, this.y.y ) );
		}

		public void Transpose()
		{
			Fix64 m00 = this.x.x;
			Fix64 m01 = this.y.x;
			Fix64 m02 = this.z.x;
			Fix64 m10 = this.x.y;
			Fix64 m11 = this.y.y;
			Fix64 m12 = this.z.y;
			Fix64 m20 = this.x.z;
			Fix64 m21 = this.y.z;
			Fix64 m22 = this.z.z;
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

		public FMat3 MultiplyTransposed( FMat3 matrix )
		{
			return new FMat3
			(
				new FVec3
				(
					this.x.x * matrix.x.x + this.y.x * matrix.y.x + this.z.x * matrix.z.x,
					this.x.x * matrix.x.y + this.y.x * matrix.y.y + this.z.x * matrix.z.y,
					this.x.x * matrix.x.z + this.y.x * matrix.y.z + this.z.x * matrix.z.z
				),
				new FVec3
				(
					this.x.y * matrix.x.x + this.y.y * matrix.y.x + this.z.y * matrix.z.x,
					this.x.y * matrix.x.y + this.y.y * matrix.y.y + this.z.y * matrix.z.y,
					this.x.y * matrix.x.z + this.y.y * matrix.y.z + this.z.y * matrix.z.z
				),
				new FVec3
				(
					this.x.z * matrix.x.x + this.y.z * matrix.y.x + this.z.z * matrix.z.x,
					this.x.z * matrix.x.y + this.y.z * matrix.y.y + this.z.z * matrix.z.y,
					this.x.z * matrix.x.z + this.y.z * matrix.y.z + this.z.z * matrix.z.z
				)
			);
		}

		public Fix64 Determinant()
		{
			return this.x.x * this.y.y * this.z.z + this.x.y * this.y.z * this.z.x + this.x.z * this.y.x * this.z.y -
				   this.z.x * this.y.y * this.x.z - this.z.y * this.y.z * this.x.x - this.z.z * this.y.x * this.x.y;
		}

		public void NonhomogeneousInverse()
		{
			FMat2 m3 = this;
			m3.Invert();
			this.x = m3.x;
			this.y = m3.y;
			FVec3 v = m3 * this.z;
			this.z.x = -v.x;
			this.z.y = -v.y;
		}

		public void Invert()
		{
			Fix64 determinant = Fix64.One / this.Determinant();

			Fix64 m00 = ( this.y.y * this.z.z - this.y.z * this.z.y ) * determinant;
			Fix64 m01 = ( this.x.z * this.z.y - this.z.z * this.x.y ) * determinant;
			Fix64 m02 = ( this.x.y * this.y.z - this.y.y * this.x.z ) * determinant;
			Fix64 m10 = ( this.y.z * this.z.x - this.y.x * this.z.z ) * determinant;
			Fix64 m11 = ( this.x.x * this.z.z - this.x.z * this.z.x ) * determinant;
			Fix64 m12 = ( this.x.z * this.y.x - this.x.x * this.y.z ) * determinant;
			Fix64 m20 = ( this.y.x * this.z.y - this.y.y * this.z.x ) * determinant;
			Fix64 m21 = ( this.x.y * this.z.x - this.x.x * this.z.y ) * determinant;
			Fix64 m22 = ( this.x.x * this.y.y - this.x.y * this.y.x ) * determinant;
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

		public void RotateAroundAxisX( Fix64 angle )
		{
			angle = Fix64.DegToRad( angle );
			Fix64 tCos = Fix64.Cos( angle ), tSin = Fix64.Sin( angle );
			Fix64 m00 = this.x.x;
			Fix64 m01 = this.y.x * tCos - this.z.x * tSin;
			Fix64 m02 = this.y.x * tSin + this.z.x * tCos;
			Fix64 m10 = this.x.y;
			Fix64 m11 = this.y.y * tCos - this.z.y * tSin;
			Fix64 m12 = this.y.y * tSin + this.z.y * tCos;
			Fix64 m20 = this.x.z;
			Fix64 m21 = this.y.z * tCos - this.z.z * tSin;
			Fix64 m22 = this.y.z * tSin + this.z.z * tCos;
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

		public void RotateAroundAxisY( Fix64 angle )
		{
			angle = Fix64.DegToRad( angle );
			Fix64 tCos = Fix64.Cos( angle ), tSin = Fix64.Sin( angle );
			Fix64 m00 = this.z.x * tSin + this.x.x * tCos;
			Fix64 m01 = this.y.x;
			Fix64 m02 = this.z.x * tCos - this.x.x * tSin;
			Fix64 m10 = this.z.y * tSin + this.x.y * tCos;
			Fix64 m11 = this.y.y;
			Fix64 m12 = this.z.y * tCos - this.x.y * tSin;
			Fix64 m20 = this.z.z * tSin + this.x.z * tCos;
			Fix64 m21 = this.y.z;
			Fix64 m22 = this.z.z * tCos - this.x.z * tSin;
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

		public void RotateAroundAxisZ( Fix64 angle )
		{
			angle = Fix64.DegToRad( angle );
			Fix64 tCos = Fix64.Cos( angle ), tSin = Fix64.Sin( angle );
			Fix64 m00 = this.x.x * tCos - this.y.x * tSin;
			Fix64 m01 = this.x.x * tSin + this.y.x * tCos;
			Fix64 m02 = this.z.x;
			Fix64 m10 = this.x.y * tCos - this.y.y * tSin;
			Fix64 m11 = this.x.y * tSin + this.y.y * tCos;
			Fix64 m12 = this.z.y;
			Fix64 m20 = this.x.z * tCos - this.y.z * tSin;
			Fix64 m21 = this.x.z * tSin + this.y.z * tCos;
			Fix64 m22 = this.z.z;
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

		public void RotateAroundWorldAxisX( Fix64 angle )
		{
			angle = Fix64.DegToRad( angle );
			angle = -angle;
			Fix64 tCos = Fix64.Cos( angle ), tSin = Fix64.Sin( angle );
			Fix64 m00 = this.x.x;
			Fix64 m01 = this.y.x;
			Fix64 m02 = this.z.x;
			Fix64 m10 = this.x.y * tCos - this.x.z * tSin;
			Fix64 m11 = this.y.y * tCos - this.y.z * tSin;
			Fix64 m12 = this.z.y * tCos - this.z.z * tSin;
			Fix64 m20 = this.x.y * tSin + this.x.z * tCos;
			Fix64 m21 = this.y.y * tSin + this.y.z * tCos;
			Fix64 m22 = this.z.y * tSin + this.z.z * tCos;
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

		public void RotateAroundWorldAxisY( Fix64 angle )
		{
			angle = Fix64.DegToRad( angle );
			angle = -angle;
			Fix64 tCos = Fix64.Cos( angle ), tSin = Fix64.Sin( angle );
			Fix64 m00 = this.x.z * tSin + this.x.x * tCos;
			Fix64 m01 = this.y.z * tSin + this.y.x * tCos;
			Fix64 m02 = this.z.z * tSin + this.z.x * tCos;
			Fix64 m10 = this.x.y;
			Fix64 m11 = this.y.y;
			Fix64 m12 = this.z.y;
			Fix64 m20 = this.x.z * tCos - this.x.x * tSin;
			Fix64 m21 = this.y.z * tCos - this.y.x * tSin;
			Fix64 m22 = this.z.z * tCos - this.z.x * tSin;
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

		public void RotateAroundWorldAxisZ( Fix64 angle )
		{
			angle = Fix64.DegToRad( angle );
			angle = -angle;
			Fix64 tCos = Fix64.Cos( angle ), tSin = Fix64.Sin( angle );
			Fix64 m00 = this.x.x * tCos - this.x.y * tSin;
			Fix64 m01 = this.y.x * tCos - this.y.y * tSin;
			Fix64 m02 = this.z.x * tCos - this.z.y * tSin;
			Fix64 m10 = this.x.x * tSin + this.x.y * tCos;
			Fix64 m11 = this.y.x * tSin + this.y.y * tCos;
			Fix64 m12 = this.z.x * tSin + this.z.y * tCos;
			Fix64 m20 = this.x.z;
			Fix64 m21 = this.y.z;
			Fix64 m22 = this.z.z;
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

		public FMat3 RotateAround( Fix64 angle, FVec3 axis )
		{
			// rotate into world space
			FQuat quaternion = FQuat.AngleAxis( Fix64.Zero, axis ).Conjugate();
			FMat3 worldSpaceMatrix = FromQuaternion( quaternion ) * this;

			// rotate back to matrix space
			quaternion = FQuat.AngleAxis( angle, axis );
			FMat3 qMat = FromQuaternion( quaternion );
			worldSpaceMatrix = qMat * worldSpaceMatrix;
			return worldSpaceMatrix;
		}
	}
}