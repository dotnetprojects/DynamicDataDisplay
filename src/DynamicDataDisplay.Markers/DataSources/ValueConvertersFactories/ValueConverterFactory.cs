using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	public abstract class ValueConverterFactory
	{
		public abstract IValueConverter TryBuildConverter(Type dataType, IValueConversionContext context);
	}
}
