using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public static class DataSource2DHelper
	{
		public static Point[,] CreateUniformGrid(int width, int height, double gridWidth, double gridHeight)
		{
			return CreateUniformGrid(width, height, 0, 0, gridWidth / width, gridHeight / height);
		}

		public static Point[,] CreateUniformGrid(int width, int height, double xStart, double yStart, double xStep, double yStep)
		{
			Point[,] result = new Point[width, height];

			double x = xStart;
			for (int ix = 0; ix < width; ix++)
			{
				double y = yStart;
				for (int iy = 0; iy < height; iy++)
				{
					result[ix, iy] = new Point(x, y);

					y += yStep;
				}
				x += xStep;
			}

			return result;
		}

		public static Vector[,] CreateVectorData(int width, int height, Func<int, int, Vector> generator)
		{
			Vector[,] result = new Vector[width, height];

			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					result[ix, iy] = generator(ix, iy);
				}
			}

			return result;
		}
	}
}
