using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class PlotterExtensions
	{
		public static void AddChild(this Plotter plotter, IPlotterElement child)
		{
			plotter.Children.Add(child);
		}
	}
}
