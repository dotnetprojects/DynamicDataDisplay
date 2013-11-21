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
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace ETopoHeightMapSample
{
    /// <summary>
    /// Interaction logic for ETopoThreadedIsoline.xaml
    /// </summary>
    public partial class ETopoThreadedIsolineWindow : Window
    {
        public ETopoThreadedIsolineWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            renderingMap.FileTileServer = new AutoDisposableFileServer();

            plotter.BeginLongOperation();

            Task task = Task.Create((unused) =>
            {
                var dataSource = ReliefReader.ReadDataSource();

                Dispatcher.BeginInvoke(() =>
                {
                    plotter.EndLongOperation();
                }, DispatcherPriority.Send);

                tileServer.Dispatcher.BeginInvoke(() =>
                {
                    tileServer.ContentBounds = new DataRect(-180, -90, 360, 180);
                }, DispatcherPriority.Send);

				tileServer.ChildCreateHandler = () =>
				{
					FastIsolineDisplay isoline = new FastIsolineDisplay { WayBeforeTextMultiplier = 100, LabelStringFormat = "F0" };
					Viewport2D.SetContentBounds(isoline, new DataRect(-180, -90, 360, 180));
					isoline.DataSource = dataSource;

					return isoline;
				};
            });
        }
    }
}
