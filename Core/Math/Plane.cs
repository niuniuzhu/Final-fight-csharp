namespace Core.Math
{
	/// <summary>
	///   <para>Representation of a plane in 3D space.</para>
	/// </summary>
	public struct Plane
	{
		private Vec3 _normal;

		private float _distance;

		/// <summary>
		///   <para>Normal vector of the plane.</para>
		/// </summary>
		public Vec3 normal
		{
			get => this._normal;
			set => this._normal = value;
		}

		/// <summary>
		///   <para>Distance from the origin to the plane.</para>
		/// </summary>
		public float distance
		{
			get => this._distance;
			set => this._distance = value;
		}

		/// <summary>
		///   <para>Returns a copy of the plane that faces in the opposite direction.</para>
		/// </summary>
		public Plane flipped => new Plane( -this._normal, -this._distance );

		/// <summary>
		///   <para>Creates a plane.</para>
		/// </summary>
		/// <param name="inNormal"></param>
		/// <param name="inPoint"></param>
		public Plane( Vec3 inNormal, Vec3 inPoint )
		{
			this._normal = Vec3.Normalize( inNormal );
			this._distance = -Vec3.Dot( inNormal, inPoint );
		}

		/// <summary>
		///   <para>Creates a plane.</para>
		/// </summary>
		/// <param name="inNormal"></param>
		/// <param name="d"></param>
		public Plane( Vec3 inNormal, float d )
		{
			this._normal = Vec3.Normalize( inNormal );
			this._distance = d;
		}

		/// <summary>
		///   <para>Creates a plane.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public Plane( Vec3 a, Vec3 b, Vec3 c )
		{
			this._normal = Vec3.Normalize( Vec3.Cross( b - a, c - a ) );
			this._distance = -Vec3.Dot( this._normal, a );
		}

		/// <summary>
		///   <para>Sets a plane using a point that lies within it along with a normal to orient it.</para>
		/// </summary>
		/// <param name="inNormal">The plane's normal vector.</param>
		/// <param name="inPoint">A point that lies on the plane.</param>
		public void SetNormalAndPosition( Vec3 inNormal, Vec3 inPoint )
		{
			this._normal = Vec3.Normalize( inNormal );
			this._distance = -Vec3.Dot( inNormal, inPoint );
		}

		/// <summary>
		///   <para>Sets a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.</para>
		/// </summary>
		/// <param name="a">First point in clockwise order.</param>
		/// <param name="b">Second point in clockwise order.</param>
		/// <param name="c">Third point in clockwise order.</param>
		public void Set3Points( Vec3 a, Vec3 b, Vec3 c )
		{
			this._normal = Vec3.Normalize( Vec3.Cross( b - a, c - a ) );
			this._distance = -Vec3.Dot( this._normal, a );
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
		public void Translate( Vec3 translation )
		{
			this._distance += Vec3.Dot( this._normal, translation );
		}

		/// <summary>
		///   <para>Returns a copy of the given plane that is moved in space by the given translation.</para>
		/// </summary>
		/// <param name="plane">The plane to move in space.</param>
		/// <param name="translation">The offset in space to move the plane with.</param>
		/// <returns>
		///   <para>The translated plane.</para>
		/// </returns>
		public static Plane Translate( Plane plane, Vec3 translation )
		{
			return new Plane( plane._normal, plane._distance += Vec3.Dot( plane._normal, translation ) );
		}

		/// <summary>
		///   <para>For a given point returns the closest point on the plane.</para>
		/// </summary>
		/// <param name="point">The point to project onto the plane.</param>
		/// <returns>
		///   <para>A point on the plane that is closest to point.</para>
		/// </returns>
		public Vec3 ClosestPointOnPlane( Vec3 point )
		{
			float d = Vec3.Dot( this._normal, point ) + this._distance;
			return point - this._normal * d;
		}

		/// <summary>
		///   <para>Returns a signed distance from plane to point.</para>
		/// </summary>
		/// <param name="point"></param>
		public float GetDistanceToPoint( Vec3 point )
		{
			return Vec3.Dot( this._normal, point ) + this._distance;
		}

		/// <summary>
		///   <para>Is a point on the positive side of the plane?</para>
		/// </summary>
		/// <param name="point"></param>
		public bool GetSide( Vec3 point )
		{
			return Vec3.Dot( this._normal, point ) + this._distance > 0f;
		}

		/// <summary>
		///   <para>Are two points on the same side of the plane?</para>
		/// </summary>
		/// <param name="inPt0"></param>
		/// <param name="inPt1"></param>
		public bool SameSide( Vec3 inPt0, Vec3 inPt1 )
		{
			float distanceToPoint = this.GetDistanceToPoint( inPt0 );
			float distanceToPoint2 = this.GetDistanceToPoint( inPt1 );
			return ( distanceToPoint > 0f && distanceToPoint2 > 0f ) || ( distanceToPoint <= 0f && distanceToPoint2 <= 0f );
		}

		public bool Raycast( Ray ray, out float enter )
		{
			float num = Vec3.Dot( ray.direction, this._normal );
			float num2 = -Vec3.Dot( ray.origin, this._normal ) - this._distance;
			bool result;
			if ( MathUtils.Approximately( num, 0f ) )
			{
				enter = 0f;
				result = false;
			}
			else
			{
				enter = num2 / num;
				result = ( enter > 0f );
			}
			return result;
		}

		public override string ToString()
		{
			return $"(normal:({this._normal.x:F1}, {this._normal.y:F1}, {this._normal.z:F1}), distance:{this._distance:F1})";
		}
	}
}
