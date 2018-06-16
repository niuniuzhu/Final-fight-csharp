namespace Core.Math
{
	public struct Mat2
	{
		public Vec2 x;
		public Vec2 y;

		public Mat2( Vec2 x, Vec2 y )
		{
			this.x = x;
			this.y = y;
		}

		public static Mat2 FromCross( Vec2 xVector )
		{
			return new Mat2( xVector, new Vec2( -xVector.y, xVector.x ) );
		}

		public static Mat2 Abs( Mat2 m )
		{
			return new Mat2( Vec2.Abs( m.x ), Vec2.Abs( m.y ) );
		}

		public static Mat2 Transpose( Mat2 m )
		{
			return new Mat2
			(
				new Vec2( m.x.x, m.y.x ),
				new Vec2( m.x.y, m.y.y )
			);
		}

		public static Mat2 Invert( Mat2 m )
		{
			float determinant = 1 / ( m.x.x * m.y.y - m.x.y * m.y.x );
			Mat2 result;
			result.x.x = m.y.y * determinant;
			result.x.y = -m.x.y * determinant;
			result.y.x = -m.y.x * determinant;
			result.y.y = m.x.x * determinant;
			return result;
		}

		public static readonly Mat2 IDENTITY = new Mat2
			(
			new Vec2( 1, 0 ),
			new Vec2( 0, 1 )
			);

		#region Operators

		public static Mat2 operator +( Mat2 p1, Mat2 p2 )
		{
			p1.x += p2.x;
			p1.y += p2.y;
			return p1;
		}

		public static Mat2 operator +( Mat2 p1, float p2 )
		{
			p1.x += p2;
			p1.y += p2;
			return p1;
		}

		public static Mat2 operator +( float p1, Mat2 p2 )
		{
			p2.x = p1 + p2.x;
			p2.y = p1 + p2.y;
			return p2;
		}

		public static Mat2 operator -( Mat2 p1, Mat2 p2 )
		{
			p1.x -= p2.x;
			p1.y -= p2.y;
			return p1;
		}

		public static Mat2 operator -( Mat2 p1, float p2 )
		{
			p1.x -= p2;
			p1.y -= p2;
			return p1;
		}

		public static Mat2 operator -( float p1, Mat2 p2 )
		{
			p2.x = p1 - p2.x;
			p2.y = p1 - p2.y;
			return p2;
		}

		public static Mat2 operator -( Mat2 p2 )
		{
			p2.x = -p2.x;
			p2.y = -p2.y;
			return p2;
		}

		public static Mat2 operator *( Mat2 m1, Mat2 m2 )
		{
			return new Mat2
			(
				new Vec2( m1.x.x * m2.x.x + m1.x.y * m2.y.x,
				          m1.x.x * m2.x.y + m1.x.y * m2.y.y ),
				new Vec2( m1.y.x * m2.x.x + m1.y.y * m2.y.x,
				          m1.y.x * m2.x.y + m1.y.y * m2.y.y )
			);
		}

		public static Vec2 operator *( Vec2 v, Mat2 m )
		{
			return m.Transform( v );
		}

		public static Mat2 operator *( Mat2 p1, float p2 )
		{
			p1.x *= p2;
			p1.y *= p2;
			return p1;
		}

		public static Mat2 operator *( float p1, Mat2 p2 )
		{
			p2.x = p1 * p2.x;
			p2.y = p1 * p2.y;
			return p2;
		}

		public static Mat2 operator /( Mat2 p1, Mat2 p2 )
		{
			p1.x /= p2.x;
			p1.y /= p2.y;
			return p1;
		}

		public static Mat2 operator /( Mat2 p1, float p2 )
		{
			p1.x /= p2;
			p1.y /= p2;
			return p1;
		}

		public static Mat2 operator /( float p1, Mat2 p2 )
		{
			p2.x = p1 / p2.x;
			p2.y = p1 / p2.y;
			return p2;
		}

		public static bool operator ==( Mat2 p1, Mat2 p2 )
		{
			return p1.x == p2.x && p1.y == p2.y;
		}

		public static bool operator !=( Mat2 p1, Mat2 p2 )
		{
			return p1.x != p2.x || p1.y != p2.y;
		}

		#endregion

		#region Methods

		public override bool Equals( object obj )
		{
			return obj != null && ( Mat2 )obj == this;
		}

		public override string ToString()
		{
			return $"(x:{this.x}\ny:{this.y})";
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Vec2 Transform( Vec2 v )
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
			this.y.x = 0;
			this.y.y = 1;
		}

		public void Transpose()
		{
			float m00 = this.x.x;
			float m01 = this.y.x;
			float m10 = this.x.y;
			float m11 = this.y.y;
			this.x.x = m00;
			this.x.y = m01;
			this.y.x = m10;
			this.y.y = m11;
		}

		public float Determinant()
		{
			return this.x.x * this.y.y - this.x.y * this.y.x;
		}

		public void Invert()
		{
			float determinant = 1 / ( this.x.x * this.y.y - this.x.y * this.y.x );
			float m00 = this.y.y * determinant;
			float m01 = -this.x.y * determinant;
			float m10 = -this.y.x * determinant;
			float m11 = this.x.x * determinant;
			this.x.x = m00;
			this.x.y = m01;
			this.y.x = m10;
			this.y.y = m11;
		}

		#endregion
	}
}