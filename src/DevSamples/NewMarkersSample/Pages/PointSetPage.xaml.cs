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
using DynamicDataDisplay.Markers.Filters;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for PointSetPage.xaml
	/// </summary>
	public partial class PointSetPage : Page
	{
		public PointSetPage()
		{
			InitializeComponent();
		}

		private int i = 0;
		private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
		private readonly List<Point> points = new List<Point>();
		private readonly List<Point> points2 = new List<Point>();
		private void plotter_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Tick += timer_Tick;
			timer.Start();

			chart.BoundsUnionMode = BoundsUnionMode.Center;
			chart2.BoundsUnionMode = BoundsUnionMode.Center;

			chart.Filters.Add(new UnitingPointGroupFilter { MarkerSize = 5 });
			chart2.Filters.Add(new UnitingPointGroupFilter { MarkerSize = 10 });

			chart.ItemsSource = points;
			chart2.ItemsSource = points2;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			GenerateNextPoints();

			if (chart.DataSource != null)
				chart.DataSource.RaiseCollectionReset();

			if (chart2.DataSource != null)
				chart2.DataSource.RaiseCollectionReset();
		}

		private void GenerateNextPoints()
		{
			Point p = GeneratePoint(i);
			points.Add(p);

			p = GeneratePoint2(i);
			points2.Add(p);

			i++;
		}

		private Point GeneratePoint(int i)
		{
			return new Point((i - 128) % 256 - 128, i);
		}

		private Point GeneratePoint2(int i)
		{
			return new Point(128 - i % 256, i);
		}

		private void timerControlBtn_Click(object sender, RoutedEventArgs e)
		{
			if (timer.IsEnabled)
			{
				timer.Stop();
				timerControlBtn.Content = "Run timer";
			}
			else
			{
				timer.Start();
				timerControlBtn.Content = "Stop timer";
			}
		}
	}
}
