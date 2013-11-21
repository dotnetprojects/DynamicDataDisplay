using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Windows.Media;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	public class ViewportPolyBezierCurve : ViewportPolylineBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ViewportPolyBezierCurve"/> class.
		/// </summary>
		public ViewportPolyBezierCurve() { }

		public PointCollection BezierPoints
		{
			get { return (PointCollection)GetValue(BezierPointsProperty); }
			set { SetValue(BezierPointsProperty, value); }
		}

		public static readonly DependencyProperty BezierPointsProperty = DependencyProperty.Register(
		  "BezierPoints",
		  typeof(PointCollection),
		  typeof(ViewportPolyBezierCurve),
		  new FrameworkPropertyMetadata(null, OnPropertyChanged));

		private bool buildBezierPoints = true;
		public bool BuildBezierPoints
		{
			get { return buildBezierPoints; }
			set { buildBezierPoints = value; }
		}

		bool updating = false;
		protected override void UpdateUIRepresentationCore()
		{
			if (updating) return;
			updating = true;

			var transform = Plotter.Viewport.Transform;

			PathGeometry geometry = PathGeometry;

			PointCollection points = Points;

			geometry.Clear();

			if (BezierPoints != null)
			{
				points = BezierPoints;

				var screenPoints = points.DataToScreen(transform).ToArray();
				PathFigure figure = new PathFigure();
				figure.StartPoint = screenPoints[0];
				figure.Segments.Add(new PolyBezierSegment(screenPoints.Skip(1), true));
				geometry.Figures.Add(figure);
				geometry.FillRule = this.FillRule;
			}
			else if (points == null) { }
			else
			{
				PathFigure figure = new PathFigure();
				if (points.Count > 0)
				{
					Point[] bezierPoints = null;
					figure.StartPoint = points[0].DataToScreen(transform);
					if (points.Count > 1)
					{
						Point[] screenPoints = points.DataToScreen(transform).ToArray();

						bezierPoints = BezierBuilder.GetBezierPoints(screenPoints).Skip(1).ToArray();

						figure.Segments.Add(new PolyBezierSegment(bezierPoints, true));
					}

					if (bezierPoints != null && buildBezierPoints)
					{
						Array.Resize(ref bezierPoints, bezierPoints.Length + 1);
						Array.Copy(bezierPoints, 0, bezierPoints, 1, bezierPoints.Length - 1);
						bezierPoints[0] = figure.StartPoint;

						BezierPoints = new PointCollection(bezierPoints.ScreenToData(transform));
					}
				}

				geometry.Figures.Add(figure);
				geometry.FillRule = this.FillRule;
			}

			updating = false;
		}
	}
}
