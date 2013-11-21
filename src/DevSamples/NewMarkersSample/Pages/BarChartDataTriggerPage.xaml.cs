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
using DynamicDataDisplay.Markers;
using System.Windows.Threading;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for BarChartBage.xaml
	/// </summary>
	public partial class BarChartDataTriggerPage : Page
	{
		public BarChartDataTriggerPage()
		{
			InitializeComponent();
		}

		int start = Environment.TickCount;
		private void timer_Tick(object sender, EventArgs e)
		{
			int time = Environment.TickCount;
			int dt = (time - start);
			double speed = 0.0005;
			for (int i = 0; i < segments.Length; i++)
			{
				var segment = segments[i];
				segment.X = i + 0.2 * Math.Sin(i + speed * dt);
				segment.YMax = 3 + 0.5 + 0.5 * Math.Sin(i + 0.32 * speed * dt);
			}
		}

		DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
		Segment[] segments;
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= Page_Loaded;

			segments = Segment.LoadSegments(10);

			DataContext = segments;

			//timer.Tick += new EventHandler(timer_Tick);
			//timer.Start();
		}
	}
}
