using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Mathematics.Randomization;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddRandomization(
		this IServiceCollection services
	) {
		services.AddScoped<IPointFactory, FastPoissonDiscPointFactory>();
		services.AddScoped<IRandom, FastRandom>();
		return services;
	}
}
