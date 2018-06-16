namespace Core.FMath
{
	public struct FMat2
	{
		public FVec2 x;
		public FVec2 y;

		public FMat2( FVec2 x, FVec2 y )
		{
			this.x = x;
			this.y = y;
		}

		public static FMat2 FromCross( FVec2 xVector )
		{
			return new FMat2( xVector, new FVec2( -xVector.y, xVector.x ) );
		}

		public static FMat2 Transpose( FMat2 m )
		{
			return new FMat2
			(
				new FVec2( m.x.x, m.y.x ),
				new FVec2( m.x.y, m.y.y )
			);
		}

		public static FMat2 Invert( FMat2 m )
		{
			Fix64 determinant = Fix64.One / ( m.x.x * m.y.y - m.x.y * m.y.x );
			FMat2 result;
			result.x.x = m.y.y * determinant;
			result.x.y = -m.x.y * determinant;
			result.y.x = -m.y.x * determinant;
			result.y.y = m.x.x * determinant;
			return result;
		}

		public static readonly FMat2 IDENTITY = new FMat2
			(
			new FVec2( 1, 0 ),
			new FVec2( 0, 1 )
			);

		#region Operators

		// +
		public static FMat2 operator +( FMat2 p1, FMat2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static FMat2 operator +( FMat2 p1, Fix64 p2 )
		{
			p1.x += p2;
			p1.y += p2;
			return p1;
		}

		public static FMat2 operator +( Fix64 p1, FMat2 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			return p2;
		}

		public static FMat2 operator -( FMat2 p1, FMat2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static FMat2 operator -( FMat2 p1, Fix64 p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			return p1;
		}

		public static FMat2 operator -( Fix64 p1, FMat2 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			return p2;
		}

		public static FMat2 operator -( FMat2 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			return p2;
		}

		public static FMat2 operator *( FMat2 m1, FMat2 m2 )
		{
			return new FMat2
			(
				new FVec2( m1.x.x * m2.x.x + m1.x.y * m2.y.x,
						   m1.x.x * m2.x.y + m1.x.y * m2.y.y ),
				new FVec2( m1.y.x * m2.x.x + m1.y.y * m2.y.x,
						   m1.y.x * m2.x.y + m1.y.y * m2.y.y )
			);
		}

		public static FVec2 operator *( FVec2 v, FMat2 m )
		{
			return m.Transform( v );
		}

		public static FMat2 operator *( FMat2 p1, Fix64 p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			return p1;
		}

		public static FMat2 operator *( Fix64 p1, FMat2 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			return p2;
		}

		// /
		public static FMat2 operator /( FMat2 p1, FMat2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static FMat2 operator /( FMat2 p1, Fix64 p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			return p1;
		}

		public static FMat2 operator /( Fix64 p1, FMat2 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			return p2;
		}

		// ==
		public static bool operator ==( FMat2 p1, FMat2 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y;
		}

		public static bool operator !=( FMat2 p1, FMat2 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y;
		}

		public static implicit operator FMat2( FMat3 v )
		{
			FMat2 o = FMat2.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		public static implicit operator FMat3( FMat2 v )
		{
			FMat3 o = FMat3.IDENTITY;
			o.x = v.x;
			o.y = v.y;
			return o;
		}

		#endregion

		#region Methods

		public override bool Equals( object obj )
		{
			return obj != null && ( FMat2 )obj == this;
		}

		public override string ToString()
		{
			return $"({this.x} : {this.y})";
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public FVec2 Transform( FVec2 v )
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
			this.y.x = Fix64.Zero;
			this.y.y = Fix64.One;
		}

		public void Transpose()
		{
			Fix64 m00 = this.x.x;
			Fix64 m01 = this.y.x;
			Fix64 m10 = this.x.y;
			Fix64 m11 = this.y.y;
			this.x.x = m00;
			this.x.y = m01;
			this.y.x = m10;
			this.y.y = m11;
		}

		public Fix64 Determinant()
		{
			return this.x.x * this.y.y - this.x.y * this.y.x;
		}

		public void Invert()
		{
			Fix64 determinant = Fix64.One / ( this.x.x * this.y.y - this.x.y * this.y.x );
			Fix64 m00 = this.y.y * determinant;
			Fix64 m01 = -this.x.y * determinant;
			Fix64 m10 = -this.y.x * determinant;
			Fix64 m11 = this.x.x * determinant;
			this.x.x = m00;
			this.x.y = m01;
			this.y.x = m10;
			this.y.y = m11;
		}

		#endregion
	}
}