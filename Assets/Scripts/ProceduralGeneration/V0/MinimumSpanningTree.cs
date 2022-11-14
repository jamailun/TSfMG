using System;
using System.Collections.Generic;
using DelaunatorSharp;

/// <summary>
/// Adapted from https://www.programmingalgorithms.com/algorithm/kruskal%27s-algorithm/.
/// </summary>
public class MinimumSpanningTree {

	public struct Edge {
		public int Source;
		public int Destination;
		public double Weight;
		public IEdge realEdgeRef;
	}

	public struct Graph {
		public int VerticesCount;
		public int EdgesCount;
		public Edge[] edge;
	}

	public struct Subset {
		public int Parent;
		public int Rank;
	}

	private static double Weight(IEdge edge) {
		double x = edge.P.X - edge.Q.X;
		double y = edge.P.Y - edge.Q.Y;
		return Math.Sqrt(x * x + y * y);
	}

	private readonly Dictionary<int, int> roomIdToArrayIndex = new();
	private int nextArrayId = 0;
	private readonly Graph graph;

	// return indexId
	private int GarantyExistArrayId(int roomId) {
		if( ! roomIdToArrayIndex.ContainsKey(roomId)) {
			roomIdToArrayIndex.Add(roomId, nextArrayId);
			nextArrayId++;
		}
		return roomIdToArrayIndex[roomId];
	}

	private int GetArrayIdSource(IEdge edge) {
		int source = ((LevelGenerator_V0.RoomPlacement) edge.P).Id;
		return GarantyExistArrayId(source);
	}
	private int GetArrayIdTarget(IEdge edge) {
		int target = ((LevelGenerator_V0.RoomPlacement) edge.Q).Id;
		return GarantyExistArrayId(target);
	}

	public MinimumSpanningTree(List<IEdge> edges, int roomsAcceptedAmount) {
		graph = CreateGraph(roomsAcceptedAmount, edges.Count);
		for(int i = 0; i < edges.Count; i++) {
			var edge = edges[i];
			graph.edge[i].Source = GetArrayIdSource(edge);
			graph.edge[i].Destination = GetArrayIdTarget(edge);
			graph.edge[i].Weight = Weight(edge);
			graph.edge[i].realEdgeRef = edge;
			//Debug.Log("(" + graph.edge[i].Source + ") => (" + graph.edge[i].Destination + ")");
		}
		//Debug.Log(roomsAcceptedAmount + " vertices. " + edges.Count + " edges.");
	}

	public List<IEdge> Resolve() {
		Edge[] res = Kruskal(graph);
		return new List<Edge>(res).ConvertAll(e => e.realEdgeRef).FindAll(e => e != null);
	}

	public static Graph CreateGraph(int verticesCount, int edgesCount) {
		Graph graph = new();
		graph.VerticesCount = verticesCount;
		graph.EdgesCount = edgesCount;
		graph.edge = new Edge[graph.EdgesCount];

		return graph;
	}

	private static int Find(Subset[] subsets, int i) {
		if(subsets[i].Parent != i)
			subsets[i].Parent = Find(subsets, subsets[i].Parent);

		return subsets[i].Parent;
	}

	private static void Union(Subset[] subsets, int x, int y) {
		int xroot = Find(subsets, x);
		int yroot = Find(subsets, y);

		if(subsets[xroot].Rank < subsets[yroot].Rank)
			subsets[xroot].Parent = yroot;
		else if(subsets[xroot].Rank > subsets[yroot].Rank)
			subsets[yroot].Parent = xroot;
		else {
			subsets[yroot].Parent = xroot;
			++subsets[xroot].Rank;
		}
	}

	public static Edge[] Kruskal(Graph graph) {
		int verticesCount = graph.VerticesCount;
		Edge[] result = new Edge[verticesCount];
		int i = 0;
		int e = 0;

		Array.Sort(graph.edge, delegate (Edge a, Edge b) {
			return a.Weight.CompareTo(b.Weight);
		});

		Subset[] subsets = new Subset[verticesCount];

		for(int v = 0; v < verticesCount; ++v) {
			subsets[v].Parent = v;
			subsets[v].Rank = 0;
		}

		while(e < verticesCount - 1) {
			Edge nextEdge = graph.edge[i++];
			int x = Find(subsets, nextEdge.Source);
			int y = Find(subsets, nextEdge.Destination);

			if(x != y) {
				result[e++] = nextEdge;
				Union(subsets, x, y);
			}
		}

		return result;
	}

}