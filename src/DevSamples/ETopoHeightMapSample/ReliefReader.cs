using System;
using System.IO;
using System.Configuration;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Windows.Media;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;

public class ReliefReader
{
    public static WarpedDataSource2D<double> ReadDataSource()
    {
        short min, max;
        double latMin = -90;
        double latMax = 90;
        double lonMin = -180;
        double lonMax = 180;
        int width = 512;
        int height = 512;

        short[,] elev = ReliefReader.ReadElevationMap(latMin, latMax, (int)width,
            lonMin, lonMax, (int)height, out min, out max);

        double[,] data = new double[width, height];
        for (int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                data[ix, iy] = elev[ix, iy];
            }
        }

        Point[,] grid = new Point[width, height];

        double xStep = (lonMax - lonMin) / width;
        double yStep = (latMax - latMin) / height;

        for (int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                grid[iy, ix] = new Point(lonMin + xStep * ix, latMin + yStep * (height - iy - 1));
            }
        }

        WarpedDataSource2D<double> dataSource = new WarpedDataSource2D<double>(data, grid);
        return dataSource;
    }

    const int ColumnsCount = 10800;
    const int RowsCount = 5400;

    /// <summary>Reads elevation map from ETOPO2 file</summary>
    /// <param name="latMin">Min. latitude (from -90 to 90). -90 is South Pole, 90 is North pole</param>
    /// <param name="latMax">Max. latitude</param>
    /// <param name="latSamples">Number of points along latitude</param>
    /// <param name="longMin">Min. londitude (from -180 to 180), 0 is Greenwich</param>
    /// <param name="longMax">Max. londitude</param>
    /// <param name="longRes">Number of points along longitude</param>
    /// <param name="minElev">Minimal elevation value in selected rectangle</param>
    /// <param name="maxElev">Maximal elevation value in selected rectangle</param>
    /// <returns>Array of latSamples * longSamples elevations measured in meters</returns>
    public static short[,] ReadElevationMap(double latMin, double latMax, int latSamples,
                                  double longMin, double longMax, int longSamples,
                                  out short minElev, out short maxElev)
    {
        longMin += 180;
        longMax += 180;

        double temp = latMin;
        latMin = 90 - latMax;
        latMax = 90 - temp;

        FileStream fs = null;
        try
        {
            fs = new FileStream(GetReliefPath(), FileMode.Open);
            short[,] result = new short[latSamples, longSamples];

            int latIdxMin = (int)(latMin * 30);
            int latIdxMax = (int)(latMax * 30);
            if (latIdxMin >= RowsCount)
                latIdxMin = RowsCount - 1;

            int longIdxMin = (int)(longMin * 30);
            int longIdxMax = (int)(longMax * 30);
            if (longIdxMin >= ColumnsCount)
                longIdxMin = ColumnsCount - 1;

            int scanLineSize = 2 * (longIdxMax - longIdxMin + 1);
            byte[] scanLine = new byte[scanLineSize];

            int row = 0;
            minElev = Int16.MaxValue;
            maxElev = Int16.MinValue;
            for (int i = latIdxMin; i <= latIdxMax; i++)
            {
                fs.Seek(2 * (10800 * i + longIdxMin), SeekOrigin.Begin);
                fs.Read(scanLine, 0, scanLineSize);
                BinaryReader br = new BinaryReader(new MemoryStream(scanLine));
                bool readRow = (row == (i - latIdxMin) * (latSamples - 1) / (latIdxMax - latIdxMin));
                int col = 0;
                for (int j = longIdxMin; j <= longIdxMax; j++)
                {
                    short e = br.ReadInt16();
                    if (e > maxElev)
                        maxElev = e;
                    if (e < minElev)
                        minElev = e;

                    if (readRow)
                        if (col == (j - longIdxMin) * (longSamples - 1) / (longIdxMax - longIdxMin))
                            result[row, col++] = e;
                }
                if (readRow)
                    row++;
                br.Close();
            }
            return result;
        }
        catch (Exception exc)
        {
            MessageBox.Show(exc.Message, "Cannot read elevation map from ETOPO2");

        }
        finally
        {
            if (fs != null)
                fs.Close();
        }

        minElev = maxElev = 0;
        return null;
    }

    public static string GetReliefPath()
    {
        string path = ETopoHeightMapSample.Properties.Settings.Default.ReliefPath;
        return path;
    }
}

public class ReliefPalette
{
    protected static UniformLinearPalette palette = null;
    public const int MinElevation = -12000;
    public const int MaxElevation = 8000;

    /// <summary>Returns color corresponding to height</summary>
    /// <param name="height">Height in meters relative to sea level</param>
    /// <returns>Color structure</returns>
    public static Color GetColor(short height)
    {
        if (palette == null)
            InitPalette();

        double alpha = (height - MinElevation) / (double)(MaxElevation - MinElevation);
        if (alpha < 0) alpha = 0;
        if (alpha > 1) alpha = 1;
        return palette.GetColor(alpha);
    }

    protected static void InitPalette()
    {
        int[] elevations = new int[] { -10000, -5000, -1000, -200, 0, 500, 1000, 2000, 3000, 4000, 5000, 6000 };
        Color[] colors = new Color[] {
            Color.FromRgb(50, 0, 100),
            Color.FromRgb(0, 0, 100),
            Color.FromRgb(0, 0, 255),
            Color.FromRgb(147, 240, 220),
            Color.FromRgb(247, 210, 194),
            Color.FromRgb(247, 210, 164),
            Color.FromRgb(245, 198, 144),
            Color.FromRgb(242, 175, 109),
            Color.FromRgb(239, 156, 89),
            Color.FromRgb(232, 123, 20),
            Color.FromRgb(228, 96, 68),
            Color.FromRgb(200, 43, 25)
        };
        palette = new UniformLinearPalette(colors);
    }
}