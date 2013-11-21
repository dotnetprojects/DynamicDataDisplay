using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Converters;

namespace DynamicDataDisplay.Markers
{
	public sealed class AcceptableRangeMultiConverter : TwoValuesMultiConverter<double, double>
	{
		protected override object ConvertCore(double value1, double value2, Type targetType, object parameter, CultureInfo culture)
		{
			double yMin = value1;
			double yMax = value2;

			return yMax > yMin ? yMax - yMin : 0;
		}
	}
}
