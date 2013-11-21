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

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for ConditionalBinding.xaml
	/// </summary>
	public partial class ConditionalBindingPage : Page
	{
		public ConditionalBindingPage()
		{
			InitializeComponent();
		}

		int startTime;
		DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
		const int len = 200;
		Point[] pts = new Point[len];
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < len; i++)
			{
				double x = i * 0.02 - 1;
				double y = Math.Sin(x * 10);

				Point p = new Point(x, y);
				pts[i] = p;
			}

			chart.AddPropertyBinding<Double>(Ellipse.FillProperty, y => y > 0 ? Brushes.Blue : Brushes.Red, "Y");
			chart.ItemsSource = pts;

			startTime = Environment.TickCount;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			int dt = Environment.TickCount - startTime;

			for (int i = 0; i < len; i++)
			{
				Point p = pts[i];
				var x = p.X;
				p.Y = Math.Sin((x + dt * 0.00005) * 10);

				pts[i] = p;
			}

			chart.DataSource.RaiseCollectionReset();
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			timer.Tick -= new EventHandler(timer_Tick);
			timer.Stop();
		}
	}
}
