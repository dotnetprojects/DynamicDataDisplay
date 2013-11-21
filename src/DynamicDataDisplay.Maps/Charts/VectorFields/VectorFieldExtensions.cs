using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	public static class VectorFieldExtensions
	{
		public static Range<double> GetMinMaxLength(this IDataSource2D<Vector> dataSource)
		{
			double minLength = Double.PositiveInfinity;
			double maxLength = Double.NegativeInfinity;

			int width = dataSource.Width;
			int height = dataSource.Height;
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					var vector = dataSource.Data[ix, iy];
					var length = vector.Length;

					if (length < minLength)
						minLength = length;
					if (length > maxLength)
						maxLength = length;
				}
			}

			if (minLength > maxLength)
			{
				minLength = maxLength = 0;
			}
			return new Range<double>(minLength, maxLength);
		}
	}
}
