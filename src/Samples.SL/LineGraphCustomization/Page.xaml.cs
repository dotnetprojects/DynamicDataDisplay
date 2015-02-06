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
using System.Windows.Data;

namespace LineGraphCustomization
{
    public partial class Page : UserControl
    {
        LineGraph graph;

        public Page()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            #region Data preparation
            const int N = 200;

            int[] thiknesses = new int[10];

            for(int i=0;i<10;i++)
                thiknesses[i] = i+1;

            Point[] points = new Point[N];
            double step = 10 / (double)N;

            for(int i=0;i<N;i++) {
                points[i].X = -5 + i*step;
                points[i].Y = Math.Exp(points[i].X);
            }

            EnumerableDataSource<Point> dataSource = points.AsDataSource<Point>();
            dataSource.SetXYMapping(point => point);
            #endregion

            graph = new LineGraph(dataSource, "Graph Description");
            graph.LineThickness = 3;
            graph.LineColor = Colors.Red;

            
            MainPlotter.Children.Add(graph);
            MainPlotter.FitToView();

            TextBoxDescription.Text = graph.Description;
        }

        private String FetchStringFromComboBoxItem(ComboBoxItem item) {
            TextBlock t = item.Content as TextBlock;
            if(t != null) return t.Text;
            else return null;
        }

        private void Thickness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (graph != null)
            {

                String str = FetchStringFromComboBoxItem(e.AddedItems[0] as ComboBoxItem);
                int newThickness;
                if (int.TryParse(str, out newThickness))
                {
                    graph.LineThickness = newThickness;
                }
            }
        }

        private void Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (graph != null)
                graph.Description = t.Text;
        }

        private void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (graph != null) {
                String str = FetchStringFromComboBoxItem(e.AddedItems[0] as ComboBoxItem);
                switch (str)
                {
                    case "RED": graph.LineColor = Colors.Red; break;
                    case "GREEN": graph.LineColor = Colors.Green; break;
                    case "BLUE": graph.LineColor = Colors.Blue; break;
                    case "PURPLE": graph.LineColor = Colors.Purple; break;
                    case "ORANGE": graph.LineColor = Colors.Orange; break;
                    case "BLACK": graph.LineColor = Colors.Black; break;
                    default: break ;
                }
            }
        }

        private void CheckBoxToolTip_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (graph != null) {
                graph.IsTooltipEnabled = (c.IsChecked == true);
            }
        }

        private void CheckBoxShownInLegend_Click(object sender, RoutedEventArgs e)
        {
            if(graph!=null)
                graph.ShowInLegend = (CheckBoxShownInLegend.IsChecked == true);
        }
    }
}
