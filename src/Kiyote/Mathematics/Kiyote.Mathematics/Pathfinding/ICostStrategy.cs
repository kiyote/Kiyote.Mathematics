namespace Kiyote.Mathematics.Pathfinding;

public interface ICostStrategy<T> {

	double GetCost(
		PathStep<T> source,
		PathStep<T> destination
	);

}
