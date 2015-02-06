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

namespace Microsoft.Research.DynamicDataDisplay
{
    public interface ICoordinateConverter
    {
        void Init(Rect screenRect, Rect dataRect);
        Point ToScreen(Point dataPoint);
        Point ToData(Point screenPoint);
    }

    public static class CoordinateConverterExtensions
    {
        public static Point ToScreen(this Point p, ICoordinateConverter converter)
        {
            return converter.ToScreen(p);
        }

        public static Point ToData(this Point p, ICoordinateConverter converter)
        {
            return converter.ToData(p);
        }

        public static IList<Point> ToScreen(this IEnumerable<Point> points, ICoordinateConverter converter)
        {
            return ToScreen(converter, points);
        }

        public static IEnumerable<Point> ToData(this IEnumerable<Point> points, ICoordinateConverter converter)
        {
            return ToData(converter, points);
        }

        public static List<Point> ToScreen(this ICoordinateConverter converter, IEnumerable<Point> dataPoints)
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

        public static IEnumerable<Point> ToData(this ICoordinateConverter converter, IEnumerable<Point> screenPoints)
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

        public static Rect ToData(this Rect rect, ICoordinateConverter converter)
        {
            Point BottomLeft = new Point(rect.Top, rect.Left);
            Point TopRight = new Point(rect.Top,rect.Right);
            Point p1 = BottomLeft.ToData(converter);
            Point p2 = TopRight.ToData(converter);

            return new Rect(p1, p2);
        }

        public static Rect ToScreen(this Rect rect, ICoordinateConverter converter)
        {
            Point BottomLeft = new Point(rect.Top, rect.Left);
            Point TopRight = new Point(rect.Top, rect.Right);
            Point p1 = BottomLeft.ToScreen(converter);
            Point p2 = TopRight.ToScreen(converter);

            return new Rect(p1, p2);
        }
    }
}
