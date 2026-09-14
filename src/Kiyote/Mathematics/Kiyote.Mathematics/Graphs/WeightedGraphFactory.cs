using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs;

[ExcludeFromCodeCoverage]
internal sealed class WeightedGraphFactory<TNode, TEdge> : IWeightedGraphFactory<TNode, TEdge> {

	private readonly IWeightedEdgeFactory<TNode, TEdge> _weightedEdgeFactory;

	public WeightedGraphFactory(
		IWeightedEdgeFactory<TNode, TEdge> weightedEdgeFactory
	) {
		ArgumentNullException.ThrowIfNull( weightedEdgeFactory );
		_weightedEdgeFactory = weightedEdgeFactory;
	}

	IWeightedGraph<TNode, TEdge> IWeightedGraphFactory<TNode, TEdge>.Create() {
		return new WeightedGraph<TNode, TEdge>( _weightedEdgeFactory );
	}
}
