using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Data;
using DynamicDataDisplay.Markers.DataSources.ValueConverters;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	public class DateTimeValueConverterFactory : ValueConverterFactory
	{
		public override IValueConverter TryBuildConverter(Type dataType, IValueConversionContext context)
		{
			var plotter = context.Plotter;

			if (dataType == typeof(DateTime))
			{
				foreach (var child in plotter.Children)
				{
					DateTimeAxis axis = child as DateTimeAxis;
					if (axis != null)
					{
						return new DateTimeValueConverter(axis);
					}
				}
			}
			return null;
		}
	}
}
