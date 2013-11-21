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
using DynamicDataDisplay.Markers.MarkerGenerators;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections.ObjectModel;
using DynamicDataDisplay.Markers.ForestDisplay;
using System.IO;
using System.Globalization;


namespace ForestDisplaySample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private readonly ObservableCollection<ForestItem> forest = new ObservableCollection<ForestItem>();
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LoadForest();

			Dictionary<string, TreeSpeciesInfo> speciesMappings = new Dictionary<string, TreeSpeciesInfo>();
			speciesMappings["Abie.bals"] = new TreeSpeciesInfo(Brushes.LimeGreen, TreeViews.RoundTriangle);
			speciesMappings["Acer.rubr"] = new TreeSpeciesInfo(Brushes.LimeGreen, TreeViews.Ellipse);
			speciesMappings["Acer.sacc"] = new TreeSpeciesInfo(Brushes.SeaGreen, TreeViews.Ellipse);
			speciesMappings["Betu.papy"] = new TreeSpeciesInfo(Brushes.DarkOrange, TreeViews.RoundTriangle);
			speciesMappings["Popu.gran"] = new TreeSpeciesInfo(Brushes.Teal, TreeViews.Rectangle);
			speciesMappings["Popu.trem"] = new TreeSpeciesInfo(Brushes.SeaGreen, TreeViews.Rectangle);
			speciesMappings["Quer.rubr"] = new TreeSpeciesInfo(Brushes.DarkOrange, TreeViews.Triangle);
			speciesMappings["Tili.amer"] = new TreeSpeciesInfo(Brushes.Teal, TreeViews.Triangle);

			forestDisplayControl.SpeciesMappings = speciesMappings;

			Loaded -= Window_Loaded;

			forestDisplayControl.DataContext = forest;
		}

		private void LoadForest()
		{
			var lines = File.ReadAllLines(@"..\..\Data.csv");

			string[] ids = new[] { "Abie.bals", "Acer.rubr", "Acer.sacc", "Betu.papy", "Popu.gran", "Popu.trem", "Quer.rubr", "Tili.amer" };

			int index = 0;
			foreach (var line in lines)
			{
				var strValues = line.Split(',');

				double basalRadius = Parse(strValues[0]);
				double crownRadius = Parse(strValues[1]);
				double crownHeight = Parse(strValues[2]);
				double treeHeight = Parse(strValues[3]);
				// skipping y
				double x = Parse(strValues[5]);
				// skipping id
				string speciesID = strValues[7];
				if (speciesID == "")
				{
					speciesID = ids[index % 8];
				}

				ForestItem forestItem = new ForestItem
				{
					TrunkWidth = 2 * basalRadius,
					TreeHeight = treeHeight,
					CrownHeight = crownHeight,
					CrownWidth = 2 * crownRadius,
					SpeciesID = speciesID,
					X = x
				};

				forest.Add(forestItem);
				index++;
			}
		}

		private static double Parse(string str)
		{
			return Double.Parse(str, CultureInfo.InvariantCulture);
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double prevPercent = e.OldValue;
			double currPercent = e.NewValue;
			double ratio = currPercent / prevPercent;

			foreach (var item in forest)
			{
				item.TreeHeight *= ratio;
				item.CrownHeight *= ratio;
			}
		}
	}
}
