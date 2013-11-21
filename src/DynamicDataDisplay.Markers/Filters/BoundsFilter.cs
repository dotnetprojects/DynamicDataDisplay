using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using System.Diagnostics;

namespace DynamicDataDisplay.Markers.Filters
{
	public sealed class BoundsFilter : PointsFilter2d
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundsFilter"/> class.
		/// </summary>
		public BoundsFilter() { }

		protected override void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				DataRect currVisible = (DataRect)e.NewValue;

				bool shouldUpdate = !bounds.Contains(currVisible) || AreDifferentRectangles(prevVisible.ToRect(), currVisible.ToRect());
				if (shouldUpdate)
				{
					bounds = DataRect.Empty;
					RaiseChanged();
				}
			}
			else if (e.PropertyName == "Output")
			{
				Rect output = (Rect)e.NewValue;

				bool shouldUpdate = AreDifferentRectangles(output, prevOutput);

				if (shouldUpdate)
				{
					prevOutput = output;
					bounds = DataRect.Empty;
					RaiseChanged();
				}
			}
		}

		private bool AreDifferentRectangles(Rect rect1, Rect rect2)
		{
			return rect1.Width / rect2.Width > outputRatio ||
				rect2.Width / rect1.Width > outputRatio ||
				rect1.Height / rect2.Height > outputRatio ||
				rect2.Height / rect1.Height > outputRatio;
		}

		private double visibleIncreaseRatio = 10;
		private double outputRatio = 1.4;
		private DataRect prevVisible;
		private Rect prevOutput;
		private DataRect bounds = DataRect.Empty;
		protected internal override IEnumerable<Point> Filter(IEnumerable<Point> series)
		{
			prevOutput = Output;
			var visible = Visible;

			prevVisible = visible;
			bounds = DataRect.FromCenterSize(visible.GetCenter(), new Size(visible.Width * visibleIncreaseRatio, visible.Height * visibleIncreaseRatio));

			//IParallelEnumerable<Point> parallel = (IParallelEnumerable<Point>)series;

			//Trace.WriteLine("In BoundsFilter: " + Environment.TickCount);

			IEnumerable<Point> result;
			if (Environment.FirstDraw)
				result = series;
			else
				result = series.Where(p => bounds.Contains(p));
			return result;
		}
	}
}
