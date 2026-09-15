using Microsoft.Extensions.Logging;

namespace Kiyote.Mathematics.Pathfinding;

internal static partial class LogMessages {

	[LoggerMessage(
		Level = LogLevel.Debug,
		Message = "Start or end coordinates are outside the bounds of the grid."
	)]
	public static partial void OutOfBounds( this ILogger logger );

	[LoggerMessage(
		Level = LogLevel.Debug,
		Message = "Start or end cell is not passable."
	)]
	public static partial void NotPassable( this ILogger logger );

}
