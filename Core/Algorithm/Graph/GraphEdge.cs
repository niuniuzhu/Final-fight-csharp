namespace Core.Algorithm.Graph
{
	public class GraphEdge
	{
		public int from { get; }
		public int to { get; }
		public float cost { get; }

		public GraphEdge( int from, int to, float cost = 0 )
		{
			this.from = from;
			this.to = to;
			this.cost = cost;
		}

		public override string ToString()
		{
			return $"from:{this.from},to:{this.to},cost:{this.cost}";
		}
	}
}