using System;
using System.Linq;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<StringCompareVsEquals>();
        }
    }

    [CoreJob(baseline: true)]
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

        [Params(10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            s1 = RandomString(N);
            s2 = RandomString(N);
        }

        [Benchmark]
        public bool Slow() => s1.ToLower() == s2.ToLower();

        [Benchmark]
        public bool Fast() => string.Compare(s1, s2, true) == 0;
    }
}
