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
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace TwoAxisSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			VerticalAxis axis = new VerticalAxis();
			axis.SetConversion(0, 100, 100, 0);

			plotter.Children.Add(axis);
			// this is only an example of visible rectange. Use here rect you actually need.
			plotter.Viewport.Visible = new Rect(0, 0, 1, 100);
		}
	}
}
