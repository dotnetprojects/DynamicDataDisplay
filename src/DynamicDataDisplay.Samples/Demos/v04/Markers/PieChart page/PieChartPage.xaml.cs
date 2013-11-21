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
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for PieChartPage.xaml
	/// </summary>
	public partial class PieChartPage : Page
	{
		public PieChartPage()
		{
			InitializeComponent();

			plotter.Children.Add(plotter.NewLegend);
		}

		private readonly ObservableCollection<SellInfo> sells = new ObservableCollection<SellInfo>();

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AddRandomSellInfo();
			AddRandomSellInfo();
			AddRandomSellInfo();

			DataContext = sells;
		}

		private void addRandomItemBtn_Click(object sender, RoutedEventArgs e)
		{
			AddRandomSellInfo();
		}

		private void AddRandomSellInfo()
		{
			sells.Add(new SellInfo { CityName = CreateRandomString(), Income = rnd.NextDouble(1, 3) });
		}

		private readonly Random rnd = new Random();
		private string CreateRandomString()
		{
			var syllablesNum = rnd.Next(2, 4);

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < syllablesNum; i++)
			{
				AppendRandomSyllable(builder);
			}

			builder[0] = Char.ToUpper(builder[0]);

			return builder.ToString();
		}

		private void AppendRandomSyllable(StringBuilder builder)
		{
			int consonantsNum = rnd.NextDouble() > 0.5 ? 1 : 2;

			for (int i = 0; i < consonantsNum; i++)
			{
				builder.Append(GetRandomConsonant());
			}
			builder.Append(GetRandomVowel());
		}

		private readonly char[] consonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'w', 'x', 'y', 'z' };
		private readonly char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };

		private char GetRandomConsonant()
		{
			return GetRandomChar(consonants);
		}

		private char GetRandomVowel()
		{
			return GetRandomChar(vowels);
		}

		private char GetRandomChar(char[] chars)
		{
			int index = rnd.Next(chars.Length);

			return chars[index];
		}

		private void defaultStyle_Checked(object sender, RoutedEventArgs e)
		{
			plotter.LegendStyle = LegendStyles.Default;
		}

		private void noScrollStyle_Checked(object sender, RoutedEventArgs e)
		{
			plotter.LegendStyle = LegendStyles.NoScroll;
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			sells.Clear();
		}
	}
}
