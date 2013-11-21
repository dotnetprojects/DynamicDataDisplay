using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using System.Threading;

namespace DynamicDataDisplay.Markers.Filters
{
	public class ParallelClusteringFilter : GroupFilter
	{
		private int xClustersNum = 10;
		private int yClustersNum = 10;

		protected internal override IEnumerable<Point> Filter(IEnumerable<Point> series)
		{
			var transform = Viewport.Transform;

			// determining bounds of point series
			DataRect bounds = DataRect.Empty;
			foreach (var point in series)
			{
				bounds.Union(point);
			}

			if (bounds.IsEmpty)
				return series;

			double xMin = bounds.XMin;
			double yMin = bounds.YMin;
			double clusterWidth = bounds.Width / xClustersNum;
			double clusterHeight = bounds.Height / yClustersNum;

			int clustersNum = xClustersNum * yClustersNum;
			List<Point>[] seriesParts = new List<Point>[clustersNum];
			for (int i = 0; i < clustersNum; i++)
			{
				seriesParts[i] = new List<Point>();
			}

			foreach (var point in series)
			{
				int clusterIndexX = (int)Math.Max(0, Math.Min(xClustersNum - 1, (int)(Math.Floor((point.X - xMin) / clusterWidth))));
				int clusterIndexY = (int)Math.Max(0, Math.Min(yClustersNum - 1, (int)(Math.Floor((point.Y - yMin) / clusterHeight))));

				int index = clusterIndexX + clusterIndexY * xClustersNum;
				seriesParts[index].Add(point);
			}

			List<Point>[] filteredParts = new List<Point>[clustersNum];
			for (int i = 0; i < clustersNum; i++)
			{
				filteredParts[i] = new List<Point>();
			}

			var markerSize = MarkerSize;

			Parallel.For(0, clustersNum, i =>
			{
				var part = seriesParts[i];
				var result = filteredParts[i];

				foreach (var point in part)
				{
					double minDistance = Double.PositiveInfinity;
					var pointOnScreen = point.DataToScreen(transform);

					foreach (var root in result)
					{
						var rootOnScreen = root.DataToScreen(transform);
						var distance = (rootOnScreen - pointOnScreen).Length;
						if (distance < markerSize)
						{
							minDistance = distance;
							break;
						}
					}

					if (minDistance > markerSize)
						result.Add(point);
				}
			});

			IEnumerable<Point> resultSeries = Enumerable.Empty<Point>();
			foreach (var part in filteredParts)
			{
				resultSeries = resultSeries.Concat(part);
			}

			return resultSeries;
		}
	}
}
