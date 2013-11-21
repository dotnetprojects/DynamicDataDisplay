using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;

public class TimeSeriesNcmlPlotter
{
    public static void PlotData(Stream outstream,int pixelWidth,int pixelHeight)
    {
        RenderTargetBitmap target = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Default);
        ChartPlotter plotter = new ChartPlotter();
        plotter.Width = 200;
        plotter.Height = 200;
        target.Render(plotter);
        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(target));
        encoder.Save(outstream);
    }
}