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
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace PaletteControlSampleApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		HSBPalette palette = new HSBPalette();
		public Window1()
		{
			InitializeComponent();
			paletteControl.Palette = palette;
		}

		private void startSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			palette.Start = startSlider.Value;
		}

		private void widthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			palette.Width = widthSlider.Value;
		}
	}
}
