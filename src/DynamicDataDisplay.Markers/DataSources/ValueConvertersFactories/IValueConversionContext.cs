using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	public interface IValueConversionContext
	{
		Plotter Plotter { get; }
	}
}
