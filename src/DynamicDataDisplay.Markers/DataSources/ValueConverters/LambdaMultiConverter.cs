using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace DynamicDataDisplay.Markers.DataSources.ValueConverters
{
	public sealed class LambdaMultiConverter : IMultiValueConverter
	{
		private Func<object[], object> conversion;
		public LambdaMultiConverter(Func<object[], object> conversion)
		{
			if (conversion == null)
				throw new ArgumentNullException("conversion");

			this.conversion = conversion;
		}

		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return conversion(values);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
