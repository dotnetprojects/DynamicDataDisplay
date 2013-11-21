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

namespace TwoHorizontalAxes
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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			innerPlotter.ViewportBindingConverter = new InjectedPlotterVerticalSyncConverter(innerPlotter);

			plotter.Visible = DataRect.Create(0, -1.1, 10, 1.1);

			var xs = Enumerable.Range(0, 10000).Select(i => i * 0.01);
			var xds = xs.AsXDataSource();

			var ys1 = xs.Select(x => Math.Sin(x)).AsYDataSource();
			plotter.AddLineGraph(ys1.Join(xds), 2.0);

			var ys2 = xs.Select(x => Math.Cos(x)).AsYDataSource();
			innerPlotter.AddLineGraph(ys2.Join(xds), 2.0);
		}
	}
}
