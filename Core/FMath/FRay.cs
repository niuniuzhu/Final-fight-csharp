namespace Core.FMath
{
	public struct FRay
	{
		/// <summary>
		///   <para>The origin point of the ray.</para>
		/// </summary>
		public FVec3 origin;

		/// <summary>
		///   <para>The direction of the ray.</para>
		/// </summary>
		public FVec3 direction;

		public FRay( FVec3 origin, FVec3 direction )
		{
			this.origin = origin;
			this.direction = FVec3.Normalize( direction );
		}

		/// <summary>
		///   <para>Returns a point at distance units along the ray.</para>
		/// </summary>
		/// <param name="distance"></param>
		public FVec3 GetPoint( Fix64 distance )
		{
			return this.origin + this.direction * distance;
		}

		public FVec3 InersectPlaneX( Fix64 planePosition )
		{
			if ( this.direction.x == Fix64.Zero ) return this.origin;
			if ( ( planePosition >= this.origin.x && this.direction.x <= this.origin.x ) ||
				 ( planePosition <= this.origin.x && this.direction.x >= this.origin.x ) ) return this.origin;

			Fix64 dis = planePosition - this.origin.x;
			Fix64 slopeY = this.direction.y / this.direction.x;
			Fix64 slopeZ = this.direction.z / this.direction.x;
			return new FVec3( planePosition, slopeY * dis + this.origin.y, slopeZ * dis + this.origin.z );
		}

		public FVec3 InersectPlaneY( Fix64 planePosition )
		{
			if ( this.direction.y == Fix64.Zero ) return this.origin;
			if ( ( planePosition >= this.origin.y && this.direction.y <= this.origin.y ) ||
				 ( planePosition <= this.origin.y && this.direction.y >= this.origin.y ) ) return this.origin;

			Fix64 dis = planePosition - this.origin.y;
			Fix64 slopeX = this.direction.x / this.direction.y;
			Fix64 slopeZ = this.direction.z / this.direction.y;
			return new FVec3( slopeX * dis + this.origin.x, planePosition, slopeZ * dis + this.origin.z );
		}

		public FVec3 InersectPlaneZ( Fix64 planePosition )
		{
			if ( this.direction.z == Fix64.Zero ) return this.origin;
			if ( ( planePosition >= this.origin.z && this.direction.z <= this.origin.z ) ||
				 ( planePosition <= this.origin.z && this.direction.z >= this.origin.z ) ) return this.origin;

			Fix64 dis = planePosition - this.origin.z;
			Fix64 slopeX = this.direction.x / this.direction.z;
			Fix64 slopeY = this.direction.y / this.direction.z;
			return new FVec3( slopeX * dis + this.origin.x, slopeY * dis + this.origin.y, planePosition );
		}

		public bool Intersects( FBounds boundingBox, out Fix64 result )
		{
			// X
			if ( Fix64.Abs( this.direction.x ) < Fix64.Epsilon &&
				 ( this.origin.x < boundingBox.min.x || this.origin.x > boundingBox.max.x ) )
			{
				//If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = Fix64.Zero;
				return false;
			}

			Fix64 tmin = Fix64.Zero, tmax = Fix64.MaxValue;
			Fix64 inverseDirection = Fix64.One / this.direction.x;
			Fix64 t1 = ( boundingBox.min.x - this.origin.x ) * inverseDirection;
			Fix64 t2 = ( boundingBox.max.x - this.origin.x ) * inverseDirection;
			if ( t1 > t2 )
			{
				Fix64 temp = t1;
				t1 = t2;
				t2 = temp;
			}

			tmin = Fix64.Max( tmin, t1 );
			tmax = Fix64.Min( tmax, t2 );
			if ( tmin > tmax )
			{
				result = Fix64.Zero;
				return false;
			}

			// Y
			if ( Fix64.Abs( this.direction.y ) < Fix64.Epsilon &&
				 ( this.origin.y < boundingBox.min.y || this.origin.y > boundingBox.max.y ) )
			{
				//If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = Fix64.Zero;
				return false;
			}

			inverseDirection = Fix64.One / this.direction.y;
			t1 = ( boundingBox.min.y - this.origin.y ) * inverseDirection;
			t2 = ( boundingBox.max.y - this.origin.y ) * inverseDirection;
			if ( t1 > t2 )
			{
				Fix64 temp = t1;
				t1 = t2;
				t2 = temp;
			}

			tmin = Fix64.Max( tmin, t1 );
			tmax = Fix64.Min( tmax, t2 );
			if ( tmin > tmax )
			{
				result = Fix64.Zero;
				return false;
			}

			// Z
			if ( Fix64.Abs( this.direction.z ) < Fix64.Epsilon &&
				 ( this.origin.z < boundingBox.min.z || this.origin.z > boundingBox.max.z ) )
			{
				//If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = Fix64.Zero;
				return false;
			}

			inverseDirection = Fix64.One / this.direction.z;
			t1 = ( boundingBox.min.z - this.origin.z ) * inverseDirection;
			t2 = ( boundingBox.max.z - this.origin.z ) * inverseDirection;
			if ( t1 > t2 )
			{
				Fix64 temp = t1;
				t1 = t2;
				t2 = temp;
			}

			tmin = Fix64.Max( tmin, t1 );
			tmax = Fix64.Min( tmax, t2 );
			if ( tmin > tmax )
			{
				result = Fix64.Zero;
				return false;
			}
			result = tmin;

			return true;
		}

		public bool IntersectRaySphere( FVec3 sphereCenter, Fix64 radius, out FVec3 point1, out FVec3 point2, out FVec3 normal1,
										out FVec3 normal2 )
		{
			FVec3 d = this.origin - sphereCenter;
			Fix64 a = this.direction.Dot( this.direction );
			Fix64 b = d.Dot( this.direction );
			Fix64 c = d.Dot() - radius * radius;

			Fix64 disc = b * b - a * c;
			if ( disc < Fix64.Zero )
			{
				point1 = this.origin;
				point2 = this.origin;
				normal1 = FVec3.zero;
				normal2 = FVec3.zero;
				return false;
			}

			Fix64 sqrtDisc = Fix64.Sqrt( disc );
			Fix64 invA = Fix64.One / a;
			Fix64 t1 = ( -b - sqrtDisc ) * invA;
			Fix64 t2 = ( -b + sqrtDisc ) * invA;

			Fix64 invRadius = Fix64.One / radius;
			point1 = this.origin + t1 * this.direction;
			point2 = this.origin + t2 * this.direction;
			normal1 = ( point1 - sphereCenter ) * invRadius;
			normal2 = ( point2 - sphereCenter ) * invRadius;

			return true;
		}

		/// <summary>
		///   <para>Returns a nicely formatted string for this ray.</para>
		/// </summary>
		public override string ToString()
		{
			return $"Origin: {this.origin}, Dir: {this.direction}";
		}
	}
}