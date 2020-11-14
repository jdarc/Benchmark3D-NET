using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Benchmark
{
    public static class Info
    {
        public const string Attribution = "Mountain King by Pierre-Antoine (https://sketchfab.com/pa)";
        public static readonly string Version = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        public static readonly Bitmap Logo = new Bitmap("Content/Logo.png");
    }
}