using System;
using System.Diagnostics;
using Avalonia.Threading;

namespace Benchmark.Engine
{
    internal sealed class GameLoop
    {
        public static readonly double Frequency = Stopwatch.Frequency;
        private readonly Stopwatch _watch = new Stopwatch();
        private readonly DispatcherTimer _timer;
        private long _tock;

        public GameLoop(IGame game)
        {
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(16)};
            _timer.Tick += delegate
            {
                var tick = _tock;
                _tock = _watch.ElapsedTicks;
                game.Update((_tock - tick) / Frequency);
                game.Render();
            };
        }

        public void Start()
        {
            _tock = 0;
            _watch.Restart();
            _timer.Start();
        }

        public void Stop() => _timer.Stop();
    }
}