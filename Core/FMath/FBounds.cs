namespace Core.FMath
{
	public struct FBounds
	{
		public enum Axis
		{
			X, Y, Z
		}

		private FVec3 _center;
		private FVec3 _extends;

		/// <summary>
		///   <para>The center of the bounding box.</para>
		/// </summary>
		public FVec3 center
		{
			get => this._center;
			set => this._center = value;
		}

		/// <summary>
		///   <para>The total size of the box. This is always twice as large as the extents.</para>
		/// </summary>
		public FVec3 size
		{
			get => this._extends * Fix64.Two;
			set => this._extends = value * Fix64.Half;
		}

		/// <summary>
		///   <para>The extents of the box. This is always half of the size.</para>
		/// </summary>
		public FVec3 extents
		{
			get => this._extends;
			set => this._extends = value;
		}

		/// <summary>
		///   <para>The minimal point of the box. This is always equal to center-extents.</para>
		/// </summary>
		public FVec3 min
		{
			get => this.center - this.extents;
			set => this.SetMinMax( value, this.max );
		}

		/// <summary>
		///   <para>The maximal point of the box. This is always equal to center+extents.</para>
		/// </summary>
		public FVec3 max
		{
			get => this.center + this.extents;
			set => this.SetMinMax( this.min, value );
		}

		public FBounds( FVec3 center, FVec3 size )
		{
			this._center = center;
			this._extends = size * Fix64.Half;
		}

		/// <summary>
		///   <para>Is point contained in the bounding box?</para>
		/// </summary>
		/// <param name="point"></param>
		public bool Contains( FVec3 point )
		{
			if ( point.x < this.min.x ||
				 point.y < this.min.y ||
				 point.z < this.min.z ||
				 point.x > this.max.x ||
				 point.y > this.max.y ||
				 point.z > this.max.z )
				return false;
			return true;
		}

		public FVec3 ClosestPoint( FVec3 point )
		{
			Fix64 distance;
			return this.ClosestPoint( point, out distance );
		}

		/// <summary>
		///   <para>The closest point on the bounding box.</para>
		/// </summary>
		/// <param name="point">Arbitrary point.</param>
		/// <param name="distance"></param>
		/// <returns>
		///   <para>The point on the bounding box || inside the bounding box.</para>
		/// </returns>
		public FVec3 ClosestPoint( FVec3 point, out Fix64 distance )
		{
			distance = Fix64.Zero;

			FVec3 t = point - this.center;
			FVec3 closest = new FVec3( t.x, t.y, t.z );
			FVec3 et = this.extents;
			Fix64[] extent = { et.x, et.y, et.z };

			for ( int i = 0; i < 3; i++ )
			{
				Fix64 delta;
				if ( closest[i] < -extent[i] )
				{
					delta = closest[i] + extent[i];
					distance = distance + delta * delta;
					closest[i] = -extent[i];
				}
				else if ( closest[i] > extent[i] )
				{
					delta = closest[i] - extent[i];
					distance = distance + delta * delta;
					closest[i] = extent[i];
				}
			}

			if ( distance == Fix64.Zero )
				return point;
			return closest + this.center;
		}

		/// <summary>
		///   <para>Sets the bounds to the min and max value of the box.</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public void SetMinMax( FVec3 min, FVec3 max )
		{
			this.extents = ( max - min ) * Fix64.Half;
			this.center = min + this.extents;
		}

		/// <summary>
		///   <para>Grows the FBounds to include the point.</para>
		/// </summary>
		/// <param name="point"></param>
		public void Encapsulate( FVec3 point )
		{
			this.SetMinMax( FVec3.Min( this.min, point ), FVec3.Max( this.max, point ) );
		}

		/// <summary>
		///   <para>Grow the bounds to encapsulate the bounds.</para>
		/// </summary>
		/// <param name="bounds"></param>
		public void Encapsulate( FBounds bounds )
		{
			this.Encapsulate( bounds.center - bounds.extents );
			this.Encapsulate( bounds.center + bounds.extents );
		}

		/// <summary>
		///   <para>Expand the bounds by increasing its size by amount along each side.</para>
		/// </summary>
		/// <param name="amount"></param>
		public void Expand( Fix64 amount )
		{
			amount *= Fix64.Half;
			this.extents += new FVec3( amount, amount, amount );
		}

		/// <summary>
		///   <para>Expand the bounds by increasing its size by amount along each side.</para>
		/// </summary>
		/// <param name="amount"></param>
		public void Expand( FVec3 amount )
		{
			this.extents += amount * Fix64.Half;
		}

		/// <summary>
		///   <para>Does another bounding box intersect with this bounding box?</para>
		/// </summary>
		/// <param name="bounds"></param>
		public bool Intersect( FBounds bounds )
		{
			return this.min.x <= bounds.max.x && this.max.x >= bounds.min.x && this.min.y <= bounds.max.y && this.max.y >= bounds.min.y && this.min.z <= bounds.max.z && this.max.z >= bounds.min.z;
		}

		public bool Intersect( FBounds bounds, ref FBounds boundsIntersect )
		{
			if ( this.min.x > bounds.max.x ) return false;
			if ( this.max.x < bounds.min.x ) return false;
			if ( this.min.y > bounds.max.y ) return false;
			if ( this.max.y < bounds.min.y ) return false;
			if ( this.min.z > bounds.max.z ) return false;
			if ( this.max.z < bounds.min.z ) return false;

			boundsIntersect.min = FVec3.Max( this.min, bounds.min );
			boundsIntersect.max = FVec3.Max( this.max, bounds.max );

			return true;
		}

		public bool IntersectMovingBoundsByAxis( FBounds bounds, Fix64 d, Axis axis, out Fix64 t )
		{
			if ( d == Fix64.Zero )
			{
				t = Fix64.MaxValue;
				return false;
			}
			Fix64 min00, max00, min01, max01;
			Fix64 min10, max10, min11, max11;
			Fix64 min20, max20, min21, max21;
			switch ( axis )
			{
				case Axis.X:
					min10 = this.min.y;
					max10 = this.max.y;
					min11 = bounds.min.y;
					max11 = bounds.max.y;
					min20 = this.min.z;
					max20 = this.max.z;
					min21 = bounds.min.z;
					max21 = bounds.max.z;
					break;

				case Axis.Y:
					min10 = this.min.z;
					max10 = this.max.z;
					min11 = bounds.min.z;
					max11 = bounds.max.z;
					min20 = this.min.x;
					max20 = this.max.x;
					min21 = bounds.min.x;
					max21 = bounds.max.x;
					break;

				default:
					min10 = this.min.x;
					max10 = this.max.x;
					min11 = bounds.min.x;
					max11 = bounds.max.x;
					min20 = this.min.y;
					max20 = this.max.y;
					min21 = bounds.min.y;
					max21 = bounds.max.y;
					break;
			}
			if ( min10 >= max11 ||
				 max10 <= min11 ||
				 min20 >= max21 ||
				 max20 <= min21 )
			{
				t = Fix64.MaxValue;
				return false;
			}
			switch ( axis )
			{
				case Axis.X:
					min00 = this.min.x;
					max00 = this.max.x;
					min01 = bounds.min.x;
					max01 = bounds.max.x;
					break;

				case Axis.Y:
					min00 = this.min.y;
					max00 = this.max.y;
					min01 = bounds.min.y;
					max01 = bounds.max.y;
					break;

				default:
					min00 = this.min.z;
					max00 = this.max.z;
					min01 = bounds.min.z;
					max01 = bounds.max.z;
					break;
			}
			Fix64 oneOverD = Fix64.One / d;
			Fix64 enter = ( min00 - max01 ) * oneOverD;
			Fix64 leave = ( max00 - min01 ) * oneOverD;

			if ( enter > leave )
				Fix64.Swap( ref enter, ref leave );

			t = enter;
			return enter < Fix64.One && enter >= Fix64.Zero;
		}

		public Fix64 IntersectMovingBounds( FBounds bounds, FVec3 d )
		{
			Fix64 tEnter = -Fix64.MaxValue;
			Fix64 tLeave = Fix64.MaxValue;

			if ( d.x == Fix64.Zero )
			{
				if ( this.min.x >= bounds.max.x ||
					 this.max.x <= bounds.min.x )
					return Fix64.MaxValue;
			}
			else
			{
				Fix64 oneOverD = Fix64.One / d.x;
				Fix64 xEnter = ( this.min.x - bounds.max.x ) * oneOverD;
				Fix64 xLeave = ( this.max.x - bounds.min.x ) * oneOverD;

				if ( xEnter > xLeave )
					Fix64.Swap( ref xEnter, ref xLeave );

				if ( xEnter > tEnter ) tEnter = xEnter;
				if ( xLeave < tLeave ) tLeave = xLeave;
				if ( tEnter > tLeave )
					return Fix64.MaxValue;
			}

			if ( d.y == Fix64.Zero )
			{
				if ( this.min.y >= bounds.max.y ||
					 this.max.y <= bounds.min.y )
					return Fix64.MaxValue;
			}
			else
			{
				Fix64 oneOverD = Fix64.One / d.y;
				Fix64 yEnter = ( this.min.y - bounds.max.y ) * oneOverD;
				Fix64 yLeave = ( this.max.y - bounds.min.y ) * oneOverD;

				if ( yEnter > yLeave )
					Fix64.Swap( ref yEnter, ref yLeave );

				if ( yEnter > tEnter ) tEnter = yEnter;
				if ( yLeave < tLeave ) tLeave = yLeave;
				if ( tEnter > tLeave )
					return Fix64.MaxValue;
			}

			if ( d.z == Fix64.Zero )
			{
				if ( this.min.z >= bounds.max.z ||
					 this.max.z <= bounds.min.z )
					return Fix64.MaxValue;
			}
			else
			{
				Fix64 oneOverD = Fix64.One / d.z;
				Fix64 zEnter = ( this.min.z - bounds.max.z ) * oneOverD;
				Fix64 zLeave = ( this.max.z - bounds.min.z ) * oneOverD;

				if ( zEnter > zLeave )
					Fix64.Swap( ref zEnter, ref zLeave );

				if ( zEnter > tEnter ) tEnter = zEnter;
				if ( zLeave < tLeave ) tLeave = zLeave;
				if ( tEnter > tLeave )
					return Fix64.MaxValue;
			}
			return tEnter;
		}

		public bool Intersect( FRay ray, Fix64 distance, out Fix64 parametric, out FVec3 normal )
		{
			parametric = Fix64.NegativeOne;
			normal = FVec3.zero;

			bool inside = true;
			FVec3 rayDelta = ray.direction * distance;

			Fix64 xt, xn = Fix64.Zero;
			if ( ray.origin.x < this.min.x )
			{
				xt = this.min.x - ray.origin.x;
				if ( xt > rayDelta.x ) return false;
				xt /= rayDelta.x;
				inside = false;
				xn = -Fix64.One;
			}
			else if ( ray.origin.x > this.max.x )
			{
				xt = this.max.x - ray.origin.x;
				if ( xt < rayDelta.x ) return false;
				xt /= rayDelta.x;
				inside = false;
				xn = Fix64.One;
			}
			else
			{
				xt = -Fix64.One;
			}

			Fix64 yt, yn = Fix64.Zero;
			if ( ray.origin.y < this.min.y )
			{
				yt = this.min.y - ray.origin.y;
				if ( yt > rayDelta.y ) return false;
				yt /= rayDelta.y;
				inside = false;
				yn = -Fix64.One;
			}
			else if ( ray.origin.y > this.max.y )
			{
				yt = this.max.y - ray.origin.y;
				if ( yt < rayDelta.y ) return false;
				yt /= rayDelta.y;
				inside = false;
				yn = Fix64.One;
			}
			else
			{
				yt = -Fix64.One;
			}

			Fix64 zt, zn = Fix64.Zero;
			if ( ray.origin.z < this.min.z )
			{
				zt = this.min.z - ray.origin.z;
				if ( zt > rayDelta.z ) return false;
				zt /= rayDelta.z;
				inside = false;
				zn = -Fix64.One;
			}
			else if ( ray.origin.z > this.max.z )
			{
				zt = this.max.z - ray.origin.z;
				if ( zt < rayDelta.z ) return false;
				zt /= rayDelta.z;
				inside = false;
				zn = Fix64.One;
			}
			else
			{
				zt = -Fix64.One;
			}

			// Inside box?
			if ( inside )
			{
				normal = -FVec3.Normalize( rayDelta );
				parametric = Fix64.Zero;
				return true;
			}

			// Select farthest plane - this is
			// the plane of intersection.
			int which = 0;
			Fix64 t = xt;
			if ( yt > t )
			{
				which = 1;
				t = yt;
			}
			if ( zt > t )
			{
				which = 2;
				t = zt;
			}

			switch ( which )
			{
				case 0: // intersect with yz plane
					{
						Fix64 y = ray.origin.y + rayDelta.y * t;
						if ( y < this.min.y || y > this.max.y ) return false;
						Fix64 z = ray.origin.z + rayDelta.z * t;
						if ( z < this.min.z || z > this.max.z ) return false;

						normal.x = xn;
						normal.y = Fix64.Zero;
						normal.z = Fix64.Zero;

					}
					break;

				case 1: // intersect with xz plane
					{
						Fix64 x = ray.origin.x + rayDelta.x * t;
						if ( x < this.min.x || x > this.max.x ) return false;
						Fix64 z = ray.origin.z + rayDelta.z * t;
						if ( z < this.min.z || z > this.max.z ) return false;

						normal.x = Fix64.Zero;
						normal.y = yn;
						normal.z = Fix64.Zero;

					}
					break;

				case 2: // intersect with xy plane
					{
						Fix64 x = ray.origin.x + rayDelta.x * t;
						if ( x < this.min.x || x > this.max.x ) return false;
						Fix64 y = ray.origin.y + rayDelta.y * t;
						if ( y < this.min.y || y > this.max.y ) return false;

						normal.x = Fix64.Zero;
						normal.y = Fix64.Zero;
						normal.z = zn;

					}
					break;
			}
			parametric = t;
			return t >= Fix64.Zero && t <= Fix64.One;
		}

		/// <summary>
		///   <para>Does ray intersect this bounding box?</para>
		/// </summary>
		/// <param name="ray"></param>
		public bool Intersect( FRay ray )
		{
			Fix64 num;
			return this.Intersect( ray, out num );
		}

		public bool Intersect( FRay ray, out Fix64 distance )
		{
			Fix64 tmin = Fix64.MinValue;
			Fix64 tmax = Fix64.MaxValue;

			distance = tmin;

			FVec3 t = this.center - ray.origin;
			Fix64[] p = { t.x, t.y, t.z };
			t = this.extents;
			Fix64[] extent = { t.x, t.y, t.z };
			t = ray.direction;
			Fix64[] dir = { t.x, t.y, t.z };

			for ( int i = 0; i < 3; i++ )
			{
				Fix64 f = Fix64.One / dir[i];
				Fix64 t0 = ( p[i] + extent[i] ) * f;
				Fix64 t1 = ( p[i] - extent[i] ) * f;

				if ( t0 < t1 )
				{
					if ( t0 > tmin ) tmin = t0;
					if ( t1 < tmax ) tmax = t1;
					if ( tmin > tmax ) return false;
					if ( tmax < Fix64.Zero ) return false;
				}
				else
				{
					if ( t1 > tmin ) tmin = t1;
					if ( t0 < tmax ) tmax = t0;
					if ( tmin > tmax ) return false;
					if ( tmax < Fix64.Zero ) return false;
				}
			}
			distance = tmin;
			return true;
		}

		public bool Intersect( FSphere sphere )
		{
			FVec3 closestPoint = this.ClosestPoint( sphere.center );

			return FVec3.DistanceSquared( sphere.center, closestPoint ) < sphere.radius * sphere.radius;
		}

		public override int GetHashCode()
		{
			return this.center.GetHashCode() ^ this.extents.GetHashCode() << 2;
		}

		public override bool Equals( object other )
		{
			bool result;
			if ( !( other is FBounds ) )
			{
				result = false;
			}
			else
			{
				FBounds bounds = ( FBounds )other;
				result = ( this.center.Equals( bounds.center ) && this.extents.Equals( bounds.extents ) );
			}
			return result;
		}

		public static bool operator ==( FBounds lhs, FBounds rhs )
		{
			return lhs.center == rhs.center && lhs.extents == rhs.extents;
		}

		public static bool operator !=( FBounds lhs, FBounds rhs )
		{
			return !( lhs == rhs );
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for the bounds.</para>
		/// </summary>
		public override string ToString()
		{
			return string.Format( "Center: {0}, Extents: {1}", new object[]
			{
				this._center,
				this._extends
			} );
		}
	}
}