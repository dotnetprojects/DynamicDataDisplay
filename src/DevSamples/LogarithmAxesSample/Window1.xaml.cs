﻿using System;
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

namespace LogarithmAxesSample
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

		private void plotter_Loaded(object sender, RoutedEventArgs e)
		{
			var genericPlotter = plotter.GetGenericPlotter();
			genericPlotter.DataRect = new GenericRect<double, double>(9, 0, 110, 2);
		}
	}
}
