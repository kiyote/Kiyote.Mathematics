namespace Kiyote.Mathematics.Pathfinding;

public interface IPassabilityStrategy<T> {

	bool IsPassable(
		PathStep<T> pathStep
	);

}
