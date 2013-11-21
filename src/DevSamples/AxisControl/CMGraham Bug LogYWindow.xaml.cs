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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace AxisControlSample
{
	/// <summary>
	/// Interaction logic for LogYWindow.xaml
	/// </summary>
	public partial class CMLogYWindow : Window
	{
		public CMLogYWindow()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(LogYWindow_Loaded);
		}

		EnumerableDataSource<TPoint> edsLog10;
		void LogYWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//ChartPlotter plotter = new ChartPlotter();

			//plotter.Children.Add(new CursorCoordinateGraph());

			//plotter.DataTransform = new Log10YTransform();
			//VerticalAxis axis = new VerticalAxis
			//{
			//    TicksProvider = new LogarithmNumericTicksProvider(10),
			//    LabelProvider = new UnroundingLabelProvider()
			//};
			//plotter.MainVerticalAxis = axis;

			//plotter.AxisGrid.DrawVerticalMinorTicks = true;

			//const int count = 500;
			//double[] xs = Enumerable.Range(1, count).Select(x => x * 0.01).ToArray();
			//EnumerableDataSource<double> xDS = xs.AsXDataSource();

			//var pows = xs.Select(x => Math.Pow(10, x));
			//var linear = xs.Select(x => x);
			//var logXs = Enumerable.Range(101, count).Select(x => x * 0.01);
			//var logarithmic = logXs.Select(x => Math.Log10(x));

			//plotter.AddLineGraph(pows.AsYDataSource().Join(xDS), "f(x) = 10^x");
			//plotter.AddLineGraph(linear.AsYDataSource().Join(xDS), "f(x) = x");
			//plotter.AddLineGraph(logarithmic.AsYDataSource().Join(logXs.AsXDataSource()), "f(x) = log(x)");

			//Content = plotter;

			ChartPlotter plotter = new ChartPlotter();
			plotter.DataTransform = new Log10YTransform();
			VerticalAxis yAxis = new VerticalAxis
			{
				TicksProvider = new LogarithmNumericTicksProvider(10),
				LabelProvider = new UnroundingLabelProvider()
			};
			plotter.MainVerticalAxis = yAxis;
			plotter.AxisGrid.DrawVerticalMinorTicks = true;

			HorizontalDateTimeAxis xAxis = new HorizontalDateTimeAxis();
			plotter.MainHorizontalAxis = xAxis;

			EnumerableDataSource<TPoint> edsPow = new EnumerableDataSource<TPoint>(
				Enumerable.Range(1, 2000).Select(s =>
					new TPoint
					{
						X = DateTime.Now.AddYears(-20).AddDays(s),
						Y = Math.Pow(10, s / 5000.0)
					}
					).ToList());
			//edsPow.SetXYMapping(s => new Point(xax.ConvertToDouble(s.X), s.Y));
			edsPow.SetXMapping(s => xAxis.ConvertToDouble(s.X));
			edsPow.SetYMapping(s => yAxis.ConvertToDouble(s.Y));
			edsPow.AddMapping(ShapeElementPointMarker.ToolTipTextProperty, s => String.Format("Vol:{0}\r\nOn:{1}", s.Y, s.X.ToShortDateString()));

			EnumerableDataSource<TPoint> edsLinear = new EnumerableDataSource<TPoint>(
				Enumerable.Range(1, 2000).Select(s =>
					new TPoint
					{
						X = DateTime.Now.AddYears(-20).AddDays(s),
						Y = s / 2000.0
					}
					).ToList());
			//edsLinear.SetXYMapping(s => new Point(xax.ConvertToDouble(s.X), s.Y));
			edsLinear.SetXMapping(s => xAxis.ConvertToDouble(s.X));
			edsLinear.SetYMapping(s => yAxis.ConvertToDouble(s.Y));
			edsLinear.AddMapping(ShapeElementPointMarker.ToolTipTextProperty, s => String.Format("Vol:{0}\r\nOn:{1}", s.Y, s.X.ToShortDateString()));

			edsLog10 = new EnumerableDataSource<TPoint>(
				Enumerable.Range(1, 2000).Select(s =>
					new TPoint
					{
						X = DateTime.Now.AddYears(-20).AddDays(s),
						Y = Math.Log10(s)
					}
					).Where(s => s.Y > 0).ToList());
			//edsLog10.SetXYMapping(s => new Point(xax.ConvertToDouble(s.X), s.Y));
			edsLog10.SetXMapping(s => xAxis.ConvertToDouble(s.X));
			edsLog10.SetYMapping(s => yAxis.ConvertToDouble(s.Y));
			edsLog10.AddMapping(ShapeElementPointMarker.ToolTipTextProperty, s => String.Format("Vol:{0}\r\nOn:{1}", s.Y, s.X.ToShortDateString()));

			SolidColorBrush brushPow = new SolidColorBrush(Colors.Green);
			Pen linePenPow = new Pen(brushPow, 2.0);
			CircleElementPointMarker markerPow = new CircleElementPointMarker
			{
				Size = 4,
				Brush = brushPow,
				Fill = brushPow
			};
			PenDescription descPow = new PenDescription("f(x) = 10 ^ x");
			//var chartPow = plotter.AddLineGraph(edsPow, linePenPow, (ShapeElementPointMarker)null/*markerPow*/, descPow);

			SolidColorBrush brushLinear = new SolidColorBrush(Colors.Blue);
			Pen linePenLinear = new Pen(brushLinear, 2.0);
			CircleElementPointMarker markerLinear = new CircleElementPointMarker
			{
				Size = 4,
				Brush = brushLinear,
				Fill = brushLinear
			};
			PenDescription descLinear = new PenDescription("f(x) = x");
			//var chartLinear = plotter.AddLineGraph(edsLinear, linePenLinear, (ShapeElementPointMarker)null/*markerLinear*/, descLinear);

			SolidColorBrush brushLog10 = new SolidColorBrush(Colors.Red);
			Pen linePenLog10 = new Pen(brushLog10, 2.0);
			CircleElementPointMarker markerLog10 = new CircleElementPointMarker
			{
				Size = 4,
				Brush = brushLog10,
				Fill = brushLog10
			};
			PenDescription descLog10 = new PenDescription("f(x) = log(x)");
			var chartLog10 = plotter.AddLineGraph(edsLog10, linePenLog10, (ShapeElementPointMarker)null/*markerLog10*/, descLog10);

			//plotter.Children.Add(new DataFollowChart(chartPow.LineGraph).WithProperty(c =>
			//{
			//    c.Marker.SetValue(Shape.FillProperty, brushPow);
			//    c.Marker.SetValue(Shape.StrokeProperty, brushPow.MakeDarker(0.2));
			//}));
			//plotter.Children.Add(new DataFollowChart(chartLinear.LineGraph).WithProperty(c =>
			//{
			//    c.Marker.SetValue(Shape.FillProperty, brushLinear);
			//    c.Marker.SetValue(Shape.StrokeProperty, brushLinear.MakeDarker(0.2));
			//}));

			plotter.Children.Add(new CursorCoordinateGraph());

			ValueStore store = new ValueStore();
			DataFollowChart dataFollowChart = new DataFollowChart(chartLog10.LineGraph);
			dataFollowChart.Marker.SetValue(Shape.FillProperty, brushLog10);
			dataFollowChart.Marker.SetValue(Shape.StrokeProperty, brushLog10.ChangeLightness(0.8));
			dataFollowChart.MarkerPositionChanged += new EventHandler(dataFollowChart_MarkerPositionChanged);
			//dataFollowChart.CustomDataContextCallback = () =>
			//    {
			//        if (dataFollowChart.ClosestPointIndex != -1)
			//        {
			//            var closestPoint = ((List<TPoint>)edsLog10.Data)[dataFollowChart.ClosestPointIndex];
			//            return store.SetValue("X", closestPoint.Y.ToString()).SetValue("Y", closestPoint.X.ToShortDateString());
			//        }
			//        else return null;
			//    };

#if true
			dataFollowChart.MarkerTemplate = (DataTemplate)FindResource("followMarkerTemplate");
#else
				dataFollowChart.MarkerAdjustCallback = marker =>
				{
				    Ellipse ellipse = (Ellipse)marker;
				    var markerPosition = c.MarkerPosition;
				    var date = xAxis.ConvertFromDouble(markerPosition.X);
				    var y = yAxis.ConvertFromDouble(markerPosition.Y);
				    ellipse.ToolTip = String.Format("Vol:{0}\r\nOn:{1}", y, date.ToShortDateString());
				};
#endif

			plotter.Children.Add(dataFollowChart);

			Content = plotter;
		}

		void dataFollowChart_MarkerPositionChanged(object sender, EventArgs e)
		{
			DataFollowChart chart = (DataFollowChart)sender;
			ValueStore store;
			if (chart.FollowDataContext.Data == null)
			{
				store = new ValueStore("X", "Y");
				chart.FollowDataContext.Data = store;
			}
			else
			{
				store = (ValueStore)chart.FollowDataContext.Data;
			}

			if (chart.ClosestPointIndex != -1)
			{
				var closestPoint = ((List<TPoint>)edsLog10.Data)[chart.ClosestPointIndex];

				store["X"] = closestPoint.Y.ToString();
				store["Y"] = closestPoint.X.ToShortDateString();
			}
		}
	}

	public class TPoint
	{
		public TPoint() { }
		public TPoint(DateTime x, double y)
		{
			_X = x;
			_Y = y;
		}

		private DateTime _X;
		public DateTime X
		{
			get { return _X; }
			set { _X = value; }
		}

		private double _Y;
		public double Y
		{
			get { return _Y; }
			set { _Y = value; }
		}
	}
}
