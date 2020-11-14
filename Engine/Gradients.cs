namespace Benchmark.Engine
{
    public readonly struct Gradients
    {
        public readonly float _1OverZdX;
        public readonly float _1OverZdY;
        public readonly float _ZOverZdX;
        public readonly float _ZOverZdY;
        public readonly float TuOverZdX;
        public readonly float TuOverZdY;
        public readonly float TvOverZdX;
        public readonly float TvOverZdY;

        public Gradients(ref Vertex a, ref Vertex c, ref Vertex b)
        {
            var pac = a.Position - c.Position;
            var pbc = b.Position - c.Position;
            var tac = a.TexCoord - c.TexCoord;
            var tbc = b.TexCoord - c.TexCoord;
            var oneOverDx = 1 / (pbc.X * pac.Y - pac.X * pbc.Y);
            
            _1OverZdX = oneOverDx * (pbc.W * pac.Y - pac.W * pbc.Y);
            _1OverZdY = oneOverDx * (pac.W * pbc.X - pbc.W * pac.X);
            
            _ZOverZdX = oneOverDx * (pbc.Z * pac.Y - pac.Z * pbc.Y);
            _ZOverZdY = oneOverDx * (pac.Z * pbc.X - pbc.Z * pac.X);
            
            TuOverZdX = oneOverDx * (tbc.X * pac.Y - tac.X * pbc.Y);
            TuOverZdY = oneOverDx * (tac.X * pbc.X - tbc.X * pac.X);
            
            TvOverZdX = oneOverDx * (tbc.Y * pac.Y - tac.Y * pbc.Y);
            TvOverZdY = oneOverDx * (tac.Y * pbc.X - tbc.Y * pac.X);
        }
    }
}