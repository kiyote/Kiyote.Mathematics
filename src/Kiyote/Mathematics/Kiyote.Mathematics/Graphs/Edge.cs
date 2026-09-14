namespace Kiyote.Mathematics.Graphs;

internal sealed class Edge<TNode, TEdge> : IEdge<TNode, TEdge> {

	private readonly TEdge? _context;
	private readonly INode<TNode, TEdge> _a;
	private readonly INode<TNode, TEdge> _b;

	public Edge(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	) {
		_context = context;
		_a = a;
		_b = b;
	}

	TEdge? IEdge<TNode, TEdge>.Context => _context;

	INode<TNode, TEdge> IEdge<TNode, TEdge>.A => _a;

	INode<TNode, TEdge> IEdge<TNode, TEdge>.B => _b;
}
