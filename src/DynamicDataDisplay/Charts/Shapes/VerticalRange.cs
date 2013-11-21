using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Paints vertical filled and outlined range in viewport coordinates.
	/// </summary>
	public sealed class VerticalRange : RangeHighlight
	{
		protected override void UpdateUIRepresentationCore()
		{
			var transform = Plotter.Viewport.Transform;
			DataRect visible = Plotter.Viewport.Visible;

			Point p1_top = new Point(Value1, visible.YMin).DataToScreen(transform);
			Point p1_bottom = new Point(Value1, visible.YMax).DataToScreen(transform);
			Point p2_top = new Point(Value2, visible.YMin).DataToScreen(transform);
			Point p2_bottom = new Point(Value2, visible.YMax).DataToScreen(transform);

			LineGeometry1.StartPoint = p1_top;
			LineGeometry1.EndPoint = p1_bottom;

			LineGeometry2.StartPoint = p2_top;
			LineGeometry2.EndPoint = p2_bottom;

			RectGeometry.Rect = new Rect(p1_top, p2_bottom);
		}
	}
}
