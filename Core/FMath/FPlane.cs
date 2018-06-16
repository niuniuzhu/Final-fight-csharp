namespace Core.FMath
{
	/// <summary>
	///   <para>Representation of a plane in 3D space.</para>
	/// </summary>
	public struct FPlane
	{
		private FVec3 _normal;

		private Fix64 _distance;

		/// <summary>
		///   <para>Normal vector of the plane.</para>
		/// </summary>
		public FVec3 normal
		{
			get => this._normal;
			set => this._normal = value;
		}

		/// <summary>
		///   <para>Distance from the origin to the plane.</para>
		/// </summary>
		public Fix64 distance
		{
			get => this._distance;
			set => this._distance = value;
		}

		/// <summary>
		///   <para>Returns a copy of the plane that faces in the opposite direction.</para>
		/// </summary>
		public FPlane flipped => new FPlane( -this._normal, -this._distance );

		/// <summary>
		///   <para>Creates a plane.</para>
		/// </summary>
		/// <param name="inNormal"></param>
		/// <param name="inPoint"></param>
		public FPlane( FVec3 inNormal, FVec3 inPoint )
		{
			this._normal = FVec3.Normalize( inNormal );
			this._distance = -FVec3.Dot( inNormal, inPoint );
		}

		/// <summary>
		///   <para>Creates a plane.</para>
		/// </summary>
		/// <param name="inNormal"></param>
		/// <param name="d"></param>
		public FPlane( FVec3 inNormal, Fix64 d )
		{
			this._normal = FVec3.Normalize( inNormal );
			this._distance = d;
		}

		/// <summary>
		///   <para>Creates a plane.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public FPlane( FVec3 a, FVec3 b, FVec3 c )
		{
			this._normal = FVec3.Normalize( FVec3.Cross( b - a, c - a ) );
			this._distance = -FVec3.Dot( this._normal, a );
		}

		/// <summary>
		///   <para>Sets a plane using a point that lies within it along with a normal to orient it.</para>
		/// </summary>
		/// <param name="inNormal">The plane's normal vector.</param>
		/// <param name="inPoint">A point that lies on the plane.</param>
		public void SetNormalAndPosition( FVec3 inNormal, FVec3 inPoint )
		{
			this._normal = FVec3.Normalize( inNormal );
			this._distance = -FVec3.Dot( inNormal, inPoint );
		}

		/// <summary>
		///   <para>Sets a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.</para>
		/// </summary>
		/// <param name="a">First point in clockwise order.</param>
		/// <param name="b">Second point in clockwise order.</param>
		/// <param name="c">Third point in clockwise order.</param>
		public void Set3Points( FVec3 a, FVec3 b, FVec3 c )
		{
			this._normal = FVec3.Normalize( FVec3.Cross( b - a, c - a ) );
			this._distance = -FVec3.Dot( this._normal, a );
		}

		/// <summary>
		///   <para>Makes the plane face in the opposite direction.</para>
		/// </summary>
		public void Flip()
		{
			this._normal = -this._normal;
			this._distance = -this._distance;
		}

		/// <summary>
		///   <para>Moves the plane in space by the translation vector.</para>
		/// </summary>
		/// <param name="translation">The offset in space to move the plane with.</param>
		public void Translate( FVec3 translation )
		{
			this._distance += FVec3.Dot( this._normal, translation );
		}

		/// <summary>
		///   <para>Returns a copy of the given plane that is moved in space by the given translation.</para>
		/// </summary>
		/// <param name="plane">The plane to move in space.</param>
		/// <param name="translation">The offset in space to move the plane with.</param>
		/// <returns>
		///   <para>The translated plane.</para>
		/// </returns>
		public static FPlane Translate( FPlane plane, FVec3 translation )
		{
			return new FPlane( plane._normal, plane._distance += FVec3.Dot( plane._normal, translation ) );
		}

		/// <summary>
		///   <para>For a given point returns the closest point on the plane.</para>
		/// </summary>
		/// <param name="point">The point to project onto the plane.</param>
		/// <returns>
		///   <para>A point on the plane that is closest to point.</para>
		/// </returns>
		public FVec3 ClosestPointOnPlane( FVec3 point )
		{
			Fix64 d = FVec3.Dot( this._normal, point ) + this._distance;
			return point - this._normal * d;
		}

		/// <summary>
		///   <para>Returns a signed distance from plane to point.</para>
		/// </summary>
		/// <param name="point"></param>
		public Fix64 GetDistanceToPoint( FVec3 point )
		{
			return FVec3.Dot( this._normal, point ) + this._distance;
		}

		/// <summary>
		///   <para>Is a point on the positive side of the plane?</para>
		/// </summary>
		/// <param name="point"></param>
		public bool GetSide( FVec3 point )
		{
			return FVec3.Dot( this._normal, point ) + this._distance > Fix64.Zero;
		}

		/// <summary>
		///   <para>Are two points on the same side of the plane?</para>
		/// </summary>
		/// <param name="inPt0"></param>
		/// <param name="inPt1"></param>
		public bool SameSide( FVec3 inPt0, FVec3 inPt1 )
		{
			Fix64 distanceToPoint = this.GetDistanceToPoint( inPt0 );
			Fix64 distanceToPoint2 = this.GetDistanceToPoint( inPt1 );
			return ( distanceToPoint > Fix64.Zero && distanceToPoint2 > Fix64.Zero ) ||
				   ( distanceToPoint <= Fix64.Zero && distanceToPoint2 <= Fix64.Zero );
		}

		public bool Raycast( FRay ray, out Fix64 enter )
		{
			Fix64 num = FVec3.Dot( ray.direction, this._normal );
			Fix64 num2 = -FVec3.Dot( ray.origin, this._normal ) - this._distance;
			bool result;
			if ( num == Fix64.Zero )
			{
				enter = Fix64.Zero;
				result = false;
			}
			else
			{
				enter = num2 / num;
				result = ( enter > Fix64.Zero );
			}
			return result;
		}

		public override string ToString()
		{
			return $"(normal:({this._normal.x:F1}, {this._normal.y:F1}, {this._normal.z:F1}), distance:{this._distance:F1})";
		}
	}
}
