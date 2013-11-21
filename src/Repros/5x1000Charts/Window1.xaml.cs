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
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace _5x1000Charts
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

		private double[] xs;
		private double[] y1;
		private double[] y2;
		private double[] y3;
		private double[] y4;
		private double[] y5;

		private const int count = 1000;
		bool useFilters = true;

		private readonly Random rnd = new Random();

		private void AddChartsBtn_Click(object sender, RoutedEventArgs e)
		{
			using (new DisposableTimer("adding charts"))
			{
				xs = Enumerable.Range(0, count).Select(i => (double)i).ToArray();

				y1 = Enumerable.Range(0, count).Select(i => 1 + rnd.NextDouble()).ToArray();
				y2 = Enumerable.Range(0, count).Select(i => 2 + rnd.NextDouble()).ToArray();
				y3 = Enumerable.Range(0, count).Select(i => 3 + rnd.NextDouble()).ToArray();
				y4 = Enumerable.Range(0, count).Select(i => 4 + rnd.NextDouble()).ToArray();
				y5 = Enumerable.Range(0, count).Select(i => 5 + rnd.NextDouble()).ToArray();

				var xds = xs.AsXDataSource();
				var ds1 = xds.Join(y1.AsYDataSource());
				var ds2 = xds.Join(y2.AsYDataSource());
				var ds3 = xds.Join(y3.AsYDataSource());
				var ds4 = xds.Join(y4.AsYDataSource());
				var ds5 = xds.Join(y5.AsYDataSource());

				plotter.Visible = new DataRect(-100, 0, 1200, 7);

				if (!useFilters)
				{
					plotter.Children.Add(new LineGraph { DataSource = ds1, Stroke = ColorHelper.RandomBrush });
					plotter.Children.Add(new LineGraph { DataSource = ds2, Stroke = ColorHelper.RandomBrush });
					plotter.Children.Add(new LineGraph { DataSource = ds3, Stroke = ColorHelper.RandomBrush });
					plotter.Children.Add(new LineGraph { DataSource = ds4, Stroke = ColorHelper.RandomBrush });
					plotter.Children.Add(new LineGraph { DataSource = ds5, Stroke = ColorHelper.RandomBrush });
				}
				else
				{
					plotter.AddLineGraph(ds1);
					plotter.AddLineGraph(ds2);
					plotter.AddLineGraph(ds3);
					plotter.AddLineGraph(ds4);
					plotter.AddLineGraph(ds5);
				}
			}
		}
	}
}
