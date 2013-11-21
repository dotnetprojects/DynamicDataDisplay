using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters
{
	public class InclinationFilter2 : PointsFilter2d
	{
		#region CriticalAngle property

		public double CriticalAngle
		{
			get { return (double)GetValue(CriticalAngleProperty); }
			set { SetValue(CriticalAngleProperty, value); }
		}

		public static readonly DependencyProperty CriticalAngleProperty = DependencyProperty.Register(
		  "CriticalAngle",
		  typeof(double),
		  typeof(InclinationFilter2),
		  new FrameworkPropertyMetadata(150.0, OnPropertyChanged, OnCoerceCriticalAngle),
		  ValidateCriticalAngle
		  );

		private static object OnCoerceCriticalAngle(DependencyObject d, object baseValue)
		{
			double angle = (double)baseValue;
			return Math.Min(180.0, Math.Max(angle, 0.0));
		}

		private static bool ValidateCriticalAngle(object value)
		{
			return ((double)value).IsFinite();
		}

		#endregion

		#region MinTriangleHeight property

		public double MinTriangleHeight
		{
			get { return (double)GetValue(MinTriangleHeightProperty); }
			set { SetValue(MinTriangleHeightProperty, value); }
		}

		/// <summary>
		/// In screen coordinates
		/// </summary>
		public static readonly DependencyProperty MinTriangleHeightProperty = DependencyProperty.Register(
		  "MinTriangleHeight",
		  typeof(double),
		  typeof(InclinationFilter2),
		  new FrameworkPropertyMetadata(1.0, OnPropertyChanged));

		#endregion

		protected internal override IEnumerable<Point> Filter(IEnumerable<Point> points)
		{
			if (!points.CountGreaterOrEqual(3))
				return points;

			// here we are sure that count of points is >= 3
			points = FilterLongerThan3(points);
			return points;
		}

		const double visibleMinLengthPercent = 0.00005;
		private IEnumerable<Point> FilterLongerThan3(IEnumerable<Point> points)
		{
			double minAngle = CriticalAngle.DegreesToRadians();
			DataRect visible = Viewport.Visible;
			Rect screen = Viewport.Output;

			double visibleSize = (visible.Width + visible.Height) / 2;
			double screenSize = (screen.Width + screen.Height) / 2;

			double minHeight = MinTriangleHeight * screenSize / visibleSize * visibleMinLengthPercent;

			using (var enumerator = points.GetEnumerator())
			{
				enumerator.MoveNext();
				Point prevPoint = enumerator.Current;

				enumerator.MoveNext();
				Point currPoint = enumerator.Current;

				yield return prevPoint;

				bool added = false;
				while (enumerator.MoveNext())
				{
					Point nextPoint = enumerator.Current;

					double a = (prevPoint - currPoint).Length;
					double b = (currPoint - nextPoint).Length;
					double c = (prevPoint - nextPoint).Length;

					double currAngle = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
					double doubledArea = a * b * Math.Sin(currAngle);
					double height = doubledArea / c;
					if (currAngle <= minAngle || height > minHeight)
					{
						yield return currPoint;
						prevPoint = currPoint;
						currPoint = nextPoint;
						added = true;
					}
					else
					{
						added = false;
						currPoint = nextPoint;
					}
				}

				if (!added)
					yield return currPoint;
			}
		}
	}
}
