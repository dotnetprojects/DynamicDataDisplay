using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public class NonUniformDataSource2D<T> : INonUniformDataSource2D<T> where T : struct
	{
		public NonUniformDataSource2D(double[] xcoordinates, double[] ycoordinates, T[,] data)
		{
			if (xcoordinates == null)
				throw new ArgumentNullException("xcoordinates");
			if (ycoordinates == null)
				throw new ArgumentNullException("ycoordinates");
			if (data == null)
				throw new ArgumentNullException("data");

			this.xCoordinates = xcoordinates;
			this.yCoordinates = ycoordinates;

			BuildGrid();

			this.data = data;
		}

		private void BuildGrid()
		{
			grid = new Point[Width, Height];
			for (int iy = 0; iy < Height; iy++)
			{
				for (int ix = 0; ix < Width; ix++)
				{
					grid[ix, iy] = new Point(xCoordinates[ix], yCoordinates[iy]);
				}
			}
		}

		#region INonUniformDataSource2D<T> Members

		private double[] xCoordinates;
		public double[] XCoordinates
		{
			get { return xCoordinates; }
		}

		private double[] yCoordinates;
		public double[] YCoordinates
		{
			get { return yCoordinates; }
		}

		#endregion

		#region IDataSource2D<T> Members

		private T[,] data;
		public T[,] Data
		{
			get { return data; }
		}

		public IDataSource2D<T> GetSubset(int x0, int y0, int countX, int countY, int stepX, int stepY)
		{
			throw new NotImplementedException();
		}

		public void ApplyMappings(DependencyObject marker, int x, int y)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IGridSource2D Members

		private Point[,] grid;
		public Point[,] Grid
		{
			get { return grid; }
		}

		public int Width
		{
			get { return xCoordinates.Length; }
		}

		public int Height
		{
			get { return yCoordinates.Length; }
		}

		public event EventHandler Changed;

		#endregion

		#region IDataSource2D<T> Members


		public Microsoft.Research.DynamicDataDisplay.Charts.Range<T>? Range
		{
			get { throw new NotImplementedException(); }
		}

		public T? MissingValue
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
