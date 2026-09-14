namespace Kiyote.Mathematics.Graphs;

public interface IWeightedGraph<TNode, TEdge> : IGraph<TNode, TEdge> {

	bool TryConnectNodes(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		TEdge? edgeContext,
		int weight,
		out IWeightedEdge<TNode, TEdge> edge
	);

	bool TryGetWeightedDistance(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out int distance
	);

	bool TryGetWeightedPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out IReadOnlyList<INode<TNode, TEdge>> path
	);

	bool TryVisitWeightedPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		Action<INode<TNode, TEdge>> visitor
	);

}
