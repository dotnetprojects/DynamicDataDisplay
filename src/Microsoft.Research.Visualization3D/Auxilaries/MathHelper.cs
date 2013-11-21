using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Microsoft.Research.Visualization3D.Auxilaries
{
    class MathHelper
    {
        public static float GetValue(Vector3 position, double[, ,] array)
        {
            int i = (int)Math.Truncate(position.X);
            int j = (int)Math.Truncate(position.Y);
            int k = (int)Math.Truncate(position.Z);

            if (i >= array.GetLength(0) - 1) i = array.GetLength(0) - 2;
            if (j >= array.GetLength(1) - 1) j = array.GetLength(1) - 2;
            if (k >= array.GetLength(2) - 1) k = array.GetLength(2) - 2;

            double x1 = Interpolate(array[i, j, k], i, array[i + 1, j, k], i + 1, position.X);
            double x2 = Interpolate(array[i, j + 1, k], i, array[i + 1, j + 1, k], i + 1, position.X);
            double x3 = Interpolate(array[i, j, k + 1], i, array[i + 1, j, k + 1], i + 1, position.X);
            double x4 = Interpolate(array[i, j + 1, k + 1], i, array[i + 1, j + 1, k + 1], i + 1, position.X);

            double z1 = Interpolate(x1, k, x3, k + 1, position.Z);
            double z2 = Interpolate(x2, k, x4, k + 1, position.Z);

            double y1 = Interpolate(z1, j, z2, j + 1, position.Y);

            float value = (float)y1;
            return value;
        }

        public static bool MissingCheck(Vector3 position, double[,,] array, double missingValue)
        {
            int i = (int)Math.Truncate(position.X);
            int j = (int)Math.Truncate(position.Y);
            int k = (int)Math.Truncate(position.Z);

            if (i >= array.GetLength(0) - 1) i = array.GetLength(0) - 2;
            if (j >= array.GetLength(1) - 1) j = array.GetLength(1) - 2;
            if (k >= array.GetLength(2) - 1) k = array.GetLength(2) - 2;

            return
                array[i, j, k] == missingValue ||
                array[i + 1, j, k] == missingValue ||
                array[i + 1, j + 1, k] == missingValue ||
                array[i, j + 1, k] == missingValue ||
                array[i, j, k + 1] == missingValue ||
                array[i + 1, j, k + 1] == missingValue ||
                array[i + 1, j + 1, k + 1] == missingValue ||
                array[i, j + 1, k + 1] == missingValue;
        }

        public static float GetValue(Vector3 position, double[, ,] array, double missingValue)
        {
            if (MathHelper.MissingCheck(position, array, missingValue))
                return (float)missingValue;
            else
                return MathHelper.GetValue(position, array);
 
        }

        public static double Interpolate(double v1, double p1, double v2, double p2, double p3)
        {
            double alpha = (p3 - p1) / (p2 - p1);
            alpha = MathHelper.Saturate(alpha);
            return v1 * alpha + v2 * (1 - alpha);
        }

        public static float GetPatrialDerivation(float frontValue, float backValue, float delta)
        {
            return (frontValue - backValue) / (2.0f * delta);
        }

        public static double FindMax(double[, ,] array, double missingValue)
        {
            double max = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    for (int k = 0; k < array.GetLength(2); k++)
                    {
                        if (max < array[i, j, k] && array[i, j, k] != missingValue) max = array[i, j, k];
                    }
                }
            }
            return max;
        }

        public static double FindMin(double[, ,] array, double missingValue)
        {
            double min = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    for (int k = 0; k < array.GetLength(2); k++)
                    {
                        if (min > array[i, j, k] && array[i, j, k] != missingValue)
                            min = array[i, j, k];
                    }
                }
            }
            return min;
        }


        public static Vector3 CalculateNormal(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 v0 = new Vector3(
                p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3 v1 = new Vector3(
                p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3.Cross(v0, v1);
        }

        public static float Lerp(float value1, float value2, float alpha)
        {
            if (alpha >= 0 && alpha <= 1)
                return (value1 * alpha + value2 * (1 - alpha));
            else
                throw new ArgumentException("Invalid alpha!");
        }
        
        public static bool CheckPosition<T>(Vector3 position, T[, ,] array)
        {
            int i = array.GetLength(0);
            int j = array.GetLength(1);
            int k = array.GetLength(2);

            if (position.X >= 0 && position.X < i && position.Y >= 0 && position.Y < j && position.Z >= 0 && position.Z < k)
                return true;
            else
                return false;

        }

        public static bool CheckPosition(Vector3 position, double[, ,] array, double missingValue)
        {
            int i = array.GetLength(0);
            int j = array.GetLength(1);
            int k = array.GetLength(2);

            if (position.X >= 0 && position.X < i && position.Y >= 0 && position.Y < j && position.Z >= 0 && position.Z < k && !MathHelper.MissingCheck(position, array, missingValue))
                return true;
            else
                return false;

        }


        public static float Saturate(float value)
        {
            if (value > 1) return 1.0f;
            if (value < 0) return 0.0f;
            return value;
        }

        public static double Saturate(double value)
        {
            if (value > 1) return 1.0f;
            if (value < 0) return 0.0f;
            return value;
        }

    }
}
