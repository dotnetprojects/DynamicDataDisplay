using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Converters;

namespace DynamicDataDisplay.Markers
{
	public sealed class StockGridColumnConverter : TwoValuesMultiConverter<double, double>
	{
		protected override object ConvertCore(double value1, double value2, Type targetType, object parameter, CultureInfo culture)
		{
			double open = value1;
			double close = value2;

			bool condition = open > close == (parameter.Equals("top"));
			int column = condition ? 0 : 1;
			return column;
		}
	}
}
