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
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace LegendCustomization
{
    public partial class Page : UserControl
    {
        private Legend legend;
        private ChartPlotter mainPlotter;
        private Button buttonAdd, buttonClear;

        private Random random = new Random();

        public Page()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        private void addRandomPolynomial()
        {
            int[] multipliers = new int[5];
            Point[] points = new Point[500];

            for (int i = 0; i < multipliers.Length; i++)
                multipliers[i] = (int)((random.NextDouble() - 0.5) * 200);

            double step = (double)(10) / points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = -5 + (step * i);
                points[i].Y = Math.Pow(points[i].X, 5) +
                    multipliers[0] * Math.Pow(points[i].X, 4) +
                    multipliers[1] * Math.Pow(points[i].X, 3) +
                    multipliers[2] * Math.Pow(points[i].X, 2) +
                    multipliers[3] * points[i].X +
                    multipliers[4];
            }

            var dataSource = points.AsDataSource<Point>();
            dataSource.SetXYMapping(point => point);


            LineGraph polinom = new LineGraph(dataSource,
                multipliers[0]+" "+multipliers[1]+" "+multipliers[2]+" "+multipliers[3]+" "+multipliers[4]);
            
            mainPlotter.Children.Add(polinom);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating plotter without a legend and button navigation
            ChartPlotterSettings settings = new ChartPlotterSettings();
            settings.IsButtonNavigationPresents = false;
            settings.IsLegendPresents = false;
            mainPlotter = new ChartPlotter(settings);
            RootStackPanel.Children.Add(mainPlotter);

            //Adding legened to custom location
            legend = new Legend(mainPlotter);
            //Wrapper to provide easy usage of the legend if it is too big
            ScrollWraper wraper = new ScrollWraper(legend);
            RootStackPanel.Children.Insert(0,wraper);

            //Adding custom buttons to plotter hovering panel
            buttonAdd = new Button() { Content="Add random polynomial"};
            buttonAdd.Click+=new RoutedEventHandler(ButtonAddClick);
            buttonClear = new Button() { Content="Remove all charts"};
            buttonClear.Click +=new RoutedEventHandler(ButtonClearClick);
            StackPanel internalStackPanel = new StackPanel() { Orientation = Orientation.Vertical, Margin= new Thickness(5) };
            internalStackPanel.Children.Add(buttonAdd);
            internalStackPanel.Children.Add(buttonClear);
            mainPlotter.HoveringStackPanel.Children.Add(internalStackPanel);

            //Adding button navigation to custom location
            Microsoft.Research.DynamicDataDisplay.Navigation.buttonsNavigation n =
                new Microsoft.Research.DynamicDataDisplay.Navigation.buttonsNavigation(mainPlotter);
            RootStackPanel.Children.Add(n);

            //Adding some data to the plotter
            for (int i = 0; i < 10; i++)
                addRandomPolynomial();
            
            mainPlotter.FitToView();
        }

        private void ButtonAddClick(object sender, RoutedEventArgs e)
        {
            addRandomPolynomial();
            mainPlotter.FitToView();
        }

        private void ButtonClearClick(object sender, RoutedEventArgs e)
        {
            mainPlotter.RemoveAllGraphs();
        }
    }
}
