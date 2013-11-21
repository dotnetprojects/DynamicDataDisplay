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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using LegendSample.Pages;

namespace LegendSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			//AddPage(new LineChartInLegendPage());
			//AddPage(new DisconnectedLegendMode());
			AddPage(new AnotherLookOfLegendItemPage());

			tabControl.SelectedIndex = 0;
		}

		private void AddPage(Page page)
		{
			TabItem item = new TabItem { Content = new Frame { Content = page }, Header = page.Title };
			tabControl.Items.Add(item);
		}
	}
}
