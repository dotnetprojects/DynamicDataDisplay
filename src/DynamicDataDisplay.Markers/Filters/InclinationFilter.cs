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
	public class InclinationFilter : PointsFilter2d
	{
		public double CriticalAngle
		{
			get { return (double)GetValue(CriticalAngleProperty); }
			set { SetValue(CriticalAngleProperty, value); }
		}

		public static readonly DependencyProperty CriticalAngleProperty = DependencyProperty.Register(
		  "CriticalAngle",
		  typeof(double),
		  typeof(InclinationFilter),
		  new FrameworkPropertyMetadata(179.1, OnPropertyChanged, OnCoerceCriticalAngle),
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

		protected internal override IEnumerable<Point> Filter(IEnumerable<Point> points)
		{
			if (!points.CountGreaterOrEqual(3))
				return points;

			double angle = CriticalAngle.DegreesToRadians();

			// here we are sure that count of points is >= 3
			points = new InclinationEnumerable(points, angle);
			return points;
		}

		private sealed class InclinationEnumerable : IEnumerable<Point>
		{
			private readonly IEnumerable<Point> series;
			private readonly double maxAngle;
			public InclinationEnumerable(IEnumerable<Point> points, double maxAngle)
			{
				this.series = points;
				this.maxAngle = maxAngle;
			}

			#region IEnumerable<Point> Members

			public IEnumerator<Point> GetEnumerator()
			{
				using (var enumerator = series.GetEnumerator())
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
						if (currAngle <= maxAngle)
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

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}
	}
}
