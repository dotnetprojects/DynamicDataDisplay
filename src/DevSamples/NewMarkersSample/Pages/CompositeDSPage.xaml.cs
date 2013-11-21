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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using DynamicDataDisplay.Markers.DataSources;

namespace NewMarkersSample
{
	/// <summary>
	/// Interaction logic for CompositeDSWindow.xaml
	/// </summary>
	public partial class CompositeDSPage : Page
	{
		public CompositeDSPage()
		{
			InitializeComponent();
		}

		double[] xs = new double[count];
		double[] ys = new double[count];
		const int count = 200;
		CompositeDataSource ds;
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < count; i++)
			{
				xs[i] = i / (double)count;
				ys[i] = i / (double)count;
			}

			ds = new CompositeDataSource(new DataSourcePartCollection(
				new DataSourcePart(xs, "X"),
				new DataSourcePart(ys, "Y")));

			chart.DataSource = ds;
		}

		Random rnd = new Random();
		private void changeAngleBtn_Click(object sender, RoutedEventArgs e)
		{
			double angle = rnd.NextDouble();
			for (int i = 0; i < count; i++)
			{
				ys[i] = angle * i / (double)count;
			}

			ds.ReplacePart("Y", ys);
		}
	}
}
