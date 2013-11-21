using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace SimpleMarkers
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Prepare data in arrays
            const int N = 100;
            double[] x = new double[N];
            double[] cs = new double[N];
            double[] sn = new double[N];

            for (int i = 0; i < N; i++)
            {
                x[i] = i * 0.1;
                cs[i] = Math.Sin(x[i]);
                sn[i] = Math.Cos(x[i]);
            }

            // Add data sources:
            // 3 partial data sources, containing each of arrays
            var snDataSource = new EnumerableDataSource<double>(sn);
            snDataSource.SetYMapping(y => y);

            var xDataSource = new EnumerableDataSource<double>(x);
            xDataSource.SetXMapping(lx => lx);

            var csDataSource = new EnumerableDataSource<double>(cs);
            csDataSource.SetYMapping(y => y);

            var csqDataSource = new EnumerableDataSource<double>(cs);
            csqDataSource.SetYMapping(y => y * y);


            // 2 composite data sources and 2 charts respectively:
            // creating composite data source
            CompositeDataSource compositeDataSource1 = new CompositeDataSource(xDataSource, snDataSource);
            // adding graph to plotter
            plotter.AddLineGraph(compositeDataSource1,
                new Pen(Brushes.DarkGoldenrod, 3),
                new CirclePointMarker { Size = 10, Fill = Brushes.Red },
                new PenDescription("Sin"));

            // creating composite data source for cs values
            CompositeDataSource compositeDataSource2 = new CompositeDataSource(xDataSource, csDataSource);
            // Adding second graph to plotter
            plotter.AddLineGraph(compositeDataSource2,
                new Pen(Brushes.Blue, 3),
                new TrianglePointMarker { Size = 20, Fill = Brushes.Blue },
                new PenDescription("Cos"));

            // creating composite data source for cs^2 values
            CompositeDataSource compositeDataSource3 = new CompositeDataSource(xDataSource, csqDataSource);
            // Adding thirs graph to plotter
            Pen dashed = new Pen(Brushes.Magenta, 3);
            dashed.DashStyle = DashStyles.Dot;
            plotter.AddLineGraph(compositeDataSource3,
                dashed,
                new PenDescription("Cos^2"));

            // Force evertyhing plotted to be visible
            plotter.FitToView();
        }
    }
}
