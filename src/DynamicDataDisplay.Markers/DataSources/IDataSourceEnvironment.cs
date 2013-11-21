using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers.DataSources
{
	public interface IDataSourceEnvironment
	{
		Plotter2D Plotter { get; }
		bool FirstDraw { get; }
	}
}
