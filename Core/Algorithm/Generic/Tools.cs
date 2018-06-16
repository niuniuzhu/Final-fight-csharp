using Core.Algorithm.Graph;
using Core.Algorithm.Triangulation;
using System.Collections.Generic;
using System.Linq;

namespace Core.Algorithm.Generic
{
	public static class Tools
	{
		public delegate float RandomFunc();

		public static GraphBase TrianglesToGraph( IEnumerable<Triangle> triangles, RandomFunc rndFunc )
		{
			Dictionary<int, GraphNode> nodeMap = new Dictionary<int, GraphNode>();
			List<GraphEdge> openEdges = new List<GraphEdge>();
			int[] indices0 = new int[3];
			int[] indices1 = new int[3];
			foreach ( Triangle triangle in triangles )
			{
				indices0[0] = triangle.v0;
				indices0[1] = triangle.v1;
				indices0[2] = triangle.v2;
				indices1[0] = triangle.v1;
				indices1[1] = triangle.v2;
				indices1[2] = triangle.v0;
				for ( int i = 0; i < 3; i++ )
				{
					int index0 = indices0[i];
					int index1 = indices1[i];
					nodeMap.TryGetValue( index0, out GraphNode node );
					if ( node == null )
					{
						node = new GraphNode( index0 );
						nodeMap[node.index] = node;
					}
					bool found = false;
					foreach ( GraphEdge edge in openEdges )
					{
						if ( ( edge.from == index0 && edge.to == index1 ) ||
							 ( edge.from == index1 && edge.to == index0 ) )
						{
							found = true;
							break;
						}
					}
					if ( !found )
					{
						openEdges.Add( node.AddEdge( index0, index1, rndFunc() ) );
					}
				}
			}
			GraphBase graph = new GraphBase();
			graph.nodes = nodeMap.Values.ToArray();
			return graph;
		}
	}
}