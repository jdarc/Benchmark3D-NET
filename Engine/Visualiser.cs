using System;
using System.Numerics;
using System.Threading;

namespace Benchmark.Engine
{
    public sealed class Visualiser
    {
        private const int Opaque = 0xFF << 24;

        private readonly Rasteriser _rasteriser;
        private Model _model;
        private double _angle;
        private bool _loaded;

        public Visualiser(int width, int height) => _rasteriser = new Rasteriser(width, height);

        public void Blit(in IntPtr dstPtr) => _rasteriser.Blit(dstPtr);

        public void RenderFrame(double seconds)
        {
            if (seconds <= 0) return;
            _angle += seconds;

            var aspectRatio = _rasteriser.Width / (float) _rasteriser.Height;
            _rasteriser.Clear(Opaque | 0x433649);
            _rasteriser.View = Matrix4x4.CreateLookAt(new Vector3(40, 30, 60), new Vector3(0, 20, 0), Vector3.UnitY);
            _rasteriser.Projection = Matrix4x4.CreatePerspectiveFieldOfView((float) (Math.PI / 4.0), aspectRatio, 1, 500);
            _rasteriser.World = Matrix4x4.CreateRotationY((float) _angle) * Matrix4x4.CreateScale(10);

            if (!_loaded)
            {
                _loaded = true;
                new Thread(() => _model = new Importer().Read("Content/Model.zip")).Start();
            }

            _model?.Render(_rasteriser);
        }
    }
}