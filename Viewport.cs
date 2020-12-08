using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using Benchmark.Engine;
using Cairo;
using Gtk;

namespace Benchmark
{
    internal sealed class Viewport : DrawingArea
    {
        private static readonly string Version = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        private static readonly Size RenderSize = new Size(1440, 1800);
        private static readonly ImageSurface Logo = new ImageSurface("Content/Logo.png");

        private readonly Stopwatch _watch = new Stopwatch();
        private readonly FpsCounter _fpsCounter = new FpsCounter();
        private readonly Visualiser _visualiser = new Visualiser(RenderSize.Width, RenderSize.Height);
        private readonly ImageSurface _surface = new ImageSurface(Format.Rgb24, RenderSize.Width, RenderSize.Height);

        public void Update(double seconds)
        {
            _watch.Restart();
            _visualiser.RenderFrame(seconds);
            _watch.Stop();

            _fpsCounter.Add(_watch.ElapsedTicks);
            _visualiser.Blit(_surface.DataPtr);
        }

        protected override bool OnDrawn(Context cr)
        {
            var scaleX = (double) AllocatedWidth / _surface.Width;
            var scaleY = (double) AllocatedHeight / _surface.Height;

            cr.Scale(scaleX, scaleY);
            cr.SetSource(_surface);
            cr.Paint();

            cr.Scale(1.0 / scaleX, 1.0 / scaleY);
            cr.SetSource(Logo, AllocatedWidth - Logo.Width - 8, AllocatedHeight - Logo.Height - 8);
            cr.Paint();

            cr.SetSourceRGB(1.0, 1.0, 0.0);
            cr.SelectFontFace("Space Mono", FontSlant.Normal, FontWeight.Normal);
            cr.SetFontSize(15.0);

            var text = $"Frames/Second: {_fpsCounter.Average().ToString("F2", CultureInfo.InvariantCulture)}";
            cr.MoveTo(16, 24);
            cr.ShowText(text);

            cr.MoveTo(AllocatedWidth - cr.FontExtents.MaxXAdvance * Version.Length - 16, 24);
            cr.ShowText(Version);
            return true;
        }

        public void Render() => QueueDraw();
    }
}