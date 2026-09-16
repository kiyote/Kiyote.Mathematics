using System.Diagnostics.CodeAnalysis;
using Kiyote.Geometry.Grids;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kiyote.Mathematics.Pathfinding.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
internal sealed class AStarPathfinderTests {

	private IPathfinder _pathfinder;

	[SetUp]
	public void SetUp() {
		_pathfinder = new AStarPathfinder( NullLogger<AStarPathfinder>.Instance );
	}

	[Test]
	public void TryFindPath_NullGrid_Throws() {
		Assert.That(
			() => _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
				null!,
				0,
				0,
				0,
				0,
				default,
				default,
				out _
			),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryFindPath_StartOutOfBounds_ReturnsFalse() {
		IGrid<char> grid = TestGrid<char>.FromRows( "..." );

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			-1,
			0,
			2,
			0,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryFindPath_EndOutOfBounds_ReturnsFalse() {
		IGrid<char> grid = TestGrid<char>.FromRows( "..." );

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			3,
			0,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryFindPath_StartNotPassable_ReturnsFalse() {
		IGrid<char> grid = TestGrid<char>.FromRows( "#.." );

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			2,
			0,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryFindPath_EndNotPassable_ReturnsFalse() {
		IGrid<char> grid = TestGrid<char>.FromRows( "..#" );

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			2,
			0,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryFindPath_StartEqualsEnd_ReturnsSingleStep() {
		IGrid<char> grid = TestGrid<char>.FromRows( "..." );

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			1,
			0,
			1,
			0,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.True );
		Assert.That( path, Is.EqualTo( [new GridCell<char>( 1, 0, '.' )] ) );
	}

	[Test]
	public void TryFindPath_OpenStraightLine_ReturnsShortestPath() {
		IGrid<char> grid = TestGrid<char>.FromRows( "....." );

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			4,
			0,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.True );
		Assert.That(
			path,
			Is.EqualTo(
				[
					new GridCell<char>( 0, 0, '.' ),
					new GridCell<char>( 1, 0, '.' ),
					new GridCell<char>( 2, 0, '.' ),
					new GridCell<char>( 3, 0, '.' ),
					new GridCell<char>( 4, 0, '.' ),
				]
			)
		);
	}

	[Test]
	public void TryFindPath_DiagonalOpen_UsesDiagonalMove() {
		IGrid<char> grid = TestGrid<char>.FromRows(
			"..",
			".."
		);

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			1,
			1,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.True );
		Assert.That(
			path,
			Is.EqualTo(
				[
					new GridCell<char>( 0, 0, '.' ),
					new GridCell<char>( 1, 1, '.' ),
				]
			)
		);
	}

	[Test]
	public void TryFindPath_DiagonalWithOneOrthogonalOpen_AllowsDiagonalMove() {
		IGrid<char> grid = TestGrid<char>.FromRows(
			"..",
			"#."
		);

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			1,
			1,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.True );
		Assert.That(
			path,
			Is.EqualTo(
				[
					new GridCell<char>( 0, 0, '.' ),
					new GridCell<char>( 1, 1, '.' ),
				]
			)
		);
	}

	[Test]
	public void TryFindPath_DiagonalWithBothOrthogonalsBlocked_DoesNotCutCorner() {
		IGrid<char> grid = TestGrid<char>.FromRows(
			".#",
			"#."
		);

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			1,
			1,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryFindPath_NoRoute_ReturnsFalse() {
		IGrid<char> grid = TestGrid<char>.FromRows(
			".#.",
			".#.",
			".#."
		);

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			2,
			0,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryFindPath_HigherCostCell_PathAvoidsSlowCell() {
		IGrid<char> grid = TestGrid<char>.FromRows(
			"...",
			".S.",
			"..."
		);

		bool result = _pathfinder.TryFindPath<char, NotWallPassabilityStrategy, AvoidSlowCellCostStrategy>(
			grid,
			0,
			0,
			2,
			2,
			default,
			default,
			out IReadOnlyList<GridCell<char>> path
		);

		Assert.That( result, Is.True );
		Assert.That( path, Has.None.Matches<GridCell<char>>( step => step.Cell == 'S' ) );
	}

	[Test]
	public void TryVisitPath_NullVisitor_Throws() {
		IGrid<char> grid = TestGrid<char>.FromRows( "..." );

		Assert.That(
			() => _pathfinder.TryVisitPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
				grid,
				0,
				0,
				2,
				0,
				default,
				default,
				null!
			),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryVisitPath_ValidPath_VisitsEachStepInOrder() {
		IGrid<char> grid = TestGrid<char>.FromRows( "..." );
		List<GridCell<char>> visited = [];

		bool result = _pathfinder.TryVisitPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			2,
			0,
			default,
			default,
			step => {
				visited.Add( step );
				return true;
			}
		);

		Assert.That( result, Is.True );
		Assert.That(
			visited,
			Is.EqualTo(
				[
					new GridCell<char>( 0, 0, '.' ),
					new GridCell<char>( 1, 0, '.' ),
					new GridCell<char>( 2, 0, '.' ),
				]
			)
		);
	}

	[Test]
	public void TryVisitPath_NoRoute_VisitorNotInvoked() {
		IGrid<char> grid = TestGrid<char>.FromRows(
			".#.",
			".#.",
			".#."
		);
		List<GridCell<char>> visited = [];

		bool result = _pathfinder.TryVisitPath<char, NotWallPassabilityStrategy, UniformCostStrategy>(
			grid,
			0,
			0,
			2,
			0,
			default,
			default,
			step => {
				visited.Add( step );
				return true;
			}
		);

		Assert.That( result, Is.False );
		Assert.That( visited, Is.Empty );
	}

}
