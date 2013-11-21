using System;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace TablePlot
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
            // Prepare table. We use standart data table
            DataTable table = new DataTable();
            table.Columns.Add("Sine", typeof(double));
            table.Columns.Add("Time", typeof(DateTime));
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("Sqrt", typeof(double));

            for (int i = 0; i < 100; i++)
            {
                table.Rows.Add(
                    Math.Sin(i / 10.0),
                    DateTime.Now + new TimeSpan(0, 0, i),
                    i,
                    Math.Sqrt(i));
            }

            // Prepare data source with mapping 
            TableDataSource data = new TableDataSource(table);
            // X is time in seconds
            data.SetXMapping(row => ((DateTime)row["Time"] - (DateTime)table.Rows[0][1]).TotalSeconds);
            // Y is value of "Sine" column
            data.SetYMapping(row => 10 * (double)row["Sine"]);
            // Map HSB color computes from "Index" column to dependency property Brush of marker
            data.AddMapping(ShapePointMarker.FillProperty,
                row => new SolidColorBrush(new HsbColor(3 * (int)row["Index"], 1, 1).ToArgbColor()));
            // Map "Sqrt" based values to marker size
            data.AddMapping(ShapePointMarker.SizeProperty,
                row => 4 + (double)row["Sqrt"]);

            // Plot first graph
            plotter.AddLineGraph(data,
                new Pen(Brushes.Blue, 2),
                new CirclePointMarker(),
                new PenDescription("First series (Sine)"));

            // Prepare data source for second graph. table.Rows is enumerable, 
            // so we can plot its contents as IEnumerable with items of TableRow type
            EnumerableDataSource<DataRow> data2 = new EnumerableDataSource<DataRow>(table.Rows);
            // X is time in seconds again
            data2.SetXMapping(row => ((DateTime)row["Time"] - (DateTime)table.Rows[0][1]).TotalSeconds);
            // Y is value of "Sqrt" column
            data2.SetYMapping(row => (double)row["Sqrt"]);

            // Plot second graph without markers
            plotter.AddLineGraph(data2,
                Colors.Red, 3.0,
                "Second series (Sqrt)");

            // Force evertyhing plotted to be visible
            plotter.Viewport.FitToView();
        }
    }
}
