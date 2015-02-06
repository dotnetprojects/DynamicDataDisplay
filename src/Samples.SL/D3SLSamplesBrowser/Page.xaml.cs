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

namespace D3SLSamplesBrowser
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        private void LoadPreview(UIElement elem)
        {
            PreviewGrid.Children.Clear();
            PreviewGrid.Children.Add(elem);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ProjectsList.SelectedItem == DataSourcesTutorialItem)
                LoadPreview(new DataSourcesTutorial.Page());
            else if (ProjectsList.SelectedItem == LayoutTutorialItem)
                LoadPreview(new LayoutTutorial.Page());
            else if (ProjectsList.SelectedItem == LineGraphCustomiztionTutorialItem)
                LoadPreview(new LineGraphCustomization.Page());
            else if (ProjectsList.SelectedItem == DateTimeAxisTutorialItem)
                LoadPreview(new DateTimeAxisTutorial.Page());
            else if (ProjectsList.SelectedItem == LegendCustomiztionTutorialItem)
                LoadPreview(new LegendCustomization.Page());
            else if (ProjectsList.SelectedItem == AlergologySampleItem)
                LoadPreview(new AllergologySample.Page());
            else if (ProjectsList.SelectedItem == AnimationSampleItem)
                LoadPreview(new AnimationSample.Page());
            else if (ProjectsList.SelectedItem == CurrencyExchangeSampleItem)
                LoadPreview(new CurrencyExchange.Page());
        }
    }
}
