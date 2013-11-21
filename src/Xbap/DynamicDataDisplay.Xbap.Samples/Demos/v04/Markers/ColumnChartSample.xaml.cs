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
	/// Interaction logic for ColumnChartSample.xaml
	/// </summary>
	public partial class ColumnChartSample : Page
	{
		private readonly ObservableCollection<double> values = new ObservableCollection<double> { 0.1, 0.2, 0.3, 0.15 };
		private readonly Random rnd = new Random();

		public ColumnChartSample()
		{
			InitPalette();

			InitializeComponent();
		}

		private void InitPalette()
		{
			var brushes = from prop in typeof(Brushes).GetProperties()
						  let brush = (SolidColorBrush)prop.GetValue(null, null)
						  let hsbColor = HsbColor.FromArgbColor(brush.Color)
						  orderby hsbColor.Hue
						  select new SolidColorBrush(hsbColor.ToArgbColor());

			AlternationConverter converter = new AlternationConverter();
			foreach (var brush in brushes)
			{
				converter.Values.Add(brush);
			}

			Resources.Add("fillsConverter", converter);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = values;
		}

		private void addBtn_Click(object sender, RoutedEventArgs e)
		{
			values.Add(rnd.NextDouble() + 0.2);
		}

		private void insertBtn_Click(object sender, RoutedEventArgs e)
		{
			values.Insert(values.Count - 3, rnd.NextDouble() + 0.2);
		}

		private void deleteBtn_Click(object sender, RoutedEventArgs e)
		{
			values.RemoveAt(0);
		}
	}
}
