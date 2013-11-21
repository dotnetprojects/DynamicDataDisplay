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
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for DifferentBuildInMarkersPage.xaml
	/// </summary>
	public partial class DifferentBuildInMarkersPage : Page
	{
		public DifferentBuildInMarkersPage()
		{
			InitializeComponent();
			plotter.Children.Add(plotter.NewLegend);

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		List<ObservableCollectionWrapper<Point>> collections = new List<ObservableCollectionWrapper<Point>>();
		int startTime;
		DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
		const int count = 50;

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			startTime = Environment.TickCount;

			chart1.ItemsSource = BuildData(1);
			chart2.ItemsSource = BuildData(2);
			chart3.ItemsSource = BuildData(3);
			chart4.ItemsSource = BuildData(4);
			chart5.ItemsSource = BuildData(5);
			chart6.ItemsSource = BuildData(6);
			chart7.ItemsSource = BuildData(7);
			chart8.ItemsSource = BuildData(8);

			timer.Tick += timer_Tick;
			timer.Start();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			timer.Tick -= timer_Tick;
			timer.Stop();

			collections.Clear();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			for (int i = 0; i < collections.Count; i++)
			{
				using (collections[i].BlockEvents())
				{
					collections[i].Clear();
					FillCollection(i, collections[i], Environment.TickCount);
				}
			}
		}

		private IEnumerable<Point> BuildData(int dataSourceNumber)
		{
			ObservableCollectionWrapper<Point> collection = new ObservableCollectionWrapper<Point>(new ObservableCollection<Point>());

			FillCollection(dataSourceNumber, collection, startTime);

			return collection;
		}

		private void FillCollection(int dataSourceNumber, ObservableCollectionWrapper<Point> collection, int time)
		{
			for (int i = 0; i < count; i++)
			{
				double x = i / (double)count;
				collection.Add(new Point(x, 0.1 * dataSourceNumber + 0.06 * Math.Sin(10 * x + Math.Sqrt(dataSourceNumber + 1) * 0.0005 * (time - startTime))));
			}

			if (time == startTime)
			{
				collections.Add(collection);
			}
		}
	}
}
