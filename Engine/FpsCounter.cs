using System;
using System.Linq;

namespace Benchmark.Engine
{
    internal sealed class FpsCounter
    {
        private readonly double[] _samples = Enumerable.Range(0, 256).Select(x => 16000000.0).ToArray();
        private int _index;

        public void Add(double sample) => _samples[255 & _index++] = sample;

        public double Average() => 1000000000.0 / Math.Max(_samples.Average(), double.MinValue);
    }
}