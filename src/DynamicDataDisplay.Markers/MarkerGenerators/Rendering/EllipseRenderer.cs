using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Markers.MarkerGenerators.Rendering
{
	public class EllipseRenderer : MarkerRenderer
	{
		public override void Render(DrawingContext dc, CoordinateTransform transform)
		{
			var x = ViewportPanel.GetX(this);
			var y = ViewportPanel.GetY(this);

			dc.DrawEllipse(Brushes.Red, null, new Point(x, y).ViewportToScreen(transform), 3, 3);
		}
	}
}
