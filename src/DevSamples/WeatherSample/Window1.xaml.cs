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

namespace WeatherSample
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

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			List<WeatherData> data = CreateData(100);

			chart.DataSource = data;

			EnumerableDataSource<WeatherData> dataSource = new EnumerableDataSource<WeatherData>(data);
			dataSource.SetXMapping(weather => weather.Day);
			dataSource.SetYMapping(weather => weather.Temperature);

			var line = plotter.AddLineGraph(dataSource);
		}

		private List<WeatherData> CreateData(int count)
		{
			List<WeatherData> res = new List<WeatherData>(count);

			Random rnd = new Random();
			for (int i = 0; i < count; i++)
			{
				WeatherData data = new WeatherData { Day = i * 4, Temperature = rnd.NextDouble() * 25 + 5, WeatherType = GetWeatherType(rnd.NextDouble()) };
				res.Add(data);
			}

			return res;
		}

		private WeatherType GetWeatherType(double randomValue)
		{
			if (randomValue < 0.25) return WeatherType.Sun;
			if (randomValue < 0.5) return WeatherType.Cloud;
			if (randomValue < 0.75) return WeatherType.Rain;
			return WeatherType.Thunderstorm;
		}
	}
}
