using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers
{
	public class VectorToBoundsMultiConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			Point location = (Point)values[0];
			Vector direction = (Vector)values[1];

			DataRect bounds = new DataRect(location, location + direction);

			return bounds;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
