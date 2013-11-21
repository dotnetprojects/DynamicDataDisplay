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
using System.Windows.Threading;
using System.Collections.ObjectModel;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DynamicPointAddSample
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

		private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
		private ObservableCollection<Point> data = new ObservableCollection<Point>();
		private int x = 0;
		private Random rnd = new Random();

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			// initial collection content
			for (int i = 0; i < 10000; i++)
			{
				AddNextPoint();
			}

			// switching off approximate content bounds' comparison, as this can cause improper behavior.
			plotter.Viewport.UseApproximateContentBoundsComparison = false;
			// adding line chart to plotter
			var line = plotter.AddLineGraph(data.AsDataSource());
			// again switching off approximate content bounds' comparison - now in coercion method of Viewport2D.ContentBounds attached dependency property.
			Viewport2D.SetUsesApproximateContentBoundsComparison(line, false);

			// simulating data being received from external source on each timer tick
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			AddNextPoint();
			plotter.FitToView();
		}

		private void AddNextPoint()
		{
			Point p = new Point { X = x++, Y = rnd.NextDouble() };
			data.Add(p);
		}
	}
}
