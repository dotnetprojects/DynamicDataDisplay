using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Filters;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DataSourcesTutorial
{
    public partial class Page : UserControl
    {
        private LineGraph sin, cos , log;
        
        public Page()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            const int N = 200;
            
            double step = Math.PI * 2 / N;

            #region CompositeDataSource
            double[] x = new double[N];
            double[] y = new double[N];
            
            for (int i = 0; i < N; i++)
            {
                x[i] = i *step;
                y[i] = Math.Sin(x[i]);
            }

            var xDataSource = x.AsXDataSource();
            var yDataSource = y.AsYDataSource();

            CompositeDataSource compositeDataSource = xDataSource.Join(yDataSource);

            sin = new LineGraph(compositeDataSource, "sin(x)");

            PlotterMain.Children.Add(sin);
            #endregion

            #region RawDataSource
            Point[] points = new Point[N];

            for (int i = 0; i < N; i++) { 
                points[i] = new Point(i*step, (0.7 * Math.Cos(x[i] * 3) + 3) + (1.5 * Math.Sin(x[i] / 2 + 4)));
            }

            RawDataSource rawDataSource = points.AsDataSource();
            
            cos = new LineGraph(rawDataSource, "(0.7 * Cos(3x)+3)+(1.5*Sin(x/2+4))");
            
            PlotterMain.Children.Add(cos);
            #endregion

            #region EnumerableDataSource and Custom Graph Settings
            
            MyClass[] myObjects = new MyClass[N];
            for (int i = 0; i < N; i++)
                myObjects[i] = new MyClass() { A = 0.1 + i * step };

            EnumerableDataSource<MyClass> enumDataSource = myObjects.AsDataSource<MyClass>();
            enumDataSource.SetXYMapping(o => new Point(o.A,o.B));

            LineGraphSettings settings = new LineGraphSettings();
            settings.LineColor = Colors.Magenta;
            settings.LineThickness = Math.PI;
            settings.Description = "Log10";
            log = new LineGraph(enumDataSource, settings);
            PlotterMain.Children.Add(log);

            #endregion

            PlotterMain.FitToView();
        }
        
        class MyClass
        {
            public double A {set;get;}
            public double B {
                get {
                    return Math.Log10(A);
                }
            }
        };
       
    }
}
