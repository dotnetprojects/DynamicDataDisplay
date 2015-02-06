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
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.CoordinateTransforms
{
    public interface ICoordinateTransform
    {
        void Init(Rect screenRect, Rect dataRect);
        Point ToScreen(Point dataPoint);
        Point ToData(Point screenPoint);
    }

    public static class CoordinateConverterExtensions
    {
        public static Point ToScreen(this Point p, ICoordinateTransform converter)
        {
            return converter.ToScreen(p);
        }

        public static Point ToData(this Point p, ICoordinateTransform converter)
        {
            return converter.ToData(p);
        }

        public static List<Point> ToScreen(this ICoordinateTransform converter, IEnumerable<Point> dataPoints)
        {
            ICollection<Point> pointsCollection = dataPoints as ICollection<Point>;
            List<Point> res;

            if (pointsCollection != null)
            {
                res = new List<Point>(pointsCollection.Count);
            }
            else
            {
                res = new List<Point>();
            }

            foreach (var point in dataPoints)
            {
                res.Add(converter.ToScreen(point));
            }

            return res;
        }

        public static IEnumerable<Point> ToData(this ICoordinateTransform converter, IEnumerable<Point> screenPoints)
        {
            ICollection<Point> pointsCollection = screenPoints as ICollection<Point>;
            List<Point> res;

            if (pointsCollection != null)
            {
                res = new List<Point>(pointsCollection.Count);
            }
            else
            {
                res = new List<Point>();
            }

            foreach (var point in screenPoints)
            {
                res.Add(converter.ToData(point));
            }

            return res;
        }
    }
}
