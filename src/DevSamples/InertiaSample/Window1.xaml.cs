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
using Microsoft.Research.DynamicDataDisplay.Navigation;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using System.Windows.Markup;

namespace InertiaSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			//plotter.Children.Clear();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.Children.Remove(plotter.Children.OfType<MouseNavigation>().FirstOrDefault());
			//plotter.Children.Remove(plotter.Children.OfType<DefaultContextMenu>().First());
			//plotter.Children.Remove(plotter.Children.OfType<Microsoft.Research.DynamicDataDisplay.Navigation.KeyboardNavigation>().First());

			//var inertialNav = new InertialMouseNavigation();
			var inertialNav = new PhysicalNavigation();
			plotter.Children.Add(inertialNav);

			List<DataPoint> data = new List<DataPoint> { 
				new DataPoint {
					X = 0.1, Y = 0.1 
				}, new DataPoint { X = 0.2, Y = 0.2 } };
			//listView.ItemsSource = data;

			//VisualDebug.Instance.DrawVector("test", new Point(0.1, 0.9), new Vector(0.5, -0.5), Colors.Indigo);

			plotter.Children.Add(new AxisCursorGraph());
		}
	}

	public class DataPoint
	{
		public double X { get; set; }
		public double Y { get; set; }
	}
}
