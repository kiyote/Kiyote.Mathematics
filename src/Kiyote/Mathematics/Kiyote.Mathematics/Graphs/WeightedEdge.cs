namespace Kiyote.Mathematics.Graphs;

internal sealed class WeightedEdge<TNode, TEdge> : IWeightedEdge<TNode, TEdge> {

	private readonly TEdge? _context;
	private readonly INode<TNode, TEdge> _a;
	private readonly INode<TNode, TEdge> _b;
	private readonly int _weight;

	public WeightedEdge(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		int weight
	) {
		_context = context;
		_a = a;
		_b = b;
		_weight = weight;
	}

	TEdge? IEdge<TNode, TEdge>.Context => _context;

	INode<TNode, TEdge> IEdge<TNode, TEdge>.A => _a;

	INode<TNode, TEdge> IEdge<TNode, TEdge>.B => _b;

	int IWeightedEdge<TNode, TEdge>.Weight => _weight;
}
