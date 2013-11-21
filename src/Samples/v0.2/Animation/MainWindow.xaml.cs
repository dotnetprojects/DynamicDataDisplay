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
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using System.Globalization;

namespace Animation
{
    /// <summary>Sample that show rendering of data changing in time</summary>
    public partial class AnimatedSampleWindow : Window
    {
        double phase = 0;
        readonly double[] animatedX = new double[1000];
        readonly double[] animatedY = new double[1000];
        EnumerableDataSource<double> animatedDataSource = null;

        /// <summary>Programmatically created header</summary>
        Header chartHeader = new Header();

        /// <summary>Text contents of header</summary>
        TextBlock headerContents = new TextBlock();

        /// <summary>Timer to animate data</summary>
        readonly DispatcherTimer timer = new DispatcherTimer();  

        public AnimatedSampleWindow()
        {
            InitializeComponent();
            headerContents.FontSize = 24;
            headerContents.Text = "Phase = 0.00";
            headerContents.HorizontalAlignment = HorizontalAlignment.Center;
            chartHeader.Content = headerContents;
            plotter.Children.Add(chartHeader);
        }

        private void AnimatedPlot_Timer(object sender, EventArgs e)
        {
            phase += 0.01;
            if (phase > 2 * Math.PI)
                phase -= 2 * Math.PI;
            for (int i = 0; i < animatedX.Length; i++)
                animatedY[i] = Math.Sin(animatedX[i] + phase);

            // Here it is - signal that data is updated
            animatedDataSource.RaiseDataChanged();
            headerContents.Text = String.Format(CultureInfo.InvariantCulture, "Phase = {0:N2}", phase);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < animatedX.Length; i++)
            {
                animatedX[i] = 2 * Math.PI * i / animatedX.Length;
                animatedY[i] = Math.Sin(animatedX[i]);
            }
            EnumerableDataSource<double> xSrc = new EnumerableDataSource<double>(animatedX);
            xSrc.SetXMapping(x => x);
            animatedDataSource = new EnumerableDataSource<double>(animatedY);
            animatedDataSource.SetYMapping(y => y);

            // Adding graph to plotter
            var line = plotter.AddLineGraph(new CompositeDataSource(xSrc, animatedDataSource),
                new Pen(Brushes.Magenta, 3),                
                new PenDescription("Sin(x + phase)"));

			timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += AnimatedPlot_Timer;
            timer.Start();
        }
    }
}
