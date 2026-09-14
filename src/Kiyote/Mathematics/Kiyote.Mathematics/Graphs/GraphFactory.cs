using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs;

[ExcludeFromCodeCoverage]
internal sealed class GraphFactory<TNode, TEdge> : IGraphFactory<TNode, TEdge> {

	private readonly IEdgeFactory<TNode, TEdge> _edgeFactory;

	public GraphFactory(
		IEdgeFactory<TNode, TEdge> edgeFactory
	) {
		ArgumentNullException.ThrowIfNull( edgeFactory );
		_edgeFactory = edgeFactory;
	}

	IGraph<TNode, TEdge> IGraphFactory<TNode, TEdge>.Create() {
		return new Graph<TNode, TEdge>( _edgeFactory );
	}
}
