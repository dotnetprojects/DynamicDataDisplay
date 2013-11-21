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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.v03
{
	/// <summary>
	/// Interaction logic for SimulationSample.xaml
	/// </summary>
	public partial class SimulationSample : Page
	{
		
		// Three observable data sources. Observable data source contains
		// inside ObservableCollection. Modification of collection instantly modify
		// visual representation of graph. 
		ObservableDataSource<Point> source1 = null;
		ObservableDataSource<Point> source2 = null;
		ObservableDataSource<Point> source3 = null;
		public SimulationSample()
		{
			InitializeComponent();
		}

		private bool unload = true;
		private void Simulation()
		{
			CultureInfo culture = CultureInfo.InvariantCulture;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			// load spim-generated data from embedded resource file
			const string spimDataName = "Microsoft.Research.DynamicDataDisplay.Xbap.Samples.Demos.v03.Repressilator.txt";
			using (Stream spimStream = executingAssembly.GetManifestResourceStream(spimDataName))
			{
				using (StreamReader r = new StreamReader(spimStream))
				{
					string line = r.ReadLine();
					while (!r.EndOfStream)
					{
						if (unload)
							return;

						line = r.ReadLine();
						string[] values = line.Split(',');

						double x = Double.Parse(values[0], culture);
						double y1 = Double.Parse(values[1], culture);
						double y2 = Double.Parse(values[2], culture);
						double y3 = Double.Parse(values[3], culture);

						Point p1 = new Point(x, y1);
						Point p2 = new Point(x, y2);
						Point p3 = new Point(x, y3);

						source1.AppendAsync(Dispatcher, p1);
						source2.AppendAsync(Dispatcher, p2);
						source3.AppendAsync(Dispatcher, p3);

						Thread.Sleep(10); // Long-long time for computations...
					}
				}
			}
		}

		private Thread simThread;
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Create first source
			source1 = new ObservableDataSource<Point>();
			// Set identity mapping of point in collection to point on plot
			source1.SetXYMapping(p => p);

			// Create second source
			source2 = new ObservableDataSource<Point>();
			// Set identity mapping of point in collection to point on plot
			source2.SetXYMapping(p => p);

			// Create third source
			source3 = new ObservableDataSource<Point>();
			// Set identity mapping of point in collection to point on plot
			source3.SetXYMapping(p => p);

			// Add all three graphs. Colors are not specified and chosen random
			plotter.AddLineGraph(source1, 2, "Data row 1");
			plotter.AddLineGraph(source2, 2, "Data row 2");
			plotter.AddLineGraph(source3, 2, "Data row 3");

			unload = false;
			// Start computation process in second thread
			simThread = new Thread(new ThreadStart(Simulation));
			simThread.IsBackground = true;
			simThread.Start();
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			unload = true;
		}
	}
}
