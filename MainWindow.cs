using System.Diagnostics;
using Gtk;

namespace Benchmark
{
    internal sealed class MainWindow : Window
    {
        private static readonly double TimerResolution = 1.0 / Stopwatch.Frequency;

        public MainWindow() : base(WindowType.Toplevel)
        {
            Title = ".NET Core Benchmark";
            Resizable = false;

            var height = Screen.Display.PrimaryMonitor.Workarea.Height * 90.0 / 100.0;
            var width = height * 720.0 / 900.0;
            SetSizeRequest((int) width, (int) height);

            DeleteEvent += (sender, args) => Application.Quit();

            var viewport = new Viewport();
            Add(viewport);

            var tock = 0L;
            var watch = new Stopwatch();
            watch.Start();
            AddTickCallback((_, frameClock) =>
            {
                var tick = tock;
                tock = watch.ElapsedTicks;
                viewport.Update((tock - tick) * TimerResolution);
                viewport.Render();
                return true;
            });
        }
    }
}