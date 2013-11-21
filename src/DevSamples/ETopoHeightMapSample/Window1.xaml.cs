#define new

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
using System.IO;

using Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay;

namespace ETopoHeightMapSample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
			if (File.Exists("etopo2.dos.bin"))
			{
#if new
				var heightMap = new ETopoHeightMapGraph();
				Viewport2D.SetContentBounds(heightMap, new DataRect(-180, -85, 360, 170));

				OneThreadRenderingMap map = new OneThreadRenderingMap(heightMap) { DrawDebugBounds = true };
				plotter.Children.Add(map);
#else
				plotter.Children.Add(new ETopoHeightMapGraph());
#endif
			}
			else
			{
				MessageBox.Show("etopo2.dos.bin file is not found.\n" +
					"Please download it from http://www.ngdc.noaa.gov/mgg/global/relief/ETOPO2/ETOPO2-2001/\n" +
					"and place in the same folder with this sample.",
					"Data file is not found!");
			}
        }
    }
}
