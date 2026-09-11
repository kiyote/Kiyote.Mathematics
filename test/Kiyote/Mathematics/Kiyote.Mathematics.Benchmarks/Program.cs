using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Kiyote.Mathematics.Noises.Benchmarks;
using Kiyote.Mathematics.Randomization.Benchmarks;

ManualConfig config = DefaultConfig.Instance
	.AddExporter( MarkdownExporter.Default )
	.AddJob( Job
		 .MediumRun
		 .WithLaunchCount( 1 )
		 .WithToolchain( InProcessNoEmitToolchain.Instance ) );

BenchmarkSwitcher
	.FromTypes( [
		typeof( FastPoissonDiscPointFactoryBenchmarks ),
		typeof( FastRandomBenchmarks ),
		typeof( FastRandomVsSystemBenchmarks ),
		typeof( MidpointDisplacementNoisyEdgeFactoryBenchmarks )
	] )
	.RunAll( config, args );


