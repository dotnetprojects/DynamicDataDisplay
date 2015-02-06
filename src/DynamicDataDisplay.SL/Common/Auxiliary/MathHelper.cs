using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay
{
    public static class MathHelper
    {
        public static long Clamp(long value, long min, long max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        /// <summary>Clamps specified value to [0,1]</summary>
        /// <param name="d">Value to clamp</param>
        /// <returns>Value in range [0,1]</returns>
        public static double Clamp(double value)
        {
            return Math.Max(0, Math.Min(value, 1));
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public static Rect CreateRectByPoints(double xmin, double ymin, double xmax, double ymax)
        {
            return new Rect(new Point(xmin, ymin), new Point(xmax, ymax));
        }

        public static Rect CreateRectFromCenterSize(Point center, Size size)
        {
            return new Rect(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
        }

        public static double Interpolate(double start, double end, double ratio)
        {
            return start * (1 - ratio) + end * ratio;
        }

        public static double ToDegrees(this double angleInRadians)
        {
            return angleInRadians * 180 / Math.PI;
        }

        /// <summary>
        /// Converts vector into angle.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Angle in degrees.</returns>
        public static double ToAngle(this Vector vector)
        {
            return Math.Atan2(-vector.Y, vector.X).ToDegrees();
        }
    }

    public class Vector {
        private double y, x;
        public double X {
            get { return x; }
            set { x = value; }
        }
        public double Y {
            get { return y; }
            set { y = value; }
        }

        public Vector(double x, double y) {
            X = x;
            Y = y;
        }
        public Vector() {
            X = 0;
            Y = 0;
        }

        public double Length {
            get {
                return Math.Sqrt(Math.Pow(x,2)+Math.Pow(y,2));
            }
        }
        public static Vector operator + (Vector fst,Vector snd) {
            return new Vector(fst.X + snd.X, fst.Y + snd.Y);
        }

        public static Vector operator *(Vector v, double b)
        {
            return new Vector(v.X*b,v.Y*b);
        }
    }
}
