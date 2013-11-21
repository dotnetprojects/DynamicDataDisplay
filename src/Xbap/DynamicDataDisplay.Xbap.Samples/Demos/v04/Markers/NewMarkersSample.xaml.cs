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
using NewMarkersSample.Pages;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v04.Markers
{
	/// <summary>
	/// Interaction logic for NewMarkersSample.xaml
	/// </summary>
	public partial class NewMarkersSample : Page
	{
		public NewMarkersSample()
		{
			InitializeComponent();
		}

		public void AddPage(Page page)
		{
			TabItem tab = new TabItem { Header = page.Title };
			tab.Content = new Frame { Content = page };
			tabControl.Items.Add(tab);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AddPage(new PieChartPage());
			AddPage(new BarChartPage());
			AddPage(new AcceptableRangePage());
			AddPage(new BigPointArrayPage());
			AddPage(new ColumnChartSample());
			AddPage(new DifferentBuildInMarkersPage());
			AddPage(new StockMarkersPage());
		}
	}
}
