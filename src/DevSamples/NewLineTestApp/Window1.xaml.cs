#define old

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

#if false
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Filters;
#endif
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;
using Microsoft.Research.DynamicDataDisplay;

namespace NewLineTestApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		private LineGraph lineGraph;
		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			//plotter.Viewport.Visible = new DataRect(-1, -1.1, 200, 2.2);

			const int count = 14000;
			Point[] pts = new Point[count];

			for (int i = 0; i < count; i++)
			{
				double x = i / 20.0 - 1000;
				pts[i] = new Point(x, Math.Sin(x));
			}

#if !old
			var ds = pts.AsDataSource();

			LineChart chart = new LineChart { DataSource = ds };
			//chart.Filters.Clear();

			//InclinationFilter filter = new InclinationFilter();
			//BindingOperations.SetBinding(filter, InclinationFilter.CriticalAngleProperty, new Binding { Path = new PropertyPath("Value"), Source = slider });
			//chart.Filters.Add(filter);
			plotter.Children.Add(chart);

			//plotter.Children.Add(new LineChart { DataSource = new FunctionalDataSource { Function = x => Math.Atan(x) } });
			//plotter.Children.Add(new LineChart { DataSource = new FunctionalDataSource { Function = x => Math.Tan(x) } });
#else
			var ds2 = new Microsoft.Research.DynamicDataDisplay.DataSources.RawDataSource(pts);
			lineGraph = plotter.AddLineGraph(ds2);
#endif

			((NumericAxis)plotter.MainHorizontalAxis).TicksProvider = new CustomBaseNumericTicksProvider(Math.PI);
			((NumericAxis)plotter.MainHorizontalAxis).LabelProvider = new CustomBaseNumericLabelProvider(Math.PI, "π");
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (lineGraph == null)
				return;

			//((Microsoft.Research.DynamicDataDisplay.Filters.InclinationFilter)lineGraph.Filters[0]).CriticalAngle = e.NewValue;
		}

		private double Sinc(double x)
		{
			return x == 0 ? 1 : Math.Sin(x) / x;
		}
	}
}
