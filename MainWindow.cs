using System.Diagnostics;
using Gtk;

namespace Benchmark
{
    internal sealed class MainWindow : Window
    {
        private static readonly double Resolution = 1000.0 / Stopwatch.Frequency;
        
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
            
            var tock = 0.0;
            AddTickCallback((_, frameClock) =>
            {
                var tick = tock;
                tock = frameClock.FrameTime;
                viewport.Update((tock - tick) * Resolution);
                viewport.Render();
                return true;
            });
        }
    }
}