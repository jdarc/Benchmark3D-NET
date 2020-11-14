using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Benchmark.Engine;

namespace Benchmark
{
    public sealed class MainWindow : Window, IGame
    {
        private readonly Viewport _viewport;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            _viewport = (Viewport) ((DockPanel) Content).Children.First();

            Height = Screens.Primary.Bounds.Height * 90.0 / 100.0;
            Width = Height * 720.0 / 900.0;

            var loop = new GameLoop(this);
            Opened += (sender, args) => loop.Start();
            Closed += (sender, args) => loop.Stop();
        }

        public override void Show()
        {
            base.Show();
            var x = (int) ((Screens.Primary.Bounds.Width - Width) / 2.0);
            var y = (int) ((Screens.Primary.Bounds.Height - Height) / 2.0);
            Position = new PixelPoint(x, y);
        }

        public void Update(double seconds) => _viewport.Update(seconds);
        public void Render() => _viewport.InvalidateVisual();
    }
}