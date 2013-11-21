using Microsoft.Research.DynamicDataDisplay;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

public class ETopoHeightMapGraph : BitmapBasedGraph
{
    private ReliefPalette palette = new ReliefPalette();

    public ETopoHeightMapGraph()
    {
    }

    protected override BitmapSource RenderFrame(DataRect dataRect, Rect output)
    {
        double left = 0;
        double lonMin = dataRect.XMin;
        double scale = output.Width / dataRect.Width;
        if (lonMin < -180)
        {
            left = (-180 - dataRect.XMin) * scale;
            lonMin = -180;
        }

        double width = output.Width - left;
        double lonMax = dataRect.XMax;
        if (lonMax > 180)
        {
            width -= (dataRect.XMax - 180) * scale;
            lonMax = 180;
        }

        if (lonMin == lonMax)
        {
            return null;
        }

        scale = output.Height / dataRect.Height;
        double top = 0;
        double latMax = dataRect.YMax;
        if (latMax > 90)
        {
            top = (dataRect.YMax - 90) * scale;
            latMax = 90;
        }

        double height = output.Height - top;
        double latMin = dataRect.YMin;
        if (latMin < -90)
        {
            height -= (-90 - dataRect.YMin) * scale;
            latMin = -90;
        }

        short min, max;
        short[,] elev = ReliefReader.ReadElevationMap(latMin, latMax, (int)height,
            lonMin, lonMax, (int)width, out min, out max);

        int pixelWidth = (int)output.Width;
        int pixelHeight = (int)output.Height;
        UInt32[] pixels = new UInt32[pixelWidth * pixelHeight];
        int i0 = (int)left;
        int j0 = (int)top;
        for (int i = 0; i < (int)width; i++)
            for (int j = 0; j < (int)height; j++)
            {
                Color color = ReliefPalette.GetColor(elev[j, i]);
                pixels[i + i0 + (j + j0) * pixelWidth] = 0x7F << 24 |
                    (((uint)color.R) << 16) |
                    (((uint)color.G) << 8) |
                    color.B;
            }

        var result = BitmapFrame.Create(pixelWidth, pixelHeight, 96, 96, PixelFormats.Bgra32, null, pixels, 4 * pixelWidth);
        return result;

        /*        Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                TextBlock lt = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Text = String.Format("({0},{1})", lonMin, latMax)
                };
                grid.Children.Add(lt);
                Grid.SetRow(lt, 0);
                TextBlock rb = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Text = String.Format("({0},{1})", lonMax, latMin)
                };
                grid.Children.Add(rb);
                Grid.SetRow(rb, 1);
                Border border = new Border();
                border.BorderThickness = new Thickness(3);
                border.BorderBrush = Brushes.Blue;
                border.Child = grid;

                Canvas canvas = new Canvas();
                canvas.Children.Add(border);
                Canvas.SetLeft(border, left);
                border.Width = width;
                Canvas.SetTop(border, top);
                border.Height = height;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)output.Width, (int)output.Height, 96, 96, PixelFormats.Default);
                canvas.Measure(new Size(output.Width, output.Height));
                canvas.Arrange(output);
                rtb.Render(canvas);

                Thread.Sleep(1000);

                return rtb;*/
    }


    protected override UIElement GetTooltipForPoint(Point point, DataRect visible, Rect output)
    {
        return null;
    }
}