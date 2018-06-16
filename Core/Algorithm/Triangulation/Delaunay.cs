using Core.Math;
using System;
using System.Collections.Generic;

namespace Core.Algorithm.Triangulation
{
	public static class Delaunay
	{
		public class DTriangle : Triangle
		{
			public float x, y, r;

			public DTriangle( int i, int j, int k, float x = 0f, float y = 0f, float r = 0f ) : base( i, j, k )
			{
				this.x = x;
				this.y = y;
				this.r = r;
			}
		}

		public class DVertex : Vertex, IComparable
		{
			public DVertex( float x, float y ) : base( x, y )
			{
			}

			public int CompareTo( object obj )
			{
				DVertex b = ( DVertex )obj;
				if ( this.x > b.x )
					return 1;
				return -1;
			}
		}

		private const float EPSILON = 1.0f / 1048576.0f;

		private static float[] SuperTriangle( List<DVertex> vertices )
		{
			float xmin = float.PositiveInfinity,
				  ymin = float.PositiveInfinity,
				  xmax = float.NegativeInfinity,
				  ymax = float.NegativeInfinity;

			foreach ( DVertex vertex in vertices )
			{
				if ( vertex.x < xmin ) xmin = vertex.x;
				if ( vertex.x > xmax ) xmax = vertex.x;
				if ( vertex.y < ymin ) ymin = vertex.y;
				if ( vertex.y > ymax ) ymax = vertex.y;
			}
			float dx = xmax - xmin;
			float dy = ymax - ymin;
			float dmax = MathUtils.Max( dx, dy );
			float xmid = xmin + dx * 0.5f;
			float ymid = ymin + dy * 0.5f;

			float[] result = new float[6];
			result[0] = xmid - 2 * dmax;
			result[1] = ymid - dmax;
			result[2] = xmid;
			result[3] = ymid + 2 * dmax;
			result[4] = xmid + 2 * dmax;
			result[5] = ymid - dmax;

			return result;
		}

		private static DTriangle Circumcircle( List<DVertex> vertices, int i, int j, int k )
		{
			float x1 = vertices[i].x,
				  y1 = vertices[i].y,
				  x2 = vertices[j].x,
				  y2 = vertices[j].y,
				  x3 = vertices[k].x,
				  y3 = vertices[k].y,
				  fabsy1y2 = MathUtils.Abs( y1 - y2 ),
				  fabsy2y3 = MathUtils.Abs( y2 - y3 ),
				  xc,
				  yc,
				  m1,
				  m2,
				  mx1,
				  mx2,
				  my1,
				  my2;

			/* Check for coincident points */
			//if ( fabsy1y2 < EPSILON &&
			//	 fabsy2y3 < EPSILON )
			//	throw new Exception( "Eek! Coincident points!" );

			if ( fabsy1y2 < EPSILON )
			{
				m2 = -( ( x3 - x2 ) / ( y3 - y2 ) );
				mx2 = ( x2 + x3 ) / 2.0f;
				my2 = ( y2 + y3 ) / 2.0f;
				xc = ( x2 + x1 ) / 2.0f;
				yc = m2 * ( xc - mx2 ) + my2;
			}
			else if ( fabsy2y3 < EPSILON )
			{
				m1 = -( ( x2 - x1 ) / ( y2 - y1 ) );
				mx1 = ( x1 + x2 ) / 2.0f;
				my1 = ( y1 + y2 ) / 2.0f;
				xc = ( x3 + x2 ) / 2.0f;
				yc = m1 * ( xc - mx1 ) + my1;
			}
			else
			{
				m1 = -( ( x2 - x1 ) / ( y2 - y1 ) );
				m2 = -( ( x3 - x2 ) / ( y3 - y2 ) );
				mx1 = ( x1 + x2 ) / 2.0f;
				mx2 = ( x2 + x3 ) / 2.0f;
				my1 = ( y1 + y2 ) / 2.0f;
				my2 = ( y2 + y3 ) / 2.0f;
				xc = ( m1 * mx1 - m2 * mx2 + my2 - my1 ) / ( m1 - m2 );
				yc = ( fabsy1y2 > fabsy2y3 ) ? m1 * ( xc - mx1 ) + my1 : m2 * ( xc - mx2 ) + my2;
			}

			float dx = x2 - xc;
			float dy = y2 - yc;

			DTriangle triangle = new DTriangle( i, j, k, xc, yc, dx * dx + dy * dy );
			return triangle;
		}

		private static void Dedup( List<int> edges )
		{
			for ( int j = 0; j < edges.Count; )
			{
				int b = edges[j++];
				int a = edges[j++];

				for ( int i = edges.Count; i > j; )
				{
					int n = edges[--i];
					int m = edges[--i];

					if ( ( a == m && b == n ) ||
						 ( a == n && b == m ) )
					{
						j -= 2;
						edges.RemoveRange( i, 2 );
						edges.RemoveRange( j, 2 );
						break;
					}
				}
			}
		}

		public static List<DTriangle> Triangulate( List<DVertex> vertices )
		{
			int n = vertices.Count;

			/* Bail if there aren't enough vertices to form any triangles. */
			if ( n < 3 )
				return new List<DTriangle>();

			vertices.Sort();

			/* Next, find the vertices of the supertriangle (which contains all other
			 * triangles), and append them onto the end of a (copy of) the vertex
			 * array. */
			float[] st = SuperTriangle( vertices );
			vertices.Add( new DVertex( st[0], st[1] ) );
			vertices.Add( new DVertex( st[2], st[3] ) );
			vertices.Add( new DVertex( st[4], st[5] ) );

			/* Initialize the open list (containing the supertriangle and nothing
			 * else) and the closed list (which is empty since we havn't processed
			 * any triangles yet). */
			List<DTriangle> open = new List<DTriangle> { Circumcircle( vertices, n + 0, n + 1, n + 2 ) };
			List<DTriangle> closed = new List<DTriangle>();
			List<int> edges = new List<int>();

			/* Incrementally add each vertex to the mesh. */
			for ( int i = 0; i < n; i++ )
			{
				edges.Clear();

				/* For each open triangle, check to see if the current point is
				 * inside it's circumcircle. If it is, remove the triangle and add
				 * it's edges to an edge list. */
				for ( int j = open.Count - 1; j >= 0; j-- )
				{
					/* If this point is to the right of this triangle's circumcircle,
					 * then this triangle should never get checked again. Remove it
					 * from the open list, add it to the closed list, and skip. */
					DTriangle openTri = open[j];
					DVertex curVertex = vertices[i];
					float dx = curVertex.x - openTri.x;
					if ( dx > 0.0f && dx * dx > openTri.r )
					{
						closed.Add( openTri );
						open.RemoveAt( j );
						continue;
					}

					/* If we're outside the circumcircle, skip this triangle. */
					float dy = curVertex.y - openTri.y;
					if ( dx * dx + dy * dy - openTri.r > EPSILON )
						continue;

					/* Remove the triangle and add it's edges to the edge list. */
					edges.Add( openTri.v0 );
					edges.Add( openTri.v1 );
					edges.Add( openTri.v1 );
					edges.Add( openTri.v2 );
					edges.Add( openTri.v2 );
					edges.Add( openTri.v0 );
					open.RemoveAt( j );
				}

				/* Remove any doubled edges. */
				Dedup( edges );

				/* Add a new triangle for each edge. */
				for ( int j = edges.Count; j > 0; )
				{
					int b = edges[--j];
					int a = edges[--j];
					open.Add( Circumcircle( vertices, a, b, i ) );
				}
			}

			/* Copy any remaining open triangles to the closed list, and then
			 * remove any triangles that share a vertex with the supertriangle,
			 * building a list of triplets that represent triangles. */
			closed.AddRange( open );
			open.Clear();

			for ( int i = closed.Count - 1; i >= 0; i-- )
			{
				DTriangle triangle = closed[i];
				if ( triangle.v0 < n && triangle.v1 < n && triangle.v2 < n )
					open.Add( triangle );
			}

			/* Yay, we're done! */
			return open;
		}
	}
}