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
using System.IO;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Globalization;
using System.Reflection;

namespace CurrencyExchangeSample
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

		List<CurrencyInfo> usd;
		List<CurrencyInfo> eur;
		List<CurrencyInfo> gbp;
		List<CurrencyInfo> jpy;
		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			usd = LoadCurrencyRates("usd.txt");
			eur = LoadCurrencyRates("eur.txt");
			gbp = LoadCurrencyRates("gbp.txt");
			jpy = LoadCurrencyRates("jpy.txt");

			Color[] colors = ColorHelper.CreateRandomColors(4);
			plotter.AddLineGraph(CreateCurrencyDataSource(usd), colors[0], 1, "RUB / $");
			plotter.AddLineGraph(CreateCurrencyDataSource(eur), colors[1], 1, "RUB / €");
			plotter.AddLineGraph(CreateCurrencyDataSource(gbp), colors[2], 1, "RUB / £");
			plotter.AddLineGraph(CreateCurrencyDataSource(jpy), colors[3], 1, "RUB / ¥");
		}

		private EnumerableDataSource<CurrencyInfo> CreateCurrencyDataSource(List<CurrencyInfo> rates)
		{
			EnumerableDataSource<CurrencyInfo> ds = new EnumerableDataSource<CurrencyInfo>(rates);
			ds.SetXMapping(ci => dateAxis.ConvertToDouble(ci.Date));
			ds.SetYMapping(ci => ci.Rate);
			return ds;
		}

		private static List<CurrencyInfo> LoadCurrencyRates(string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (Stream resourceStream = assembly.GetManifestResourceStream("CurrencyExchangeSample." + fileName))
			{
				using (StreamReader reader = new StreamReader(resourceStream))
				{
					var strings = reader.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

					var res = new List<CurrencyInfo>(strings.Length - 1);
					for (int i = 1; i < strings.Length; i++)
					{
						string line = strings[i];
						string[] subLines = line.Split('\t');

						DateTime date = DateTime.Parse(subLines[1]);
						double rate = Double.Parse(subLines[5], CultureInfo.InvariantCulture);

						res.Add(new CurrencyInfo { Date = date, Rate = rate });
					}

					return res;
				}
			}

		}
	}
}
