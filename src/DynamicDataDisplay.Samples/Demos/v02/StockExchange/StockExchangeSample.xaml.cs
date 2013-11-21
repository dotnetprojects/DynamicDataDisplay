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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v02.StockExchange
{
	/// <summary>
	/// Interaction logic for Page1.xaml
	/// </summary>
	public partial class Page1 : Page
	{
		public Page1()
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
			using (Stream resourceStream = assembly.GetManifestResourceStream("Microsoft.Research.DynamicDataDisplay.Samples.Demos.v02.StockExchange." + fileName))
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

	public struct StockInfo
	{
		public DateTime Date { get; set; }
		public double Rate { get; set; }
	}
}
