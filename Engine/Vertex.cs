using System.Numerics;

namespace Benchmark.Engine
{
    internal struct Vertex
    {
        public Vector4 Position;
        public Vector2 TexCoord;

        public Vertex(int offset, float[] src)
        {
            Position = new Vector4(src[offset + 0], src[offset + 1], src[offset + 2], 1);
            TexCoord = new Vector2(src[offset + 3], src[offset + 4]);
        }

        public void Transform(ref Matrix4x4 transform) => Position = Vector4.Transform(Position, transform);

        public void ToScreen(int width, int height)
        {
            Position.W = 1 / Position.W;
            Position.X = 0.5F * (1 + Position.X * Position.W) * width;
            Position.Y = 0.5F * (1 - Position.Y * Position.W) * height;
            Position.Z = 0.5F * (1 + Position.Z * Position.W);
            TexCoord *= Position.W;
        }
    }
}