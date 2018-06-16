using Core.Structure;
using System.Collections.Generic;

namespace Core.Algorithm.Graph
{
	public static class GraphSearcher
	{
		public static List<GraphEdge> PrimSearch( GraphBase graph, int start )
		{
			List<GraphEdge> shortestPathPredecessors = new List<GraphEdge>();
			HashSet<int> visitedNodes = new HashSet<int>();
			SimplePriorityQueue<GraphEdge> nodeQueue = new SimplePriorityQueue<GraphEdge>();

			GraphNode node = graph[start];
			//把该节点的作为父节点标记为已访问
			while ( node != null )
			{
				//把该节点标记为已访问
				visitedNodes.Add( node.index );

				//插入所有连接该节点的边
				foreach ( GraphEdge edge in node.edges )
					nodeQueue.Enqueue( edge, edge.cost );

				nodeQueue.TryDequeue( out GraphEdge edage );//获取消费最小的边
				while ( edage != null && visitedNodes.Contains( edage.to ) )//是否和访问过的节点形成闭合回路
				{
					nodeQueue.TryDequeue( out edage );//是的话跳过该边
				}

				//当剩下的边和访问过的边能组合成闭合回路时搜索完成
				if ( edage == null )
					break;

				//此时该边已是有效的最小消费边
				shortestPathPredecessors.Add( edage );
				node = graph[edage.to];
			}
			return shortestPathPredecessors;
		}

		public static int[] AStarSearch( GraphBase graph, int start, int end )
		{
			Dictionary<int, GraphEdge> shortestPathPredecessors = new Dictionary<int, GraphEdge>();
			Dictionary<int, GraphEdge> frontierPredecessors = new Dictionary<int, GraphEdge>();
			SimplePriorityQueue<int> nodeQueue = new SimplePriorityQueue<int>();
			Dictionary<int, float> costToNode = new Dictionary<int, float>();

			costToNode[start] = 0;
			frontierPredecessors[start] = null;

			// Create an indexed priority queue of nodes. The nodes with the
			// lowest estimated total cost to target via the node are positioned at the front.
			// Put the source node on the queue.
			nodeQueue.Enqueue( start, 0f );

			// if the queue is not empty..
			while ( nodeQueue.count > 0 )
			{
				// get lowest cost node from the queue
				int nextClosestNode = nodeQueue.Dequeue();

				// move this node from the frontier to the spanning tree
				frontierPredecessors.TryGetValue( nextClosestNode, out GraphEdge predecessor );
				shortestPathPredecessors[nextClosestNode] = predecessor;

				// If the target has been found, return
				if ( end == nextClosestNode )
					break;

				// Now to test all the edges attached to this node
				List<GraphEdge> edages = graph[nextClosestNode].edges;
				foreach ( GraphEdge edge in edages )
				{
					float totalCost = costToNode[nextClosestNode] + edge.cost;
					float estimatedTotalCostViaNode = totalCost + 0f;//todo

					// if the node has not been added to the frontier, add it and update the costs
					if ( !frontierPredecessors.ContainsKey( edge.to ) )
					{
						costToNode[edge.to] = totalCost;
						frontierPredecessors[edge.to] = edge;
						nodeQueue.Enqueue( edge.to, estimatedTotalCostViaNode );
					}

					// if this node is already on the frontier but the cost to get here
					// is cheaper than has been found previously, update the node
					// costs and frontier accordingly.
					else if ( totalCost < costToNode[edge.to] &&
							  !shortestPathPredecessors.ContainsKey( edge.to ) )
					{
						costToNode[edge.to] = totalCost;
						frontierPredecessors[edge.to] = edge;
						nodeQueue.UpdatePriority( edge.to, estimatedTotalCostViaNode );
					}
				}
			}

			List<int> pathList = new List<int>();
			for ( int node = end;
				  shortestPathPredecessors[node] != null;
				  node = shortestPathPredecessors[node].from )
				pathList.Add( node );
			pathList.Add( start );
			pathList.Reverse();
			return pathList.ToArray();
		}
	}
}