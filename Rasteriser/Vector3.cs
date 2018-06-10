using System;

namespace Rasteriser
{
    public struct Vector3
    {
        public static Vector3 Zero => new Vector3(0f, 0f, 0f);
        public static Vector3 Up => new Vector3(0f, 1f, 0f);

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, float scalar) => new Vector3(a.X * scalar, a.Y * scalar, a.Z * scalar);
        public static Vector3 operator /(Vector3 a, float scalar) => new Vector3(a.X / scalar, a.Y / scalar, a.Z / scalar);

        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{X};{Y};{Z}";
        }

        public float Length() => (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

        public Vector3 Normalize()
        {
            var len = Length();
            return new Vector3(X / len, Y / len, Z / len);
        }

        public Vector3 Cross(Vector3 other) => new Vector3(
            Y * other.Z - other.Y * Z,
            X * other.Z - other.X * Z,
            X * other.Y - other.X * Y);

        public float Dot(Vector3 other) => (X * other.X) + (Y * other.Y) + (Z * other.Z);
    }
}
