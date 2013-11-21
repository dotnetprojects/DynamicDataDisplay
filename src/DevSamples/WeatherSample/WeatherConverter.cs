using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace WeatherSample
{
	public sealed class WeatherConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			WeatherType type = (WeatherType)value;
			switch (type)
			{
				case WeatherType.Sun:
					return Application.Current.Resources["Sun"];
				case WeatherType.Rain:
					return Application.Current.Resources["Rain"];
				case WeatherType.Cloud:
					return Application.Current.Resources["Cloud"];
				case WeatherType.Thunderstorm:
					return Application.Current.Resources["Thunderstorm"];
				default:
					break;
			}

			throw new NotSupportedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
