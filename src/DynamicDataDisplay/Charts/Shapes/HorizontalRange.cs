using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Paints horizontal filled and outlined range in viewport coordinates.
	/// </summary>
	public sealed class HorizontalRange : RangeHighlight
	{
		protected override void UpdateUIRepresentationCore()
		{
			var transform = Plotter.Viewport.Transform;
			DataRect visible = Plotter.Viewport.Visible;

			Point p1_left = new Point(visible.XMin, Value1).DataToScreen(transform);
			Point p1_right = new Point(visible.XMax, Value1).DataToScreen(transform);
			Point p2_left = new Point(visible.XMin, Value2).DataToScreen(transform);
			Point p2_right = new Point(visible.XMax, Value2).DataToScreen(transform);

			LineGeometry1.StartPoint = p1_left;
			LineGeometry1.EndPoint = p1_right;

			LineGeometry2.StartPoint = p2_left;
			LineGeometry2.EndPoint = p2_right;

			RectGeometry.Rect = new Rect(p1_left, p2_right);
		}
	}
}
