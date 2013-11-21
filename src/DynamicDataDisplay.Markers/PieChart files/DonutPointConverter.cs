using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public class DonutPointConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double startRadius = (double)value;

			if (targetType == typeof(Point))
			{
				Point location = new Point();
				if (parameter.Equals("start"))
					location = new Point(startRadius, 1);
				else if (parameter.Equals("end"))
					location = new Point(0, 1 - startRadius);
				else if (parameter.Equals("endInner"))
					location = new Point(startRadius, 1);

				return location;
			}
			else if (targetType == typeof(Size))
			{
				return new Size(startRadius, startRadius);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
