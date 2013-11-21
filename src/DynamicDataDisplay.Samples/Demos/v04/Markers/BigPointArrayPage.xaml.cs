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
using System.ComponentModel;
using System.Diagnostics;
using DynamicDataDisplay.Markers.Filters;
using Microsoft.Research.DynamicDataDisplay;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for BigPointArrayPage.xaml
	/// </summary>
	public partial class BigPointArrayPage : Page
	{
		public BigPointArrayPage()
		{
			InitializeComponent();
			plotter.Children.Add(plotter.NewLegend);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.Visible = new DataRect(0, 0, 1, 1);

			int count = (int)1e5;
			Point[] pts = new Point[count];

			Random rnd = new Random();
			for (int i = 0; i < count; i++)
			{
				pts[i] = new Point(rnd.NextDouble(), rnd.NextDouble());
			}

			markerChart.AddPropertyBinding<Point>(Shape.ToolTipProperty, p =>
			{
				return String.Format("X: {0:F2}   Y: {1:F2}", p.X, p.Y);
			});

			HSBPalette palette = new HSBPalette();
			markerChart.AddPropertyBinding<Point>(Shape.FillProperty, p =>
			{
				double length = Math.Sqrt(p.X * p.X + p.Y * p.Y) / Math.Sqrt(2);

				return new SolidColorBrush(palette.GetColor(length));
			});

			//markerChart.Filters.Add(new BoundsFilter());
			//markerChart.Filters.Add(new ParallelUnitingPointGroupFilter());
			markerChart.Filters.Add(new ParallelClusteringFilter { MarkerSize = 8 });
			markerChart.Filters.Add(new UnitingPointGroupFilter { MarkerSize = 6 });
			markerChart.GetDataAsyncronously = true;
			//markerChart.ShowMarkersConsequently = false;

			markerChart.ItemsSource = pts;
		}
	}
}
