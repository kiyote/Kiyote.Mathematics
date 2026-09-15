using System.Diagnostics.CodeAnalysis;
using Kiyote.Geometry.Grids;

namespace Kiyote.Mathematics.Pathfinding.Benchmarks;

[ExcludeFromCodeCoverage]
internal sealed class BenchmarkGrid<T> : IGrid<T> {

	private readonly T[][] _cells;

	public BenchmarkGrid(
		T[][] cells
	) {
		_cells = cells;
		Width = cells.Length;
		Height = cells.Length == 0 ? 0 : cells[0].Length;
	}

	public T this[int column, int row] => _cells[column][row];

	public int Column => 0;

	public int Row => 0;

	public int Width { get; }

	public int Height { get; }

	public bool TryAttach( IGrid<T> grid, int column, int row ) {
		throw new NotSupportedException();
	}

	public bool TryDetach( IGrid<T> grid ) {
		throw new NotSupportedException();
	}

	public IGrid<T> GetGrid( int column, int row ) {
		throw new NotSupportedException();
	}

	public IGrid<T> GetGrid( int column, int row, bool recursive ) {
		throw new NotSupportedException();
	}

	public void VisitGrids( int column, int row, Action<IGrid<T>, int, int> visitor ) {
		throw new NotSupportedException();
	}

}
