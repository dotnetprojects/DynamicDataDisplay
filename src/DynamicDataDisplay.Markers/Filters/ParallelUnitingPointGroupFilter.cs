using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Threading.Collections;

namespace DynamicDataDisplay.Markers.Filters
{
	public sealed class ParallelUnitingPointGroupFilter : GroupFilter
	{
		protected internal override IEnumerable<Point> Filter(IEnumerable<Point> series)
		{
			var visible = Visible;
			var screenRect = Output;
			var transform = Transform;
			var markerSize = MarkerSize;

			ConcurrentStack<Point> rootPoints = new ConcurrentStack<Point>();

			using (new DisposableTimer("filter", false))
			{
				Parallel.ForEach(series/*.OrderBy(p => p.X).ThenBy(p => p.Y)*/, point =>
				{
					var pointInScreen = point.DataToScreen(transform);

					double minDistance = Double.PositiveInfinity;

					foreach (var root in rootPoints)
					{
						var rootInScreen = root.DataToScreen(transform);
						var distance = (rootInScreen - pointInScreen).Length;
						if (distance < markerSize)
						{
							minDistance = distance;
							break;
						}
					}

					if (minDistance > markerSize)
					{
						rootPoints.Push(point);
					}
				});
			}

			//Trace.WriteLine("Points: " + rootPoints.Count);

			return rootPoints;
		}
	}
}
