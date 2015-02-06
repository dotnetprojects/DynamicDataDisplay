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

namespace AllergologySample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
            mainPlotter.Loaded += new RoutedEventHandler(mainPlotter_Loaded);
        }

        void mainPlotter_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        private List<LineGraph> linegraphs = new List<LineGraph>();

        private void ReloadData() {
            if (mainPlotter != null)
            {
                mainPlotter.RemoveAllGraphs();
                linegraphs.Clear();
                string location = (comboboxLocation.SelectedItem as ContentControl).Content as string;
                AllergologySample.DataProvider dataProvider = new DataProvider(location + ".xml");
                mainPlotter.HorizontalAxis = new HorizontalDateTimeAxis();
                try
                {
                    dataProvider.Load(checkBoxGroupAlergens.IsChecked == true);
                    foreach (string alergen in dataProvider.GetAlergens())
                    {
                        var xs = dataProvider.GetXcomponents(alergen).AsXDataSource();
                        xs.SetXMapping(d => (mainPlotter.HorizontalAxis as HorizontalDateTimeAxis).ConvertToDouble(d));
                        var ys = dataProvider.GetYcomponents(alergen).AsYDataSource();
                        CompositeDataSource dataSource = xs.Join(ys);
                        LineGraph linegraph = new LineGraph(dataSource, alergen);
                        linegraphs.Add(linegraph);
                        mainPlotter.Children.Add(linegraph);
                    }
                    mainPlotter.FitToView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " " + ex.StackTrace);
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadData();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }
    }
}
