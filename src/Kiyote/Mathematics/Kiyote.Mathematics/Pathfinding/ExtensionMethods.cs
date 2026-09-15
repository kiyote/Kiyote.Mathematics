using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Mathematics.Pathfinding;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddAStarPathfinder(
		this IServiceCollection services
	) {
		return services
			.AddSingleton<IPathfinder, AStarPathfinder>();
	}
}
