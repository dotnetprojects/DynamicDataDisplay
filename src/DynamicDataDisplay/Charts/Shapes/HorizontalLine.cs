using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents an infinite horizontal line with y-coordinate.
	/// </summary>
	public sealed class HorizontalLine : SimpleLine
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalLine"/> class.
		/// </summary>
		public HorizontalLine() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalLine"/> class with specified y coordinate.
		/// </summary>
		/// <param name="yCoordinate">The y coordinate of line.</param>
		public HorizontalLine(double yCoordinate)
		{
			Value = yCoordinate;
		}

		protected override void UpdateUIRepresentationCore()
		{
			var transform = Plotter.Viewport.Transform;

			Point p1 = new Point(Plotter.Viewport.Visible.XMin, Value).DataToScreen(transform);
			Point p2 = new Point(Plotter.Viewport.Visible.XMax, Value).DataToScreen(transform);

			LineGeometry.StartPoint = p1;
			LineGeometry.EndPoint = p2;
		}
	}
}
