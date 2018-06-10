using System;
using System.Diagnostics;
using Rasteriser.ObjMdl;
using System.Threading;

namespace Rasteriser
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var window = new Window(800, 800, "Rasteriser"))
            {
                var draw = new Drawer(window);
                var obj = new ObjModelRenderer(ObjModel.FromFile("E:/triangle.obj"), draw);

                var timer = new Stopwatch();
                var frametime = 0f;

                while (true)
                {
                    timer.Restart();

                    window.Clear();
                    obj.Draw(frametime);
                    draw.End();

                    if (window.DispatchMessages())
                        break;

                    timer.Stop();
                    frametime = (float)timer.Elapsed.TotalMilliseconds;
                    window.SetTitle($"Rasteriser ({Math.Round(1 / timer.Elapsed.TotalSeconds, 2):0.00} FPS)");
                }
            }
        }
    }
}
