#define MULTIPLE

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace MultipleDataPlotting
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		List<AnimatedDataSource> data = new List<AnimatedDataSource>();
		const int colNum = 5;
		const int rowNum = 5;

		DispatcherTimer timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(10)
		};

		public Window1()
		{
			InitializeComponent();

#if MULTIPLE
			for (int ix = 0; ix < colNum; ix++)
			{
				content.ColumnDefinitions.Add(new ColumnDefinition());
			}
			for (int iy = 0; iy < rowNum; iy++)
			{
				content.RowDefinitions.Add(new RowDefinition());
			}

			for (int ix = 0; ix < colNum; ix++)
			{
				for (int iy = 0; iy < rowNum; iy++)
				{
					ChartPlotter plotter = new ChartPlotter();
					plotter.MainHorizontalAxis = null;
					plotter.MainVerticalAxis = null;
					plotter.BorderThickness = new Thickness(1);
					Grid.SetColumn(plotter, ix);
					Grid.SetRow(plotter, iy);
					content.Children.Add(plotter);
					plotter.LegendVisibility = Visibility.Hidden;
					plotter.Legend.AutoShowAndHide = false;

					AnimatedDataSource ds = new AnimatedDataSource();
					data.Add(ds);

					LineGraph line = new LineGraph(ds.DataSource);
					line.Stroke = BrushHelper.CreateBrushWithRandomHue();
					line.StrokeThickness = 2;
					line.Filters.Add(new FrequencyFilter());
					plotter.Children.Add(line);
				}
			}
#else
			ChartPlotter plotter = new ChartPlotter();
			plotter.HorizontalAxis = null;
			plotter.VerticalAxis = null;

			content.Children.Add(plotter);
			for (int i = 0; i < rowNum * colNum; i++)
			{
				AnimatedDataSource ds = new AnimatedDataSource();
				data.Add(ds);

				LineGraph line = new LineGraph(ds.DataSource);
				line.LineBrush = BrushHelper.CreateBrushWithRandomHue();
				line.LineThickness = 1;
				line.Filters.Add(new FrequencyFilter());
				plotter.Children.Add(line);
			}
#endif

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			foreach (var ds in data)
			{
				ds.Update();
			}
		}
	}
}
