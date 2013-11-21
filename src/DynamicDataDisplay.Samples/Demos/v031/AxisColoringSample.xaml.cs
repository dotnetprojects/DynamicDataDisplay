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
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.GenericLocational;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v031
{
	/// <summary>
	/// Interaction logic for AxisColoringSample.xaml
	/// </summary>
	public partial class AxisColoringSample : Page
	{
		public AxisColoringSample()
		{
			InitializeComponent();

			// setting custom colors of background, axis text labels and axis ticks:
			{
				// background brush is for axis's background
				plotter.MainHorizontalAxis.Background = Brushes.Aqua.MakeTransparent(0.1);
				// foreground bruhs is for axis's labels foreground
				plotter.MainHorizontalAxis.Foreground = Brushes.DarkMagenta;

				plotter.MainVerticalAxis.Background = new LinearGradientBrush(Colors.White, Colors.LightBlue, 90);
				plotter.MainVerticalAxis.Foreground = Brushes.DarkGoldenrod;

				// stroke brush is 
				// ------ /*to rule them all*/ ------
				// for ticks' fill
				((NumericAxis)plotter.MainHorizontalAxis).AxisControl.TicksPath.Stroke = Brushes.OrangeRed;
			}

			// this will make the most left axis to display ticks as percents
			secondAxis.LabelProvider = new ToStringLabelProvider();
			secondAxis.LabelProvider.LabelStringFormat = "{0}%";
			secondAxis.LabelProvider.SetCustomFormatter(info => (info.Tick * 100).ToString());
			// percent values that can be divided by 50 without a remainder will be red and with bigger font size
			secondAxis.LabelProvider.SetCustomView((info, ui) =>
			{
				if (((int)Math.Round(info.Tick * 100)) % 50 == 0)
				{
					TextBlock text = (TextBlock)ui;
					text.Foreground = Brushes.Red;
					text.FontSize = 20;
				}
			});

			// you can add new axes not only from XAML, but from C#, too:
			HorizontalDateTimeAxis thirdAxis = new HorizontalDateTimeAxis();
			thirdAxis.LabelProvider.SetCustomFormatter(info =>
			{
				DifferenceIn dateTimeDifference = (DifferenceIn)info.Info;
				if (dateTimeDifference == DifferenceIn.Minute)
				{
					return info.Tick.ToString("%m:ss");
				}

				// null should be returned if you want to use default label text
				return null;
			});

			// let's have major labels for hours in Spanish,
			// for other time periods your Thread.CurrentThread.CurrentCulture will be used to format date.
			// You can change this default thread culture and get desired look of labels.

			CultureInfo culture = new CultureInfo("es-ES");
			thirdAxis.MajorLabelProvider.SetCustomFormatter(info =>
			{
				MajorLabelsInfo mInfo = (MajorLabelsInfo)info.Info;
				if ((DifferenceIn)mInfo.Info == DifferenceIn.Hour)
				{
					return info.Tick.ToString("MMMM/dd %m:ss", culture);
				}
				return null;
			});
			plotter.Children.Add(thirdAxis);

			SetupLocationalAxis();

			SetupPiAxis();

			SetupOneThirdAxis();
		}

		private void SetupLocationalAxis()
		{
			HorizontalAxis axis = new HorizontalAxis();

			List<CityInfo> cities = new List<CityInfo>
			{
				new CityInfo{ Name = "Paris", Coordinate = 0.05},
				new CityInfo{ Name = "Berlin", Coordinate = 0.2},
				new CityInfo{ Name = "Minsk", Coordinate = 0.3},
				new CityInfo{ Name = "Moscow", Coordinate = 0.5},
				new CityInfo{ Name = "Perm", Coordinate = 0.7},
				new CityInfo{ Name = "Ekaterinburg", Coordinate = 0.85},
				new CityInfo{ Name = "Vladivostok", Coordinate = 0.9}
			};

			GenericLocationalLabelProvider<CityInfo, double> labelProvider = new GenericLocationalLabelProvider<CityInfo, double>(cities, city => city.Name);
			labelProvider.SetCustomView((li, uiElement) =>
			{
				FrameworkElement element = (FrameworkElement)uiElement;
				element.LayoutTransform = new RotateTransform(-15, 0, 0);
			});
			GenericLocationalTicksProvider<CityInfo, double> ticksProvider = new GenericLocationalTicksProvider<CityInfo, double>(cities, city => city.Coordinate);

			axis.LabelProvider = labelProvider;
			axis.TicksProvider = ticksProvider;

			plotter.Children.Add(axis);
		}

		private void SetupPiAxis()
		{
			HorizontalAxis axis = new HorizontalAxis();

			CustomBaseNumericTicksProvider ticksProvider = new CustomBaseNumericTicksProvider(Math.PI);
			CustomBaseNumericLabelProvider labelProvider = new CustomBaseNumericLabelProvider(Math.PI, "π");
			axis.LabelProvider = labelProvider;
			axis.TicksProvider = ticksProvider;

			plotter.Children.Add(axis);
		}

		private void SetupOneThirdAxis()
		{
			HorizontalAxis axis = new HorizontalAxis();

			CustomBaseNumericTicksProvider ticksProvider = new CustomBaseNumericTicksProvider(0.5);
			CustomBaseNumericLabelProvider labelProvider = new CustomBaseNumericLabelProvider(0.5, "·½");
			axis.LabelProvider = labelProvider;
			axis.TicksProvider = ticksProvider;

			plotter.Children.Add(axis);
		}
	}

	internal class CityInfo
	{
		public string Name { get; set; }
		public double Coordinate { get; set; }
	}
}
