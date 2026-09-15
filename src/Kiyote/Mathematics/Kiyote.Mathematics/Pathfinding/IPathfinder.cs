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
		out IReadOnlyList<PathStep<T>> path
	)
		where TPassability : IPassabilityStrategy<T>
		where TCost : ICostStrategy<T>;

	bool TryVisitPath<T, TPassability, TCost>(
		IGrid<T> grid,
		int startColumn,
		int startRow,
		int endColumn,
		int endRow,
		TPassability isPassable,
		TCost cost,
		Func<PathStep<T>, bool> visitor
	)
		where TPassability : IPassabilityStrategy<T>
		where TCost : ICostStrategy<T>;

}
