using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters
{
	public class FrequencyScreenXFilter : PointsFilter2d
	{
		protected internal override IEnumerable<Point> Filter(IEnumerable<Point> points)
		{
			Rect screenRect = Viewport.Output;

			using (var enumerator = points.GetEnumerator())
			{
				double currentX = Double.NegativeInfinity;

				double minX = 0, maxX = 0, minY = 0, maxY = 0;

				Point left = new Point(), right = new Point(), top = new Point(), bottom = new Point();

				bool isFirstPoint = true;
				while (enumerator.MoveNext())
				{
					Point currPoint = enumerator.Current;
					double x = currPoint.X;
					double y = currPoint.Y;
					double xInt = Math.Floor(x);
					if (xInt == currentX)
					{
						if (x > maxX)
						{
							maxX = x;
							right = currPoint;
						}

						if (y > maxY)
						{
							maxY = y;
							top = currPoint;
						}
						else if (y < minY)
						{
							minY = y;
							bottom = currPoint;
						}
					}
					else
					{
						if (!isFirstPoint)
						{
							yield return left;

							Point leftY = top.X < bottom.X ? top : bottom;
							Point rightY = top.X > bottom.X ? top : bottom;
							
							if (top != bottom)
							{
								yield return leftY;
								yield return rightY;
							}
							else if (top != left)
								yield return top;

							if (right != rightY)
								yield return right;
						}

						currentX = xInt;
						left = right = top = bottom = currPoint;
						minX = maxX = x;
						minY = maxY = y;
					}

					isFirstPoint = false;
				}
			}
		}
	}
}
