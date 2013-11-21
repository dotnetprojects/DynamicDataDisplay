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
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace VerticalRangeNotVisibleRepro
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private readonly Random rnd = new Random();
		private void AddRandomRangeBtn_Click(object sender, RoutedEventArgs e)
		{
			double min = rnd.NextDouble();
			double max = min + rnd.NextDouble();
			VerticalRange range = new VerticalRange { Value1 = min, Value2 = max, Fill = Brushes.Green };

			plotter.Children.Add(range);
		}
	}
}
