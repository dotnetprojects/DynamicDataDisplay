using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	internal sealed class ValueConversionContext : IValueConversionContext
	{
		#region IValueConversionContext Members

		public Plotter Plotter { get; set; }

		#endregion
	}
}
