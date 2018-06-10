using System;
using System.Drawing;

namespace Rasteriser
{
    public static class Helper
    {
        public static int RGB(byte r, byte g, byte b)
        {
            return r | (g << 8) | (b << 16);
        }

        public static bool PointInTriangle(Point p, Point p0, Point p1, Point p2)
        {
            var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
            var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;

            if (A < 0)
            {
                s = -s;
                t = -t;
                A = -A;
            }

            return s > 0 && t > 0 && (s + t) <= A;
        }

        public static Vector3 Barycentric(Point p, Vector3 p0, Vector3 p1, Vector3 p3)
        {
            var v0 = new Vector3(p1.X - p0.X, p1.Y - p0.Y, 0);
            var v1 = new Vector3(p3.X - p0.X, p3.Y - p0.Y, 0);
            var v2 = new Vector3(p.X - p0.X, p.Y - p0.Y, 0);

            var d00 = v0.Dot(v0);
            var d01 = v0.Dot(v1);
            var d11 = v1.Dot(v1);
            var d20 = v2.Dot(v0);
            var d21 = v2.Dot(v1);
            var denom = d00 * d11 - d01 * d01;

            var v = (d11 * d20 - d01 * d21) / denom;
            var w = (d00 * d21 - d01 * d20) / denom;
            var u = 1f - v - w;

            return new Vector3(u, v, w);
        }

        public static float AreaOfTriangle(Vector3 p0, Vector3 p1, Vector3 p2) =>
            Math.Abs((p0.X * (p1.Y - p2.Y) + p1.X * (p2.Y - p0.Y) + p2.X * (p0.Y - p1.Y)) / 2f);

        public static float FovYFromFovX(float fovX, float aspect) => 2f * (float)Math.Atan(aspect * Math.Tan(fovX / 2f));

        public static Matrix ProjectionMatrix(float fovY, float aspect, float zNear, float zFar)
        {
            var tanY = (float)Math.Tan(fovY / 2);

            if (tanY == 0.0f)
                tanY = 0.001f;

            if (aspect == 0.0f)
                aspect = 0.001f;

            var yScale = 1f / tanY;
            var xScale = yScale / aspect;

            var proj = Matrix.Identity(4, 4);

            proj[0, 0] = xScale;
            proj[1, 1] = yScale;
            proj[2, 2] = zFar / (zFar - zNear);
            proj[3, 2] = -1f;
            proj[2, 3] = (zFar * zNear) / (zFar - zNear);

            return proj;
        }

        public static Matrix ViewportMatrix(int maxX, int maxY, int maxZ, int w, int h)
        {
            var view = Matrix.Identity(4, 4);

            view[0, 0] = w / 2;
            view[1, 1] = -h / 2;
            view[2, 2] = maxZ;

            view[3, 0] = w / 2;
            view[3, 1] = h / 2;
            view[3, 3] = 1f;

            return view;
        }

        public static Matrix LookAtMatrix(Vector3 eye, Vector3 at, Vector3 up)
        {
            var z = (eye - at).Normalize();
            var x = up.Cross(z).Normalize();
            var y = z.Cross(x);

            var mat = Matrix.Identity(4, 4);

            mat[0, 0] = x.X;
            mat[1, 0] = y.X;
            mat[2, 0] = z.X;

            mat[0, 1] = x.Y;
            mat[1, 2] = y.Y;
            mat[2, 3] = z.Y;

            mat[0, 1] = x.Z;
            mat[1, 2] = y.Z;
            mat[2, 3] = z.Z;

            mat[0, 3] = x.Dot(eye);
            mat[1, 3] = y.Dot(eye);
            mat[2, 3] = z.Dot(eye);
            mat[3, 3] = 1f;

            return mat;
        }

        public static float DegToRad(float deg) => deg * (float)(Math.PI / 180f);
        public static float RadToDeg(float rad) => rad * (float)(180f / Math.PI);
    }
}
