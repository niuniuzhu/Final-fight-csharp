namespace Core.Math
{
	public struct Sphere
	{
		public Vec3 center;
		public float radius;

		public Sphere( Vec3 center, float radius )
		{
			this.center = center;
			this.radius = radius;
		}

		public bool Intersects( Bounds boundingBox )
		{
			Vec3 clampedLocation;
			if ( this.center.x > boundingBox.max.x )
				clampedLocation.x = boundingBox.max.x;
			else if ( this.center.x < boundingBox.min.x )
				clampedLocation.x = boundingBox.min.x;
			else
				clampedLocation.x = this.center.x;

			if ( this.center.y > boundingBox.max.y )
				clampedLocation.y = boundingBox.max.y;
			else if ( this.center.y < boundingBox.min.y )
				clampedLocation.y = boundingBox.min.y;
			else
				clampedLocation.y = this.center.y;

			if ( this.center.z > boundingBox.max.z )
				clampedLocation.z = boundingBox.max.z;
			else if ( this.center.z < boundingBox.min.z )
				clampedLocation.z = boundingBox.min.z;
			else
				clampedLocation.z = this.center.z;

			return clampedLocation.DistanceSquared( this.center ) <= this.radius * this.radius;
		}
	}
}