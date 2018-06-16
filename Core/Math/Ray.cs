namespace Core.Math
{
	public struct Ray
	{
		/// <summary>
		///   <para>The origin point of the ray.</para>
		/// </summary>
		public Vec3 origin;

		/// <summary>
		///   <para>The direction of the ray.</para>
		/// </summary>
		public Vec3 direction;

		public Ray( Vec3 origin, Vec3 direction )
		{
			this.origin = origin;
			this.direction = Vec3.Normalize( direction );
		}

		/// <summary>
		///   <para>Returns a point at distance units along the ray.</para>
		/// </summary>
		/// <param name="distance"></param>
		public Vec3 GetPoint( float distance )
		{
			return this.origin + this.direction * distance;
		}

		public Vec3 InersectPlaneX( float planePosition )
		{
			if ( this.direction.x == 0 ) return this.origin;
			if ( ( planePosition >= this.origin.x && this.direction.x <= this.origin.x ) ||
				 ( planePosition <= this.origin.x && this.direction.x >= this.origin.x ) ) return this.origin;

			float dis = planePosition - this.origin.x;
			float slopeY = this.direction.y / this.direction.x;
			float slopeZ = this.direction.z / this.direction.x;
			return new Vec3( planePosition, slopeY * dis + this.origin.y, slopeZ * dis + this.origin.z );
		}

		public Vec3 InersectPlaneY( float planePosition )
		{
			if ( this.direction.y == 0 ) return this.origin;
			if ( ( planePosition >= this.origin.y && this.direction.y <= this.origin.y ) ||
				 ( planePosition <= this.origin.y && this.direction.y >= this.origin.y ) ) return this.origin;

			float dis = planePosition - this.origin.y;
			float slopeX = this.direction.x / this.direction.y;
			float slopeZ = this.direction.z / this.direction.y;
			return new Vec3( slopeX * dis + this.origin.x, planePosition, slopeZ * dis + this.origin.z );
		}

		public Vec3 InersectPlaneZ( float planePosition )
		{
			if ( this.direction.z == 0 ) return this.origin;
			if ( ( planePosition >= this.origin.z && this.direction.z <= this.origin.z ) ||
				 ( planePosition <= this.origin.z && this.direction.z >= this.origin.z ) ) return this.origin;

			float dis = planePosition - this.origin.z;
			float slopeX = this.direction.x / this.direction.z;
			float slopeY = this.direction.y / this.direction.z;
			return new Vec3( slopeX * dis + this.origin.x, slopeY * dis + this.origin.y, planePosition );
		}

		public bool Intersects( Bounds boundingBox, out float result )
		{
			// X
			if ( MathUtils.Abs( this.direction.x ) < MathUtils.EPSILON &&
				 ( this.origin.x < boundingBox.min.x || this.origin.x > boundingBox.max.x ) )
			{
				//If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
				return false;
			}

			float tmin = 0, tmax = float.MaxValue;
			float inverseDirection = 1 / this.direction.x;
			float t1 = ( boundingBox.min.x - this.origin.x ) * inverseDirection;
			float t2 = ( boundingBox.max.x - this.origin.x ) * inverseDirection;
			if ( t1 > t2 )
			{
				float temp = t1;
				t1 = t2;
				t2 = temp;
			}

			tmin = MathUtils.Max( tmin, t1 );
			tmax = MathUtils.Min( tmax, t2 );
			if ( tmin > tmax )
			{
				result = 0;
				return false;
			}

			// Y
			if ( MathUtils.Abs( this.direction.y ) < MathUtils.EPSILON &&
				 ( this.origin.y < boundingBox.min.y || this.origin.y > boundingBox.max.y ) )
			{
				//If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
				return false;
			}

			inverseDirection = 1 / this.direction.y;
			t1 = ( boundingBox.min.y - this.origin.y ) * inverseDirection;
			t2 = ( boundingBox.max.y - this.origin.y ) * inverseDirection;
			if ( t1 > t2 )
			{
				float temp = t1;
				t1 = t2;
				t2 = temp;
			}

			tmin = MathUtils.Max( tmin, t1 );
			tmax = MathUtils.Min( tmax, t2 );
			if ( tmin > tmax )
			{
				result = 0;
				return false;
			}

			// Z
			if ( MathUtils.Abs( this.direction.z ) < MathUtils.EPSILON &&
				 ( this.origin.z < boundingBox.min.z || this.origin.z > boundingBox.max.z ) )
			{
				//If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
				return false;
			}

			inverseDirection = 1 / this.direction.z;
			t1 = ( boundingBox.min.z - this.origin.z ) * inverseDirection;
			t2 = ( boundingBox.max.z - this.origin.z ) * inverseDirection;
			if ( t1 > t2 )
			{
				float temp = t1;
				t1 = t2;
				t2 = temp;
			}

			tmin = MathUtils.Max( tmin, t1 );
			tmax = MathUtils.Min( tmax, t2 );
			if ( tmin > tmax )
			{
				result = 0;
				return false;
			}
			result = tmin;

			return true;
		}

		public bool IntersectRaySphere( Vec3 sphereCenter, float radius, out Vec3 point1, out Vec3 point2, out Vec3 normal1,
										out Vec3 normal2 )
		{
			Vec3 d = this.origin - sphereCenter;
			float a = this.direction.Dot( this.direction );
			float b = d.Dot( this.direction );
			float c = d.SqrMagnitude() - radius * radius;

			float disc = b * b - a * c;
			if ( disc < 0.0f )
			{
				point1 = this.origin;
				point2 = this.origin;
				normal1 = Vec3.zero;
				normal2 = Vec3.zero;
				return false;
			}

			float sqrtDisc = MathUtils.Sqrt( disc );
			float invA = 1.0f / a;
			float t1 = ( -b - sqrtDisc ) * invA;
			float t2 = ( -b + sqrtDisc ) * invA;

			float invRadius = 1.0f / radius;
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