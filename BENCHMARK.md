# Benchmarks

```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.8893/24H2/2024Update/HudsonValley)
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.400-preview.0.26322.102
  [Host] : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3

Job=MediumRun  Toolchain=InProcessNoEmitToolchain  IterationCount=15  
LaunchCount=1  WarmupCount=10
```

## FastPoissonDiscPointFactory
| Method         | Mean        | Error     | StdDev    | Allocated |
|--------------- |------------:|----------:|----------:|----------:|
| Fill_100x100   |    148.9 us |   3.16 us |   2.96 us |   4.93 KB |
| Fill_500x500   |  3,958.4 us |  68.09 us |  63.70 us | 117.57 KB |
| Fill_1000x1000 | 15,608.4 us | 130.05 us | 115.29 us | 469.55 KB |

# FastRandom
| Method                                    | Mean      | Error     | StdDev    | Allocated |
|------------------------------------------ |----------:|----------:|----------:|----------:|
| FastRandom_NextInt                        |  1.336 ns | 0.0149 ns | 0.0139 ns |         - |
| FastRandom_NextInt_UpperBound             |  3.551 ns | 0.3087 ns | 0.2737 ns |         - |
| FastRandom_NextInt_LowerBoundUpperBound   |  4.559 ns | 0.0741 ns | 0.0657 ns |         - |
| FastRandom_NextBool                       |  2.019 ns | 0.1915 ns | 0.1791 ns |         - |
| FastRandom_NextUInt                       |  1.554 ns | 0.0698 ns | 0.0653 ns |         - |
| FastRandom_NextDouble                     |  1.277 ns | 0.1057 ns | 0.0937 ns |         - |
| FastRandom_NextFloat                      |  1.302 ns | 0.0742 ns | 0.0694 ns |         - |
| FastRandom_NextFloat_LowerBoundUpperBound |  3.241 ns | 0.1602 ns | 0.1499 ns |         - |
| FastRandom_NextBytes                      | 32.398 ns | 1.1589 ns | 1.0840 ns |         - |

# FastRandom vs System.Random
| Method                  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| FastRandom_NextBytes    | 32.791 ns | 1.0293 ns | 0.9628 ns |  1.62 |    0.08 |         - |          NA |
| SystemRandom_NextBytes  | 20.314 ns | 0.8628 ns | 0.8071 ns |  1.00 |    0.06 |         - |          NA |
|                         |           |           |           |       |         |           |             |
| FastRandom_NextDouble   |  1.439 ns | 0.0953 ns | 0.0891 ns |  0.45 |    0.03 |         - |          NA |
| SystemRandom_NextDouble |  3.189 ns | 0.1441 ns | 0.1348 ns |  1.00 |    0.06 |         - |          NA |
|                         |           |           |           |       |         |           |             |
| FastRandom_NextFloat    |  1.302 ns | 0.0373 ns | 0.0331 ns |  0.40 |    0.01 |         - |          NA |
| SystemRandom_NextFloat  |  3.226 ns | 0.0644 ns | 0.0602 ns |  1.00 |    0.03 |         - |          NA |
|                         |           |           |           |       |         |           |             |
| FastRandom_NextInt      |  1.394 ns | 0.0686 ns | 0.0608 ns |  0.56 |    0.03 |         - |          NA |
| SystemRandom_NextInt    |  2.482 ns | 0.0758 ns | 0.0709 ns |  1.00 |    0.04 |         - |          NA |

# MidpointDisplacementNoisyEdgeFactory
| Method                     | Mean       | Error    | StdDev   | Allocated |
|--------------------------- |-----------:|---------:|---------:|----------:|
| Create_Amplitude05_Levels3 |   494.3 ns | 10.59 ns |  9.91 ns |     912 B |
| Create_Amplitude05_Levels4 | 1,020.3 ns | 18.96 ns | 17.74 ns |    1680 B |


# AStarPathfinder
| Method                       | Mean     | Error    | StdDev   | Allocated |
|----------------------------- |---------:|---------:|---------:|----------:|
| AStarPathfinder_TryFindPath  | 367.6 us | 16.02 us | 14.98 us |    3288 B |
| AStarPathfinder_TryVisitPath | 362.5 us | 22.85 us | 20.26 us |       2 B |
