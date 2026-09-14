namespace Kiyote.Mathematics.Graphs;

internal sealed class Node<TNode, TEdge> : INode<TNode, TEdge> {

	private readonly TNode? _context;

	public Node(
		TNode? context
	) {
		_context = context;
	}

	TNode? INode<TNode, TEdge>.Context => _context;
}
