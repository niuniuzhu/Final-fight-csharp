using System.Collections.Generic;

namespace Core.Algorithm.Graph
{
	public class GraphBase
	{
		public delegate void LoopFunc( GraphNode node );

		public int size => this._nodes.Length;

		private GraphNode[] _nodes;
		private readonly Dictionary<int, GraphNode> _idToNodes = new Dictionary<int, GraphNode>();
		public GraphNode[] nodes
		{
			set
			{
				this._nodes = value;
				this._idToNodes.Clear();
				int count = this._nodes.Length;
				for ( int i = 0; i < count; i++ )
				{
					GraphNode node = this._nodes[i];
					this._idToNodes[node.index] = node;
				}
			}
		}

		public GraphNode this[int index]
		{
			get
			{
				this._idToNodes.TryGetValue( index, out GraphNode node );
				return node;
			}
		}

		public GraphBase()
		{
		}

		public GraphBase( int size )
		{
			GraphNode[] ns = new GraphNode[size];
			for ( int i = 0; i < size; i++ )
				ns[i] = new GraphNode( i );
			this.nodes = ns;
		}

		public void ForeachNode( LoopFunc loopFunc )
		{
			foreach ( GraphNode node in this._nodes )
				loopFunc( node );
		}
	}
}