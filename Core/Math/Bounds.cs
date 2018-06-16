namespace Core.Math
{
	public struct Bounds
	{
		public enum Axis
		{
			X, Y, Z
		}

		private Vec3 _center;
		private Vec3 _extents;

		/// <summary>
		///   <para>The center of the bounding box.</para>
		/// </summary>
		public Vec3 center
		{
			get => this._center;
			set => this._center = value;
		}

		/// <summary>
		///   <para>The total size of the box. This is always twice as large as the extents.</para>
		/// </summary>
		public Vec3 size
		{
			get => this._extents * 2f;
			set => this._extents = value * 0.5f;
		}

		/// <summary>
		///   <para>The extents of the box. This is always half of the size.</para>
		/// </summary>
		public Vec3 extents
		{
			get => this._extents;
			set => this._extents = value;
		}

		/// <summary>
		///   <para>The minimal point of the box. This is always equal to center-extents.</para>
		/// </summary>
		public Vec3 min
		{
			get => this.center - this.extents;
			set => this.SetMinMax( value, this.max );
		}

		/// <summary>
		///   <para>The maximal point of the box. This is always equal to center+extents.</para>
		/// </summary>
		public Vec3 max
		{
			get => this.center + this.extents;
			set => this.SetMinMax( this.min, value );
		}

		public Bounds( Vec3 center, Vec3 size )
		{
			this._center = center;
			this._extents = size * 0.5f;
		}

		/// <summary>
		///   <para>Is point contained in the bounding box?</para>
		/// </summary>
		/// <param name="point"></param>
		public bool Contains( Vec3 point )
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

		public Vec3 ClosestPoint( Vec3 point )
		{
			float distance;
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
		public Vec3 ClosestPoint( Vec3 point, out float distance )
		{
			distance = 0;

			Vec3 t = point - this.center;
			Vec3 closest = new Vec3( t.x, t.y, t.z );
			for ( int i = 0; i < 3; i++ )
			{
				float delta;
				float f = this.extents[i];
				if ( closest[i] < -f )
				{
					delta = closest[i] + f;
					distance = distance + delta * delta;
					closest[i] = -f;
				}
				else if ( closest[i] > f )
				{
					delta = closest[i] - f;
					distance = distance + delta * delta;
					closest[i] = f;
				}
			}

			if ( distance == 0 )
				return point;
			return closest + this.center;
		}

		/// <summary>
		///   <para>Sets the bounds to the min and max value of the box.</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public void SetMinMax( Vec3 min, Vec3 max )
		{
			this.extents = ( max - min ) * 0.5f;
			this.center = min + this.extents;
		}

		/// <summary>
		///   <para>Grows the Bounds to include the point.</para>
		/// </summary>
		/// <param name="point"></param>
		public void Encapsulate( Vec3 point )
		{
			this.SetMinMax( Vec3.Min( this.min, point ), Vec3.Max( this.max, point ) );
		}

		/// <summary>
		///   <para>Grow the bounds to encapsulate the bounds.</para>
		/// </summary>
		/// <param name="bounds"></param>
		public void Encapsulate( Bounds bounds )
		{
			this.Encapsulate( bounds.center - bounds.extents );
			this.Encapsulate( bounds.center + bounds.extents );
		}

		/// <summary>
		///   <para>Expand the bounds by increasing its size by amount along each side.</para>
		/// </summary>
		/// <param name="amount"></param>
		public void Expand( float amount )
		{
			amount *= 0.5f;
			this.extents += new Vec3( amount, amount, amount );
		}

		/// <summary>
		///   <para>Expand the bounds by increasing its size by amount along each side.</para>
		/// </summary>
		/// <param name="amount"></param>
		public void Expand( Vec3 amount )
		{
			this.extents += amount * 0.5f;
		}

		/// <summary>
		///   <para>Does another bounding box intersect with this bounding box?</para>
		/// </summary>
		/// <param name="bounds"></param>
		public bool Intersect( Bounds bounds )
		{
			return this.min.x <= bounds.max.x && this.max.x >= bounds.min.x && this.min.y <= bounds.max.y && this.max.y >= bounds.min.y && this.min.z <= bounds.max.z && this.max.z >= bounds.min.z;
		}

		public bool Intersect( Bounds bounds, ref Bounds boundsIntersect )
		{
			if ( this.min.x > bounds.max.x ) return false;
			if ( this.max.x < bounds.min.x ) return false;
			if ( this.min.y > bounds.max.y ) return false;
			if ( this.max.y < bounds.min.y ) return false;
			if ( this.min.z > bounds.max.z ) return false;
			if ( this.max.z < bounds.min.z ) return false;

			boundsIntersect.min = Vec3.Max( this.min, bounds.min );
			boundsIntersect.max = Vec3.Max( this.max, bounds.max );

			return true;
		}

		public bool IntersectMovingBoundsByAxis( Bounds bounds, float d, Axis axis, out float t )
		{
			if ( MathUtils.Approximately( d, 0.0f ) )
			{
				t = MathUtils.MAX_VALUE;
				return false;
			}
			float min00, max00, min01, max01;
			float min10, max10, min11, max11;
			float min20, max20, min21, max21;
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
				t = MathUtils.MAX_VALUE;
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
			float oneOverD = 1.0f / d;
			float enter = ( min00 - max01 ) * oneOverD;
			float leave = ( max00 - min01 ) * oneOverD;

			if ( enter > leave )
				MathUtils.Swap( ref enter, ref leave );

			t = enter;
			return enter < 1.0f && enter >= 0f;
		}

		public float IntersectMovingBounds( Bounds bounds, Vec3 d )
		{
			float tEnter = -MathUtils.MAX_VALUE;
			float tLeave = MathUtils.MAX_VALUE;

			if ( MathUtils.Approximately( d.x, 0.0f ) )
			{
				if ( this.min.x >= bounds.max.x ||
					 this.max.x <= bounds.min.x )
					return MathUtils.MAX_VALUE;
			}
			else
			{
				float oneOverD = 1.0f / d.x;
				float xEnter = ( this.min.x - bounds.max.x ) * oneOverD;
				float xLeave = ( this.max.x - bounds.min.x ) * oneOverD;

				if ( xEnter > xLeave )
					MathUtils.Swap( ref xEnter, ref xLeave );

				if ( xEnter > tEnter ) tEnter = xEnter;
				if ( xLeave < tLeave ) tLeave = xLeave;
				if ( tEnter > tLeave )
					return MathUtils.MAX_VALUE;
			}

			if ( MathUtils.Approximately( d.y, 0.0f ) )
			{
				if ( this.min.y >= bounds.max.y ||
					 this.max.y <= bounds.min.y )
					return MathUtils.MAX_VALUE;
			}
			else
			{
				float oneOverD = 1.0f / d.y;
				float yEnter = ( this.min.y - bounds.max.y ) * oneOverD;
				float yLeave = ( this.max.y - bounds.min.y ) * oneOverD;

				if ( yEnter > yLeave )
					MathUtils.Swap( ref yEnter, ref yLeave );

				if ( yEnter > tEnter ) tEnter = yEnter;
				if ( yLeave < tLeave ) tLeave = yLeave;
				if ( tEnter > tLeave )
					return MathUtils.MAX_VALUE;
			}

			if ( MathUtils.Approximately( d.z, 0.0f ) )
			{
				if ( this.min.z >= bounds.max.z ||
					 this.max.z <= bounds.min.z )
					return MathUtils.MAX_VALUE;
			}
			else
			{
				float oneOverD = 1.0f / d.z;
				float zEnter = ( this.min.z - bounds.max.z ) * oneOverD;
				float zLeave = ( this.max.z - bounds.min.z ) * oneOverD;

				if ( zEnter > zLeave )
					MathUtils.Swap( ref zEnter, ref zLeave );

				if ( zEnter > tEnter ) tEnter = zEnter;
				if ( zLeave < tLeave ) tLeave = zLeave;
				if ( tEnter > tLeave )
					return MathUtils.MAX_VALUE;
			}
			return tEnter;
		}

		public bool Intersect( Ray ray, float distance, out float parametric, out Vec3 normal )
		{
			parametric = -1f;
			normal = Vec3.zero;

			bool inside = true;
			Vec3 rayDelta = ray.direction * distance;

			float xt, xn = 0f;
			if ( ray.origin.x < this.min.x )
			{
				xt = this.min.x - ray.origin.x;
				if ( xt > rayDelta.x ) return false;
				xt /= rayDelta.x;
				inside = false;
				xn = -1.0f;
			}
			else if ( ray.origin.x > this.max.x )
			{
				xt = this.max.x - ray.origin.x;
				if ( xt < rayDelta.x ) return false;
				xt /= rayDelta.x;
				inside = false;
				xn = 1.0f;
			}
			else
			{
				xt = -1.0f;
			}

			float yt, yn = 0f;
			if ( ray.origin.y < this.min.y )
			{
				yt = this.min.y - ray.origin.y;
				if ( yt > rayDelta.y ) return false;
				yt /= rayDelta.y;
				inside = false;
				yn = -1.0f;
			}
			else if ( ray.origin.y > this.max.y )
			{
				yt = this.max.y - ray.origin.y;
				if ( yt < rayDelta.y ) return false;
				yt /= rayDelta.y;
				inside = false;
				yn = 1.0f;
			}
			else
			{
				yt = -1.0f;
			}

			float zt, zn = 0f;
			if ( ray.origin.z < this.min.z )
			{
				zt = this.min.z - ray.origin.z;
				if ( zt > rayDelta.z ) return false;
				zt /= rayDelta.z;
				inside = false;
				zn = -1.0f;
			}
			else if ( ray.origin.z > this.max.z )
			{
				zt = this.max.z - ray.origin.z;
				if ( zt < rayDelta.z ) return false;
				zt /= rayDelta.z;
				inside = false;
				zn = 1.0f;
			}
			else
			{
				zt = -1.0f;
			}

			// Inside box?
			if ( inside )
			{
				normal = -Vec3.Normalize( rayDelta );
				parametric = 0f;
				return true;
			}

			// Select farthest plane - this is
			// the plane of intersection.
			int which = 0;
			float t = xt;
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
						float y = ray.origin.y + rayDelta.y * t;
						if ( y < this.min.y || y > this.max.y ) return false;
						float z = ray.origin.z + rayDelta.z * t;
						if ( z < this.min.z || z > this.max.z ) return false;

						normal.x = xn;
						normal.y = 0.0f;
						normal.z = 0.0f;

					}
					break;

				case 1: // intersect with xz plane
					{
						float x = ray.origin.x + rayDelta.x * t;
						if ( x < this.min.x || x > this.max.x ) return false;
						float z = ray.origin.z + rayDelta.z * t;
						if ( z < this.min.z || z > this.max.z ) return false;

						normal.x = 0.0f;
						normal.y = yn;
						normal.z = 0.0f;

					}
					break;

				case 2: // intersect with xy plane
					{
						float x = ray.origin.x + rayDelta.x * t;
						if ( x < this.min.x || x > this.max.x ) return false;
						float y = ray.origin.y + rayDelta.y * t;
						if ( y < this.min.y || y > this.max.y ) return false;

						normal.x = 0.0f;
						normal.y = 0.0f;
						normal.z = zn;

					}
					break;
			}
			parametric = t;
			return t >= 0 && t <= 1f;
		}

		/// <summary>
		///   <para>Does ray intersect this bounding box?</para>
		/// </summary>
		/// <param name="ray"></param>
		public bool Intersect( Ray ray )
		{
			float num;
			return this.Intersect( ray, out num );
		}

		public bool Intersect( Ray ray, out float distance )
		{
			float tmin = -MathUtils.INFINITY;
			float tmax = MathUtils.INFINITY;

			distance = tmin;

			Vec3 t = this.center - ray.origin;
			float[] p = { t.x, t.y, t.z };
			t = this.extents;
			float[] extent = { t.x, t.y, t.z };
			t = ray.direction;
			float[] dir = { t.x, t.y, t.z };

			for ( int i = 0; i < 3; i++ )
			{
				float f = 1 / dir[i];
				float t0 = ( p[i] + extent[i] ) * f;
				float t1 = ( p[i] - extent[i] ) * f;

				if ( t0 < t1 )
				{
					if ( t0 > tmin ) tmin = t0;
					if ( t1 < tmax ) tmax = t1;
					if ( tmin > tmax ) return false;
					if ( tmax < 0 ) return false;
				}
				else
				{
					if ( t1 > tmin ) tmin = t1;
					if ( t0 < tmax ) tmax = t0;
					if ( tmin > tmax ) return false;
					if ( tmax < 0 ) return false;
				}
			}
			distance = tmin;
			return true;
		}

		public bool Intersect( Sphere sphere )
		{
			Vec3 closestPoint = this.ClosestPoint( sphere.center );

			return Vec3.DistanceSquared( sphere.center, closestPoint ) < sphere.radius * sphere.radius;
		}

		public override int GetHashCode()
		{
			return this.center.GetHashCode() ^ this.extents.GetHashCode() << 2;
		}

		public override bool Equals( object other )
		{
			bool result;
			if ( !( other is Bounds ) )
			{
				result = false;
			}
			else
			{
				Bounds bounds = ( Bounds )other;
				result = ( this.center.Equals( bounds.center ) && this.extents.Equals( bounds.extents ) );
			}
			return result;
		}

		public static bool operator ==( Bounds lhs, Bounds rhs )
		{
			return lhs.center == rhs.center && lhs.extents == rhs.extents;
		}

		public static bool operator !=( Bounds lhs, Bounds rhs )
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
				this._extents
			} );
		}
	}
}