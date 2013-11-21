using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace ChartBringToFrontSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		LineGraph first;
		LineGraph second;
		private void AddTwoSampleCharts()
		{
			var xs = Enumerable.Range(0, 800).Select(i => i * 0.01);
			var xds = xs.AsXDataSource();
			var yds1 = xs.Select(x => Math.Cos(x)).AsYDataSource();
			var yds2 = xs.Select(x => Math.Sin(x)).AsYDataSource();

			first = plotter.AddLineGraph(xds.Join(yds1), Colors.Red, 5);
			second = plotter.AddLineGraph(xds.Join(yds2), Colors.Blue, 5);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			AddTwoSampleCharts();
		}

		bool secondOnTop = true;
		private void swapChartsBtn_Click(object sender, RoutedEventArgs e)
		{
			// changing z-index of charts according to their relative layout:
			// bigger z-index means closer to observer (to screen:))
			if (secondOnTop)
			{
				Panel.SetZIndex(second, 0);
				Panel.SetZIndex(first, 1);
				secondOnTop = false;
			}
			else
			{
				Panel.SetZIndex(second, 1);
				Panel.SetZIndex(first, 0);
				secondOnTop = true;
			}
		}
	}
}
