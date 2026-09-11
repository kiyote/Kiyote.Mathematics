using Kiyote.Geometry;
using Kiyote.Mathematics.Visualizer.Noises;
using Kiyote.Mathematics.Visualizer.Randomization;

namespace Kiyote.Mathematics.Visualizer;

public static class Program {

	public static void Main() {
		string outputFolder = Path.Combine( Path.GetTempPath(), "Kiyote.Mathematics.Visualizer" );
		if( !Directory.Exists( outputFolder ) ) {
			Directory.CreateDirectory( outputFolder );
		}
		ISize bounds = new Point( 1000, 1000 );


		// FastPoissonDiscPointFactory
		var fastPoissonDiscPointFactory = new FastPoissonDiscPointFactoryVisualizer(
			outputFolder,
			bounds
		);
		fastPoissonDiscPointFactory.Visualize();

		// FastRandom
		var fastRandom = new FastRandomVisualizer(
			outputFolder,
			bounds
		);
		fastRandom.Visualize();

		// MidpointDisplacementNoisyEdgeFactory
		var midpointDisplacementNoisyEdgeFactory = new MidpointDisplacementNoisyEdgeFactoryVisualizer(
			outputFolder,
			bounds
		);
		midpointDisplacementNoisyEdgeFactory.Visualize();
	}
}
