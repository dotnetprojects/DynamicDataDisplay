using System;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows;

public class IsolinePlotterBenchmark
{
	public static void Main(string[] args)
	{
		var ds = new BenchmarkDataSource(200, 200);
		var builder = new IsolineBuilder(ds);
		Console.WriteLine("Data field size is {0}x{1}", ds.Width, ds.Height);
		DateTime start = DateTime.Now;
		int pointCount = 0;
		for (double level = -2.0; level <= 2.0; level += 0.01)
		{
			Console.WriteLine("Building coutour line at {0}", level);
			foreach (LevelLine line in builder.BuildIsoline(level).Lines)
				pointCount += 1 + line.OtherPoints.Count;
		}
		TimeSpan elapsed = DateTime.Now - start;
		Console.WriteLine("Points generated: {0}", pointCount);
		Console.WriteLine("Elapsed time: {0} ms", elapsed.TotalMilliseconds);
	}
}

public class BenchmarkDataSource : IDataSource2D<double>
{
	private int width, height;
	private double[,] data;
	private Point[,] grid;

	public BenchmarkDataSource(int width, int height)
	{
		this.width = width;
		this.height = height;
		grid = new Point[width, height];
		data = new double[width, height];
		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				grid[i, j] = new Point(-10 + 20.0 * i / (width - 1), -10 + 20.0 * j / (height - 1));
				data[i, j] = Math.Sin(3 * grid[i, j].X) + Math.Cos(3 * grid[i, j].Y);
			}
	}

	public double[,] Data
	{
		get
		{
			return data;
		}
	}

	public IDataSource2D<double> GetSubset(int x0, int y0, int countX, int countY, int stepX, int stepY)
	{
		throw new NotImplementedException();
	}

	public Point[,] Grid
	{
		get
		{
			return grid;
		}
	}

	public int Width
	{
		get { return width; }
	}

	public int Height
	{
		get { return height; }
	}

	public event EventHandler Changed;

	#region IDataSource2D<double> Members


	public Microsoft.Research.DynamicDataDisplay.Charts.Range<double>? Range
	{
		get { throw new NotImplementedException(); }
	}

	public double? MissingValue
	{
		get { throw new NotImplementedException(); }
	}

	#endregion
}