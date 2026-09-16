using Kiyote.Geometry.Grids;

namespace Kiyote.Mathematics.Pathfinding.Benchmarks;

internal readonly struct NotWallPassabilityStrategy : ICellStrategy<char, bool> {

	public bool Evaluate( GridCell<char> pathStep ) {
		return pathStep.Cell != '#';
	}

}

internal readonly struct UniformCostStrategy : ICellsStrategy<char, double> {

	public double Evaluate( GridCell<char> source, GridCell<char> destination ) {
		return 1.0;
	}

}
