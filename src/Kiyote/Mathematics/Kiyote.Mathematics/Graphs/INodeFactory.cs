namespace Kiyote.Mathematics.Graphs;

public interface INodeFactory<TNode, TEdge> {

	INode<TNode, TEdge> Create(
		TNode? context
	);

}
