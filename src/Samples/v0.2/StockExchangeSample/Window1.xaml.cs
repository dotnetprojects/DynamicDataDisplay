using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Reflection;

namespace StockExchangeSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		List<StockInfo> dj65;
		List<StockInfo> rts;
		List<StockInfo> micex;
		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			// setting custom position of legend:
			// it will be placed not in the top right corner of plotter,
			// but in the top left one
			plotter.Legend.LegendLeft = 10;
			plotter.Legend.LegendRight = Double.NaN;

			dj65 = LoadStockRates("DJ65.txt");
			rts = LoadStockRates("RTS.txt");
			micex = LoadStockRates("MICEX.txt");

			Color[] colors = ColorHelper.CreateRandomColors(3);
			plotter.AddLineGraph(CreateCurrencyDataSource(dj65), colors[0], 1, "Dow Jones 65 Composite");
			plotter.AddLineGraph(CreateCurrencyDataSource(rts), colors[1], 1, "RTS");
			plotter.AddLineGraph(CreateCurrencyDataSource(micex), colors[2], 1, "MICEX");
		}

		private EnumerableDataSource<StockInfo> CreateCurrencyDataSource(List<StockInfo> rates)
		{
			EnumerableDataSource<StockInfo> ds = new EnumerableDataSource<StockInfo>(rates);
			ds.SetXMapping(ci => dateAxis.ConvertToDouble(ci.Date));
			ds.SetYMapping(ci => ci.Rate);
			return ds;
		}

		private static List<StockInfo> LoadStockRates(string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (Stream resourceStream = assembly.GetManifestResourceStream("StockExchangeSample." + fileName))
			{
				using (StreamReader reader = new StreamReader(resourceStream))
				{
					var strings = reader.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);


					var res = new List<StockInfo>(strings.Length - 1);
					for (int i = 1; i < strings.Length; i++)
					{
						string line = strings[i];
						string[] subLines = line.Split('\t');

						DateTime date = DateTime.Parse(subLines[1]);
						double rate = Double.Parse(subLines[5], CultureInfo.InvariantCulture);

						res.Add(new StockInfo { Date = date, Rate = rate });
					}

					return res;
				}
			}
		}
	}
}
