using Kiyote.Geometry.Grids;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kiyote.Mathematics.Pathfinding.Benchmarks;

[MemoryDiagnoser( false )]
[MarkdownExporterAttribute.GitHub]
public class AStarPathfinderBenchmarks {

	private const int Size = 100;

	private readonly IPathfinder _pathfinder;
	private readonly IGrid<char> _grid;
	private readonly NotWallPassabilityStrategy _isPassable;
	private readonly UniformCostStrategy _cost;

	public AStarPathfinderBenchmarks() {
		_pathfinder = new AStarPathfinder( NullLogger<AStarPathfinder>.Instance );
		_grid = BuildGrid( Size );
		_isPassable = new NotWallPassabilityStrategy();
		_cost = new UniformCostStrategy();
	}

	[Benchmark]
	public void AStarPathfinder_TryFindPath() {
		_pathfinder.TryFindPath(
			_grid,
			0,
			0,
			Size - 1,
			Size - 1,
			_isPassable,
			_cost,
			out IReadOnlyList<GridCell<char>> _
		);
	}

	[Benchmark]
	public void AStarPathfinder_TryVisitPath() {
		_pathfinder.TryVisitPath(
			_grid,
			0,
			0,
			Size - 1,
			Size - 1,
			_isPassable,
			_cost,
			static step => true
		);
	}

	private static IGrid<char> BuildGrid(
		int size
	) {
		char[][] cells = new char[size][];
		for( int column = 0; column < size; column++ ) {
			cells[column] = new char[size];
			for( int row = 0; row < size; row++ ) {
				// A fixed, deterministic wall pattern that leaves a path open
				// between the top-left and bottom-right corners of the grid.
				bool isWall = ( column % 4 == 3 ) && ( row % 5 != 4 );
				cells[column][row] = isWall ? '#' : '.';
			}
		}

		return new BenchmarkGrid<char>( cells );
	}
}
