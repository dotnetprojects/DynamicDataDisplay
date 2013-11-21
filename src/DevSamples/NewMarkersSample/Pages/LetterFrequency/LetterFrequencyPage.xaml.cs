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
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay;
using DynamicDataDisplay.Markers.DataSources.ValueConverters;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for LetterFrequencyPage.xaml
	/// </summary>
	public partial class LetterFrequencyPage : Page
	{
		public LetterFrequencyPage()
		{
			InitializeComponent();

			chart3.IndependentValueBinding = new Binding
			{
				Path = new PropertyPath("Key"),
				Converter = new LambdaConverter(v =>
				{
					char c = (char)v;
					return (double)c;
				})
			};
			plotter3.Loaded += OnLoaded;
		}

		private void inputTb_TextChanged(object sender, TextChangedEventArgs e)
		{
			string text = inputTb.Text;

			chars.Clear();
			charFrequencies.Clear();
			for (int i = 0; i < text.Length; i++)
			{
				var symbol = Char.ToLower(text[i]);
				if (Char.IsLetter(symbol))
				{
					int count = 0;
					charFrequencies.TryGetValue(symbol, out count);
					count++;
					charFrequencies[symbol] = count;

					if (!chars.Contains(symbol))
					{
						chars.Add(symbol);
					}
				}
			}

			chars = new ObservableCollection<char>(chars.OrderBy(c => c));
			UpdateHorizontalAxis();

			DataContext = null;
			DataContext = charFrequencies;
		}

		private void UpdateHorizontalAxis()
		{
			foreach (var plotter in GetPlottersToUpdateAxis())
			{
				var axis = (IntegerAxis)plotter.MainHorizontalAxis;
				var collectionLabelProvider = (CollectionLabelProvider<char>)axis.LabelProvider;
				collectionLabelProvider.Collection = chars;
			}
		}

		Dictionary<char, int> charFrequencies = new Dictionary<char, int>();
		ObservableCollection<char> chars = new ObservableCollection<char>();

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			foreach (var plotter in GetPlottersToUpdateAxis())
			{
				var axis = new IntegerAxis { LabelProvider = new CollectionLabelProvider<char>(chars) };
				plotter.MainHorizontalAxis = axis;
			}

			inputTb.Text = "Sample text";

			var hAxis = (NumericAxis)plotter3.MainHorizontalAxis;
			hAxis.LabelProvider.CustomFormatter = d => ((char)d.Tick).ToString();
		}

		private IEnumerable<ChartPlotter> GetPlottersToUpdateAxis()
		{
			//yield break;
			yield return plotter1;
		}
	}
}
