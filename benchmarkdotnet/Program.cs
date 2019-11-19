using System;
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
            var summary = BenchmarkRunner.Run<StringCompareVsEquals>();
        }
    }

    // We are using .Net Core so add we are adding the CoreJobAttribute here.
    [SimpleJob(baseline: true)]
    [RPlotExporter, RankColumn]
    public class StringCompareVsEquals
    {
        private static Random random = new Random();
        private string s1;
        private string s2;

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // We wil run the the test for 2 diff string lengths: 10 & 100
        [Params(10, 100, 10000)]
        public int N;


        // Create two random strings for each set of params
        [GlobalSetup]
        public void Setup()
        {
            s1 = RandomString(N);
            s2 = RandomString(N);
        }

        // This is the slow way of comparing strings, so let's benchmark it.
        [Benchmark]
        public bool Slow() => s1.ToLower() == s2.ToLower();

        // This is the fast way of comparing strings, so let's benchmark it.
        [Benchmark]
        public bool Fast() => string.Compare(s1, s2, true) == 0;

        // This is the fastest!!! way of comparing strings, so let's benchmark it.
        [Benchmark]
        public bool Faster() => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
    }
}
