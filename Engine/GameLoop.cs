using System;
using System.Diagnostics;
using Avalonia.Threading;

namespace Benchmark.Engine
{
    public sealed class GameLoop
    {
        private readonly Stopwatch _watch = new Stopwatch();
        private readonly DispatcherTimer _timer;

        public GameLoop(IGame game)
        {
            _watch.Restart();
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(8)};
            var tock = _watch.ElapsedMilliseconds;
            _timer.Tick += delegate
            {
                var tick = tock;
                tock = _watch.ElapsedTicks;
                game.Update((tock - tick) / 1000000000.0);
                game.Render();
            };
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}