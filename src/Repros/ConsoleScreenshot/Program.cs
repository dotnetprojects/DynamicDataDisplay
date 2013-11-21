using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Media;

namespace ConsoleScreenshot
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			ChartPlotter plotter = new ChartPlotter();
			plotter.PerformLoad();
			plotter.Background = Brushes.Transparent;
			plotter.SaveScreenshot("1.png");
		}
	}
}
