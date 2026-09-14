namespace Kiyote.Mathematics.Graphs;

public interface IWeightedEdge<TNode, TEdge> : IEdge<TNode, TEdge> {

	int Weight { get; }
}
