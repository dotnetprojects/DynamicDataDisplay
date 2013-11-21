using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class BoundsHelper
	{
		/// <summary>Computes bounding rectangle for sequence of points</summary>
		/// <param name="points">Points sequence</param>
		/// <returns>Minimal axis-aligned bounding rectangle</returns>
		public static DataRect GetViewportBounds(IEnumerable<Point> viewportPoints)
		{
			DataRect bounds = DataRect.Empty;

			double xMin = Double.PositiveInfinity;
			double xMax = Double.NegativeInfinity;

			double yMin = Double.PositiveInfinity;
			double yMax = Double.NegativeInfinity;

			foreach (Point p in viewportPoints)
			{
				xMin = Math.Min(xMin, p.X);
				xMax = Math.Max(xMax, p.X);

				yMin = Math.Min(yMin, p.Y);
				yMax = Math.Max(yMax, p.Y);
			}

			// were some points in collection
			if (!Double.IsInfinity(xMin))
			{
				bounds = DataRect.Create(xMin, yMin, xMax, yMax);
			}

			return bounds;
		}

		public static DataRect GetViewportBounds(IEnumerable<Point> dataPoints, DataTransform transform)
		{
			return GetViewportBounds(dataPoints.DataToViewport(transform));
		}
	}
}
