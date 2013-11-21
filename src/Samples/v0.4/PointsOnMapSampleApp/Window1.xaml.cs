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
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.IO;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Diagnostics;

namespace PointSetOnMap
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			dataTypeCombobox.ItemsSource = new List<DataType> 
			{ 
				DataType.Temp, DataType.Rainfall, DataType.SoilDepth 
			};

			Loaded += Window1_Loaded;
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			LoadData();

			dataTypeCombobox.SelectedIndex = 0;
			bool res = dataTypeCombobox.Focus();
		}

		private EnumerableDataSource<DataPoint> CreateDataSource(IEnumerable<DataPoint> data)
		{
			EnumerableDataSource<DataPoint> ds = new EnumerableDataSource<DataPoint>(data);

			MercatorTransform transform = new MercatorTransform();

			ds.SetXMapping(p => p.X);
			ds.SetYMapping(p => transform.DataToViewport(new Point(0, p.Y)).Y);
			ds.AddMapping(CirclePointMarker.FillProperty, dp =>
			{
				double alpha = (dp.Data - currentRange.Min) / (currentRange.Max - currentRange.Min);

				Debug.Assert(0 <= alpha && alpha <= 1);

				const double hueWidth = 100;
				double hue = hueWidth * (alpha - 0.5) + hueSlider.Value;

				if (hue > 360) hue -= 360;
				else if (hue < 0) hue += 360;

				Debug.Assert(0 <= hue && hue <= 360);

				Color mainColor = new HsbColor(hue, 1, 0 + 1 * alpha, 0.3 + 0.7 * alpha).ToArgbColor();

				const int colorCount = 5;
				GradientStopCollection colors = new GradientStopCollection(colorCount);

				double step = 1.0 / (colorCount - 1);
				for (int i = 0; i < colorCount; i++)
				{
					Color color = mainColor;
					double x = attSlider.Value * step * i;
					color.A = (byte)(255 * Math.Exp(-x * x));
					colors.Add(new GradientStop(color, step * i));
				}

				return new RadialGradientBrush(colors);
			});
			return ds;
		}

		private List<SampleDataPoint> loadedData = new List<SampleDataPoint>();
		private void LoadData()
		{
			string[] strings = File.ReadAllLines("example_for_visualization.csv");

			// skipping 1st line, parsing all other lines
			for (int i = 1; i < strings.Length; i++)
			{
				SampleDataPoint point = ParseDataPoint(strings[i]);
				loadedData.Add(point);
			}

			tempRange.Min = loadedData.Min(p => p.Temp);
			tempRange.Max = loadedData.Max(p => p.Temp);

			rainfallRange.Min = loadedData.Min(p => p.RainFall);
			rainfallRange.Max = loadedData.Max(p => p.RainFall);

			soildepthRange.Min = loadedData.Min(p => p.SoilDepth);
			soildepthRange.Max = loadedData.Max(p => p.SoilDepth);
		}

		MinMax tempRange = new MinMax();
		MinMax rainfallRange = new MinMax();
		MinMax soildepthRange = new MinMax();

		MinMax currentRange;
		private SampleDataPoint ParseDataPoint(string str)
		{
			var pieces = str.Split(',');

			SampleDataPoint res = new SampleDataPoint();

			res.Lat = Double.Parse(pieces[0], CultureInfo.InvariantCulture);
			res.Lon = Double.Parse(pieces[1], CultureInfo.InvariantCulture);

			res.Temp = Double.Parse(pieces[2], CultureInfo.InvariantCulture);
			res.RainFall = Double.Parse(pieces[3], CultureInfo.InvariantCulture);
			res.SoilDepth = Double.Parse(pieces[4], CultureInfo.InvariantCulture);

			return res;
		}

		private IEnumerable<DataPoint> GetSampleData(DataType dataType)
		{
			switch (dataType)
			{
				case DataType.Temp:
					currentRange = tempRange;
					return loadedData.Select(dp => new DataPoint { X = dp.Lon, Y = dp.Lat, Data = dp.Temp }).ToList();
				case DataType.Rainfall:
					currentRange = rainfallRange;
					return loadedData.Select(dp => new DataPoint { X = dp.Lon, Y = dp.Lat, Data = dp.RainFall }).ToList();
				case DataType.SoilDepth:
					currentRange = soildepthRange;
					return loadedData.Select(dp => new DataPoint { X = dp.Lon, Y = dp.Lat, Data = dp.SoilDepth }).ToList();
				default:
					throw new InvalidOperationException();
			}
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (plotter == null)
				return;

			var graph = plotter.Children.OfType<MarkerPointsGraph>().FirstOrDefault();
			if (graph != null)
				graph.InvalidateVisual();
		}

		private void dataTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (graph == null || plotter == null)
				return;

			graph.DataSource = CreateDataSource(GetSampleData((DataType)dataTypeCombobox.SelectedValue));
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink link = (Hyperlink)sender;
			Process.Start(link.NavigateUri.ToString());
		}
	}

	class MinMax
	{
		public double Min { get; set; }
		public double Max { get; set; }
	}

	class SampleDataPoint
	{
		public double Lat { get; set; }
		public double Lon { get; set; }
		public double Temp { get; set; }
		public double RainFall { get; set; }
		public double SoilDepth { get; set; }
	}

	class DataPoint
	{
		public double X { get; set; }
		public double Y { get; set; }

		public double Data { get; set; }
	}

	enum DataType
	{
		Temp,
		Rainfall,
		SoilDepth
	}
}
