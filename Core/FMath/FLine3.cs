namespace Core.FMath
{
	public struct FLine3
	{
		#region Properties

		public FVec3 point1, point2;

		#endregion

		#region Constructors

		public FLine3( FVec3 point1, FVec3 point2 )
		{
			this.point1 = point1;
			this.point2 = point2;
		}

		#endregion

		#region Methods

		public FLine3 Transform( FMat3 matrix )
		{
			return new FLine3( this.point1.Transform( matrix ), this.point2.Transform( matrix ) );
		}

		public Fix64 Length()
		{
			return ( this.point1 - this.point2 ).Magnitude();
		}

		public FVec3 InersectPlane( FVec3 planeNormal, FVec3 planeLocation )
		{
			Fix64 dot = -( planeNormal.x * planeLocation.x ) - planeNormal.y * planeLocation.y - planeNormal.z * planeLocation.z;
			Fix64 dot3 = planeNormal.x * ( this.point2.x - this.point1.x ) + planeNormal.y * ( this.point2.y - this.point1.y ) +
			             planeNormal.z * ( this.point2.z - this.point1.z );
			Fix64 dot2 =
				-( ( dot + planeNormal.x * this.point1.x + planeNormal.y * this.point1.y + planeNormal.z * this.point1.z ) / dot3 );
			return this.point1 + dot2 * ( this.point2 - this.point1 );
		}

		public static void InersectPlane( ref FLine3 line, ref FVec3 planeNormal, ref FVec3 planeLocation, out FVec3 result )
		{
			Fix64 dot = -( planeNormal.x * planeLocation.x ) - planeNormal.y * planeLocation.y - planeNormal.z * planeLocation.z;
			Fix64 dot3 = planeNormal.x * ( line.point2.x - line.point1.x ) + planeNormal.y * ( line.point2.y - line.point1.y ) +
			             planeNormal.z * ( line.point2.z - line.point1.z );
			Fix64 dot2 =
				-( ( dot + planeNormal.x * line.point1.x + planeNormal.y * line.point1.y + planeNormal.z * line.point1.z ) / dot3 );
			result = line.point1 + dot2 * ( line.point2 - line.point1 );
		}

		public FLine3 Inersect( FLine3 line )
		{
			FVec3 vector = this.point1 - line.point1, vector2 = line.point2 - line.point1, vector3 = this.point2 - this.point1;
			Fix64 dot1 = vector.Dot( vector2 );
			Fix64 dot2 = vector2.Dot( vector3 );
			Fix64 dot3 = vector.Dot( vector3 );
			Fix64 dot4 = vector2.Dot();
			Fix64 dot5 = vector3.Dot();
			Fix64 mul1 = ( dot1 * dot2 - dot3 * dot4 ) / ( dot5 * dot4 - dot2 * dot2 );
			Fix64 mul2 = ( dot1 + dot2 * mul1 ) / dot4;
			return new FLine3( this.point1 + mul1 * vector3, line.point1 + mul2 * vector2 );
		}

		#endregion
	}
}