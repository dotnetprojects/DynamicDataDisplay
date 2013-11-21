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
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;

namespace Line3DSample
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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var dataSource =
				//LoadDataSourceRhw();
			IsolineSampleApp.Window1.LoadField();
			LoadDataSource();
			DataContext = dataSource;
		}

		private WarpedDataSource2D<double> LoadDataSourceRhw()
		{
			int size = 10;
			Point[,] grid = new Point[size, size];
			double[,] data = new double[size, size];
			for (int iy = 0; iy < size; iy++)
			{
				for (int ix = 0; ix < size; ix++)
				{
					Point pt = new Point(50 + 50 * ix, 50 + 50 * iy);
					grid[ix, iy] = pt;

					data[ix, iy] = ix + ix * iy;
				}
			}

			WarpedDataSource2D<double> ds = new WarpedDataSource2D<double>(data, grid);
			return ds;
		}

		private WarpedDataSource2D<double> LoadDataSource()
		{
			int size = 10;
			Point[,] grid = new Point[size, size];
			double[,] data = new double[size, size];
			for (int iy = 0; iy < size; iy++)
			{
				for (int ix = 0; ix < size; ix++)
				{
					Point pt = new Point(ix * 0.02, iy * 0.02);
					grid[ix, iy] = pt;

					data[ix, iy] = ix + iy;
				}
			}

			WarpedDataSource2D<double> ds = new WarpedDataSource2D<double>(data, grid);
			return ds;
		}
	}
}
