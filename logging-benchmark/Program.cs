using BenchmarkDotNet.Running;
using logging_benchmark;

BenchmarkRunner.Run([typeof(LoggingBenchmarks)]);