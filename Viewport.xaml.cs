using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Visuals.Media.Imaging;
using Benchmark.Engine;

namespace Benchmark
{
    internal sealed class Viewport : UserControl
    {
        private static readonly string Version = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        private static readonly Bitmap Logo = new Bitmap("Content/Logo.png");
        private static readonly System.Drawing.Size RenderSize = new System.Drawing.Size(1440, 1800);
        private static readonly Brush Yellow = new SolidColorBrush(Colors.Yellow);
        private static readonly Typeface Typeface = new Typeface("Space Mono", 15, FontStyle.Normal, FontWeight.Medium);
        private static readonly FormattedText FormattedText = new FormattedText {Typeface = Typeface};

        private readonly Stopwatch _watch = new Stopwatch();
        private readonly FpsCounter _fpsCounter = new FpsCounter();
        private readonly Visualiser _visualiser = new Visualiser(RenderSize.Width, RenderSize.Height);
        private readonly WriteableBitmap _bitmap = new WriteableBitmap(new PixelSize(RenderSize.Width, RenderSize.Height), new Vector(96, 96));

        public Viewport() => AvaloniaXamlLoader.Load(this);

        public void Update(double seconds)
        {
            _watch.Restart();
            _visualiser.RenderFrame(seconds);
            _watch.Stop();

            _fpsCounter.Add(_watch.ElapsedTicks);
        }

        public override void Render(DrawingContext context)
        {
            using var frameBuffer = _bitmap.Lock();
            _visualiser.Blit(frameBuffer.Address);

            context.DrawImage(_bitmap, 1, new Rect(_bitmap.Size), Bounds, BitmapInterpolationMode.HighQuality);

            FormattedText.Text = $"Frames/Second: {_fpsCounter.Average().ToString("F2", CultureInfo.InvariantCulture)}";
            context.DrawText(Yellow, new Point(16, 16), FormattedText);

            FormattedText.Text = Version;
            context.DrawText(Yellow, new Point(Bounds.Width - FormattedText.Bounds.Width - 16, 16), FormattedText);
            
            var destRect = new Rect(new Point(Bounds.Width - Logo.Size.Width - 8, Bounds.Height - Logo.Size.Height - 8), Logo.Size);
            context.DrawImage(Logo, 1, new Rect(Logo.Size), destRect);
        }
    }
}