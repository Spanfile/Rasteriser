using System;
using System.Drawing;
using static Rasteriser.Helper;

namespace Rasteriser.ObjMdl
{
    public class ObjModelRenderer
    {
        const int UnitSizePixels = 110;
        const int ViewDepth = 10;
        const float ZNear = 0f;
        const float ZFar = 50f;

        ObjModel model;
        Vector3[] vertices;
        Tuple<int, int, int>[] faces;

        Vector3 light;

        Matrix projection;
        Matrix viewport;
        Matrix view;

        Matrix rot;

        Drawer drawer;

        public ObjModelRenderer(ObjModel model, Drawer drawer)
        {
            this.drawer = drawer;
            light = new Vector3(0, 0, -1).Normalize();

            var eye = new Vector3(0f, 0f, 8f);
            var center = Vector3.Zero;
            var aspect = (float)drawer.Width / drawer.Height;

            projection = ProjectionMatrix(FovYFromFovX(90f, aspect), aspect, ZNear, ZFar);
            viewport = ViewportMatrix(drawer.Width, drawer.Height, ViewDepth, drawer.Width, drawer.Height);
            view = LookAtMatrix(eye, center, Vector3.Up);

            rot = Matrix.Identity(4, 4);

            SetModel(model);
        }

        public void SetModel(ObjModel model)
        {
            this.model = model;
            vertices = model.Vertices;
            faces = model.Faces;
        }

        public void Draw(float frametime)
        {
            if (frametime > 0)
                rot = rot.RotateY(DegToRad(1f / frametime));

            //Parallel.ForEach(model.Faces, face =>
            foreach (var face in model.Faces)
            {
                var v1 = vertices[face.Item1] * view * rot * projection * viewport;
                var v2 = vertices[face.Item2] * view * rot * projection * viewport;
                var v3 = vertices[face.Item3] * view * rot * projection * viewport;

                var normal = (v3 - v1).Cross(v2 - v1).Normalize();
                var intensity = light.Dot(normal);

                if (intensity > 0)
                    drawer.DepthTestTriangle(v1, v2, v3, Color.FromArgb(RGB((byte)(intensity * 255), (byte)(intensity * 255), (byte)(intensity * 255))));
                //drawer.DepthTestTriangle(v1, v2, v3, Color.FromArgb(RGB(255, 255, 255)));
            }
            //});
        }
    }
}
