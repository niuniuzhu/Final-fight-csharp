namespace Core.Math
{
	public struct Mat4
	{
		public Vec4 x;
		public Vec4 y;
		public Vec4 z;
		public Vec4 w;

		public Mat4( Vec4 x, Vec4 y, Vec4 z, Vec4 w )
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static Mat4 FromScale( float scale )
		{
			return FromScale( scale, scale, scale );
		}

		public static Mat4 FromScale( Vec3 scale )
		{
			return FromScale( scale.x, scale.y, scale.z );
		}

		public static Mat4 FromScale( float scaleX, float scaleY, float scaleZ )
		{
			return new Mat4
			(
				new Vec4( scaleX, 0, 0, 0 ),
				new Vec4( 0, scaleY, 0, 0 ),
				new Vec4( 0, 0, scaleZ, 0 ),
				new Vec4( 0, 0, 0, 1 )
			);
		}

		public static Mat4 FromEuler( Vec3 euler )
		{
			return FromEuler( euler.x, euler.y, euler.z );
		}

		public static Mat4 FromEuler( float eulerX, float eulerY, float eulerZ )
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

			return new Mat4
			(
				new Vec4( cy * cz,
						  cy * sz,
						  -sy,
						  0 ),
				new Vec4( cz * sx * sy - cx * sz,
						  cx * cz + sx * sy * sz,
						  cy * sx,
						  0 ),
				new Vec4( cx * cz * sy + sx * sz,
						  -cz * sx + cx * sy * sz,
						  cx * cy,
						  0 ),
				new Vec4( 0, 0, 0, 1 )
			);
		}

		public static Mat4 FromQuaternion( Quat quaternion )
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

			return new Mat4
			(
				new Vec4( ( squared.x - squared.y - squared.z + squared.w ) * invSqLength,
						  2 * ( temp1 + temp2 ) * invSqLength,
						  2 * ( temp3 - temp4 ) * invSqLength,
						  0 ),
				new Vec4( 2 * ( temp1 - temp2 ) * invSqLength,
						  ( -squared.x + squared.y - squared.z + squared.w ) * invSqLength,
						  2 * ( temp5 + temp6 ) * invSqLength,
						  0 ),
				new Vec4( 2 * ( temp3 + temp4 ) * invSqLength,
						  2 * ( temp5 - temp6 ) * invSqLength,
						  ( -squared.x - squared.y + squared.z + squared.w ) * invSqLength,
						  0 ),
				new Vec4( 0, 0, 0, 1 )
			);
		}

		public static Mat4 FromRotationAxis( float angle, Vec3 axis )
		{
			Quat quaternion = Quat.AngleAxis( angle, axis );
			return FromQuaternion( quaternion );
		}

		public static Mat4 FromTRS( Vec3 pos, Quat q, Vec3 scale )
		{
			Mat4 m = FromScale( scale ) * FromQuaternion( q );
			m.w.x = pos.x;
			m.w.y = pos.y;
			m.w.z = pos.z;
			return m;
		}

		public static Mat4 NonhomogeneousInvert( Mat4 m )
		{
			Mat4 m1 = m;
			m1.NonhomogeneousInvert();
			return m1;
		}

		public static Mat4 Invert( Mat4 m )
		{
			float determinant = 1 / m.Determinant();
			Mat4 mat = new Mat4
			(
				new Vec4
				(
					( m.y.y * m.z.z * m.w.w + m.y.z * m.z.w * m.w.y + m.y.w * m.z.y * m.w.z -
					  m.y.y * m.z.w * m.w.z - m.y.z * m.z.y * m.w.w - m.y.w * m.z.z * m.w.y ) * determinant,
					( m.x.y * m.z.w * m.w.z + m.x.z * m.z.y * m.w.w + m.x.w * m.z.z * m.w.y -
					  m.x.y * m.z.z * m.w.w - m.x.z * m.z.w * m.w.y - m.x.w * m.z.y * m.w.z ) * determinant,
					( m.x.y * m.y.z * m.w.w + m.x.z * m.y.w * m.w.y + m.x.w * m.y.y * m.w.z -
					  m.x.y * m.y.w * m.w.z - m.x.z * m.y.y * m.w.w - m.x.w * m.y.z * m.w.y ) * determinant,
					( m.x.y * m.y.w * m.z.z + m.x.z * m.y.y * m.z.w + m.x.w * m.y.z * m.z.y -
					  m.x.y * m.y.z * m.z.w - m.x.z * m.y.w * m.z.y - m.x.w * m.y.y * m.z.z ) * determinant
				),
				new Vec4
				(
					( m.y.x * m.z.w * m.w.z + m.y.z * m.z.x * m.w.w + m.y.w * m.z.z * m.w.x -
					  m.y.x * m.z.z * m.w.w - m.y.z * m.z.w * m.w.x - m.y.w * m.z.x * m.w.z ) * determinant,
					( m.x.x * m.z.z * m.w.w + m.x.z * m.z.w * m.w.x + m.x.w * m.z.x * m.w.z -
					  m.x.x * m.z.w * m.w.z - m.x.z * m.z.x * m.w.w - m.x.w * m.z.z * m.w.x ) * determinant,
					( m.x.x * m.y.w * m.w.z + m.x.z * m.y.x * m.w.w + m.x.w * m.y.z * m.w.x -
					  m.x.x * m.y.z * m.w.w - m.x.z * m.y.w * m.w.x - m.x.w * m.y.x * m.w.z ) * determinant,
					( m.x.x * m.y.z * m.z.w + m.x.z * m.y.w * m.z.x + m.x.w * m.y.x * m.z.z -
					  m.x.x * m.y.w * m.z.z - m.x.z * m.y.x * m.z.w - m.x.w * m.y.z * m.z.x ) * determinant
				),
				new Vec4
				(
					( m.y.x * m.z.y * m.w.w + m.y.y * m.z.w * m.w.x + m.y.w * m.z.x * m.w.y -
					  m.y.x * m.z.w * m.w.y - m.y.y * m.z.x * m.w.w - m.y.w * m.z.y * m.w.x ) * determinant,
					( m.x.x * m.z.w * m.w.y + m.x.y * m.z.x * m.w.w + m.x.w * m.z.y * m.w.x -
					  m.x.x * m.z.y * m.w.w - m.x.y * m.z.w * m.w.x - m.x.w * m.z.x * m.w.y ) * determinant,
					( m.x.x * m.y.y * m.w.w + m.x.y * m.y.w * m.w.x + m.x.w * m.y.x * m.w.y -
					  m.x.x * m.y.w * m.w.y - m.x.y * m.y.x * m.w.w - m.x.w * m.y.y * m.w.x ) * determinant,
					( m.x.x * m.y.w * m.z.y + m.x.y * m.y.x * m.z.w + m.x.w * m.y.y * m.z.x -
					  m.x.x * m.y.y * m.z.w - m.x.y * m.y.w * m.z.x - m.x.w * m.y.x * m.z.y ) * determinant
				),
				new Vec4
				(
					( m.y.x * m.z.z * m.w.y + m.y.y * m.z.x * m.w.z + m.y.z * m.z.y * m.w.x -
					  m.y.x * m.z.y * m.w.z - m.y.y * m.z.z * m.w.x - m.y.z * m.z.x * m.w.y ) * determinant,
					( m.x.x * m.z.y * m.w.z + m.x.y * m.z.z * m.w.x + m.x.z * m.z.x * m.w.y -
					  m.x.x * m.z.z * m.w.y - m.x.y * m.z.x * m.w.z - m.x.z * m.z.y * m.w.x ) * determinant,
					( m.x.x * m.y.z * m.w.y + m.x.y * m.y.x * m.w.z + m.x.z * m.y.y * m.w.x -
					  m.x.x * m.y.y * m.w.z - m.x.y * m.y.z * m.w.x - m.x.z * m.y.x * m.w.y ) * determinant,
					( m.x.x * m.y.y * m.z.z + m.x.y * m.y.z * m.z.x + m.x.z * m.y.x * m.z.y -
					  m.x.x * m.y.z * m.z.y - m.x.y * m.y.x * m.z.z - m.x.z * m.y.y * m.z.x ) * determinant
				)
			);
			return mat;
		}

		public static Mat4 Transpose( Mat4 m )
		{
			return new Mat4
			(
				new Vec4( m.x.x, m.y.x, m.z.x, m.w.x ),
				new Vec4( m.x.y, m.y.y, m.z.y, m.w.y ),
				new Vec4( m.x.z, m.y.z, m.z.z, m.w.z ),
				new Vec4( m.x.w, m.y.w, m.z.w, m.w.w )
			);
		}

		public static Mat4 Abs( Mat4 m )
		{
			return new Mat4( Vec4.Abs( m.x ), Vec4.Abs( m.y ), Vec4.Abs( m.z ), Vec4.Abs( m.w ) );
		}

		public static readonly Mat4 IDENTITY = new Mat4
			(
			new Vec4( 1, 0, 0, 0 ),
			new Vec4( 0, 1, 0, 0 ),
			new Vec4( 0, 0, 1, 0 ),
			new Vec4( 0, 0, 0, 1 )
			);

		#region Operators

		public static Mat4 operator +( Mat4 p1, Mat4 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			p1.w += p2.w;
			return p1;
		}

		public static Mat4 operator +( Mat4 p1, float p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			p1.w += p2;
			return p1;
		}

		public static Mat4 operator +( float p1, Mat4 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			p2.w = p1 + p2.w;
			return p2;
		}

		public static Mat4 operator -( Mat4 p1, Mat4 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			p1.w -= p2.w;
			return p1;
		}

		public static Mat4 operator -( Mat4 p1, float p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			p1.w -= p2;
			return p1;
		}

		public static Mat4 operator -( float p1, Mat4 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			p2.w = p1 - p2.w;
			return p2;
		}

		public static Mat4 operator -( Mat4 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			p2.w = -p2.w;
			return p2;
		}

		public static Mat4 operator *( Mat4 m1, Mat4 m2 )
		{
			return new Mat4
			(
				new Vec4( m1.x.x * m2.x.x + m1.x.y * m2.y.x + m1.x.z * m2.z.x + m1.x.w * m2.w.x,
						  m1.x.x * m2.x.y + m1.x.y * m2.y.y + m1.x.z * m2.z.y + m1.x.w * m2.w.y,
						  m1.x.x * m2.x.z + m1.x.y * m2.y.z + m1.x.z * m2.z.z + m1.x.w * m2.w.z,
						  m1.x.x * m2.x.w + m1.x.y * m2.y.w + m1.x.z * m2.z.w + m1.x.w * m2.w.w ),
				new Vec4( m1.y.x * m2.x.x + m1.y.y * m2.y.x + m1.y.z * m2.z.x + m1.y.w * m2.w.x,
						  m1.y.x * m2.x.y + m1.y.y * m2.y.y + m1.y.z * m2.z.y + m1.y.w * m2.w.y,
						  m1.y.x * m2.x.z + m1.y.y * m2.y.z + m1.y.z * m2.z.z + m1.y.w * m2.w.z,
						  m1.y.x * m2.x.w + m1.y.y * m2.y.w + m1.y.z * m2.z.w + m1.y.w * m2.w.w ),
				new Vec4( m1.z.x * m2.x.x + m1.z.y * m2.y.x + m1.z.z * m2.z.x + m1.z.w * m2.w.x,
						  m1.z.x * m2.x.y + m1.z.y * m2.y.y + m1.z.z * m2.z.y + m1.z.w * m2.w.y,
						  m1.z.x * m2.x.z + m1.z.y * m2.y.z + m1.z.z * m2.z.z + m1.z.w * m2.w.z,
						  m1.z.x * m2.x.w + m1.z.y * m2.y.w + m1.z.z * m2.z.w + m1.z.w * m2.w.w ),
				new Vec4( m1.w.x * m2.x.x + m1.w.y * m2.y.x + m1.w.z * m2.z.x + m1.w.w * m2.w.x,
						  m1.w.x * m2.x.y + m1.w.y * m2.y.y + m1.w.z * m2.z.y + m1.w.w * m2.w.y,
						  m1.w.x * m2.x.z + m1.w.y * m2.y.z + m1.w.z * m2.z.z + m1.w.w * m2.w.z,
						  m1.w.x * m2.x.w + m1.w.y * m2.y.w + m1.w.z * m2.z.w + m1.w.w * m2.w.w )
			);
		}

		public static Vec4 operator *( Vec4 v, Mat4 m )
		{
			return m.Transform( v );
		}

		public static Mat4 operator *( Mat4 p1, float p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			p1.w *= p2;
			return p1;
		}

		public static Mat4 operator *( float p1, Mat4 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			p2.z = p1 * p2.z;
			p2.w = p1 * p2.w;
			return p2;
		}

		public static Mat4 operator /( Mat4 p1, Mat4 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			p1.w /= p2.w;
			return p1;
		}

		public static Mat4 operator /( Mat4 p1, float p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			p1.w /= p2;
			return p1;
		}

		public static Mat4 operator /( float p1, Mat4 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			p2.w = p1 / p2.w;
			return p2;
		}

		public static bool operator ==( Mat4 p1, Mat4 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z && p1.w == p2.w;
		}

		public static bool operator !=( Mat4 p1, Mat4 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z || p1.w != p2.w;
		}

		public static implicit operator Mat4( Mat3 v )
		{
			Mat4 o = IDENTITY;
			o.x = v.x;
			o.y = v.y;
			o.z = v.z;
			return o;
		}

		public static implicit operator Mat3( Mat4 v )
		{
			Mat3 o = Mat3.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			o.z = v.z;
			return o;
		}

		public static implicit operator Mat4( Mat2 v )
		{
			Mat4 o = Mat4.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		public static implicit operator Mat2( Mat4 v )
		{
			Mat2 o = Mat2.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		#endregion

		#region Methods

		public override bool Equals( object obj )
		{
			return obj != null && ( Mat4 )obj == this;
		}

		public override string ToString()
		{
			return $"(x:{this.x}\ny:{this.y}\nz:{this.z}\nw:{this.w})";
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Vec4 Transform( Vec4 v )
		{
			return new Vec4
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x + v.w * this.w.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y + v.w * this.w.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z + v.w * this.w.z,
				v.x * this.x.w + v.y * this.y.w + v.z * this.z.w + v.w * this.w.w
			);
		}

		public Vec3 TransformPoint( Vec3 v )
		{
			return new Vec3
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x + this.w.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y + this.w.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z + this.w.z
			);
		}

		public Vec3 TransformVector( Vec3 v )
		{
			return new Vec3
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z
			);
		}

		public void Identity()
		{
			this.x.x = 1;
			this.x.y = 0;
			this.x.z = 0;
			this.x.w = 0;
			this.y.x = 0;
			this.y.y = 1;
			this.y.z = 0;
			this.y.w = 0;
			this.z.x = 0;
			this.z.y = 0;
			this.z.z = 1;
			this.z.w = 0;
			this.w.x = 0;
			this.w.y = 0;
			this.w.z = 0;
			this.w.w = 1;
		}

		public void Transpose()
		{
			float m00 = this.x.x;
			float m01 = this.y.x;
			float m02 = this.z.x;
			float m03 = this.w.x;
			float m10 = this.x.y;
			float m11 = this.y.y;
			float m12 = this.z.y;
			float m13 = this.w.y;
			float m20 = this.x.z;
			float m21 = this.y.z;
			float m22 = this.z.z;
			float m23 = this.w.z;
			float m30 = this.x.w;
			float m31 = this.y.w;
			float m32 = this.z.w;
			float m33 = this.w.w;
			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.x.w = m03;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.y.w = m13;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
			this.z.w = m23;
			this.w.x = m30;
			this.w.y = m31;
			this.w.z = m32;
			this.w.w = m33;
		}

		public float Determinant()
		{
			float det1 = this.z.z * this.w.w - this.z.w * this.w.z;
			float det2 = this.z.y * this.w.w - this.z.w * this.w.y;
			float det3 = this.z.y * this.w.z - this.z.z * this.w.y;
			float det4 = this.z.x * this.w.w - this.z.w * this.w.x;
			float det5 = this.z.x * this.w.z - this.z.z * this.w.x;
			float det6 = this.z.x * this.w.y - this.z.y * this.w.x;

			return
				this.x.x * ( this.y.y * det1 - this.y.z * det2 + this.y.w * det3 ) -
				this.x.y * ( this.y.x * det1 - this.y.z * det4 + this.y.w * det5 ) +
				this.x.z * ( this.y.x * det2 - this.y.y * det4 + this.y.w * det6 ) -
				this.x.w * ( this.y.x * det3 - this.y.y * det5 + this.y.z * det6 );
		}

		public void NonhomogeneousInvert()
		{
			Mat3 m3 = this;
			m3.Invert();
			this.x = m3.x;
			this.y = m3.y;
			this.z = m3.z;
			Vec3 v = m3.Transform( this.w );
			this.w.x = -v.x;
			this.w.y = -v.y;
			this.w.z = -v.z;
		}

		public void Invert()
		{
			float determinant = 1 / this.Determinant();

			float m00 = ( this.y.y * this.z.z * this.w.w + this.y.z * this.z.w * this.w.y + this.y.w * this.z.y * this.w.z -
			  this.y.y * this.z.w * this.w.z - this.y.z * this.z.y * this.w.w - this.y.w * this.z.z * this.w.y ) * determinant;
			float m01 = ( this.x.y * this.z.w * this.w.z + this.x.z * this.z.y * this.w.w + this.x.w * this.z.z * this.w.y -
			  this.x.y * this.z.z * this.w.w - this.x.z * this.z.w * this.w.y - this.x.w * this.z.y * this.w.z ) * determinant;
			float m02 = ( this.x.y * this.y.z * this.w.w + this.x.z * this.y.w * this.w.y + this.x.w * this.y.y * this.w.z -
			  this.x.y * this.y.w * this.w.z - this.x.z * this.y.y * this.w.w - this.x.w * this.y.z * this.w.y ) * determinant;
			float m03 = ( this.x.y * this.y.w * this.z.z + this.x.z * this.y.y * this.z.w + this.x.w * this.y.z * this.z.y -
			  this.x.y * this.y.z * this.z.w - this.x.z * this.y.w * this.z.y - this.x.w * this.y.y * this.z.z ) * determinant;

			float m10 = ( this.y.x * this.z.w * this.w.z + this.y.z * this.z.x * this.w.w + this.y.w * this.z.z * this.w.x -
				  this.y.x * this.z.z * this.w.w - this.y.z * this.z.w * this.w.x - this.y.w * this.z.x * this.w.z ) * determinant;
			float m11 = ( this.x.x * this.z.z * this.w.w + this.x.z * this.z.w * this.w.x + this.x.w * this.z.x * this.w.z -
			  this.x.x * this.z.w * this.w.z - this.x.z * this.z.x * this.w.w - this.x.w * this.z.z * this.w.x ) * determinant;
			float m12 = ( this.x.x * this.y.w * this.w.z + this.x.z * this.y.x * this.w.w + this.x.w * this.y.z * this.w.x -
			  this.x.x * this.y.z * this.w.w - this.x.z * this.y.w * this.w.x - this.x.w * this.y.x * this.w.z ) * determinant;
			float m13 = ( this.x.x * this.y.z * this.z.w + this.x.z * this.y.w * this.z.x + this.x.w * this.y.x * this.z.z -
			  this.x.x * this.y.w * this.z.z - this.x.z * this.y.x * this.z.w - this.x.w * this.y.z * this.z.x ) * determinant;

			float m20 = ( this.y.x * this.z.y * this.w.w + this.y.y * this.z.w * this.w.x + this.y.w * this.z.x * this.w.y -
			  this.y.x * this.z.w * this.w.y - this.y.y * this.z.x * this.w.w - this.y.w * this.z.y * this.w.x ) * determinant;
			float m21 = ( this.x.x * this.z.w * this.w.y + this.x.y * this.z.x * this.w.w + this.x.w * this.z.y * this.w.x -
			  this.x.x * this.z.y * this.w.w - this.x.y * this.z.w * this.w.x - this.x.w * this.z.x * this.w.y ) * determinant;
			float m22 = ( this.x.x * this.y.y * this.w.w + this.x.y * this.y.w * this.w.x + this.x.w * this.y.x * this.w.y -
			  this.x.x * this.y.w * this.w.y - this.x.y * this.y.x * this.w.w - this.x.w * this.y.y * this.w.x ) * determinant;
			float m23 = ( this.x.x * this.y.w * this.z.y + this.x.y * this.y.x * this.z.w + this.x.w * this.y.y * this.z.x -
			  this.x.x * this.y.y * this.z.w - this.x.y * this.y.w * this.z.x - this.x.w * this.y.x * this.z.y ) * determinant;

			float m30 = ( this.y.x * this.z.z * this.w.y + this.y.y * this.z.x * this.w.z + this.y.z * this.z.y * this.w.x -
			  this.y.x * this.z.y * this.w.z - this.y.y * this.z.z * this.w.x - this.y.z * this.z.x * this.w.y ) * determinant;
			float m31 = ( this.x.x * this.z.y * this.w.z + this.x.y * this.z.z * this.w.x + this.x.z * this.z.x * this.w.y -
			  this.x.x * this.z.z * this.w.y - this.x.y * this.z.x * this.w.z - this.x.z * this.z.y * this.w.x ) * determinant;
			float m32 = ( this.x.x * this.y.z * this.w.y + this.x.y * this.y.x * this.w.z + this.x.z * this.y.y * this.w.x -
			  this.x.x * this.y.y * this.w.z - this.x.y * this.y.z * this.w.x - this.x.z * this.y.x * this.w.y ) * determinant;
			float m33 = ( this.x.x * this.y.y * this.z.z + this.x.y * this.y.z * this.z.x + this.x.z * this.y.x * this.z.y -
			  this.x.x * this.y.z * this.z.y - this.x.y * this.y.x * this.z.z - this.x.z * this.y.y * this.z.x ) * determinant;

			this.x.x = m00;
			this.x.y = m01;
			this.x.z = m02;
			this.x.w = m03;
			this.y.x = m10;
			this.y.y = m11;
			this.y.z = m12;
			this.y.w = m13;
			this.z.x = m20;
			this.z.y = m21;
			this.z.z = m22;
			this.z.w = m23;
			this.w.x = m30;
			this.w.y = m31;
			this.w.z = m32;
			this.w.w = m33;
		}

		public static Mat4 View( Vec3 position, Vec3 lookAt, Vec3 upVector )
		{
			Vec3 forward = Vec3.Normalize( lookAt - position );
			Vec3 xVec = Vec3.Normalize( forward.Cross( upVector ) );
			upVector = xVec.Cross( forward );

			Mat4 result;
			result.x.x = xVec.x;
			result.x.y = xVec.y;
			result.x.z = xVec.z;
			result.x.w = position.Dot( -xVec );

			result.y.x = upVector.x;
			result.y.y = upVector.y;
			result.y.z = upVector.z;
			result.y.w = position.Dot( -upVector );

			result.z.x = -forward.x;
			result.z.y = -forward.y;
			result.z.z = -forward.z;
			result.z.w = position.Dot( forward );

			result.w.x = 0;
			result.w.y = 0;
			result.w.z = 0;
			result.w.w = 1;

			return result;
		}

		public static Mat4 Perspective( float fov, float aspect, float near, float far )
		{
			float top = near * MathUtils.Tan( fov * .5f );
			float bottom = -top;
			float right = top * aspect;
			float left = -right;

			return Frustum( left, right, bottom, top, near, far );
		}

		public static Mat4 Frustum( float left, float right, float bottom, float top, float near, float far )
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;
			float n = near * 2;

			Mat4 result;
			result.x.x = n / width;
			result.x.y = 0;
			result.x.z = ( right + left ) / width;
			result.x.w = 0;

			result.y.x = 0;
			result.y.y = n / height;
			result.y.z = ( top + bottom ) / height;
			result.y.w = 0;

			result.z.x = 0;
			result.z.y = 0;
			result.z.z = -( far + near ) / depth;
			result.z.w = -( n * far ) / depth;

			result.w.x = 0;
			result.w.y = 0;
			result.w.z = -1;
			result.w.w = 0;

			return result;
		}

		public static Mat4 Orthographic( float width, float height, float near, float far )
		{
			return Orthographic( 0, width, 0, height, near, far );
		}

		public static Mat4 Orthographic( float left, float right, float bottom, float top, float near, float far )
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;

			Mat4 result;
			result.x.x = 2 / width;
			result.x.y = 0;
			result.x.z = 0;
			result.x.w = -( right + left ) / width;

			result.y.x = 0;
			result.y.y = 2 / height;
			result.y.z = 0;
			result.y.w = -( top + bottom ) / height;

			result.z.x = 0;
			result.z.y = 0;
			result.z.z = -2 / depth;
			result.z.w = -( far + near ) / depth;

			result.w.x = 0;
			result.w.y = 0;
			result.w.z = 0;
			result.w.w = 1;

			return result;
		}

		public static Mat4 OrthographicCentered( float width, float height, float near, float far )
		{
			return OrthographicCentered( 0, width, 0, height, near, far );
		}

		public static Mat4 OrthographicCentered( float left, float right, float bottom, float top, float near, float far )
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;

			Mat4 result;
			result.x.x = 2 / width;
			result.x.y = 0;
			result.x.z = 0;
			result.x.w = 0;

			result.y.x = 0;
			result.y.y = 2 / height;
			result.y.z = 0;
			result.y.w = 0;

			result.z.x = 0;
			result.z.y = 0;
			result.z.z = -2 / depth;
			result.z.w = -( ( far + near ) / depth );

			result.w.x = 0;
			result.w.y = 0;
			result.w.z = 0;
			result.w.w = 1;
			return result;
		}

		#endregion
	}
}