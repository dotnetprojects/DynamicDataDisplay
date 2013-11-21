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

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for PieChartAPIPage.xaml
	/// </summary>
	public partial class PieChartAPIPage : Page
	{
		public PieChartAPIPage()
		{
			InitializeComponent();
		}

		private void addBtn_Click(object sender, RoutedEventArgs e)
		{
			var result = editor.GetValue();
			pieChart.AddPieItem(result.Caption, result.Value, result.Fill);
		}

		private void resetBtn_Click(object sender, RoutedEventArgs e)
		{
			editor.Reset();
		}
	}
}
