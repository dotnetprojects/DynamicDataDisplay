using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers
{
	public class AngleRadiusToPointConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double radius = (double)values[0];
			double angle = (double)values[1];

			angle = angle.DegreesToRadians();

			double x = 0;
			double y = 0;

			if ("end".Equals(parameter))
			{
				x = Math.Cos(angle);
				y = 1 - Math.Sin(angle);
			}
			else if ("startInner".Equals(parameter))
			{
				x = radius * Math.Cos(angle);
				y = 1 - radius * Math.Sin(angle);
			}
			else if ("endInner".Equals(parameter))
			{
				x = radius;
				y = 1;
			}

			return new Point(x, y);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
