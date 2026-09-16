using System.Buffers;
using System.Runtime.CompilerServices;
using Kiyote.Geometry.Grids;
using Microsoft.Extensions.Logging;

namespace Kiyote.Mathematics.Pathfinding;

internal sealed class AStarPathfinder : IPathfinder {

	private static readonly (int ColumnOffset, int RowOffset)[] _neighbours = [
		( -1, -1 ), ( 0, -1 ), ( 1, -1 ),
		( -1, 0 ), ( 1, 0 ),
		( -1, 1 ), ( 0, 1 ), ( 1, 1 )
	];

	private readonly ILogger<AStarPathfinder> _logger;

	public AStarPathfinder(
		ILogger<AStarPathfinder> logger
	) {
		_logger = logger;
	}

	bool IPathfinder.TryFindPath<T, TPassability, TCost>(
		IGrid<T> grid,
		int startColumn,
		int startRow,
		int endColumn,
		int endRow,
		TPassability isPassable,
		TCost cost,
		out IReadOnlyList<GridCell<T>> path
	) where T : default {
		return DoTryFindPath(
			_logger,
			grid,
			startColumn,
			startRow,
			endColumn,
			endRow,
			isPassable,
			cost,
			out path
		);
	}

	bool IPathfinder.TryVisitPath<T, TPassability, TCost>(
		IGrid<T> grid,
		int startColumn,
		int startRow,
		int endColumn,
		int endRow,
		TPassability isPassable,
		TCost cost,
		Func<GridCell<T>, bool> visitor
	) where T : default {
		return DoTryVisitPath(
			_logger,
			grid,
			startColumn,
			startRow,
			endColumn,
			endRow,
			isPassable,
			cost,
			visitor
		);
	}

	private static bool DoTryFindPath<T, TPassability, TCost>(
		ILogger<AStarPathfinder> logger,
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
		where TCost : ICellsStrategy<T, double>
	{
		ArgumentNullException.ThrowIfNull( grid );
		ArgumentNullException.ThrowIfNull( isPassable );
		ArgumentNullException.ThrowIfNull( cost );

		path = [];

		if( !TryPrepare<T, TPassability>( logger, grid, startColumn, startRow, endColumn, endRow, isPassable, out (int Column, int Row) start, out (int Column, int Row) end ) ) {
			return false;
		}

		if( start == end ) {
			path = [new GridCell<T>( start.Column, start.Row, grid[start.Column, start.Row] )];
			return true;
		}

		bool found = RunSearch<T, TPassability, TCost, object?, IReadOnlyList<GridCell<T>>>(
			grid,
			start,
			end,
			isPassable,
			cost,
			null,
			static ( g, cameFrom, width, s, e, _ ) => BuildPath( g, cameFrom, width, s, e ),
			out IReadOnlyList<GridCell<T>> result
		);

		if( found ) {
			path = result;
		}

		return found;
	}

	private static bool DoTryVisitPath<T, TPassability, TCost>(
		ILogger<AStarPathfinder> logger,
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
		where TCost : ICellsStrategy<T, double>
	{
		ArgumentNullException.ThrowIfNull( grid );
		ArgumentNullException.ThrowIfNull( isPassable );
		ArgumentNullException.ThrowIfNull( cost );
		ArgumentNullException.ThrowIfNull( visitor );

		if( !TryPrepare<T, TPassability>( logger, grid, startColumn, startRow, endColumn, endRow, isPassable, out (int Column, int Row) start, out (int Column, int Row) end ) ) {
			return false;
		}

		if( start == end ) {
			visitor( new GridCell<T>( start.Column, start.Row, grid[start.Column, start.Row] ) );
			return true;
		}

		return RunSearch<T, TPassability, TCost, Func<GridCell<T>, bool>, bool>(
			grid,
			start,
			end,
			isPassable,
			cost,
			visitor,
			static ( g, cameFrom, width, s, e, v ) => {
				VisitPath( g, cameFrom, width, s, e, v );
				return true;
			},
			out _
		);
	}

	private static bool TryPrepare<T, TPassability>(
		ILogger<AStarPathfinder> logger,
		IGrid<T> grid,
		int startColumn,
		int startRow,
		int endColumn,
		int endRow,
		TPassability isPassable,
		out (int Column, int Row) start,
		out (int Column, int Row) end
	)
		where TPassability : ICellStrategy<T, bool>
	{
		start = default;
		end = default;

		if( !IsInBounds( grid, startColumn, startRow ) || !IsInBounds( grid, endColumn, endRow ) ) {
			logger.OutOfBounds();
			return false;
		}

		if( !isPassable.Evaluate( new GridCell<T>( startColumn, startRow, grid[startColumn, startRow] ) )
			|| !isPassable.Evaluate( new GridCell<T>( endColumn, endRow, grid[endColumn, endRow] ) )
		) {
			logger.NotPassable();
			return false;
		}

		start = (startColumn, startRow);
		end = (endColumn, endRow);
		return true;
	}

	private static bool RunSearch<T, TPassability, TCost, TState, TResult>(
		IGrid<T> grid,
		(int Column, int Row) start,
		(int Column, int Row) end,
		TPassability isPassable,
		TCost cost,
		TState state,
		Func<IGrid<T>, int[], int, (int Column, int Row), (int Column, int Row), TState, TResult> onFound,
		out TResult result
	)
		where TPassability : ICellStrategy<T, bool>
		where TCost : ICellsStrategy<T, double>
	{
		result = default!;

		int width = grid.Width;
		int size = width * grid.Height;

		double[] bestCost = ArrayPool<double>.Shared.Rent( size );
		int[] cameFrom = ArrayPool<int>.Shared.Rent( size );
		(int Column, int Row)[] heapElements = ArrayPool<(int Column, int Row)>.Shared.Rent( size );
		double[] heapPriorities = ArrayPool<double>.Shared.Rent( size );
		try {
			Array.Fill( bestCost, double.PositiveInfinity, 0, size );
			Array.Fill( cameFrom, -1, 0, size );

			bestCost[Index( start, width )] = 0.0;

			int heapCount = 0;
			HeapPush( ref heapElements, ref heapPriorities, ref heapCount, start, Heuristic( start, end ) );

			while( HeapTryPop( heapElements, heapPriorities, ref heapCount, out (int Column, int Row) current ) ) {
				if( current == end ) {
					result = onFound( grid, cameFrom, width, start, end, state );
					return true;
				}

				double currentCost = bestCost[Index( current, width )];

				foreach( (int columnOffset, int rowOffset) in _neighbours ) {
					(int Column, int Row) neighbour = (current.Column + columnOffset, current.Row + rowOffset);

					if( !IsInBounds( grid, neighbour.Column, neighbour.Row ) ) {
						continue;
					}

					T? neighbourCell = grid[neighbour.Column, neighbour.Row];
					GridCell<T> neighbourStep = new( neighbour.Column, neighbour.Row, neighbourCell );
					if( !isPassable.Evaluate( neighbourStep ) ) {
						continue;
					}

					if( columnOffset != 0
						&& rowOffset != 0
						&& !IsDiagonalMovePossible( grid, isPassable, current, columnOffset, rowOffset )
					) {
						continue;
					}

					GridCell<T> currentStep = new( current.Column, current.Row, grid[current.Column, current.Row] );
					double candidateCost = currentCost + cost.Evaluate( currentStep, neighbourStep );
					int neighbourIndex = Index( neighbour, width );
					if( bestCost[neighbourIndex] <= candidateCost ) {
						continue;
					}

					bestCost[neighbourIndex] = candidateCost;
					cameFrom[neighbourIndex] = Index( current, width );
					HeapPush( ref heapElements, ref heapPriorities, ref heapCount, neighbour, candidateCost + Heuristic( neighbour, end ) );
				}
			}

			return false;
		} finally {
			ArrayPool<double>.Shared.Return( bestCost );
			ArrayPool<int>.Shared.Return( cameFrom );
			ArrayPool<(int Column, int Row)>.Shared.Return( heapElements );
			ArrayPool<double>.Shared.Return( heapPriorities );
		}
	}

	private static void HeapPush(
		ref (int Column, int Row)[] elements,
		ref double[] priorities,
		ref int count,
		(int Column, int Row) element,
		double priority
	) {
		if( count == elements.Length ) {
			int newCapacity = elements.Length * 2;
			(int Column, int Row)[] newElements = ArrayPool<(int Column, int Row)>.Shared.Rent( newCapacity );
			double[] newPriorities = ArrayPool<double>.Shared.Rent( newCapacity );

			Array.Copy( elements, newElements, count );
			Array.Copy( priorities, newPriorities, count );

			ArrayPool<(int Column, int Row)>.Shared.Return( elements );
			ArrayPool<double>.Shared.Return( priorities );

			elements = newElements;
			priorities = newPriorities;
		}

		int i = count++;
		elements[i] = element;
		priorities[i] = priority;

		while( i > 0 ) {
			int parent = (i - 1) / 2;
			if( priorities[parent] <= priorities[i] ) {
				break;
			}

			(elements[parent], elements[i]) = (elements[i], elements[parent]);
			(priorities[parent], priorities[i]) = (priorities[i], priorities[parent]);
			i = parent;
		}
	}

	private static bool HeapTryPop(
		(int Column, int Row)[] elements,
		double[] priorities,
		ref int count,
		out (int Column, int Row) element
	) {
		if( count == 0 ) {
			element = default;
			return false;
		}

		element = elements[0];
		count--;
		elements[0] = elements[count];
		priorities[0] = priorities[count];

		int i = 0;
		while( true ) {
			int left = (2 * i) + 1;
			int right = (2 * i) + 2;
			int smallest = i;

			if( left < count && priorities[left] < priorities[smallest] ) {
				smallest = left;
			}

			if( right < count && priorities[right] < priorities[smallest] ) {
				smallest = right;
			}

			if( smallest == i ) {
				break;
			}

			(elements[smallest], elements[i]) = (elements[i], elements[smallest]);
			(priorities[smallest], priorities[i]) = (priorities[i], priorities[smallest]);
			i = smallest;
		}

		return true;
	}

	private static int Index(
		(int Column, int Row) position,
		int width
	) {
		return (position.Row * width) + position.Column;
	}

	private static bool IsInBounds<T>(
		IGrid<T> grid,
		int column,
		int row
	) {
		return column >= 0
			&& row >= 0
			&& column < grid.Width
			&& row < grid.Height;
	}

	private static bool IsDiagonalMovePossible<T, TPassability>(
		IGrid<T> grid,
		TPassability isPassable,
		(int Column, int Row) current,
		int columnOffset,
		int rowOffset
	)
		where TPassability : ICellStrategy<T, bool>
	{
		bool horizontalOpen = IsInBounds( grid, current.Column + columnOffset, current.Row )
			&& isPassable.Evaluate( new GridCell<T>( current.Column + columnOffset, current.Row, grid[current.Column + columnOffset, current.Row] ) );

		bool verticalOpen = IsInBounds( grid, current.Column, current.Row + rowOffset )
			&& isPassable.Evaluate( new GridCell<T>( current.Column, current.Row + rowOffset, grid[current.Column, current.Row + rowOffset] ) );

		return horizontalOpen || verticalOpen;
	}

	private static double Heuristic(
		(int Column, int Row) a,
		(int Column, int Row) b
	) {
		return Math.Max( Math.Abs( a.Column - b.Column ), Math.Abs( a.Row - b.Row ) );
	}

	private static IReadOnlyList<GridCell<T>> BuildPath<T>(
		IGrid<T> grid,
		int[] cameFrom,
		int width,
		(int Column, int Row) start,
		(int Column, int Row) end
	) {
		List<GridCell<T>> steps = [
			new GridCell<T>( end.Column, end.Row, grid[end.Column, end.Row] )
		];

		(int Column, int Row) current = end;
		while( current != start ) {
			int parentIndex = cameFrom[Index( current, width )];
			current = (parentIndex % width, parentIndex / width);
			steps.Add( new GridCell<T>( current.Column, current.Row, grid[current.Column, current.Row] ) );
		}

		steps.Reverse();
		return steps;
	}

	private static void VisitPath<T>(
		IGrid<T> grid,
		int[] cameFrom,
		int width,
		(int Column, int Row) start,
		(int Column, int Row) end,
		Func<GridCell<T>, bool> visitor
	) {
		int length = 1;
		(int Column, int Row) current = end;
		while( current != start ) {
			int parentIndex = cameFrom[Index( current, width )];
			current = (parentIndex % width, parentIndex / width);
			length++;
		}

		GridCell<T>[] steps = ArrayPool<GridCell<T>>.Shared.Rent( length );
		try {
			current = end;
			for( int i = length - 1; i >= 0; i-- ) {
				steps[i] = new GridCell<T>( current.Column, current.Row, grid[current.Column, current.Row] );
				if( i > 0 ) {
					int parentIndex = cameFrom[Index( current, width )];
					current = (parentIndex % width, parentIndex / width);
				}
			}

			for( int i = 0; i < length; i++ ) {
				if( !visitor( steps[i] ) ) {
					break;
				}
			}
		} finally {
			ArrayPool<GridCell<T>>.Shared.Return( steps, RuntimeHelpers.IsReferenceOrContainsReferences<GridCell<T>>() );
		}
	}
}
