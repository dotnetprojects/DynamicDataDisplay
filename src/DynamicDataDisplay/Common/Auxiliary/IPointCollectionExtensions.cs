using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class IPointCollectionExtensions
	{
		public static DataRect GetBounds(this IEnumerable<Point> points)
		{
			return BoundsHelper.GetViewportBounds(points);
		}

		public static IEnumerable<Point> Skip(this IList<Point> points, int skipCount)
		{
			for (int i = skipCount; i < points.Count; i++)
			{
				yield return points[i];
			}
		}
	}
}
