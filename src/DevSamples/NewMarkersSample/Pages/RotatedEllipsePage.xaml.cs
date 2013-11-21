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
using System.Collections.ObjectModel;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for RotatedEllipses.xaml
	/// </summary>
	public partial class RotatedEllipsesPage : Page
	{
		public RotatedEllipsesPage()
		{
			InitializeComponent();
		}

		ObservableCollection<Point> points = new ObservableCollection<Point>();
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AddRandomPoint();
			AddRandomPoint();
			AddRandomPoint();

			// these are functions that sets different properties of drawn ellipses;
			// these functions can be replaced by real, non-demo ones.
			chart.AddPropertyBindingForNamedPart<Point>("ellipse", Ellipse.FillProperty, p =>
			{
				double hue = Math.Sin(1000 * (p.X + p.Y)) * 180 + 180;
				var color = new HsbColor(hue, 0.2, 1);
				return new SolidColorBrush(color.ToArgbColor()).MakeTransparent(0.7);
			}); 
			chart.AddPropertyBindingForNamedPart<Point>("ellipse", Ellipse.StrokeProperty, p =>
			{
				double hue = Math.Sin(1000 * (p.X + p.Y)) * 180 + 180;
				var color = new HsbColor(hue, 1, 1);
				return new SolidColorBrush(color.ToArgbColor());
			});
			chart.AddPropertyBindingForNamedPart<Point>("ellipse", Ellipse.RenderTransformProperty, p => new RotateTransform(120 * Math.Atan2(p.Y, p.X)));
			chart.AddPropertyBinding<Point>(ViewportPanel.ViewportWidthProperty, p => 0.2 * Math.Abs(Math.Sin(p.X)));
			chart.AddPropertyBinding<Point>(ViewportPanel.ViewportHeightProperty, p => 0.2 * Math.Abs(Math.Cos(p.X)));

			DataContext = points;
		}

		private readonly Random rnd = new Random();
		private void AddRandomPoint()
		{
			Point p = new Point(rnd.NextDouble(-1, 1), rnd.NextDouble(-1, 1));
			points.Add(p);
		}

		private void AddRandomEllipseBtn_Click(object sender, RoutedEventArgs e)
		{
			AddRandomPoint();
		}
	}
}
