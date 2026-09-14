namespace Kiyote.Mathematics.Graphs;

public interface IEdge<TNode, TEdge> {

	TEdge? Context { get; }

	INode<TNode, TEdge> A { get; }

	INode<TNode, TEdge> B { get; }
}
