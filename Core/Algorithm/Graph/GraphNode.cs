using System.Collections.Generic;
using System.Text;

namespace Core.Algorithm.Graph
{
	public class GraphNode
	{
		public int index { get; }

		public List<GraphEdge> edges { get; } = new List<GraphEdge>();

		public GraphNode( int index )
		{
			this.index = index;
		}

		public GraphEdge AddEdge( int from, int to, float cost )
		{
			GraphEdge edge = new GraphEdge( @from, to, cost );
			this.edges.Add( edge );
			return edge;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			int count = this.edges.Count;
			for ( int i = 0; i < count; i++ )
			{
				GraphEdge edge = this.edges[i];
				sb.Append( edge );
				if ( i < count - 1 )
					sb.Append( "|" );
			}
			return $"index:{this.index},edges:{sb}";
		}
	}
}