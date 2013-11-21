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
using System.Windows.Shapes;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	internal partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink source = (Hyperlink)sender;
			Process.Start(source.NavigateUri.ToString());
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			// close on Esc or Enter pressed
			if (e.Key == Key.Escape || e.Key == Key.Enter)
			{
				Close();
			}
		}

		private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
		{
			Hyperlink source = (Hyperlink)sender;
			Process.Start(source.NavigateUri.ToString());
		}
	}
}
