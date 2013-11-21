using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers.MarkerGenerators
{
	internal sealed class ForestFillConverter : IValueConverter
	{
		private readonly ForestConverter converter;

		public ForestFillConverter(ForestConverter converter)
		{
			if (converter == null)
				throw new ArgumentNullException("converter");

			this.converter = converter;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stroke = (Brush) converter.Convert(value, targetType, parameter, culture);

			var solidBrush = stroke as SolidColorBrush;
			if (solidBrush != null)
			{
				Color color = solidBrush.Color;
				color.A = 128;

				return new SolidColorBrush(color);
			}
			else
			{
				return stroke;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}