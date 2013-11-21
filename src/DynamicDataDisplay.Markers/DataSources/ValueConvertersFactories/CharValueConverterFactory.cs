using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Converters;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	internal sealed class CharValueConverterFactory : ValueConverterFactory
	{
		public override IValueConverter TryBuildConverter(Type dataType, IValueConversionContext context)
		{
			if (dataType == typeof(char))
			{
				return new GenericValueConverter<char>(c => (double)c);
			}
			return null;
		}
	}
}
