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

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for StockMarkersPage.xaml
	/// </summary>
	public partial class StockMarkersPage : Page
	{
		public StockMarkersPage()
		{
			InitializeComponent();
		}

		private readonly ObservableCollection<StockInfo> stockData = new ObservableCollection<StockInfo>();
		private Random rnd = new Random();

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			// making both plotters' viewport connected
			plotter.Viewport.SetBinding(Viewport2D.VisibleProperty, new Binding("Visible") { Source = plotter2.Viewport, Mode = BindingMode.TwoWay });

			DateTime now = DateTime.Now;
			dateAxis.SetConversion(1000, now, 1010, now.AddDays(10));
			dateAxis2.SetConversion(1000, now, 1010, now.AddDays(10));

			// setting visible region in terms of <DateTime, double> rectangle, not usual <double, double> one.
			var genericPlotter = plotter.GetGenericPlotter<DateTime, double>();
			genericPlotter.ViewportRect = new GenericRect<DateTime, double>(DateTime.Now.AddDays(-2), -2.5, DateTime.Now.AddDays(60), 2.5);

			// initialialize stock data
			const int count = 35;
			for (int i = 0; i < count; i++)
			{
				stockData.Add(CreateNewStockInfo());
			}

			// this will make charts to display data,
			// as they are inside window, and their DataContext properties will change as Window's DataContext changes.
			// This happens because DataContextProperty inherits its value from parent in LogicalTree.
			DataContext = stockData;
		}

		private int daysCounter = 0;
		private StockInfo CreateNewStockInfo()
		{
			double open;
			if (stockData.Count == 0)
			{
				open = rnd.NextDouble();
			}
			else
			{
				open = stockData[stockData.Count - 1].Close;
			}

			double close = rnd.NextDouble();

			double low = Math.Min(open, close) - rnd.NextDouble();
			double high = Math.Max(open, close) + rnd.NextDouble();

			DateTime time = DateTime.Now.Date.AddDays(daysCounter++);

			StockInfo info = new StockInfo { Open = open, Close = close, High = high, Low = low, Time = time };
			return info;
		}

		private void addStockInfo_Click(object sender, RoutedEventArgs e)
		{
			stockData.Add(CreateNewStockInfo());
		}
	}

	public sealed class StockInfo
	{
		public double Open { get; set; }
		public double Close { get; set; }
		public double High { get; set; }
		public double Low { get; set; }

		public DateTime Time { get; set; }
	}
}
