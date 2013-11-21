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

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for EndlessRandomValuesPage.xaml
	/// </summary>
	public partial class EndlessRandomValuesPage : Page
	{
		public EndlessRandomValuesPage()
		{
			InitializeComponent();
		}

		private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.Visible = new DataRect(0, -0.1, 10000, 1.2);

			for (int i = 0; i < 10; i++)
			{
				AddNextPoint();
			}

			chart.AddPropertyBinding<Point>(Shape.FillProperty, p => p.Y > 0.5 ? Brushes.Green : Brushes.Red);
			chart.ItemsSource = collection;

			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		private Random rnd = new Random();
		private double x = 0;
		private readonly ObservableCollection<Point> collection = new ObservableCollection<Point>();
		private void timer_Tick(object sender, EventArgs e)
		{
			AddNextPoint();
		}

		private void AddNextPoint()
		{
			collection.Add(new Point(++x, rnd.NextDouble()));
		}
	}
}
