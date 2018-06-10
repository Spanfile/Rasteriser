using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace Rasteriser.ObjMdl
{
    public class ObjModel
    {
        public static ObjModel FromFile(string filename)
        {
            var timer = Stopwatch.StartNew();

            var mdl = new ObjModel();
            var lines = File.ReadLines(filename);
            var vertices = new List<Vector3>();

            var maxX = 0f;
            var minX = 0f;
            var maxY = 0f;
            var minY = 0f;
            var maxZ = 0f;
            var minZ = 0f;

            foreach (var line in lines)
            {
                if (line.StartsWith("#"))
                    continue;

                var args = line.Split(' ');
                switch (args[0])
                {
                    default:
                        continue;

                    case "v":
                        var x = float.Parse(args[1], CultureInfo.InvariantCulture);
                        var y = float.Parse(args[2], CultureInfo.InvariantCulture);
                        var z = float.Parse(args[3], CultureInfo.InvariantCulture);

                        maxX = Math.Max(x, maxX);
                        minX = Math.Min(x, minX);
                        maxY = Math.Max(y, maxY);
                        minY = Math.Min(y, minY);
                        maxZ = Math.Max(z, maxZ);
                        minZ = Math.Min(z, minZ);

                        vertices.Add(new Vector3(x, y, z));
                        break;

                    case "f":
                        var i1Args = args[1].Split('/');
                        var i1 = int.Parse(i1Args[0], CultureInfo.InvariantCulture);

                        var i2Args = args[2].Split('/');
                        var i2 = int.Parse(i2Args[0], CultureInfo.InvariantCulture);

                        var i3Args = args[3].Split('/');
                        var i3 = int.Parse(i3Args[0], CultureInfo.InvariantCulture);

                        mdl.faces.Add(Tuple.Create(i1 - 1, i2 - 1, i3 - 1));
                        break;
                }
            }

            mdl.dimOrigin = new Vector3(minX, minY, minZ);
            mdl.dimSize = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            mdl.dimCenter = new Vector3(mdl.dimOrigin.X + (mdl.dimSize.X / 2f), mdl.dimOrigin.Y + (mdl.dimSize.Y / 2f), mdl.dimOrigin.Z + (mdl.dimSize.Z / 2f));

            foreach (var vert in vertices)
            {
                mdl.vertices.Add(new Vector3(
                    vert.X,
                    vert.Y,
                    vert.Z));
            }

            timer.Stop();
            Console.WriteLine($"{Path.GetFileName(filename)} loaded in {timer.Elapsed.TotalMilliseconds} ms with {mdl.vertices.Count} vertices and {mdl.faces.Count} faces, dimensions {mdl.dimOrigin} : {mdl.dimSize} : {mdl.dimCenter}");

            return mdl;
        }

        public Vector3[] Vertices => vertices.ToArray();
        public Tuple<int, int, int>[] Faces => faces.ToArray();

        public Vector3 DimensionOrigin => dimOrigin;
        public Vector3 DimensionSize => dimSize;
        public Vector3 DimensionCenter => dimCenter;

        List<Vector3> vertices;
        List<Tuple<int, int, int>> faces;

        Vector3 dimOrigin;
        Vector3 dimSize;
        Vector3 dimCenter;

        private ObjModel()
        {
            vertices = new List<Vector3>();
            faces = new List<Tuple<int, int, int>>();
        }
    }
}
