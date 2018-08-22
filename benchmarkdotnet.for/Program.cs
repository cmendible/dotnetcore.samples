using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Use BenchmarkRunner.Run to Benchmark your code
            var summary = BenchmarkRunner.Run<ForEachVsFor>();
        }
    }

    // We are using .Net Core we are adding the CoreJobAttribute here.
    [CoreJob(baseline: true)]
    [RPlotExporter, RankColumn]
    public class ForEachVsFor
    {
        private static Random random = new Random();
        private List<int> list;

        public static List<int> RandomIntList(int length)
        {
            int Min = 1;
            int Max = 10;
            return Enumerable
                .Repeat(0, length)
                .Select(i => random.Next(Min, Max))
                .ToList();
        }

        // We wil run the the test for 3 diff list sizes
        [Params(10, 100, 1000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            list = RandomIntList(N);
        }

        // Foreach is ~2 times slower than for
        [Benchmark]
        public void Foreach()
        {
            int total = 0;
            foreach (int i in list)
            {
                total += i;
            }
        }

        // For is ~2 times faster than foreach
        [Benchmark]
        public void For()
        {
            int total = 0;
            for (int i = 0; i < list.Count; i++)
            {
                total += list[i];
            }
        }
    }
}
