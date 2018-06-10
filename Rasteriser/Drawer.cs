using System;
using System.Drawing;
using static Rasteriser.Helper;

namespace Rasteriser
{
    public class Drawer
    {
        public int Width => window.Width;
        public int Height => window.Height;

        Window window;

        float[,] zBuffer;

        public Drawer(Window window)
        {
            this.window = window;

            zBuffer = new float[window.Width, window.Height];
        }

        public void End()
        {
            window.Invalidate();
            Array.Clear(zBuffer, 0, zBuffer.Length);
        }

        public void Point(Point point, Color color) => window.SetPixel(point.X, point.Y, color);

        public void Line(Point start, Point end, Color color)
        {
            var x = start.X;
            var y = start.Y;

            var w = end.X - x;
            var h = end.Y - y;

            var dx1 = 0;
            var dy1 = 0;
            var dx2 = 0;
            var dy2 = 0;

            if (w < 0)
                dx1 = -1;
            else if (w > 0)
                dx1 = 1;

            if (h < 0)
                dy1 = -1;
            else if (h > 0)
                dy1 = 1;

            if (w < 0)
                dx2 = -1;
            else if (w > 0)
                dx2 = 1;

            var longest = Math.Abs(w);
            var shortest = Math.Abs(h);

            if (longest <= shortest)
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);

                if (h < 0)
                    dy2 = -1;
                else if (h > 0)
                    dy2 = 1;

                dx2 = 0;
            }

            var numerator = longest >> 1;
            for (var i = 0; i <= longest; i++)
            {
                window.SetPixel(x, y, color);
                numerator += shortest;

                if (numerator >= longest)
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        public void Triangle(Point p1, Point p2, Point p3, Color color)
        {
            //if (p1.Y > p2.Y)
            //{
            //    var temp = p1;
            //    p1 = p2;
            //    p2 = temp;
            //}

            //if (p1.Y > p3.Y)
            //{
            //    var temp = p1;
            //    p1 = p3;
            //    p3 = temp;
            //}

            //if (p2.Y > p3.Y)
            //{
            //    var temp = p2;
            //    p2 = p3;
            //    p3 = temp;
            //}

            Line(p1, p2, color);
            Line(p2, p3, color);
            Line(p3, p1, color);
        }

        public void DepthTestTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
        {
            var a = AreaOfTriangle(p1, p2, p3);

            var boundMin = new Point(
                (int)Math.Max(Math.Min(Math.Min(p1.X, p2.X), p3.X), 0),
                (int)Math.Max(Math.Min(Math.Min(p1.Y, p2.Y), p3.Y), 0));

            var boundMax = new Point(
                (int)Math.Min(Math.Max(Math.Max(p1.X, p2.X), p3.X), Width - 1),
                (int)Math.Min(Math.Max(Math.Max(p1.Y, p2.Y), p3.Y), Height - 1));

            for (var y = boundMin.Y; y <= boundMax.Y; y++)
            {
                for (var x = boundMin.X; x <= boundMax.X; x++)
                {
                    var p = new Point(x, y);
                    var bc = Barycentric(p, p1, p2, p3);

                    if (bc.X < 0 || bc.Y < 0 || bc.Z < 0)
                        continue;

                    var z = bc.X * p1.Z + bc.Y * p2.Z + bc.Z * p3.Z;

                    if (zBuffer[x, y] > z)
                    {
                        zBuffer[x, y] = z;
                        window.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
}
