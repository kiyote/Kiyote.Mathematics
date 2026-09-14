using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Mathematics.Graphs;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddGraphFactory<TNode, TEdge>(
		this IServiceCollection services
	) {
		return services
			.AddSingleton<INodeFactory<TNode, TEdge>, NodeFactory<TNode, TEdge>>()
			.AddSingleton<IEdgeFactory<TNode, TEdge>, EdgeFactory<TNode, TEdge>>()
			.AddSingleton<IGraphFactory<TNode, TEdge>, GraphFactory<TNode, TEdge>>();
	}

	public static IServiceCollection AddWeightedGraphFactory<TNode, TEdge>(
		this IServiceCollection services
	) {
		return services
			.AddSingleton<INodeFactory<TNode, TEdge>, NodeFactory<TNode, TEdge>>()
			.AddSingleton<IWeightedEdgeFactory<TNode, TEdge>, WeightedEdgeFactory<TNode, TEdge>>()
			.AddSingleton<IWeightedGraphFactory<TNode, TEdge>, WeightedGraphFactory<TNode, TEdge>>();
	}
}
