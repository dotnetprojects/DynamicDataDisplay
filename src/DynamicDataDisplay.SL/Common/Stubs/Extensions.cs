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

namespace Microsoft.Research.DynamicDataDisplay.Common.Stubs
{
    public class RectExt
    {
        public static Rect Intersect(Rect r1, Rect r2) {
            if (r1.Left > r2.Right || r2.Left > r1.Right || r1.Bottom > r2.Top || r2.Bottom > r1.Top)
                return Rect.Empty;
            //Rect res = new Rect(
            Point leftTop= new Point(Math.Max(r1.Left, r2.Left),Math.Min(r1.Top,r2.Top));
            Point rightBottom = new Point(Math.Min(r1.Right,r2.Right),Math.Max(r1.Bottom,r2.Bottom));
            return new Rect(leftTop, rightBottom);
        }
    }
}
