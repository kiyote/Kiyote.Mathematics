namespace Kiyote.Mathematics.Pathfinding.Benchmarks;

internal readonly struct NotWallPassabilityStrategy : IPassabilityStrategy<char> {

	public bool IsPassable( PathStep<char> pathStep ) {
		return pathStep.Cell != '#';
	}

}

internal readonly struct UniformCostStrategy : ICostStrategy<char> {

	public double GetCost( PathStep<char> source, PathStep<char> destination ) {
		return 1.0;
	}

}
