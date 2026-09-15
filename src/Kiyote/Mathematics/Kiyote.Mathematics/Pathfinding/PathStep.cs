namespace Kiyote.Mathematics.Pathfinding;

public readonly record struct PathStep<T>(
	int Column,
	int Row,
	T? Cell
);
