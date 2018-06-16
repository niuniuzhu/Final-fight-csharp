namespace Core.Math
{
	/// <summary>
	///   <para>A 2D Rectangle defined by X and Y position, width and height.</para>
	/// </summary>
	public struct Rect
	{
		private float _xMin;

		private float _yMin;

		private float _width;

		private float _height;

		/// <summary>
		///   <para>Shorthand for writing new Rect(0,0,0,0).</para>
		/// </summary>
		public static Rect zero => new Rect( 0f, 0f, 0f, 0f );

		/// <summary>
		///   <para>The X coordinate of the rectangle.</para>
		/// </summary>
		public float x
		{
			get => this._xMin;
			set => this._xMin = value;
		}

		/// <summary>
		///   <para>The Y coordinate of the rectangle.</para>
		/// </summary>
		public float y
		{
			get => this._yMin;
			set => this._yMin = value;
		}

		/// <summary>
		///   <para>The X and Y position of the rectangle.</para>
		/// </summary>
		public Vec2 position
		{
			get => new Vec2( this._xMin, this._yMin );
			set
			{
				this._xMin = value.x;
				this._yMin = value.y;
			}
		}

		/// <summary>
		///   <para>The position of the center of the rectangle.</para>
		/// </summary>
		public Vec2 center
		{
			get => new Vec2( this.x + this._width / 2f, this.y + this._height / 2f );
			set
			{
				this._xMin = value.x - this._width / 2f;
				this._yMin = value.y - this._height / 2f;
			}
		}

		/// <summary>
		///   <para>The position of the minimum corner of the rectangle.</para>
		/// </summary>
		public Vec2 min
		{
			get => new Vec2( this.xMin, this.yMin );
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
			}
		}

		/// <summary>
		///   <para>The position of the maximum corner of the rectangle.</para>
		/// </summary>
		public Vec2 max
		{
			get => new Vec2( this.xMax, this.yMax );
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
			}
		}

		/// <summary>
		///   <para>The width of the rectangle, measured from the X position.</para>
		/// </summary>
		public float width
		{
			get => this._width;
			set => this._width = value;
		}

		/// <summary>
		///   <para>The height of the rectangle, measured from the Y position.</para>
		/// </summary>
		public float height
		{
			get => this._height;
			set => this._height = value;
		}

		/// <summary>
		///   <para>The width and height of the rectangle.</para>
		/// </summary>
		public Vec2 size
		{
			get => new Vec2( this._width, this._height );
			set
			{
				this._width = value.x;
				this._height = value.y;
			}
		}

		/// <summary>
		///   <para>The minimum X coordinate of the rectangle.</para>
		/// </summary>
		public float xMin
		{
			get => this._xMin;
			set
			{
				float m = this.xMax;
				this._xMin = value;
				this._width = m - this._xMin;
			}
		}

		/// <summary>
		///   <para>The minimum Y coordinate of the rectangle.</para>
		/// </summary>
		public float yMin
		{
			get => this._yMin;
			set
			{
				float m = this.yMax;
				this._yMin = value;
				this._height = m - this._yMin;
			}
		}

		/// <summary>
		///   <para>The maximum X coordinate of the rectangle.</para>
		/// </summary>
		public float xMax
		{
			get => this._width + this._xMin;
			set => this._width = value - this._xMin;
		}

		/// <summary>
		///   <para>The maximum Y coordinate of the rectangle.</para>
		/// </summary>
		public float yMax
		{
			get => this._height + this._yMin;
			set => this._height = value - this._yMin;
		}

		/// <summary>
		///   <para>Creates a new rectangle.</para>
		/// </summary>
		/// <param name="x">The X value the rect is measured from.</param>
		/// <param name="y">The Y value the rect is measured from.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public Rect( float x, float y, float width, float height )
		{
			this._xMin = x;
			this._yMin = y;
			this._width = width;
			this._height = height;
		}

		/// <summary>
		///   <para>Creates a rectangle given a size and position.</para>
		/// </summary>
		/// <param name="position">The position of the minimum corner of the rect.</param>
		/// <param name="size">The width and height of the rect.</param>
		public Rect( Vec2 position, Vec2 size )
		{
			this._xMin = position.x;
			this._yMin = position.y;
			this._width = size.x;
			this._height = size.y;
		}

		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="source"></param>
		public Rect( Rect source )
		{
			this._xMin = source._xMin;
			this._yMin = source._yMin;
			this._width = source._width;
			this._height = source._height;
		}

		/// <summary>
		///   <para>Creates a rectangle from min/max coordinate values.</para>
		/// </summary>
		/// <param name="xmin">The minimum X coordinate.</param>
		/// <param name="ymin">The minimum Y coordinate.</param>
		/// <param name="xmax">The maximum X coordinate.</param>
		/// <param name="ymax">The maximum Y coordinate.</param>
		/// <returns>
		///   <para>A rectangle matching the specified coordinates.</para>
		/// </returns>
		public static Rect MinMaxRect( float xmin, float ymin, float xmax, float ymax )
		{
			return new Rect( xmin, ymin, xmax - xmin, ymax - ymin );
		}

		/// <summary>
		///   <para>Set components of an existing Rect.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void Set( float x, float y, float width, float height )
		{
			this._xMin = x;
			this._yMin = y;
			this._width = width;
			this._height = height;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains( Vec2 point )
		{
			return point.x >= this.xMin && point.x < this.xMax && point.y >= this.yMin && point.y < this.yMax;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains( Vec3 point )
		{
			return point.x >= this.xMin && point.x < this.xMax && point.y >= this.yMin && point.y < this.yMax;
		}

		/// <summary>
		///   <para>Returns true if the x and y components of point is a point inside this rectangle. If allowInverse is present and true, the width and height of the Rect are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="point">Point to test.</param>
		/// <param name="allowInverse">Does the test allow the Rect's width and height to be negative?</param>
		/// <returns>
		///   <para>True if the point lies within the specified rectangle.</para>
		/// </returns>
		public bool Contains( Vec3 point, bool allowInverse )
		{
			bool result;
			if ( !allowInverse )
			{
				result = this.Contains( point );
			}
			else
			{
				bool flag = ( this.width < 0f && point.x <= this.xMin && point.x > this.xMax ) || ( this.width >= 0f && point.x >= this.xMin && point.x < this.xMax );
				result = ( flag && ( ( this.height < 0f && point.y <= this.yMin && point.y > this.yMax ) || ( this.height >= 0f && point.y >= this.yMin && point.y < this.yMax ) ) );
			}
			return result;
		}

		private static Rect OrderMinMax( Rect rect )
		{
			if ( rect.xMin > rect.xMax )
			{
				float xMin = rect.xMin;
				rect.xMin = rect.xMax;
				rect.xMax = xMin;
			}
			if ( rect.yMin > rect.yMax )
			{
				float yMin = rect.yMin;
				rect.yMin = rect.yMax;
				rect.yMax = yMin;
			}
			return rect;
		}

		/// <summary>
		///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		public bool Overlaps( Rect other )
		{
			return other.xMax > this.xMin && other.xMin < this.xMax && other.yMax > this.yMin && other.yMin < this.yMax;
		}

		/// <summary>
		///   <para>Returns true if the other rectangle overlaps this one. If allowInverse is present and true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		/// <param name="allowInverse">Does the test allow the widths and heights of the Rects to be negative?</param>
		public bool Overlaps( Rect other, bool allowInverse )
		{
			Rect rect = this;
			if ( allowInverse )
			{
				rect = OrderMinMax( rect );
				other = OrderMinMax( other );
			}
			return rect.Overlaps( other );
		}

		/// <summary>
		///   <para>Returns a point inside a rectangle, given normalized coordinates.</para>
		/// </summary>
		/// <param name="rectangle">Rectangle to get a point inside.</param>
		/// <param name="normalizedRectCoordinates">Normalized coordinates to get a point for.</param>
		public static Vec2 NormalizedToPoint( Rect rectangle, Vec2 normalizedRectCoordinates )
		{
			return new Vec2( MathUtils.Lerp( rectangle.x, rectangle.xMax, normalizedRectCoordinates.x ), MathUtils.Lerp( rectangle.y, rectangle.yMax, normalizedRectCoordinates.y ) );
		}

		/// <summary>
		///   <para>Returns the normalized coordinates cooresponding the the point.</para>
		/// </summary>
		/// <param name="rectangle">Rectangle to get normalized coordinates inside.</param>
		/// <param name="point">A point inside the rectangle to get normalized coordinates for.</param>
		public static Vec2 PointToNormalized( Rect rectangle, Vec2 point )
		{
			return new Vec2( MathUtils.InverseLerp( rectangle.x, rectangle.xMax, point.x ), MathUtils.InverseLerp( rectangle.y, rectangle.yMax, point.y ) );
		}

		public static bool operator !=( Rect lhs, Rect rhs )
		{
			return !( lhs == rhs );
		}

		public static bool operator ==( Rect lhs, Rect rhs )
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.width.GetHashCode() << 2 ^ this.y.GetHashCode() >> 2 ^ this.height.GetHashCode() >> 1;
		}

		public override bool Equals( object other )
		{
			bool result;
			if ( !( other is Rect ) )
			{
				result = false;
			}
			else
			{
				Rect rect = ( Rect ) other;
				result = ( this.x.Equals( rect.x ) && this.y.Equals( rect.y ) && this.width.Equals( rect.width ) && this.height.Equals( rect.height ) );
			}
			return result;
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for this Rect.</para>
		/// </summary>
		public override string ToString()
		{
			return $"(x:{this.x:F2}, y:{this.y:F2}, width:{this.width:F2}, height:{this.height:F2})";
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for this Rect.</para>
		/// </summary>
		/// <param name="format"></param>
		public string ToString( string format )
		{
			return
				$"(x:{this.x.ToString( format )}, y:{this.y.ToString( format )}, width:{this.width.ToString( format )}, height:{this.height.ToString( format )})";
		}
	}
}
