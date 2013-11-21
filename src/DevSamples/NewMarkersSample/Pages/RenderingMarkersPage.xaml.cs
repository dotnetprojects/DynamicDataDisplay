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

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for RenderingMarkersPage.xaml
	/// </summary>
	public partial class RenderingMarkersPage : Page
	{
		public RenderingMarkersPage()
		{
			InitializeComponent();
		}

		Random rnd = new Random();
		ObservableCollection<Point> pts = new ObservableCollection<Point>();
		private void ChartPlotter_Loaded(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 5000; i++)
			{
				pts.Add(new Point(rnd.NextDouble(), rnd.NextDouble()));
			}

			DataContext = pts;
		}
	}
}
