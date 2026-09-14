namespace Kiyote.Mathematics.Graphs;

public interface IWeightedEdgeFactory<TNode, TEdge> : IEdgeFactory<TNode, TEdge> {

	IWeightedEdge<TNode, TEdge> Create(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		int weight
	);

}
