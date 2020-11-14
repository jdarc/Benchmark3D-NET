using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Benchmark.Engine
{
    public sealed class Rasteriser
    {
        private readonly int[] _colorBuffer;
        private readonly float[] _depthBuffer;

        public readonly int Width;
        public readonly int Height;

        public Matrix4x4 World = Matrix4x4.Identity;
        public Matrix4x4 View = Matrix4x4.Identity;
        public Matrix4x4 Projection = Matrix4x4.Identity;

        public Rasteriser(int width, int height)
        {
            Width = width;
            Height = height;
            _colorBuffer = new int[width * height];
            _depthBuffer = new float[width * height];
        }

        public void Blit(in IntPtr dstPtr) => _colorBuffer.CopyTo(dstPtr);

        public void Clear(int color, float depth = 1)
        {
            _colorBuffer.Fill(color);
            _depthBuffer.Fill(depth);
        }

        public void Draw(Material material, float[] buffer)
        {
            var transform = World * View * Projection;
            Parallel.For(0, buffer.Length / 15, i =>
            {
                unsafe
                {
                    var p0 = new Vertex(i * 15 + 0, buffer);
                    var p1 = new Vertex(i * 15 + 5, buffer);
                    var p2 = new Vertex(i * 15 + 10, buffer);

                    p0.Transform(ref transform);
                    p1.Transform(ref transform);
                    p2.Transform(ref transform);
                    if (IsBackFacing(ref p0, ref p1, ref p2)) return;

                    p0.ToScreen(Width, Height);
                    p1.ToScreen(Width, Height);
                    p2.ToScreen(Width, Height);
                    if (Math.Max(p0.Position.X, Math.Max(p1.Position.X, p2.Position.X)) < 0 ||
                        Math.Min(p0.Position.X, Math.Min(p1.Position.X, p2.Position.X)) >= Width) return;

                    Vertex* a;
                    Vertex* b;
                    Vertex* c;
                    var leftIsMiddle = true;
                    if (p0.Position.Y < p1.Position.Y)
                    {
                        if (p2.Position.Y < p0.Position.Y)
                        {
                            a = &p2;
                            b = &p0;
                            c = &p1;
                        }
                        else if (p1.Position.Y < p2.Position.Y)
                        {
                            a = &p0;
                            b = &p1;
                            c = &p2;
                        }
                        else
                        {
                            a = &p0;
                            b = &p2;
                            c = &p1;
                            leftIsMiddle = false;
                        }
                    }
                    else
                    {
                        if (p2.Position.Y < p1.Position.Y)
                        {
                            a = &p2;
                            b = &p1;
                            c = &p0;
                            leftIsMiddle = false;
                        }
                        else if (p0.Position.Y < p2.Position.Y)
                        {
                            a = &p1;
                            b = &p0;
                            c = &p2;
                            leftIsMiddle = false;
                        }
                        else
                        {
                            a = &p1;
                            b = &p2;
                            c = &p0;
                        }
                    }

                    if (c->Position.Y < 0 || a->Position.Y >= Height) return;
                    var gradients = new Gradients(ref *a, ref *b, ref *c);

                    var ttb = new Edge(ref gradients, ref *a, ref *c);
                    if (ttb.Height <= 0) return;

                    var ttm = new Edge(ref gradients, ref *a, ref *b);
                    if (ttm.Height > 0 && ttm.Y < Height)
                        if (leftIsMiddle) ScanConvert(material, ref gradients, ref ttm, ref ttb, ttm.Height);
                        else ScanConvert(material, ref gradients, ref ttb, ref ttm, ttm.Height);

                    var mtb = new Edge(ref gradients, ref *b, ref *c);
                    if (mtb.Height <= 0 || mtb.Y >= Height) return;
                    if (leftIsMiddle) ScanConvert(material, ref gradients, ref mtb, ref ttb, mtb.Height);
                    else ScanConvert(material, ref gradients, ref ttb, ref mtb, mtb.Height);
                }
            });
        }

        private void ScanConvert(Material material, ref Gradients gradients, ref Edge left, ref Edge right, int total)
        {
            for (var y = 0; y < total; ++y)
            {
                var offset = left.Y * Width;

                var x1 = (int) Math.Max(0, Math.Ceiling(left.X));
                var x2 = (int) Math.Min(Width, Math.Ceiling(right.X));

                var preStepX = x1 - left.X;
                var z = left.Z + preStepX * gradients._ZOverZdX;
                var _1OverZ = left._1OverZ + preStepX * gradients._1OverZdX;
                var tuOverZ = left.TuOverZ + preStepX * gradients.TuOverZdX;
                var tvOverZ = left.TvOverZ + preStepX * gradients.TvOverZdX;

                Rasterise(material, gradients, offset + x1, offset + x2, z, _1OverZ, tuOverZ, tvOverZ);

                left.Step();
                right.StepXOnly();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Rasterise(Material mat, Gradients grad, int x1, int x2, float z, float _1OverZ, float tuOverZ, float tvOverZ)
        {
            for (var mem = x1; mem < x2; ++mem)
            {
                if (z < _depthBuffer[mem])
                {
                    _depthBuffer[mem] = z;
                    _colorBuffer[mem] = mat.Sample(tuOverZ / _1OverZ, tvOverZ / _1OverZ);
                }

                z += grad._ZOverZdX;
                _1OverZ += grad._1OverZdX;
                tuOverZ += grad.TuOverZdX;
                tvOverZ += grad.TvOverZdX;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBackFacing(ref Vertex a, ref Vertex b, ref Vertex c)
        {
            var bvw = 1 / b.Position.W;
            var cvw = 1 / c.Position.W;
            var avy = a.Position.Y / a.Position.W;
            var avx = a.Position.X / a.Position.W;
            return (avy - b.Position.Y * bvw) * (c.Position.X * cvw - avx) <= (avy - c.Position.Y * cvw) * (b.Position.X * bvw - avx);
        }
    }
}