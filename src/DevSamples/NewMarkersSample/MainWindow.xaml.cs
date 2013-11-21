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
using Microsoft.Research.DynamicDataDisplay;
using NewMarkersSample.Pages;
using DynamicDataDisplay.Markers;
using System.Diagnostics;

namespace NewMarkersSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public void AddPage(Page page)
		{
			Frame frame = new Frame { Content = page };
			TabItem tab = new TabItem { Content = frame, Header = page.Title };
			tabControl.Items.Add(tab);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//AddPage(new BigPointArrayPage());
			//AddPage(new BarChartPage());
			AddPage(new BarChartDataTriggerPage());
			//AddPage(new LiveTooltipPage());
			//AddPage(new ColumnChartSample());
			//AddPage(new AcceptableValuePage());
			//AddPage(new PieChartPage());
			//AddPage(new StockMarkersPage());
			//AddPage(new CompositeDSPage());
			//AddPage(new DifferentBuildInMarkersPage());
			//AddPage(new VectorFieldPage());
			//AddPage(new EndlessRandomValuesPage());
			//AddPage(new ConditionalBindingPage());
			//AddPage(new LetterFrequencyPage());
			//AddPage(new PieChartAPIPage());
			//AddPage(new DateTimeRectanglesPage());
			//AddPage(new PointSetPage());
			//AddPage(new RotatedEllipsesPage());
			//AddPage(new RenderingMarkersPage());
			//AddPage(new XmlChartPage());

			tabControl.SelectedIndex = 0;
		}
	}
}
