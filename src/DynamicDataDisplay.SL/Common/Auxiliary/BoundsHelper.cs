using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay
{
    public static class BoundsHelper
    {
        /// <summary>Computes bounding rectangle for sequence of points</summary>
        /// <param name="points">Points sequence</param>
        /// <returns>Minimal axis-aligned bounding rectangle</returns>
        //public static Rect GetDataBounds(IEnumerable<Point> points)
        //{
        //    Rect bounds = Rect.Empty;

        //    double xMin = Double.PositiveInfinity;
        //    double xMax = Double.NegativeInfinity;

        //    double yMin = Double.PositiveInfinity;
        //    double yMax = Double.NegativeInfinity;

        //    foreach (Point p in points)
        //    {
        //        xMin = Math.Min(xMin, p.X);
        //        xMax = Math.Max(xMax, p.X);

        //        yMin = Math.Min(yMin, p.Y);
        //        yMax = Math.Max(yMax, p.Y);
        //    }

        //    // were some points in collection
        //    if (!Double.IsInfinity(xMin))
        //    {
        //        bounds = MathHelper.CreateRectByPoints(xMin, yMin, xMax, yMax);
        //    }

        //    return bounds;
        //}

        public static Rect GetDataBounds(IEnumerable<Point> points)
        {
            Rect bounds = Rect.Empty;

            double minx, miny, maxx, maxy;
            minx = double.MaxValue;
            miny = double.MaxValue;
            maxx = double.MinValue;
            maxy = double.MinValue;
            foreach (Point point in points) {
                if (point.X < minx) minx = point.X;
                if (point.X > maxx) maxx = point.X;
                if (point.Y < miny) miny = point.Y;
                if (point.Y > maxy) maxy = point.Y;
            }

                bounds = MathHelper.CreateRectByPoints(minx,miny,maxx,maxy);
            

            return bounds;
        }


        public static Rect GetViewportBounds(IEnumerable<Point> dataPoints, DataTransform transform)
        {
            return GetDataBounds(dataPoints.DataToViewport(transform));
        }
    }
}
