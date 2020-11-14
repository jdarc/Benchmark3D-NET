using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Benchmark.Engine
{
    public sealed class Importer
    {
        private const string NewMaterialDirective = "newmtl ";
        private const string UseMaterialDirective = "usemtl ";
        private const string VertexDirective = "v ";
        private const string TextureCoordinateDirective = "vt ";
        private const string FaceDirective = "f ";

        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector2> _uvs = new List<Vector2>();
        private readonly Dictionary<string, Material> _materials = new Dictionary<string, Material>();
        private readonly Dictionary<string, List<float>> _triangleBuckets = new Dictionary<string, List<float>>();
        private List<float> _currentBucket;

        public Model Read(string path)
        {
            foreach (var line in Encoding.Default.GetString(Unzip(path)).Split("\n"))
            {
                if (line.StartsWith(NewMaterialDirective, StringComparison.OrdinalIgnoreCase))
                {
                    var parts = line.Substring(NewMaterialDirective.Length).Split(' ');
                    _materials[parts[0]] = Material.Create((Bitmap) Image.FromStream(new MemoryStream(Convert.FromBase64String(parts[1]))));
                    _triangleBuckets[parts[0]] = new List<float>(65536);
                }
                else if (line.StartsWith(VertexDirective, StringComparison.OrdinalIgnoreCase))
                {
                    var data = line.Substring(VertexDirective.Length).Split(' ').Select(float.Parse).ToArray();
                    _vertices.Add(new Vector3(data[0], data[1], data[2]));
                }
                else if (line.StartsWith(TextureCoordinateDirective, StringComparison.OrdinalIgnoreCase))
                {
                    var data = line.Substring(TextureCoordinateDirective.Length).Split(' ').Select(float.Parse).ToArray();
                    _uvs.Add(new Vector2(data[0], 1 - data[1]));
                }
                else if (line.StartsWith(FaceDirective, StringComparison.OrdinalIgnoreCase))
                {
                    var chunks = line.Substring(FaceDirective.Length).Split(' ');
                    var indices = chunks.Select(s => s.Split('/').Select(s => int.Parse(s) - 1).ToArray()).ToArray();
                    var a = indices[0]; var va = _vertices[a[0]]; var ta = _uvs[a[1]];
                    var b = indices[1]; var vb = _vertices[b[0]]; var tb = _uvs[b[1]];
                    var c = indices[2]; var vc = _vertices[c[0]]; var tc = _uvs[c[1]];
                    _currentBucket.AddRange(new[] {va.X, va.Y, va.Z, ta.X, ta.Y, vb.X, vb.Y, vb.Z, tb.X, tb.Y, vc.X, vc.Y, vc.Z, tc.X, tc.Y});
                }
                else if (line.StartsWith(UseMaterialDirective, StringComparison.OrdinalIgnoreCase))
                {
                    _currentBucket = _triangleBuckets[line.Substring(UseMaterialDirective.Length)];
                }
            }

            return new Model(_triangleBuckets.ToDictionary(k => _materials[k.Key], k => k.Value.ToArray()));
        }

        private static byte[] Unzip(string path)
        {
            var archive = ZipFile.OpenRead(path);
            var bytes = new byte[archive.Entries.First().Length];
            var stream = archive.Entries.First().Open();
            stream.Read(bytes);
            stream.Close();
            return bytes;
        }
    }
}