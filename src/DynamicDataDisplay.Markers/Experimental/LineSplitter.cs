using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	public sealed class LineSplitter
	{
		private readonly double xMissingValue = Double.NegativeInfinity;
		private readonly double yMissingValue = Double.NegativeInfinity;

		public IEnumerable<LinePart> Split(IEnumerable<Point> points)
		{
			List<Point> list = new List<Point>();
			double parameter = 0;
			bool split = false;
			foreach (var point in points)
			{
				if (point.X == xMissingValue)
				{
					parameter = point.Y;
					split = true;
				}
				else if (point.Y == yMissingValue)
				{
					parameter = point.X;
					split = true;
				}
				else
				{
					list.Add(point);
				}

				if (split)
				{
					split = false;
					yield return new LinePart { Points = list, Parameter = parameter };
					list = new List<Point>();
				}
			}
		}
	}
}
