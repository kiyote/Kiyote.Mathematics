using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs;

[ExcludeFromCodeCoverage]
internal sealed class EdgeFactory<TNode, TEdge> : IEdgeFactory<TNode, TEdge> {

	IEdge<TNode, TEdge> IEdgeFactory<TNode, TEdge>.Create(
		TEdge? context,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	) {
		return new Edge<TNode, TEdge>( context, a, b );
	}
}
