using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	/// <summary>
	/// Defines warped two-dimensional data source.
	/// </summary>
	/// <typeparam name="T">Data piece type</typeparam>
	public sealed class WarpedDataSource2D<T> : IDataSource2D<T> where T : struct
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WarpedDataSource2D&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="grid">Grid.</param>
		public WarpedDataSource2D(T[,] data, Point[,] grid)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (grid == null)
				throw new ArgumentNullException("grid");

			Verify.IsTrue(data.GetLength(0) == grid.GetLength(0));
			Verify.IsTrue(data.GetLength(1) == grid.GetLength(1));

			this.data = data;
			this.grid = grid;
			width = data.GetLength(0);
			height = data.GetLength(1);
		}

		#region DataSource<T> Members

		private readonly T[,] data;
		/// <summary>
		/// Gets two-dimensional data array.
		/// </summary>
		/// <value>The data.</value>
		public T[,] Data
		{
			get { return data; }
		}

		private readonly Point[,] grid;
		/// <summary>
		/// Gets the grid of data source.
		/// </summary>
		/// <value>The grid.</value>
		public Point[,] Grid
		{
			get { return grid; }
		}

		private readonly int width;
		/// <summary>
		/// Gets data grid width.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get { return width; }
		}

		private readonly int height;
		/// <summary>
		/// Gets data grid height.
		/// </summary>
		/// <value>The height.</value>
		public int Height
		{
			get { return height; }
		}

		public IDataSource2D<T> GetSubset(int x0, int y0, int countX, int countY, int stepX, int stepY)
		{
			throw new NotImplementedException();
		}

		private void RaiseChanged()
		{
			if (Changed != null)
			{
				Changed(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// Occurs when data source changes.
		/// </summary>
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
