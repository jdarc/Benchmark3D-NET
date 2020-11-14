using System.Collections.Generic;

namespace Benchmark.Engine
{
    internal sealed class Model
    {
        private readonly IDictionary<Material, float[]> _parts;

        public Model(IDictionary<Material, float[]> parts)
        {
            _parts = parts;
        }

        public void Render(Rasteriser rasteriser)
        {
            foreach (var (material, buffer) in _parts) rasteriser.Draw(material, buffer);
        }
    }
}