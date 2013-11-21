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

namespace VectorFieldSampleApp
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
			int width = 200;
			int height = 200;
			Vector[,] data =
				DataSource2DHelper.CreateVectorData(width, height, (x, y) => ((int)(x / 40)) % 2 == 0 && ((int)(y / 40)) % 2 == 0 ? new Vector(1, 0) : new Vector(0, 1));
			//DataSource2DHelper.CreateVectorData(width, height, (ix, iy) => new Vector(Math.Sin(((double)ix) / 30), Math.Cos(((double)iy) / 30)));
			//DataSource2DHelper.CreateVectorData(width, height, (x, y) =>
			//{
			//    Vector result;

			//    double xc = x - width / 2;
			//    double yc = y - height / 2;
			//    if (xc != 0)
			//    {
			//        double beta = Math.Sqrt(1.0 / (1 + yc * yc / (xc * xc)));
			//        double alpha = -beta * yc / xc;
			//        result = new Vector(alpha, beta);
			//    }
			//    else
			//    {
			//        double alpha = Math.Sqrt(1.0 / (1 + xc * xc / (yc * yc)));
			//        double beta = -alpha * xc / yc;
			//        result = new Vector(alpha, beta);
			//    }

			//    if (Double.IsNaN(result.X))
			//    {
			//        result = new Vector(0, 0);
			//    }

			//    return result;
			//});

			double[] xs = Enumerable.Range(0, width).Select(i => (double)i).ToArray();
			double[] ys = Enumerable.Range(0, height).Select(i => (double)i).ToArray();

			var dataSource = new NonUniformDataSource2D<Vector>(xs, ys, data);
			DataContext = dataSource;
		}
	}
}
