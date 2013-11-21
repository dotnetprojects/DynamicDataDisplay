using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace DynamicDataDisplay.Markers.DataSources.ValueConverters
{
	public sealed class LambdaConverter : IValueConverter
	{
		public LambdaConverter(Func<object, object> lambda)
		{
			if (lambda == null)
				throw new ArgumentNullException("lambda");

			this.lambda = lambda;
		}

		private readonly Func<object, object> lambda;

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return lambda(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
