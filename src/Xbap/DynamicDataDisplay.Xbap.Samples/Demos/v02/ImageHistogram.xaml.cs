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
using Microsoft.Win32;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v02
{
	/// <summary>
	/// Interaction logic for ImageHistogram.xaml
	/// </summary>
	public partial class ImageHistogram : Page
	{
		public ImageHistogram()
		{
			InitializeComponent();
		}

		private void OnOpenImageClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Image|*.bmp;*.jpg;*.png;*.gif";
			if (dlg.ShowDialog(Window.GetWindow(this)).GetValueOrDefault(false))
			{
				OpenImage(dlg.FileName);
			}
		}

		private void OpenImage(string fileName)
		{
			BitmapImage bmp = new BitmapImage(new Uri(fileName));
			bmp.CacheOption = BitmapCacheOption.OnLoad;

			image.Source = bmp;
			ProcessImage(bmp);
		}

		EnumerableDataSource<int> red;
		EnumerableDataSource<int> green;
		EnumerableDataSource<int> blue;

		int[] reds = new int[256];
		int[] greens = new int[256];
		int[] blues = new int[256];
		byte[] pixels;
		private void ProcessImage(BitmapImage bmp)
		{
			byte[] pixels = new byte[bmp.PixelWidth * bmp.PixelHeight * 4];
			bmp.CopyPixels(pixels, bmp.PixelWidth * 4, 0);

			for (int i = 0; i < pixels.Length; )
			{
				//BGRA
				blues[pixels[i++]]++;
				greens[pixels[i++]]++;
				reds[pixels[i++]]++;
				i++;
			}

			CreateHistograms();
		}

		private void CreateHistograms()
		{
			EnumerableDataSource<int> x = new EnumerableDataSource<int>(Enumerable.Range(0, 256).ToArray());
			x.SetXMapping(_x => _x);

			Func<int, double> mapping;
			if (check.IsChecked.GetValueOrDefault())
				mapping = logMapping;
			else
				mapping = linearMapping;

			red = new EnumerableDataSource<int>(reds);
			red.SetYMapping(mapping);
			green = new EnumerableDataSource<int>(greens);
			green.SetYMapping(mapping);
			blue = new EnumerableDataSource<int>(blues);
			blue.SetYMapping(mapping);

			CompositeDataSource rDS = new CompositeDataSource(x, red);
			CompositeDataSource gDS = new CompositeDataSource(x, green);
			CompositeDataSource bDS = new CompositeDataSource(x, blue);

			plotter.RemoveUserElements();
			plotter.AddLineGraph(rDS, Colors.Red, 1, "Red").FilteringEnabled = false;
			plotter.AddLineGraph(gDS, Colors.Green, 1, "Green").FilteringEnabled = false;
			plotter.AddLineGraph(bDS, Colors.Blue, 1, "Blue").FilteringEnabled = false;
		}

		private Func<int, double> logMapping = i => i > 0 ? Math.Log10(i) : 0;
		private Func<int, double> linearMapping = i => i;

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			Func<int, double> mapping;
			if (check.IsChecked.GetValueOrDefault())
				mapping = logMapping;
			else
				mapping = linearMapping;

			red.SetYMapping(mapping);
			green.SetYMapping(mapping);
			blue.SetYMapping(mapping);
		}
	}
}
