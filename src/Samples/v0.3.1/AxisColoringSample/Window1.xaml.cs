using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace AxisColoringSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
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
		}
	}
}
