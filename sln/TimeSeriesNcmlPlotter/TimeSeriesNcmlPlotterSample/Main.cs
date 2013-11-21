using System;
using System.IO;

public class App
{
    [STAThread]
    public static void Main(string[] args)
    {
        using (Stream fs = new FileStream("output.png", FileMode.Create))
            TimeSeriesNcmlPlotter.PlotData(fs, 640, 480);
    }
}