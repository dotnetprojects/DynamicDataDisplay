using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	/// <summary>
	/// Represents a closed filled figure with points in Viewport coordinates.
	/// </summary>
	public sealed class ViewportPolygon : ViewportPolylineBase
	{
		static ViewportPolygon()
		{
			Type type = typeof(ViewportPolygon);
			Shape.FillProperty.AddOwner(type, new FrameworkPropertyMetadata(Brushes.Coral));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewportPolygon"/> class.
		/// </summary>
		public ViewportPolygon() { }

		protected override void UpdateUIRepresentationCore()
		{
			var transform = Plotter.Viewport.Transform;

			PathGeometry geometry = PathGeometry;

			PointCollection points = Points;
			geometry.Clear();

			if (points == null) { }
			else
			{
				PathFigure figure = new PathFigure();
				if (points.Count > 0)
				{
					figure.StartPoint = points[0].DataToScreen(transform);
					if (points.Count > 1)
					{
						Point[] pointArray = new Point[points.Count - 1];
						for (int i = 1; i < points.Count; i++)
						{
							pointArray[i - 1] = points[i].DataToScreen(transform);
						}
						figure.Segments.Add(new PolyLineSegment(pointArray, true));
						figure.IsClosed = true;
					}
				}
				geometry.Figures.Add(figure);
				geometry.FillRule = this.FillRule;
			}
		}
	}
}
