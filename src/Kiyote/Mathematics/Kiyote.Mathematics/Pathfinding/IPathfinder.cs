using Kiyote.Geometry.Grids;

namespace Kiyote.Mathematics.Pathfinding;

public interface IPathfinder {

	bool TryFindPath<T, TPassability, TCost>(
		IGrid<T> grid,
		int startColumn,
		int startRow,
		int endColumn,
		int endRow,
		TPassability isPassable,
		TCost cost,
		out IReadOnlyList<GridCell<T>> path
	)
		where TPassability : ICellStrategy<T, bool>
		where TCost : ICellsStrategy<T, double>;

	bool TryVisitPath<T, TPassability, TCost>(
		IGrid<T> grid,
		int startColumn,
		int startRow,
		int endColumn,
		int endRow,
		TPassability isPassable,
		TCost cost,
		Func<GridCell<T>, bool> visitor
	)
		where TPassability : ICellStrategy<T, bool>
		where TCost : ICellsStrategy<T, double>;

}
