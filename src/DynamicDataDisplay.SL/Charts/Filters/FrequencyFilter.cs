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

namespace Microsoft.Research.DynamicDataDisplay.Filters
{
    public sealed class FrequencyFilter : PointsFilterBase
    {

        /// <summary>Visible region in screen coordinates</summary>
        private Rect screenRect;

        #region IPointFilter Members

        public override void SetScreenRect(Rect screenRect)
        {
            this.screenRect = screenRect;
        }

        // todo probably use LINQ here.
        public override List<Point> Filter(List<Point> points)
        {
            if (points.Count == 0) return points;

            List<Point> resultPoints = points;

            if (points.Count > 2 * screenRect.Width)
            {
                resultPoints = new List<Point>();

                List<Point> currentChain = new List<Point>();
                double currentX = Math.Floor(points[0].X);
                foreach (Point p in points)
                {
                    if (Math.Floor(p.X) == currentX)
                    {
                        currentChain.Add(p);
                    }
                    else
                    {
                        // Analyse current chain
                        if (currentChain.Count <= 2)
                        {
                            resultPoints.AddRange(currentChain);
                        }
                        else
                        {
                            Point first = MinByX(currentChain);
                            Point last = MaxByX(currentChain);
                            Point min = MinByY(currentChain);
                            Point max = MaxByY(currentChain);
                            resultPoints.Add(first);

                            Point smaller = min.X < max.X ? min : max;
                            Point greater = min.X > max.X ? min : max;
                            if (smaller != resultPoints[resultPoints.Count-1])
                            {
                                resultPoints.Add(smaller);
                            }
                            if (greater != resultPoints[resultPoints.Count-1])
                            {
                                resultPoints.Add(greater);
                            }
                            if (last != resultPoints[resultPoints.Count-1])
                            {
                                resultPoints.Add(last);
                            }
                        }
                        currentChain.Clear();
                        currentChain.Add(p);
                        currentX = Math.Floor(p.X);
                    }
                }
            }

            return resultPoints;
        }

        #endregion

        private static Point MinByX(IList<Point> points)
        {
            Point minPoint = points[0];
            foreach (Point p in points)
            {
                if (p.X < minPoint.X)
                {
                    minPoint = p;
                }
            }
            return minPoint;
        }

        private static Point MaxByX(IList<Point> points)
        {
            Point maxPoint = points[0];
            foreach (Point p in points)
            {
                if (p.X > maxPoint.X)
                {
                    maxPoint = p;
                }
            }
            return maxPoint;
        }

        private static Point MinByY(IList<Point> points)
        {
            Point minPoint = points[0];
            foreach (Point p in points)
            {
                if (p.Y < minPoint.Y)
                {
                    minPoint = p;
                }
            }
            return minPoint;
        }

        private static Point MaxByY(IList<Point> points)
        {
            Point maxPoint = points[0];
            foreach (Point p in points)
            {
                if (p.Y > maxPoint.Y)
                {
                    maxPoint = p;
                }
            }
            return maxPoint;
        }
    }
}
