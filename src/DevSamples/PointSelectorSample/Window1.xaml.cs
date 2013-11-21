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

namespace PointSelectorSample
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

		private void ClearPointsBtn_Click(object sender, RoutedEventArgs e)
		{
			selector.Points.Clear();
		}
	}

	public class NegativeYDataTransform : DataTransform
	{
		public override Point DataToViewport(Point pt)
		{
			return new Point(pt.X, -pt.Y);
		}

		public override Point ViewportToData(Point pt)
		{
			return new Point(pt.X, -pt.Y);
		}
	}

}
