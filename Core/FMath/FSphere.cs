namespace Core.FMath
{
	public struct FSphere
	{
		public FVec3 center;
		public Fix64 radius;

		public FSphere( FVec3 center, Fix64 radius )
		{
			this.center = center;
			this.radius = radius;
		}

		public bool Intersects( FBounds boundingBox )
		{
			FVec3 clampedLocation;
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