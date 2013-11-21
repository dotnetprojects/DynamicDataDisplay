using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers
{
	public class PiePanel : Panel, INotifyCollectionChanged
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PiePanel"/> class.
		/// </summary>
		public PiePanel()
		{
		}

		#region Properties

		#region StartAngle property

		public double StartAngle
		{
			get { return (double)GetValue(StartAngleProperty); }
			set { SetValue(StartAngleProperty, value); }
		}

		public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
		  "StartAngle",
		  typeof(double),
		  typeof(PiePanel),
		  new FrameworkPropertyMetadata(0.0, OnStartAngleChanged));

		private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PiePanel owner = (PiePanel)d;
			owner.InvalidateMeasure();
		}

		#endregion // end of StartAngle property

		#region ArcWidth property

		[AttachedPropertyBrowsableForChildren]
		public static double GetArcWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(ArcWidthProperty);
		}

		public static void SetArcWidth(DependencyObject obj, double value)
		{
			obj.SetValue(ArcWidthProperty, value);
		}

		public static readonly DependencyProperty ArcWidthProperty = DependencyProperty.RegisterAttached(
		  "ArcWidth",
		  typeof(double),
		  typeof(PiePanel),
		  new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

		#endregion // end of ArcWidth property

		#endregion // end of Properties

		#region Panel overrides

		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
		{
			NotifyingUIElementCollection collection = new NotifyingUIElementCollection(this, logicalParent);
			collection.CollectionChanged += collection_CollectionChanged;
			return collection;
		}

		private void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged.Raise(this, e);
		}

		private double summAngle = 0;
		private double elementRatio = 1;
		protected override Size MeasureOverride(Size availableSize)
		{
			if (availableSize.Width.IsInfinite() || availableSize.Height.IsInfinite())
				return new Size();

			summAngle = 0.0;
			foreach (FrameworkElement child in base.InternalChildren)
			{
				if (child.Visibility != Visibility.Visible)
					continue;

				var childArc = GetArcWidth(child);
				summAngle += childArc;
			}

			double diameter = Math.Min(availableSize.Width, availableSize.Height);
			double radius = diameter / 2;

			elementRatio = 2 * Math.PI / summAngle;
			summAngle = 0;

			if (InternalChildren.Count > 1)
			{
				foreach (FrameworkElement child in InternalChildren)
				{
					if (child.Visibility != Visibility.Visible)
						continue;

					double angle = GetArcWidth(child);
					double realAngle = angle * elementRatio;
					summAngle += realAngle;

					Size childSize = GetChildSize(radius, realAngle, realAngle.RadiansToDegrees());

					child.Measure(childSize);
				}
			}
			else if (InternalChildren.Count > 0)
			{
				var pieItem = InternalChildren[0];
				var binding = BindingOperations.GetBindingExpression(pieItem, ArcWidthProperty);
				if (binding != null)
				{
					binding.UpdateTarget();
				}
			}

			return new Size(diameter, diameter);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			double diameter = Math.Min(finalSize.Height, finalSize.Width);
			double radius = diameter / 2;

			double startAngle = StartAngle.DegreesToRadians();

			double childStartAngle = startAngle;
			double childEndAngle = 0;
			for (int i = 0; i < InternalChildren.Count; i++)
			{
				FrameworkElement child = (FrameworkElement)InternalChildren[i];
				if (child.Visibility != Visibility.Visible)
					continue;

				double childAngle = GetArcWidth(child);

				var angleInRadians = childAngle * elementRatio;
				var angleInDegrees = angleInRadians.RadiansToDegrees();

				var childStartAngleInDegrees = childStartAngle.RadiansToDegrees();

				childEndAngle = childStartAngle + angleInRadians;

				child.SetValue(PieChartItem.AngleInChartProperty, angleInDegrees);

				double centerX = 0;
				if (angleInDegrees <= 90)
					centerX = 0;
				else if (angleInDegrees < 180)
					centerX = -radius * Math.Cos(angleInRadians);
				else centerX = radius;

				double centerY = 0;
				if (angleInDegrees <= 90)
					centerY = radius * Math.Cos(angleInRadians);
				else centerY = radius;

				Point renderTransformOrigin = new Point();
				if (angleInDegrees < 90)
					renderTransformOrigin = new Point(0, 1);
				else if (angleInDegrees < 180)
					renderTransformOrigin = new Point(-Math.Cos(angleInRadians) / (1 - Math.Cos(angleInRadians)), 1);
				else if (angleInDegrees < 270)
					renderTransformOrigin = new Point(0.5, 1 / ((1 - Math.Sin(angleInRadians))));
				else renderTransformOrigin = new Point(0.5, 0.5);

				double rotationAngle = -childStartAngleInDegrees;
				if (childAngle > 180)
					rotationAngle = -rotationAngle;

				RotateTransform rotateTransform = new RotateTransform(rotationAngle);

				child.RenderTransformOrigin = renderTransformOrigin;
				child.RenderTransform = rotateTransform;
				//child.LayoutTransform = rotateTransform;

				Size childSize = GetChildSize(radius, angleInRadians, angleInDegrees);
				Point childLocation = GetChildLocation(radius, angleInRadians);

				Rect childBounds = new Rect(childLocation, childSize);
				child.Arrange(childBounds);

				childStartAngle = childEndAngle;
			}

			return new Size(diameter, diameter);
		}

		private static Point GetChildLocation(double radius, double angleInRadians)
		{
			double angleInDegrees = angleInRadians.RadiansToDegrees();

			Point location = new Point();

			if (angleInDegrees < 90) { location = new Point(radius, radius * (1 - Math.Sin(angleInRadians))); }
			else if (angleInDegrees < 180) { location = new Point(2 * radius - radius * (1 - Math.Cos(angleInRadians)), 0); }
			else if (angleInDegrees < 270) { location = new Point(0, 0); }
			else { location = new Point(0, 0); }

			return location;
		}

		private static Size GetChildSize(double radius, double angleInRadians, double angleInDegrees)
		{
			Size childSize = new Size(radius, radius);
			if (angleInDegrees <= 90)
			{
				childSize = new Size(radius, radius * Math.Sin(angleInRadians));
			}
			else if (angleInDegrees < 180)
			{
				childSize = new Size(radius * (1 - Math.Cos(angleInRadians)), radius);
			}
			else if (angleInDegrees <= 270)
			{
				childSize = new Size(2 * radius, radius * (1 - Math.Sin(angleInRadians)));
			}
			else
			{
				childSize = new Size(2 * radius, 2 * radius);
			}
			return childSize;
		}

		#endregion // end of Panel overrides

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
	}
}
