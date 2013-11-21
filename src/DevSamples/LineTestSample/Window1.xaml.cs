using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Threading;

namespace LineTestSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
		public Window1()
		{
			//Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

			InitializeComponent();

			plotter.Viewport.Domain = new Rect(-1, -1.2, 20, 2.4);
			//plotter.Viewport.Restrictions.Add(new DomainRestriction { });
			plotter.Children.Add(new HorizontalScrollBar());
			//plotter.HorizontalAxis.Remove();
			//plotter.VerticalAxis.Remove();
			//plotter.AxisGrid.Remove();
			//plotter.Children.Add(new VerticalScrollBar());

			//plotter.Viewport.Restrictions.Add(new DataHeightRestriction());

			plotter.AxisGrid.DrawHorizontalMinorTicks = false;
			plotter.AxisGrid.DrawVerticalMinorTicks = false;

			lineGraph.DataSource = CreateSineDataSource(1.0);

			timer.Tick += new EventHandler(timer_Tick);
			//timer.Start();
		}

		TimeSpan elapsed = new TimeSpan();
		void timer_Tick(object sender, EventArgs e)
		{
			elapsed = elapsed.Add(timer.Interval);
			double millis = elapsed.TotalMilliseconds;

			double x = ((millis % 1000) - 500) / 500;
			var visible = plotter.Viewport.Visible;
			visible.XMin = x;
			plotter.Visible = visible;

			if (millis > 10000)
				Close();
		}

		Random rnd = new Random();
		private void addDataSourceBtn_Click(object sender, RoutedEventArgs e)
		{
			var line = plotter.AddLineGraph(CreateSineDataSource(rnd.NextDouble()));
			DataFollowChart followChart = new DataFollowChart(line);
			plotter.Children.Add(followChart);

			//plotter.AddLineGraph(CreateLineDataSource(rnd.NextDouble()), 2);
		}

		private IPointDataSource CreateSineDataSource(double phase)
		{
			const int N = 100;

			Point[] pts = new Point[N];
			for (int i = 0; i < N; i++)
			{
				double x = i / (N / 10.0) + phase;
				pts[i] = new Point(x, Math.Sin(x - phase));
			}

			var ds = new EnumerableDataSource<Point>(pts);
			ds.SetXYMapping(pt => pt);

			return ds;
		}

		private IPointDataSource CreateLineDataSource(double phase)
		{
			const int N = 100;

			Point[] pts = new Point[N];
			for (int i = 0; i < N; i++)
			{
				double x = i / (N / 10.0);
				pts[i] = new Point(x, x * phase + phase);
			}

			var ds = new EnumerableDataSource<Point>(pts);
			ds.SetXYMapping(pt => pt);

			return ds;
		}
	}
}
