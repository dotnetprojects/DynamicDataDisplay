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
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using System.Windows.Controls.Primitives;

namespace BarChartSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{

		double x = 0.6;
		DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
		ObservableCollection<SampleData> data;
		public Window1()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(Window1_Loaded);

		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			Button button = new Button { Content = "Add value" };
			button.Click += new RoutedEventHandler(button_Click);
			Canvas.SetRight(button, 10);
			Canvas.SetBottom(button, 10);
			plotter.MainCanvas.Children.Add(button);

			barChart.MarkerBindCallback = ea =>
			{
				SampleData d = (SampleData)ea.Data;

				//DefaultContextMenu.SetPlotterContextMenu(ea.Marker, new ObjectCollection
				//{
				//    CreateMenuItem(d)
				//});
				if (d.Value > 0.95)
				{
					ea.Marker.SetValue(Rectangle.FillProperty, Brushes.Yellow);
				}
			};
			//barChart.IsRecycling = false;

			data = new ObservableCollection<SampleData>
			{
				new SampleData{X = 0.1, Value = 1},
				new SampleData{X = 0.2, Value = 0.5},
				new SampleData{X = 0.3, Value = 0.4},
				new SampleData{X = 0.4, Value = -0.4},
				new SampleData{X = 0.5, Value = -0.2}
			};

			barChart.DataSource = data;

			plotter.Viewport.FitToViewRestrictions.Add(new FollowWidthRestriction());

			timer.Tick += new EventHandler(timer_Tick);
			//timer.Start();
		}

		void button_Click(object sender, RoutedEventArgs e)
		{
			timer_Tick(sender, null);
		}

		private MenuItem CreateMenuItem(SampleData data)
		{
			MenuItem item = new MenuItem { Header = "Change Item" };
			item.Tag = data;

			item.Click += new RoutedEventHandler(item_Click);

			return item;
		}

		void item_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			SampleData dataItem = (SampleData)item.Tag;

			int index = data.IndexOf(dataItem);

			SampleData newData = new SampleData { X = dataItem.X, Value = -dataItem.Value };

			data[index] = newData;
		}

		Random rnd = new Random();
		void timer_Tick(object sender, EventArgs e)
		{
			data.Add(new SampleData { X = x, Value = rnd.NextDouble() * 2 - 1 });
			x += 0.1;

			var visible = plotter.Viewport.Visible;
			visible.XMin = x - 10;
			//plotter.Viewport.Visible = visible;
		}

		public class SampleData
		{
			public double X { get; set; }
			public double Value { get; set; }
		}
	}
}
