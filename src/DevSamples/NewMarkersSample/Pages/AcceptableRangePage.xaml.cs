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

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for AcceptableValuePage.xaml
	/// </summary>
	public partial class AcceptableValuePage : Page
	{
		public AcceptableValuePage()
		{
			InitializeComponent();
		}

		Segment[] segments = Segment.LoadSegments(40);
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			var newLegend = plotter.Children.OfType<NewLegend>().FirstOrDefault();
			plotter.Children.Remove(newLegend);
			DataContext = segments;
		}
	}
}
