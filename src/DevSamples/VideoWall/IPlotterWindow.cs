using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;

namespace VideoWall
{
	public interface IPlotterWindow
	{
		int X { get; }
		int Y { get; }

		ChartPlotter Plotter { get; }
		event EventHandler VisibleChanged;
	}
}
