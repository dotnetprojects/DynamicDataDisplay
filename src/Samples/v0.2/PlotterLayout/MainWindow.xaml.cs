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
using Microsoft.Research.DynamicDataDisplay;

namespace PlotterLayoutSample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Header header;
        private Footer footer;

        public MainWindow()
        {
            InitializeComponent();

            header = new Header();
            TextBlock headerText = new TextBlock();
            headerText.Text = "Additional header";
            headerText.FontSize = 20;
            headerText.FontWeight = FontWeights.Bold;
            headerText.Foreground = Brushes.DarkBlue;
            header.Content = headerText;

            plotter.Children.Add(header);

            footer = new Footer();
            TextBlock footerText = new TextBlock();
            footerText.Text = "Small footer";
            footerText.FontSize = 8;
            footerText.FontStyle = FontStyles.Italic;
            footerText.Foreground = Brushes.DarkGreen;
            footer.Content = footerText;
        }

        private void secondHeader_Click(object sender, RoutedEventArgs e)
        {
            if (plotter.Children.Contains(header))
            {
                plotter.Children.Remove(header);
                secondHeader.IsChecked = false;
            }
            else
            {
                plotter.Children.Add(header);
                secondHeader.IsChecked = true;
            }
        }

        private void secondFooter_Click(object sender, RoutedEventArgs e)
        {
            if (plotter.Children.Contains(footer))
            {
                plotter.Children.Remove(footer);
                secondFooter.IsChecked = false;
            }
            else
            {
                plotter.Children.Add(footer);
                secondFooter.IsChecked = true;
            }
        }
    }
}
