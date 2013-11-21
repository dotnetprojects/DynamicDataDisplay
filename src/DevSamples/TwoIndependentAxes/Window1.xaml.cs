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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace TwoIndependentAxes
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

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			innerPlotter.SetVerticalTransform(0, 0, 124, 58);

			var rpms = Enumerable.Range(0, 9).Select(i => i * 1000.0).AsXDataSource();
			var hps = new double[] { 0, 24, 52, 74, 98, 112, 124, 122, 116 }.AsYDataSource();

			var horsePowersDS = rpms.Join(hps);
			plotter.AddLineGraph(horsePowersDS, Colors.Red, 2, "HP per RPM");

			var torque = new double[] { 0, 22, 45, 54, 58, 55, 50, 47, 45 }.AsYDataSource();
			var torqueDS = rpms.Join(torque);

			var line = innerPlotter.AddLineGraph(torqueDS, Colors.Blue, 2, "Torque per RPM");
		}

		private void removeAllChartsBtn_Click(object sender, RoutedEventArgs e)
		{
			innerPlotter.Children.RemoveAll<LineGraph>();
			plotter.Children.RemoveAll<LineGraph>();
		}
	}
}
