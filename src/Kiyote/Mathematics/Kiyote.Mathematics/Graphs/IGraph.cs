namespace Kiyote.Mathematics.Graphs;

public interface IGraph<TNode, TEdge> {

	IReadOnlyList<INode<TNode, TEdge>> Nodes { get; }

	IReadOnlyList<IEdge<TNode, TEdge>> Edges { get; }

	IReadOnlyList<IEdge<TNode, TEdge>> GetEdgesFor( INode<TNode, TEdge> node );

	IReadOnlyList<INode<TNode, TEdge>> GetConnectedTo( INode<TNode, TEdge> node );

	void AddNode(
		INode<TNode, TEdge> node
	);

	bool RemoveNode(
		INode<TNode, TEdge> node
	);

	bool TryConnectNodes(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		TEdge? edgeContext,
		out IEdge<TNode, TEdge> edge
	);

	bool TryDisconnectNodes(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	);

	bool TryGetDistance(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out int distance
	);

	bool TryGetPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out IReadOnlyList<INode<TNode, TEdge>> path
	);

	bool TryVisitPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		Action<INode<TNode, TEdge>> visitor
	);

	bool IsConnectedTo(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	);

}
