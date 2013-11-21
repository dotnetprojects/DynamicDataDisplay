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

using Microsoft.Research.DynamicDataDisplay;

namespace LegendSample.Pages
{
	/// <summary>
	/// Interaction logic for DisconnectedLegendMode.xaml
	/// </summary>
	public partial class DisconnectedLegendMode : Page
	{
		public DisconnectedLegendMode()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			//plotter.AddLineGraph(DataSourceHelper.CreateSineDataSource(), "Sin(x)");
		}

		private Random rnd = new Random();
		private void addRandomChartBtn_Click(object sender, RoutedEventArgs e)
		{
			double freq = rnd.NextDouble();
			plotter.AddLineGraph(DataSourceHelper.CreateDataSource(x => Math.Sin(freq * x)));
		}
	}
}
