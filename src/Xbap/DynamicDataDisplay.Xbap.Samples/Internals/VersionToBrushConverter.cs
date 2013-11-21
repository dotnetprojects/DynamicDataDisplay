using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals
{
	public sealed class VersionToBrushConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string version = value.ToString();

			if (version == "0.2.0")
				return Brushes.Green;
			else if (version == "0.3.0")
				return Brushes.Orange;
			else if (version == "0.3.1")
				return Brushes.DarkMagenta;
			else if (version == "0.4.0")
				return Brushes.Blue;

			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
