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
using System.Diagnostics;
using System.Collections.Specialized;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Collections.ObjectModel;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace PerfCounterChart
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			LineGraph chart = CreatePerformanceGraph("Memory", "Available MBytes");
			chart.DataChanged += chart_DataChanged;

			// uncomment two next lines to create a processor usage chart
			//CreatePerformanceGraph("Processor", "% Processor Time", "0");
			//CreatePerformanceGraph("Processor", "% Processor Time", "1");

			// these perf counters are device-specific, so
			// tune them to work on your machine and then uncomment
#if false
			string networkAdapterName = "Intel[R] PRO_Wireless 3945ABG Network Connection - Packet Scheduler Miniport";
			//string networkAdapterName = "Broadcom NetXtreme Gigabit Ethernet - Packet Scheduler Miniport";
			CreatePerformanceGraph("Network Interface", "Bytes Received/sec", networkAdapterName);
			CreateFilteredPerformanceGraph("Network Interface", "Bytes Received/sec",
				networkAdapterName,
				new FilterChain(new MaxSizeFilter(), new AverageFilter { Number = 10 }));
			CreateFilteredPerformanceGraph("Network Interface", "Bytes Received/sec",
				networkAdapterName,
				new FilterChain(new MaxSizeFilter(), new AverageFilter { Number = 20 }));
#endif
		}

		private LineGraph CreatePerformanceGraph(string categoryName, string counterName, string instanceName)
		{
			PerformanceData data = new PerformanceData(new PerformanceCounter(categoryName, counterName, instanceName));

			var filteredData = new FilteringDataSource<PerformanceInfo>(data, new MaxSizeFilter());

			var ds = new EnumerableDataSource<PerformanceInfo>(filteredData);
			ds.SetXMapping(pi => pi.Time.TimeOfDay.TotalSeconds);
			ds.SetYMapping(pi => pi.Value);

			LineGraph chart = plotter.AddLineGraph(ds, 2.0, String.Format("{0} - {1}", categoryName, counterName));
			return chart;
		}

		private LineGraph CreateFilteredPerformanceGraph(string categoryName, string counterName, string instanceName, IFilter<PerformanceInfo> filter)
		{
			PerformanceData data = new PerformanceData(new PerformanceCounter(categoryName, counterName, instanceName));

			var filteredData = new FilteringDataSource<PerformanceInfo>(data, filter);

			var ds = new EnumerableDataSource<PerformanceInfo>(filteredData);
			ds.SetXMapping(pi => pi.Time.TimeOfDay.TotalSeconds);
			ds.SetYMapping(pi => pi.Value);

			LineGraph chart = plotter.AddLineGraph(ds, 2.0, String.Format("{0} - {1}", categoryName, counterName));
			return chart;
		}

		private LineGraph CreatePerformanceGraph(string categoryName, string counterName)
		{
			PerformanceData data = new PerformanceData(categoryName, counterName);

			var filteredData = new FilteringDataSource<PerformanceInfo>(data, new MaxSizeFilter());

			var ds = new EnumerableDataSource<PerformanceInfo>(filteredData);
			ds.SetXMapping(pi => pi.Time.TimeOfDay.TotalSeconds);
			ds.SetYMapping(pi => pi.Value);

			LineGraph chart = plotter.AddLineGraph(ds, 2.0, String.Format("{0} - {1}", categoryName, counterName));
			return chart;
		}

		private void chart_DataChanged(object sender, EventArgs e)
		{
			LineGraph graph = (LineGraph)sender;

			double mbytes = graph.DataSource.GetPoints().LastOrDefault().Y;

			graph.Description = new PenDescription(String.Format("Memory - available {0} MBytes", mbytes));
		}
	}
}
