using System;
using System.Diagnostics;
using System.Linq;

namespace Benchmark.Engine
{
    internal sealed class FpsCounter
    {
        private readonly double[] _samples = Enumerable.Range(0, 256).Select(x => Stopwatch.Frequency / 60.0).ToArray();
        private int _index;

        public void Add(double sample) => _samples[255 & _index++] = sample;

        public double Average() => Stopwatch.Frequency / Math.Max(_samples.Average(), double.MinValue);
    }
}