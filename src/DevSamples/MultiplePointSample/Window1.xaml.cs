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
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace MultiplePointSample
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

		double size;
		const int pointsNum = 100000;
		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			size = 0 + 1 * (Math.Sqrt(pointsNum) / 10);
			List<Point> pts = new List<Point>(pointsNum);

			using (new DisposableTimer("Points creating"))
			{
				for (int i = 0; i < pointsNum; i++)
				{
					pts.Add(CreateRandomPoint());
				}
			}

			using (new DisposableTimer("Setting ItemsSource"))
			{
				chart.DataSource = pts;
			}
		}

		private Random rnd = new Random();
		private Point CreateRandomPoint()
		{
			Point pt = new Point();
			pt.X = rnd.NextDouble() * size - 1 * size / 2;
			pt.Y = rnd.NextDouble() * size - 1 * size / 2;
			return pt;
		}
	}
}
