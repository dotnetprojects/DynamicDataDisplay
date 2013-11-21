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

namespace MercatorMapSample
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

		private void SetVisibleBtn_Click(object sender, RoutedEventArgs e)
		{
			plotter.Visible = new DataRect(-0.1, -0.1, 0.01, 0.01);
		}

		private void JumpLeft_Click(object sender, RoutedEventArgs e)
		{
			plotter.Visible = DataRect.Offset(plotter.Visible, -plotter.Visible.Width, 0);
		}
	}
}
