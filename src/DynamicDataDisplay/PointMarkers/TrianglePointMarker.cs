using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    /// <summary>Class that renders triangular marker at every point of graph</summary>
	public class TrianglePointMarker : ShapePointMarker {
		public override void Render(DrawingContext dc, Point screenPoint) {
			Point pt0 = Point.Add(screenPoint, new Vector(-Size / 2, -Size / 2));
			Point pt1 = Point.Add(screenPoint, new Vector(0, Size / 2));
			Point pt2 = Point.Add(screenPoint, new Vector(Size / 2, -Size / 2));
			
			StreamGeometry streamGeom = new StreamGeometry();
			using (var context = streamGeom.Open()) {
				context.BeginFigure(pt0, true, true);
				context.LineTo(pt1, true, true);
				context.LineTo(pt2, true, true);
			}
			dc.DrawGeometry(Fill, Pen, streamGeom);
		}
	}
}
