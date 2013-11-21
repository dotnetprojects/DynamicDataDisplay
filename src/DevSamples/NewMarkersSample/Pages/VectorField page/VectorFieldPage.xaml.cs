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

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for VectorField.xaml
	/// </summary>
	public partial class VectorFieldPage : Page
	{
		public VectorFieldPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			WarpedDataSource2D<Vector> vectorField = LoadVectorField();
			vectorChart.ItemsSource = vectorField;
		}

		private WarpedDataSource2D<Vector> LoadVectorField()
		{
			Random rnd = new Random();
			const int count = 20;
			var grid = DataSource2DHelper.CreateUniformGrid(count, count, 1, 1);
			var data = DataSource2DHelper.CreateVectorData(count, count, (x, y) =>
			{
				const double scale = Math.PI;
				return new Vector(Math.Sin(scale * rnd.NextDouble()), Math.Sin(scale * rnd.NextDouble())) / count / 2;
			});

			WarpedDataSource2D<Vector> dataSource = new WarpedDataSource2D<Vector>(data, grid);
			return dataSource;
		}
	}
}
