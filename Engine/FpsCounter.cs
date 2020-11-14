using System;
using System.Linq;

namespace Benchmark.Engine
{
    public sealed class FpsCounter
    {
        private readonly double[] _samples = new double[256];
        private int _index;

        public void Add(double sample) => _samples[255 & _index++] = sample;

        public double Average() => 1000000000.0 / Math.Max(_samples.Average(), double.MinValue);
    }
}