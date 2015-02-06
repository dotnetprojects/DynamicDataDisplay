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
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay
{
    public static class RectExtensions
    {
        public static Point GetCenter(this Rect rect)
        {
            return new Point(rect.Left + rect.Width * 0.5f, rect.Top + rect.Height * 0.5f);
        }

        public static Rect FromCenterSize(Point center, Size size)
        {
            return FromCenterSize(center, size.Width, size.Height);
        }

        public static Rect FromCenterSize(Point center, double width, double height)
        {
            Rect res = new Rect(center.X - width / 2, center.Y - height / 2, width, height);
            return res;
        }

        public static Rect Zoom(this Rect rect, Point to, double ratio)
        {
            return CoordinateUtilities.RectZoom(rect, to, ratio);
        }

        public static Rect ZoomX(this Rect rect, Point to, double ratio)
        {
            return CoordinateUtilities.RectZoomX(rect, to, ratio);
        }

        public static Rect ZoomY(this Rect rect, Point to, double ratio)
        {
            return CoordinateUtilities.RectZoomY(rect, to, ratio);
        }

        public static Int32Rect ToInt32Rect(this Rect rect)
        {
            Int32Rect intRect = new Int32Rect(
                (int)rect.X,
                (int)rect.Y,
                (int)rect.Width,
                (int)rect.Height);

            return intRect;
        }

        [DebuggerStepThrough]
        public static DataRect ToDataRect(this Rect rect)
        {
            return new DataRect(rect);
        }
    }
}
