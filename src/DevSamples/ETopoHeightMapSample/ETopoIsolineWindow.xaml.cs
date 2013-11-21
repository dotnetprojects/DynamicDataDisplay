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
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

namespace ETopoHeightMapSample
{
    /// <summary>
    /// Interaction logic for ETopoIsolineWindow.xaml
    /// </summary>
    public partial class ETopoIsolineWindow : Window
    {
        public ETopoIsolineWindow()
        {
            InitializeComponent();
        }

        private void plotter_Loaded(object sender, RoutedEventArgs e)
        {
            plotter.BeginLongOperation();

            Task task = Task.Create((unused) =>
            {
                var dataSource = ReliefReader.ReadDataSource();

                Dispatcher.BeginInvoke(() =>
                {
                    plotter.EndLongOperation();
                    DataContext = dataSource;
                }, DispatcherPriority.Send);
            });
        }
    }
}
