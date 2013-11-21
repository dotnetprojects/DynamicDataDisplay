using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class GenericDataSource2D<T> : PointDataSourceBase where T : struct
	{
		public GenericDataSource2D(IDataSource2D<T> dataSource)
		{
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");

			this.dataSource = dataSource;
		}

		private readonly IDataSource2D<T> dataSource;

		protected override IEnumerable GetDataCore()
		{
			int width = dataSource.Width;
			int height = dataSource.Height;

			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					Point position = dataSource.Grid[ix, iy];
					T data = dataSource.Data[ix, iy];

					DataSource2DPiece<T> piece = new DataSource2DPiece<T> { Position = position, Data = data };
					yield return piece;
				}
			}
		}

		public override IEnumerable GetData(int startingIndex)
		{
			throw new NotImplementedException();
		}

		public override object GetDataType()
		{
			throw new NotImplementedException();
		}
	}
}
