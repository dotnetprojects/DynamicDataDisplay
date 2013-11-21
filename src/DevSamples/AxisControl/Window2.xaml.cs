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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.GenericLocational;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;

namespace AxisControlSample
{
	/// <summary>
	/// Interaction logic for Window2.xaml
	/// </summary>
	public partial class Window2 : Window
	{
		public Window2()
		{
			InitializeComponent();

			plotter.MainVerticalAxis.Placement = AxisPlacement.Right;
			//dateAxis.SetConversion(0, new DateTime(2008, 1, 1), 1, new DateTime(2008, 12, 1));
			//dateAxis.LabelProvider += LabelProvider_LabelCreated;

			horizTimeAxis.SetConversion(0, new TimeSpan(), 1, TimeSpan.FromHours(1));
			vertTimeAxis.SetConversion(0, new TimeSpan(), 1, TimeSpan.FromDays(1));
			//horizDateAxis.SetConversion(0, new DateTime(2008, 1, 1), 1, new DateTime(2008, 12, 1));
			//vertDateAxis.SetConversion(0, new DateTime(2008, 1, 1), 1, new DateTime(2008, 12, 1));

			//TypifiedChartPlotter<double, double> pl1 = new TypifiedChartPlotter<double, double>();
			//pl1.HorizontalAxis = new HorizontalTimeSpanAxis();



			HorizontalAxis axis = new HorizontalAxis();

			List<CityInfo> cities = new List<CityInfo>
			{
				new CityInfo{ Name = "Paris", Coordinate = -3},
				new CityInfo{ Name = "Berlin", Coordinate = -1.9},
				new CityInfo{ Name = "Minsk", Coordinate = -0.8},
				new CityInfo{ Name = "Moscow", Coordinate = 0},
				new CityInfo{ Name = "Perm", Coordinate = 2},
				new CityInfo{ Name = "Ekaterinburg", Coordinate = 4},
				new CityInfo{ Name = "Vladivostok", Coordinate = 9}
			};

			GenericLocationalLabelProvider<CityInfo, double> labelProvider = new GenericLocationalLabelProvider<CityInfo, double>(cities, city => city.Name);
			labelProvider.SetCustomView((li, uiElement) =>
			{
				FrameworkElement element = (FrameworkElement)uiElement;
				element.LayoutTransform = new RotateTransform(-45, 1, 0);
			});
			GenericLocationalTicksProvider<CityInfo, double> ticksProvider = new GenericLocationalTicksProvider<CityInfo, double>(cities, city => city.Coordinate);

			axis.LabelProvider = labelProvider;
			axis.TicksProvider = ticksProvider;

			plotter.Children.Add(axis);

			HorizontalAxis axis2 = new HorizontalAxis();
			CustomBaseNumericTicksProvider provider2 = new CustomBaseNumericTicksProvider(Math.PI);
			CustomBaseNumericLabelProvider labelProvider2 = new CustomBaseNumericLabelProvider(Math.PI, "π");
			axis2.LabelProvider = labelProvider2;
			axis2.TicksProvider = provider2;

			plotter.Children.Add(axis2);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//vertDateAxis.AxisControl.UpdateUIRepresentation();
			plotter.MainHorizontalAxis = new HorizontalDateTimeAxis();
		}

		private class CityInfo
		{
			public string Name { get; set; }
			public double Coordinate { get; set; }
		}
	}
}
