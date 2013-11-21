using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	/// <summary>
	/// Paints an arrow with start and end points in viewport coordinates.
	/// </summary>
	public class Arrow : Segment
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Arrow"/> class.
		/// </summary>
		public Arrow()
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Arrow"/> class.
		/// </summary>
		/// <param name="startPoint">The start point of arrow.</param>
		/// <param name="endPoint">The end pointof arrow .</param>
		public Arrow(Point startPoint, Point endPoint)
			: base(startPoint, endPoint)
		{
			Init();
		}

		private void Init()
		{
			geometryGroup.Children.Add(LineGeometry);
			geometryGroup.Children.Add(leftLineGeometry);
			geometryGroup.Children.Add(rightLineGeometry);
		}

		#region ArrowLength property

		/// <summary>
		/// Gets or sets the length of the arrow.
		/// </summary>
		/// <value>The length of the arrow.</value>
		public double ArrowLength
		{
			get { return (double)GetValue(ArrowLengthProperty); }
			set { SetValue(ArrowLengthProperty, value); }
		}

		/// <summary>
		/// Identifies ArrowLength dependency property.
		/// </summary>
		public static readonly DependencyProperty ArrowLengthProperty = DependencyProperty.Register(
		  "ArrowLength",
		  typeof(double),
		  typeof(Arrow),
		  new FrameworkPropertyMetadata(0.1, OnPointChanged));

		#endregion

		#region ArrowAngle property

		/// <summary>
		/// Gets or sets the arrow angle in degrees.
		/// </summary>
		/// <value>The arrow angle.</value>
		public double ArrowAngle
		{
			get { return (double)GetValue(ArrowAngleProperty); }
			set { SetValue(ArrowAngleProperty, value); }
		}

		/// <summary>
		/// Identifies ArrowAngle dependency property.
		/// </summary>
		public static readonly DependencyProperty ArrowAngleProperty = DependencyProperty.Register(
		  "ArrowAngle",
		  typeof(double),
		  typeof(Arrow),
		  new FrameworkPropertyMetadata(15.0, OnPointChanged));

		#endregion

		protected override void UpdateUIRepresentationCore()
		{
			base.UpdateUIRepresentationCore();

			var transform = Plotter.Viewport.Transform;

			Point p1 = StartPoint.DataToScreen(transform);
			Point p2 = EndPoint.DataToScreen(transform);

			Vector arrowVector = p1 - p2;
			Vector arrowCapVector = ArrowLength * arrowVector;

			Matrix leftMatrix = Matrix.Identity;
			leftMatrix.Rotate(ArrowAngle);

			Matrix rightMatrix = Matrix.Identity;
			rightMatrix.Rotate(-ArrowAngle);

			Vector leftArrowLine = leftMatrix.Transform(arrowCapVector);
			Vector rightArrowLine = rightMatrix.Transform(arrowCapVector);

			leftLineGeometry.StartPoint = p2;
			rightLineGeometry.StartPoint = p2;

			leftLineGeometry.EndPoint = p2 + leftArrowLine;
			rightLineGeometry.EndPoint = p2 + rightArrowLine;
		}

		private LineGeometry leftLineGeometry = new LineGeometry();
		private LineGeometry rightLineGeometry = new LineGeometry();
		private GeometryGroup geometryGroup = new GeometryGroup();
		protected override Geometry DefiningGeometry
		{
			get { return geometryGroup; }
		}
	}
}
