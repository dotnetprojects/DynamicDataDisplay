using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class DataRectExtensions
	{
		internal static bool IsNaN(this DataRect rect)
		{
			return !rect.IsEmpty &&
				(
				rect.XMin.IsNaN() ||
				rect.YMin.IsNaN() ||
				rect.XMax.IsNaN() ||
				rect.YMax.IsNaN()
				);
		}

		public static Point GetCenter(this DataRect rect)
		{
			return new Point(rect.XMin + rect.Width * 0.5, rect.YMin + rect.Height * 0.5);
		}

		public static DataRect Zoom(this DataRect rect, Point to, double ratio)
		{
			return CoordinateUtilities.RectZoom(rect, to, ratio);
		}

		public static DataRect ZoomOutFromCenter(this DataRect rect, double ratio)
		{
			return CoordinateUtilities.RectZoom(rect, rect.GetCenter(), ratio);
		}

		public static DataRect ZoomInToCenter(this DataRect rect, double ratio)
		{
			return CoordinateUtilities.RectZoom(rect, rect.GetCenter(), 1 / ratio);
		}

		public static DataRect ZoomX(this DataRect rect, Point to, double ratio)
		{
			return CoordinateUtilities.RectZoomX(rect, to, ratio);
		}

		public static DataRect ZoomY(this DataRect rect, Point to, double ratio)
		{
			return CoordinateUtilities.RectZoomY(rect, to, ratio);
		}

		public static double GetSquare(this DataRect rect)
		{
			if (rect.IsEmpty)
				return 0;

			return rect.Width * rect.Height;
		}

		/// <summary>
		/// Determines whether one DataRect is close to another DataRect.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <param name="difference">The difference.</param>
		/// <returns>
		/// 	<c>true</c> if [is close to] [the specified rect1]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsCloseTo(this DataRect rect1, DataRect rect2, double difference)
		{
			DataRect intersection = DataRect.Intersect(rect1, rect2);
			double square1 = rect1.GetSquare();
			double square2 = rect2.GetSquare();
			double intersectionSquare = intersection.GetSquare();

			bool areClose = MathHelper.AreClose(square1, intersectionSquare, difference) &&
				MathHelper.AreClose(square2, intersectionSquare, difference);
			return areClose;
		}
	}
}
