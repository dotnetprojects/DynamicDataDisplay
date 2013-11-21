using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a segment with start and end points bound to viewport coordinates.
	/// </summary>
	public class Segment : ViewportShape
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Segment"/> class.
		/// </summary>
		public Segment() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Segment"/> class.
		/// </summary>
		/// <param name="startPoint">The start point of segment.</param>
		/// <param name="endPoint">The end point of segment.</param>
		public Segment(Point startPoint, Point endPoint)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
		}

		/// <summary>
		/// Gets or sets the start point of segment.
		/// </summary>
		/// <value>The start point.</value>
		public Point StartPoint
		{
			get { return (Point)GetValue(StartPointProperty); }
			set { SetValue(StartPointProperty, value); }
		}

		/// <summary>
		/// Identifies the StartPoint dependency property.
		/// </summary>
		public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
		  "StartPoint",
		  typeof(Point),
		  typeof(Segment),
		  new FrameworkPropertyMetadata(new Point(), OnPointChanged));

		protected static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Segment segment = (Segment)d;
			segment.UpdateUIRepresentation();
		}

		protected virtual void OnPointChanged() { }

		/// <summary>
		/// Gets or sets the end point of segment.
		/// </summary>
		/// <value>The end point.</value>
		public Point EndPoint
		{
			get { return (Point)GetValue(EndPointProperty); }
			set { SetValue(EndPointProperty, value); }
		}

		/// <summary>
		/// Identifies the EndPoint dependency property.
		/// </summary>
		public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
		  "EndPoint",
		  typeof(Point),
		  typeof(Segment),
		  new FrameworkPropertyMetadata(new Point(), OnPointChanged));

		protected override void UpdateUIRepresentationCore()
		{
			if (Plotter == null)
				return;

			var transform = Plotter.Viewport.Transform;

			Point p1 = StartPoint.DataToScreen(transform);
			Point p2 = EndPoint.DataToScreen(transform);

			lineGeometry.StartPoint = p1;
			lineGeometry.EndPoint = p2;
		}

		LineGeometry lineGeometry = new LineGeometry();
		protected LineGeometry LineGeometry
		{
			get { return lineGeometry; }
		}

		protected override Geometry DefiningGeometry
		{
			get { return lineGeometry; }
		}
	}
}
