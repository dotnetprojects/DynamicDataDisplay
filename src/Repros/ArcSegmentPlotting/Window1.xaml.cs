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
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace ArcSegmentPlotting
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
			AddLineGraph(new Point(0, 0), new Point(0, 50));
			AddCircularArcGraph(new Point(0, 0), new Point(50, -50), new Size(50, 50));
			AddLineGraph(new Point(50, 50), new Point(0, 50));
		}

		private void AddLineGraph(Point startPoint, Point endPoint)
		{
			var ds = new EnumerableDataSource<Point>(new List<Point> {
        new Point { X = startPoint.X, Y = startPoint.Y }, 
        new Point { X = endPoint.X, Y = endPoint.Y } 
      });
			ds.SetXYMapping(pt => pt);

			plotter.AddLineGraph(ds);
		}

		private void AddCircularArcGraph(Point startPoint, Point endPoint, Size size)
		{
			PathFigure pf = new PathFigure();
			pf.StartPoint = new Point(startPoint.X, startPoint.Y);

			ArcSegment arcSegment = new ArcSegment();
			arcSegment.Point = new Point(endPoint.X, endPoint.Y);
			arcSegment.Size = size;
			arcSegment.SweepDirection = SweepDirection.Counterclockwise;

			PathSegmentCollection psc = new PathSegmentCollection();
			psc.Add(arcSegment);

			pf.Segments = psc;

			PathFigureCollection pfc = new PathFigureCollection();
			pfc.Add(pf);

			PathGeometry pg = new PathGeometry();
			pg.Figures = pfc;

			var path = new Path();
			path.Stroke = Brushes.Black;
			path.StrokeThickness = 1;
			path.Data = pg;
			path.Fill = Brushes.Orange;
			path.Stretch = Stretch.Fill;

			var viewportPanel = new ViewportHostPanel();
			ViewportPanel.SetViewportBounds(path, new DataRect(0, 0, 50, 50));
			viewportPanel.Children.Add(path);
			plotter.Children.Add(viewportPanel);
		}
	}
}
