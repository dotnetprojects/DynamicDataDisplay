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
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

namespace AxisControlSample
{
	/// <summary>
	/// Interaction logic for LogYWindow.xaml
	/// </summary>
	public partial class LogYWindow : Window
	{
		public LogYWindow()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(LogYWindow_Loaded);
		}

		void LogYWindow_Loaded(object sender, RoutedEventArgs e)
		{
			ChartPlotter plotter = new ChartPlotter();

			plotter.Children.Add(new CursorCoordinateGraph());

			plotter.DataTransform = new Log10YTransform();
			VerticalAxis axis = new VerticalAxis
			{
				TicksProvider = new LogarithmNumericTicksProvider(10),
				LabelProvider = new UnroundingLabelProvider()
			};
			plotter.MainVerticalAxis = axis;

			plotter.AxisGrid.DrawVerticalMinorTicks = true;

			const int count = 500;
			double[] xs = Enumerable.Range(1, count).Select(x => x * 0.01).ToArray();
			EnumerableDataSource<double> xDS = xs.AsXDataSource();

			var pows = xs.Select(x => Math.Pow(10, x));
			var linear = xs.Select(x => x);
			var logXs = Enumerable.Range(101, count).Select(x => x * 0.01);
			var logarithmic = logXs.Select(x => Math.Log10(x));

			plotter.AddLineGraph(pows.AsYDataSource().Join(xDS), "f(x) = 10^x");
			plotter.AddLineGraph(linear.AsYDataSource().Join(xDS), "f(x) = x");
			plotter.AddLineGraph(logarithmic.AsYDataSource().Join(logXs.AsXDataSource()), "f(x) = log(x)");

			Content = plotter;
		}
	}
}
