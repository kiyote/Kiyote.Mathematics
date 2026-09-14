using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs;

[ExcludeFromCodeCoverage]
internal sealed class NodeFactory<TNode, TEdge> : INodeFactory<TNode, TEdge> {

	INode<TNode, TEdge> INodeFactory<TNode, TEdge>.Create(
		TNode? context
	) {
		return new Node<TNode, TEdge>( context );
	}
}
