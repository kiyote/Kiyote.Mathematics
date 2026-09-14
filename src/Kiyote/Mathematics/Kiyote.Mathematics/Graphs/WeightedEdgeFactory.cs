using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs;

[ExcludeFromCodeCoverage]
internal sealed class WeightedEdgeFactory<TNode, TEdge> : IWeightedEdgeFactory<TNode, TEdge> {

	IEdge<TNode, TEdge> IEdgeFactory<TNode, TEdge>.Create(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	) {
		return DoCreate( context, a, b, 1 );
	}

	IWeightedEdge<TNode, TEdge> IWeightedEdgeFactory<TNode, TEdge>.Create(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		int weight
	) {
		return DoCreate( context, a, b, weight );
	}

	private static IWeightedEdge<TNode, TEdge> DoCreate(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		int weight
	) {
		return new WeightedEdge<TNode, TEdge>( context, a, b, weight );
	}
}
