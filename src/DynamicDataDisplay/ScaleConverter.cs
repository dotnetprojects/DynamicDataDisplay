using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Converters;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay
{
	public sealed class ScaleConverter : GenericValueConverter<DataRect>
	{
		public void SetHorizontalTransform(double parentMin, double childMin, double parentMax, double childMax)
		{
			xScale = (childMax - childMin) / (parentMax - parentMin);
			xShift = childMin - parentMin;
		}

		public void SetVerticalTransform(double parentMin, double childMin, double parentMax, double childMax)
		{
			yScale = (childMax - childMin) / (parentMax - parentMin);
			yShift = childMin - parentMin;
		}

		private double xShift = 0;
		private double xScale = 1;
		private double yShift = 0;
		private double yScale = 1;

		public override object ConvertCore(DataRect value, Type targetType, object parameter, CultureInfo culture)
		{
			DataRect parentVisible = value;

			double xmin = parentVisible.XMin * xScale + xShift;
			double xmax = parentVisible.XMax * xScale + xShift;
			double ymin = parentVisible.YMin * yScale + yShift;
			double ymax = parentVisible.YMax * yScale + yShift;

			return DataRect.Create(xmin, ymin, xmax, ymax);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType == typeof(DataRect))
			{
				DataRect childVisible = (DataRect)value;
				double xmin = (childVisible.XMin - xShift) / xScale;
				double xmax = (childVisible.XMax - xShift) / xScale;
				double ymin = (childVisible.YMin - yShift) / yScale;
				double ymax = (childVisible.YMax - yShift) / yScale;

				return DataRect.Create(xmin, ymin, xmax, ymax);
			}
			return base.ConvertBack(value, targetType, parameter, culture);
		}
	}
}
