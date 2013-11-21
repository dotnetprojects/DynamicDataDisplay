using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Converters;
using System.Globalization;

namespace NewMarkersSample.Pages
{
	public class CharToStringConverter : GenericValueConverter<char>
	{
		public override object ConvertCore(char value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.ToString();
		}
	}
}
