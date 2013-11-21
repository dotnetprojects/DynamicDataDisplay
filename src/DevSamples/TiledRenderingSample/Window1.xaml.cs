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
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace TiledRenderingSample
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

		private const int count = 50000;
		private readonly Point[] data = new Point[count];
		private readonly Random rnd = new Random();
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			GenerateData();

			HSBPalette palette = new HSBPalette();
			//chart1.AddPropertyBinding<Point>(Shape.FillProperty, p =>
			//{
			//    double length = Math.Sqrt(p.X * p.X + p.Y * p.Y) / Math.Sqrt(2);
			//    return new SolidColorBrush(palette.GetColor(length));
			//});
			chart2.AddPropertyBinding<Point>(Shape.FillProperty, p =>
			{
				double length = Math.Sqrt(p.X * p.X + p.Y * p.Y) / Math.Sqrt(2);
				return new SolidColorBrush(palette.GetColor(length));
			});

			//chart1.ItemsSource = data;
			chart2.ItemsSource = data;
		}

		private void GenerateData()
		{
			for (int i = 0; i < count; i++)
			{
				data[i] = new Point(rnd.NextDouble(), rnd.NextDouble());
			}
		}
	}
}
