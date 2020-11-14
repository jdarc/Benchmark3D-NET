using System;
using System.Runtime.CompilerServices;

namespace Benchmark.Engine
{
    internal struct Edge
    {
        public int Y;
        public readonly int Height;

        public float X;
        private readonly float _xStep;

        public float Z;
        private readonly float _zStep;

        public float _1OverZ;
        private readonly float _1OverZStep;

        public float TuOverZ;
        private readonly float _tuOverZStep;

        public float TvOverZ;
        private readonly float _tvOverZStep;

        public Edge(ref Gradients g, ref Vertex a, ref Vertex b)
        {
            Y = Math.Max(0, (int) Math.Ceiling(a.Position.Y));
            Height = (int) Math.Ceiling(b.Position.Y) - Y;

            if (Height > 0)
            {
                var yPreStep = Y - a.Position.Y;
                _xStep = (b.Position.X - a.Position.X) / (b.Position.Y - a.Position.Y);
                X = yPreStep * _xStep + a.Position.X;

                var xPreStep = X - a.Position.X;
                Z = a.Position.Z + yPreStep * g._ZOverZdY + xPreStep * g._ZOverZdX;
                _zStep = _xStep * g._ZOverZdX + g._ZOverZdY;

                _1OverZ = a.Position.W + yPreStep * g._1OverZdY + xPreStep * g._1OverZdX;
                _1OverZStep = _xStep * g._1OverZdX + g._1OverZdY;

                TuOverZ = a.TexCoord.X + yPreStep * g.TuOverZdY + xPreStep * g.TuOverZdX;
                _tuOverZStep = _xStep * g.TuOverZdX + g.TuOverZdY;

                TvOverZ = a.TexCoord.Y + yPreStep * g.TvOverZdY + xPreStep * g.TvOverZdX;
                _tvOverZStep = _xStep * g.TvOverZdX + g.TvOverZdY;
            }
            else
            {
                X = _xStep = Z = _zStep = _1OverZ = _1OverZStep = TuOverZ = _tuOverZStep = TvOverZ = _tvOverZStep = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Step()
        {
            Y++;
            X += _xStep;
            Z += _zStep;
            _1OverZ += _1OverZStep;
            TuOverZ += _tuOverZStep;
            TvOverZ += _tvOverZStep;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StepXOnly() => X += _xStep;
    }
}