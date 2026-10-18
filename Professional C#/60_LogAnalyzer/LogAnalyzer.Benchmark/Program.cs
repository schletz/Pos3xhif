using BenchmarkDotNet.Running;
using LogAnalyzer.Application;
using LogAnalyzer.Benchmark;
using System.Diagnostics;

#if DEBUG
    // For testing in debug mode.
    var analyzer = new Analyzer("log.txt");
    analyzer.WriteDemoFile(1000);
    var count1 = analyzer.FindIpLinq("0.0.0.1").Count;
    var count2 = analyzer.FindIpStream("0.0.0.1").Count;
    var count3 = analyzer.FindIpSpan("0.0.0.1").Count;
    var count4 = analyzer.FindIpSpanBlock("0.0.0.1").Count;
    Debug.Assert(count1 > 0 && count1 == count2 && count2 == count3 && count3 == count4);
#else
    // For benchmarking
    BenchmarkRunner.Run<AnalyzerBenchmarks>();
#endif