using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace NewMarkersSample.Pages
{
	public class TooltipConverter : IMultiValueConverter
	{
		public TooltipConverter()
		{
			FormatString = "{0}: {1}->{2}";
		}

		public string FormatString { get; set; }

		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			string formatString = FormatString.Replace('[', '{').Replace(']', '}');
			return String.Format(formatString, values);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
