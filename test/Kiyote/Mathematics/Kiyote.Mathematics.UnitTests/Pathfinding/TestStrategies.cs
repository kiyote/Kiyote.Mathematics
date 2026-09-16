using Kiyote.Geometry.Grids;

namespace Kiyote.Mathematics.Pathfinding.UnitTests;

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

internal readonly struct AvoidSlowCellCostStrategy : ICellsStrategy<char, double> {

	public double Evaluate( GridCell<char> source, GridCell<char> destination ) {
		return destination.Cell == 'S' ? 10.0 : 1.0;
	}

}
