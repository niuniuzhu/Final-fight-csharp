using System.Text;

namespace Core.Algorithm.Graph
{
	public class Graph2D : GraphBase
	{
		public delegate float RandomFunc();

		public int row { get; }
		public int col { get; }

		public Graph2D( int row, int col ) : base( row * col )
		{
			this.row = row;
			this.col = col;
		}

		public GraphNode GetNode( int row, int col )
		{
			return this[row * this.col + col];
		}

		public static Graph2D CreateFullDigraph( int row, int col, RandomFunc rndFunc )
		{
			Graph2D graph = new Graph2D( row, col );
			int r = graph.row;
			int c = graph.col;
			for ( int i = 0; i < r; i++ )
			{
				for ( int j = 0; j < c; j++ )
				{
					int cur = i * c + j;
					GraphNode node = graph[cur];
					if ( j < c - 1 )
						node.AddEdge( cur, cur + 1, rndFunc() );
					if ( j > 0 )
						node.AddEdge( cur, cur - 1, rndFunc() );
					if ( i < r - 1 )
						node.AddEdge( cur, cur + c, rndFunc() );
					if ( i > 0 )
						node.AddEdge( cur, cur - c, rndFunc() );
				}
			}
			return graph;
		}

		public int CoordToIndex( int x, int y )
		{
			return y * this.col + x;
		}

		public int[] IndexToCoord( int index )
		{
			int[] coord = new int[2];
			coord[0] = index % this.col;
			coord[1] = index / this.col;
			return coord;
		}

		public string Dump()
		{
			int r = this.row;
			int c = this.col;
			StringBuilder sb = new StringBuilder();
			for ( int i = 0; i < r; i++ )
			{
				sb.AppendLine();
				for ( int j = 0; j < c; j++ )
				{
					sb.Append( this[i * c + j] );
					if ( j < c - 1 )
						sb.Append( "," );
				}
			}
			return sb.ToString();
		}
	}
}