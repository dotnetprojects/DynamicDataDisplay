using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Media.Imaging;
using System.Windows;

namespace BitmapbaseGraphSample
{
    public class BitmapGraph : BitmapBasedGraph
    {
        protected override BitmapSource RenderFrame(DataRect visible, Rect output)
        {
            return new BitmapImage(new Uri(@"C:\Users\Mikhail\Pictures\Wallpapers\img10.jpg"));
        }
    }
}
