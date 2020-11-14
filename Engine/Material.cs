using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Benchmark.Engine
{
    internal sealed class Material
    {
        private readonly int _mask;
        private readonly int _width;
        private readonly int _height;
        private readonly int[] _buffer;

        private Material(int width, int height, int[] buffer)
        {
            _width = width;
            _height = height;
            _buffer = buffer;
            _mask = _width * _height - 1;
        }

        public int Sample(float u, float v) => _buffer[_mask & (int) (v * _height) * _width + (int) (u * _width)];

        public static Material Create(Bitmap image)
        {
            var pixels = new int[image.Width * image.Height];
            var data = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
            image.UnlockBits(data);
            return new Material(image.Width, image.Height, pixels);
        }
    }
}