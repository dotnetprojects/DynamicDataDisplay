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
using System.Threading;

namespace BitmapBasedGraphSample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
    }

    public class BitmapBasedSampleGraph : BitmapBasedGraph
    {
        protected override BitmapSource RenderFrame(DataRect dataRect, Rect output)
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            TextBlock lt = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = String.Format("({0},{1})", dataRect.XMin, dataRect.YMax)
            };
            grid.Children.Add(lt);
            Grid.SetRow(lt, 0);
            TextBlock rb = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Text = String.Format("({0},{1})", dataRect.XMax, dataRect.YMin)
            };
            grid.Children.Add(rb);
            Grid.SetRow(rb, 1);
            Border border = new Border();
            border.BorderThickness = new Thickness(3);
            border.BorderBrush = Brushes.Blue;
            border.Child = grid;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)output.Width, (int)output.Height, 96, 96, PixelFormats.Default);
            border.Measure(new Size(output.Width, output.Height));
            border.Arrange(output);
            rtb.Render(border);

            Thread.Sleep(1000);

            return rtb;
        }
	}
}
