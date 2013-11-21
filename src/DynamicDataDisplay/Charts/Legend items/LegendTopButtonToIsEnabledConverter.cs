using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Converters;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	internal sealed class LegendTopButtonToIsEnabledConverter : GenericValueConverter<double>
	{
		public override object ConvertCore(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double verticalOffset = value;

			return verticalOffset > 0;
		}
	}
}
