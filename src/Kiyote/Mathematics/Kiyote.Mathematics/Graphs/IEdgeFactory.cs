namespace Kiyote.Mathematics.Graphs;

public interface IEdgeFactory<TNode, TEdge> {

	IEdge<TNode, TEdge> Create(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	);

}
