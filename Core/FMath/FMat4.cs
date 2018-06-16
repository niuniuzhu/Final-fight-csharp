namespace Core.FMath
{
	public struct FMat4
	{
		public FVec4 x, y, z, w;

		public FMat4( FVec4 x, FVec4 y, FVec4 z, FVec4 w )
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static FMat4 FromScale( Fix64 scale )
		{
			return FromScale( scale, scale, scale );
		}

		public static FMat4 FromScale( FVec3 scale )
		{
			return FromScale( scale.x, scale.y, scale.z );
		}

		public static FMat4 FromScale( Fix64 scaleX, Fix64 scaleY, Fix64 scaleZ )
		{
			return new FMat4
			(
				new FVec4( scaleX, Fix64.Zero, Fix64.Zero, Fix64.Zero ),
				new FVec4( Fix64.Zero, scaleY, Fix64.Zero, Fix64.Zero ),
				new FVec4( Fix64.Zero, Fix64.Zero, scaleZ, Fix64.Zero ),
				new FVec4( Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One )
			);
		}

		public static FMat4 FromEuler( FVec3 euler )
		{
			return FromEuler( euler.x, euler.x, euler.z );
		}

		public static FMat4 FromEuler( Fix64 eulerX, Fix64 eulerY, Fix64 eulerZ )
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

			return new FMat4
			(
				new FVec4( cy * cz,
						  cy * sz,
						  -sy,
						  Fix64.Zero ),
				new FVec4( cz * sx * sy - cx * sz,
						  cx * cz + sx * sy * sz,
						  cy * sx,
						  Fix64.Zero ),
				new FVec4( cx * cz * sy + sx * sz,
						  -cz * sx + cx * sy * sz,
						  cx * cy,
						  Fix64.Zero ),
				new FVec4( Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One )
			);
		}

		public static FMat4 FromQuaternion( FQuat quaternion )
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

			return new FMat4
			(
				new FVec4( ( squared.x - squared.y - squared.z + squared.w ) * invSqLength,
						  Fix64.Two * ( temp1 + temp2 ) * invSqLength,
						   Fix64.Two * ( temp3 - temp4 ) * invSqLength,
						   Fix64.Zero ),
				new FVec4( Fix64.Two * ( temp1 - temp2 ) * invSqLength,
						  ( -squared.x + squared.y - squared.z + squared.w ) * invSqLength,
						   Fix64.Two * ( temp5 + temp6 ) * invSqLength,
						   Fix64.Zero ),
				new FVec4( Fix64.Two * ( temp3 + temp4 ) * invSqLength,
						   Fix64.Two * ( temp5 - temp6 ) * invSqLength,
						  ( -squared.x - squared.y + squared.z + squared.w ) * invSqLength,
						  Fix64.Zero ),
				new FVec4( Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One )
			);
		}

		public static FMat4 FromRotationAxis( Fix64 angle, FVec3 axis )
		{
			FQuat quaternion = FQuat.AngleAxis( angle, axis );
			return FromQuaternion( quaternion );
		}

		public static FMat4 FromTRS( FVec3 pos, FQuat q, FVec3 scale )
		{
			FMat4 m = FromScale( scale ) * FromQuaternion( q );
			m.w.x = pos.x;
			m.w.y = pos.y;
			m.w.z = pos.z;
			return m;
		}

		public static FMat4 NonhomogeneousInverse( FMat4 m )
		{
			FMat3 m3 = m;
			m3.Invert();
			FMat4 o = m3;
			FVec4 v = m3 * m.w;
			o.w.x = -v.x;
			o.w.y = -v.y;
			o.w.z = -v.z;
			return o;
		}

		public static FMat4 Invert( FMat4 m )
		{
			Fix64 determinant = Fix64.One / m.Determinant();
			FMat4 mat = new FMat4
			(
				new FVec4
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
				new FVec4
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
				new FVec4
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
				new FVec4
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

		public static FMat4 Transpose( FMat4 m )
		{
			return new FMat4
			(
				new FVec4( m.x.x, m.y.x, m.z.x, m.w.x ),
				new FVec4( m.x.y, m.y.y, m.z.y, m.w.y ),
				new FVec4( m.x.z, m.y.z, m.z.z, m.w.z ),
				new FVec4( m.x.w, m.y.w, m.z.w, m.w.w )
			);
		}

		public static FMat4 Abs( FMat4 m )
		{
			return new FMat4( FVec4.Abs( m.x ), FVec4.Abs( m.y ), FVec4.Abs( m.z ), FVec4.Abs( m.w ) );
		}

		public static readonly FMat4 IDENTITY = new FMat4
			(
			new FVec4( Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero ),
			new FVec4( Fix64.Zero, Fix64.One, Fix64.Zero, Fix64.Zero ),
			new FVec4( Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero ),
			new FVec4( Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One )
			);

		#region Operators

		// +
		public static FMat4 operator +( FMat4 p1, FMat4 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			p1.z += p2.z;
			p1.w += p2.w;
			return p1;
		}

		public static FMat4 operator +( FMat4 p1, Fix64 p2 )
		{
			p1.x += p2;
			p1.y += p2;
			p1.z += p2;
			p1.w += p2;
			return p1;
		}

		public static FMat4 operator +( Fix64 p1, FMat4 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			p2.z = p1 + p2.z;
			p2.w = p1 + p2.w;
			return p2;
		}

		// -
		public static FMat4 operator -( FMat4 p1, FMat4 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			p1.z -= p2.z;
			p1.w -= p2.w;
			return p1;
		}

		public static FMat4 operator -( FMat4 p1, Fix64 p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			p1.z -= p2;
			p1.w -= p2;
			return p1;
		}

		public static FMat4 operator -( Fix64 p1, FMat4 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			p2.z = p1 - p2.z;
			p2.w = p1 - p2.w;
			return p2;
		}

		public static FMat4 operator -( FMat4 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			p2.z = -p2.z;
			p2.w = -p2.w;
			return p2;
		}

		// *
		public static FMat4 operator *( FMat4 m1, FMat4 m2 )
		{
			return new FMat4
			(
				new FVec4( m1.x.x * m2.x.x + m1.x.y * m2.y.x + m1.x.z * m2.z.x + m1.x.w * m2.w.x,
						   m1.x.x * m2.x.y + m1.x.y * m2.y.y + m1.x.z * m2.z.y + m1.x.w * m2.w.y,
						   m1.x.x * m2.x.z + m1.x.y * m2.y.z + m1.x.z * m2.z.z + m1.x.w * m2.w.z,
						   m1.x.x * m2.x.w + m1.x.y * m2.y.w + m1.x.z * m2.z.w + m1.x.w * m2.w.w ),
				new FVec4( m1.y.x * m2.x.x + m1.y.y * m2.y.x + m1.y.z * m2.z.x + m1.y.w * m2.w.x,
						   m1.y.x * m2.x.y + m1.y.y * m2.y.y + m1.y.z * m2.z.y + m1.y.w * m2.w.y,
						   m1.y.x * m2.x.z + m1.y.y * m2.y.z + m1.y.z * m2.z.z + m1.y.w * m2.w.z,
						   m1.y.x * m2.x.w + m1.y.y * m2.y.w + m1.y.z * m2.z.w + m1.y.w * m2.w.w ),
				new FVec4( m1.z.x * m2.x.x + m1.z.y * m2.y.x + m1.z.z * m2.z.x + m1.z.w * m2.w.x,
						   m1.z.x * m2.x.y + m1.z.y * m2.y.y + m1.z.z * m2.z.y + m1.z.w * m2.w.y,
						   m1.z.x * m2.x.z + m1.z.y * m2.y.z + m1.z.z * m2.z.z + m1.z.w * m2.w.z,
						   m1.z.x * m2.x.w + m1.z.y * m2.y.w + m1.z.z * m2.z.w + m1.z.w * m2.w.w ),
				new FVec4( m1.w.x * m2.x.x + m1.w.y * m2.y.x + m1.w.z * m2.z.x + m1.w.w * m2.w.x,
						   m1.w.x * m2.x.y + m1.w.y * m2.y.y + m1.w.z * m2.z.y + m1.w.w * m2.w.y,
						   m1.w.x * m2.x.z + m1.w.y * m2.y.z + m1.w.z * m2.z.z + m1.w.w * m2.w.z,
						   m1.w.x * m2.x.w + m1.w.y * m2.y.w + m1.w.z * m2.z.w + m1.w.w * m2.w.w )
			);
		}

		public static FVec4 operator *( FVec4 v, FMat4 m )
		{
			return m.Transform( v );
		}

		public static FMat4 operator *( FMat4 p1, Fix64 p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			p1.z *= p2;
			p1.w *= p2;
			return p1;
		}

		public static FMat4 operator *( Fix64 p1, FMat4 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			p2.z = p1 * p2.z;
			p2.w = p1 * p2.w;
			return p2;
		}

		// /
		public static FMat4 operator /( FMat4 p1, FMat4 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			p1.z /= p2.z;
			p1.w /= p2.w;
			return p1;
		}

		public static FMat4 operator /( FMat4 p1, Fix64 p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			p1.z /= p2;
			p1.w /= p2;
			return p1;
		}

		public static FMat4 operator /( Fix64 p1, FMat4 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			p2.z = p1 / p2.z;
			p2.w = p1 / p2.w;
			return p2;
		}

		// ==
		public static bool operator ==( FMat4 p1, FMat4 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z && p1.w == p2.w;
		}

		public static bool operator !=( FMat4 p1, FMat4 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z || p1.w != p2.w;
		}

		public static implicit operator FMat4( FMat3 v )
		{
			FMat4 o = FMat4.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			o.z = v.z;
			return o;
		}

		public static implicit operator FMat3( FMat4 v )
		{
			FMat3 o = FMat3.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			o.z = v.z;
			return o;
		}

		public static implicit operator FMat4( FMat2 v )
		{
			FMat4 o = FMat4.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		public static implicit operator FMat2( FMat4 v )
		{
			FMat2 o = FMat2.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		#endregion

		#region Methods

		public override bool Equals( object obj )
		{
			return obj != null && ( FMat4 )obj == this;
		}

		public override string ToString()
		{
			return $"({this.x} : {this.y} : {this.z} : {this.w})";
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public FVec4 Transform( FVec4 v )
		{
			return new FVec4
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x + v.w * this.w.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y + v.w * this.w.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z + v.w * this.w.z,
				v.x * this.x.w + v.y * this.y.w + v.z * this.z.w + v.w * this.w.w
			);
		}

		public FVec3 TransformPoint( FVec3 v )
		{
			return new FVec3
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x + this.w.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y + this.w.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z + this.w.z
			);
		}

		public FVec3 TransformVector( FVec3 v )
		{
			return new FVec3
			(
				v.x * this.x.x + v.y * this.y.x + v.z * this.z.x,
				v.x * this.x.y + v.y * this.y.y + v.z * this.z.y,
				v.x * this.x.z + v.y * this.y.z + v.z * this.z.z
			);
		}

		public void Identity()
		{
			this.x.x = Fix64.One;
			this.x.y = Fix64.Zero;
			this.x.z = Fix64.Zero;
			this.x.w = Fix64.Zero;
			this.y.x = Fix64.Zero;
			this.y.y = Fix64.One;
			this.y.z = Fix64.Zero;
			this.y.w = Fix64.Zero;
			this.z.x = Fix64.Zero;
			this.z.y = Fix64.Zero;
			this.z.z = Fix64.One;
			this.z.w = Fix64.Zero;
			this.w.x = Fix64.Zero;
			this.w.y = Fix64.Zero;
			this.w.z = Fix64.Zero;
			this.w.w = Fix64.One;
		}

		public void Transpose()
		{
			Fix64 m00 = this.x.x;
			Fix64 m01 = this.y.x;
			Fix64 m02 = this.z.x;
			Fix64 m03 = this.w.x;
			Fix64 m10 = this.x.y;
			Fix64 m11 = this.y.y;
			Fix64 m12 = this.z.y;
			Fix64 m13 = this.w.y;
			Fix64 m20 = this.x.z;
			Fix64 m21 = this.y.z;
			Fix64 m22 = this.z.z;
			Fix64 m23 = this.w.z;
			Fix64 m30 = this.x.w;
			Fix64 m31 = this.y.w;
			Fix64 m32 = this.z.w;
			Fix64 m33 = this.w.w;
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

		public Fix64 Determinant()
		{
			Fix64 det1 = this.z.z * this.w.w - this.z.w * this.w.z;
			Fix64 det2 = this.z.y * this.w.w - this.z.w * this.w.y;
			Fix64 det3 = this.z.y * this.w.z - this.z.z * this.w.y;
			Fix64 det4 = this.z.x * this.w.w - this.z.w * this.w.x;
			Fix64 det5 = this.z.x * this.w.z - this.z.z * this.w.x;
			Fix64 det6 = this.z.x * this.w.y - this.z.y * this.w.x;

			return
				this.x.x * ( this.y.y * det1 - this.y.z * det2 + this.y.w * det3 ) -
				this.x.y * ( this.y.x * det1 - this.y.z * det4 + this.y.w * det5 ) +
				this.x.z * ( this.y.x * det2 - this.y.y * det4 + this.y.w * det6 ) -
				this.x.w * ( this.y.x * det3 - this.y.y * det5 + this.y.z * det6 );
		}

		public void NonhomogeneousInverse()
		{
			FMat3 m3 = this;
			m3.Invert();
			this.x = m3.x;
			this.y = m3.y;
			this.z = m3.z;
			FVec4 v = m3 * this.w;
			this.w.x = -v.x;
			this.w.y = -v.y;
			this.w.z = -v.z;
		}

		public void Invert()
		{
			Fix64 determinant = Fix64.One / this.Determinant();

			Fix64 m00 = ( this.y.y * this.z.z * this.w.w + this.y.z * this.z.w * this.w.y + this.y.w * this.z.y * this.w.z -
			  this.y.y * this.z.w * this.w.z - this.y.z * this.z.y * this.w.w - this.y.w * this.z.z * this.w.y ) * determinant;
			Fix64 m01 = ( this.x.y * this.z.w * this.w.z + this.x.z * this.z.y * this.w.w + this.x.w * this.z.z * this.w.y -
			  this.x.y * this.z.z * this.w.w - this.x.z * this.z.w * this.w.y - this.x.w * this.z.y * this.w.z ) * determinant;
			Fix64 m02 = ( this.x.y * this.y.z * this.w.w + this.x.z * this.y.w * this.w.y + this.x.w * this.y.y * this.w.z -
			  this.x.y * this.y.w * this.w.z - this.x.z * this.y.y * this.w.w - this.x.w * this.y.z * this.w.y ) * determinant;
			Fix64 m03 = ( this.x.y * this.y.w * this.z.z + this.x.z * this.y.y * this.z.w + this.x.w * this.y.z * this.z.y -
			  this.x.y * this.y.z * this.z.w - this.x.z * this.y.w * this.z.y - this.x.w * this.y.y * this.z.z ) * determinant;

			Fix64 m10 = ( this.y.x * this.z.w * this.w.z + this.y.z * this.z.x * this.w.w + this.y.w * this.z.z * this.w.x -
				  this.y.x * this.z.z * this.w.w - this.y.z * this.z.w * this.w.x - this.y.w * this.z.x * this.w.z ) * determinant;
			Fix64 m11 = ( this.x.x * this.z.z * this.w.w + this.x.z * this.z.w * this.w.x + this.x.w * this.z.x * this.w.z -
			  this.x.x * this.z.w * this.w.z - this.x.z * this.z.x * this.w.w - this.x.w * this.z.z * this.w.x ) * determinant;
			Fix64 m12 = ( this.x.x * this.y.w * this.w.z + this.x.z * this.y.x * this.w.w + this.x.w * this.y.z * this.w.x -
			  this.x.x * this.y.z * this.w.w - this.x.z * this.y.w * this.w.x - this.x.w * this.y.x * this.w.z ) * determinant;
			Fix64 m13 = ( this.x.x * this.y.z * this.z.w + this.x.z * this.y.w * this.z.x + this.x.w * this.y.x * this.z.z -
			  this.x.x * this.y.w * this.z.z - this.x.z * this.y.x * this.z.w - this.x.w * this.y.z * this.z.x ) * determinant;

			Fix64 m20 = ( this.y.x * this.z.y * this.w.w + this.y.y * this.z.w * this.w.x + this.y.w * this.z.x * this.w.y -
			  this.y.x * this.z.w * this.w.y - this.y.y * this.z.x * this.w.w - this.y.w * this.z.y * this.w.x ) * determinant;
			Fix64 m21 = ( this.x.x * this.z.w * this.w.y + this.x.y * this.z.x * this.w.w + this.x.w * this.z.y * this.w.x -
			  this.x.x * this.z.y * this.w.w - this.x.y * this.z.w * this.w.x - this.x.w * this.z.x * this.w.y ) * determinant;
			Fix64 m22 = ( this.x.x * this.y.y * this.w.w + this.x.y * this.y.w * this.w.x + this.x.w * this.y.x * this.w.y -
			  this.x.x * this.y.w * this.w.y - this.x.y * this.y.x * this.w.w - this.x.w * this.y.y * this.w.x ) * determinant;
			Fix64 m23 = ( this.x.x * this.y.w * this.z.y + this.x.y * this.y.x * this.z.w + this.x.w * this.y.y * this.z.x -
			  this.x.x * this.y.y * this.z.w - this.x.y * this.y.w * this.z.x - this.x.w * this.y.x * this.z.y ) * determinant;

			Fix64 m30 = ( this.y.x * this.z.z * this.w.y + this.y.y * this.z.x * this.w.z + this.y.z * this.z.y * this.w.x -
			  this.y.x * this.z.y * this.w.z - this.y.y * this.z.z * this.w.x - this.y.z * this.z.x * this.w.y ) * determinant;
			Fix64 m31 = ( this.x.x * this.z.y * this.w.z + this.x.y * this.z.z * this.w.x + this.x.z * this.z.x * this.w.y -
			  this.x.x * this.z.z * this.w.y - this.x.y * this.z.x * this.w.z - this.x.z * this.z.y * this.w.x ) * determinant;
			Fix64 m32 = ( this.x.x * this.y.z * this.w.y + this.x.y * this.y.x * this.w.z + this.x.z * this.y.y * this.w.x -
			  this.x.x * this.y.y * this.w.z - this.x.y * this.y.z * this.w.x - this.x.z * this.y.x * this.w.y ) * determinant;
			Fix64 m33 = ( this.x.x * this.y.y * this.z.z + this.x.y * this.y.z * this.z.x + this.x.z * this.y.x * this.z.y -
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

		public static FMat4 View( FVec3 position, FVec3 lookAt, FVec3 upVector )
		{
			FVec3 forward = FVec3.Normalize( lookAt - position );
			FVec3 xVec = FVec3.Normalize( forward.Cross( upVector ) );
			upVector = xVec.Cross( forward );

			FMat4 result;
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

			result.w.x = Fix64.Zero;
			result.w.y = Fix64.Zero;
			result.w.z = Fix64.Zero;
			result.w.w = Fix64.One;

			return result;
		}

		public static FMat4 Perspective( Fix64 fov, Fix64 aspect, Fix64 near, Fix64 far )
		{
			Fix64 top = near * Fix64.Tan( fov * Fix64.Half );
			Fix64 bottom = -top;
			Fix64 right = top * aspect;
			Fix64 left = -right;

			return Frustum( left, right, bottom, top, near, far );
		}

		public static FMat4 Frustum( Fix64 left, Fix64 right, Fix64 bottom, Fix64 top, Fix64 near, Fix64 far )
		{
			Fix64 width = right - left;
			Fix64 height = top - bottom;
			Fix64 depth = far - near;
			Fix64 n = near * Fix64.Two;

			FMat4 result;
			result.x.x = n / width;
			result.x.y = Fix64.Zero;
			result.x.z = ( right + left ) / width;
			result.x.w = Fix64.Zero;

			result.y.x = Fix64.Zero;
			result.y.y = n / height;
			result.y.z = ( top + bottom ) / height;
			result.y.w = Fix64.Zero;

			result.z.x = Fix64.Zero;
			result.z.y = Fix64.Zero;
			result.z.z = -( far + near ) / depth;
			result.z.w = -( n * far ) / depth;

			result.w.x = Fix64.Zero;
			result.w.y = Fix64.Zero;
			result.w.z = -Fix64.One;
			result.w.w = Fix64.Zero;

			return result;
		}

		public static FMat4 Orthographic( Fix64 width, Fix64 height, Fix64 near, Fix64 far )
		{
			return Orthographic( Fix64.Zero, width, Fix64.Zero, height, near, far );
		}

		public static FMat4 Orthographic( Fix64 left, Fix64 right, Fix64 bottom, Fix64 top, Fix64 near, Fix64 far )
		{
			Fix64 width = right - left;
			Fix64 height = top - bottom;
			Fix64 depth = far - near;

			FMat4 result;
			result.x.x = Fix64.Two / width;
			result.x.y = Fix64.Zero;
			result.x.z = Fix64.Zero;
			result.x.w = -( right + left ) / width;

			result.y.x = Fix64.Zero;
			result.y.y = Fix64.Two / height;
			result.y.z = Fix64.Zero;
			result.y.w = -( top + bottom ) / height;

			result.z.x = Fix64.Zero;
			result.z.y = Fix64.Zero;
			result.z.z = -Fix64.Two / depth;
			result.z.w = -( far + near ) / depth;

			result.w.x = Fix64.Zero;
			result.w.y = Fix64.Zero;
			result.w.z = Fix64.Zero;
			result.w.w = Fix64.One;

			return result;
		}

		public static FMat4 OrthographicCentered( Fix64 width, Fix64 height, Fix64 near, Fix64 far )
		{
			return OrthographicCentered( Fix64.Zero, width, Fix64.Zero, height, near, far );
		}

		public static FMat4 OrthographicCentered( Fix64 left, Fix64 right, Fix64 bottom, Fix64 top, Fix64 near, Fix64 far )
		{
			Fix64 width = right - left;
			Fix64 height = top - bottom;
			Fix64 depth = far - near;

			FMat4 result;
			result.x.x = Fix64.Two / width;
			result.x.y = Fix64.Zero;
			result.x.z = Fix64.Zero;
			result.x.w = Fix64.Zero;

			result.y.x = Fix64.Zero;
			result.y.y = Fix64.Two / height;
			result.y.z = Fix64.Zero;
			result.y.w = Fix64.Zero;

			result.z.x = Fix64.Zero;
			result.z.y = Fix64.Zero;
			result.z.z = -Fix64.Two / depth;
			result.z.w = -( ( far + near ) / depth );

			result.w.x = Fix64.Zero;
			result.w.y = Fix64.Zero;
			result.w.z = Fix64.Zero;
			result.w.w = Fix64.One;
			return result;
		}

		#endregion
	}
}