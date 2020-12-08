using System;
using Gtk;

namespace Benchmark
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.Init();

            var app = new Application("org.zynaps.benchmark", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.ShowAll();
            Application.Run();
        }
    }
}