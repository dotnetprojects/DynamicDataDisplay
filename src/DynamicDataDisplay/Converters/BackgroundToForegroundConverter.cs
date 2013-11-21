using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Converters
{
	public class BackgroundToForegroundConverter : GenericValueConverter<SolidColorBrush>
	{
		public override object ConvertCore(SolidColorBrush value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush back = value;
			Color diff = back.Color - Colors.Black;
			int summ = diff.R + diff.G + diff.B;

			int border = 3 * 255 / 2;

			return summ > border ? Brushes.Black : Brushes.White;
		}
	}
}
