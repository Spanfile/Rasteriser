using System;

namespace Rasteriser
{
    public struct Matrix
    {
        public static Matrix Identity(int rows, int columns)
        {
            var id = new Matrix(rows, columns);

            for (var i = 0; i < Math.Min(rows, columns); i++)
                id[i, i] = 1f;

            return id;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            //if (a.columns != b.rows)
            //    throw new ArgumentException("Invalid matrices");

            var result = new Matrix(a.rows, b.columns);

            for (var row = 0; row < a.rows; row++)
            {
                for (var col = 0; col < b.columns; col++)
                {
                    for (var inner = 0; inner < a.columns; inner++)
                    {
                        result[row, col] += a[row, inner] * b[inner, col];
                    }
                }
            }

            return result;
        }

        public static Vector3 operator *(Vector3 vec, Matrix mat) => mat * vec;
        public static Vector3 operator *(Matrix mat, Vector3 vec)
        {
            var vecMat = new Matrix(1, 4);
            vecMat[0, 0] = vec.X;
            vecMat[0, 1] = vec.Y;
            vecMat[0, 2] = vec.Z;
            vecMat[0, 3] = 1f;

            var resultMat = vecMat * mat;
            var divider = resultMat[0, 3];
            return new Vector3(resultMat[0, 0] / divider, resultMat[0, 1] / divider, resultMat[0, 2] / divider);
        }

        public int Rows => rows;
        public int Columns => columns;

        int rows;
        int columns;
        float[,] inner;

        public Matrix(int rows, int columns)
        {
            if (rows < 1)
                throw new ArgumentOutOfRangeException(nameof(rows));

            if (columns < 1)
                throw new ArgumentOutOfRangeException(nameof(rows));

            this.rows = rows;
            this.columns = columns;

            inner = new float[rows, columns];
        }

        public float this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= rows)
                    throw new ArgumentOutOfRangeException(nameof(x));

                if(y < 0 || x >= columns)
                    throw new ArgumentOutOfRangeException(nameof(y));

                return inner[x, y];
            }
            set
            {
                if (x < 0 || x >= rows)
                    throw new ArgumentOutOfRangeException(nameof(x));

                if (y < 0 || x >= columns)
                    throw new ArgumentOutOfRangeException(nameof(y));

                inner[x, y] = value;
            }
        }

        public Matrix RotateX(float rad)
        {
            var rotMat = Identity(4, 4);

            rotMat[1, 1] = (float)Math.Cos(rad);
            rotMat[2, 1] = (float)-Math.Sin(rad);
            rotMat[1, 2] = (float)Math.Sin(rad);
            rotMat[2, 2] = (float)Math.Cos(rad);

            return this * rotMat;
        }

        public Matrix RotateY(float rad)
        {
            var rotMat = Identity(4, 4);

            rotMat[0, 0] = (float)Math.Cos(rad);
            rotMat[2, 0] = (float)Math.Sin(rad);
            rotMat[0, 2] = (float)-Math.Sin(rad);
            rotMat[2, 2] = (float)Math.Cos(rad);

            return this * rotMat;
        }

        public Matrix RotateZ(float rad)
        {
            var rotMat = Identity(4, 4);

            rotMat[0, 0] = (float)Math.Cos(rad);
            rotMat[1, 0] = (float)-Math.Sin(rad);
            rotMat[0, 1] = (float)Math.Sin(rad);
            rotMat[1, 1] = (float)Math.Cos(rad);

            return this * rotMat;
        }
    }
}
