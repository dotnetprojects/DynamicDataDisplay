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
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace AxisControlSample
{
	/// <summary>
	/// Interaction logic for IntegerAxisWindow.xaml
	/// </summary>
	public partial class IntegerAxisWindow : Window
	{
		public IntegerAxisWindow()
		{
			InitializeComponent();

			VerticalIntegerAxis verticalAxis = new VerticalIntegerAxis();
			var labels = new string[] { "One", "Two", "Three", "Four", "Five", "Six" };
			verticalAxis.LabelProvider = new CollectionLabelProvider<string>(labels);
			((IntegerTicksProvider)verticalAxis.TicksProvider).MaxStep = 1;

			plotter.MainVerticalAxis = verticalAxis;
		}
	}
}
