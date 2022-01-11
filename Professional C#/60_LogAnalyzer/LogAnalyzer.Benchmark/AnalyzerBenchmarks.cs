using BenchmarkDotNet.Attributes;
using LogAnalyzer.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Benchmark
{
    [MemoryDiagnoser]
    public class AnalyzerBenchmarks
    {
        private readonly string _filename = "log.txt";
        private readonly int _lines = 10000;
        private readonly Analyzer _analyzer;
        public AnalyzerBenchmarks()
        {
            _analyzer = new Analyzer(_filename);
        }
        /// <summary>
        /// GlobalSetup. Wird vor jedem Methodenbenchmark, aber nicht vor jedem Lauf ausgeführt.
        /// Es wird also 3x eine Datei geschrieben.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Schreibe {_lines} Zeilen in {_filename}...");
            _analyzer.WriteDemoFile(_lines);
            var size = new FileInfo(_filename).Length / (decimal)(1 << 20);
            Console.WriteLine($"{size:0.0} MB geschrieben.");
            Console.ForegroundColor = color;
        }
        [Benchmark]
        public void FindIpLinq()
        {
            _analyzer.FindIpLinq("0.0.0.1");
        }
        [Benchmark]
        public void FindIpStream()
        {
            _analyzer.FindIpStream("0.0.0.1");
        }
        [Benchmark]
        public void FindIpSpan()
        {
            _analyzer.FindIpSpan("0.0.0.1");
        }
        [Benchmark]
        public void FindIpSpanBlock()
        {
            _analyzer.FindIpSpanBlock("0.0.0.1");
        }
    }
}
