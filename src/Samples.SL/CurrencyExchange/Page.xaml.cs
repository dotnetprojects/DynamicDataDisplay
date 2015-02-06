using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Reflection;
using System.IO;

namespace CurrencyExchange
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        List<CurrencyInfo> usd;
        List<CurrencyInfo> eur;
        List<CurrencyInfo> gbp;
        List<CurrencyInfo> jpy;

        HorizontalDateTimeAxis dateAxis = new HorizontalDateTimeAxis();

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            plotter.HorizontalAxis = dateAxis;

            usd = LoadCurrencyRates("usd.txt");
            eur = LoadCurrencyRates("eur.txt");
            gbp = LoadCurrencyRates("gbp.txt");
            jpy = LoadCurrencyRates("jpy.txt");

            plotter.Children.Add(new LineGraph(CreateCurrencyDataSource(usd), "RUB / USD"));
            plotter.Children.Add(new LineGraph(CreateCurrencyDataSource(eur), "RUB / EURO"));
            plotter.Children.Add(new LineGraph(CreateCurrencyDataSource(gbp), "RUB / GBP"));
            plotter.Children.Add(new LineGraph(CreateCurrencyDataSource(jpy),  "RUB / JPY"));
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
            using (Stream resourceStream = assembly.GetManifestResourceStream("CurrencyExchange." + fileName))
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
                        double rate = Double.Parse(subLines[5], System.Globalization.CultureInfo.InvariantCulture);

                        res.Add(new CurrencyInfo { Date = date, Rate = rate });
                    }

                    return res;
                }
            }

        }
    }
}
