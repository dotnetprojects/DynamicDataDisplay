using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class GenericIDataSource2DFactory : DataSourceFactory
	{
		public override bool TryBuild(object data, out PointDataSourceBase dataSource)
		{
			dataSource = null;

			var types = IEnumerableHelper.GetGenericInterfaceArgumentTypes(data, typeof(IDataSource2D<>));

			if (types != null && types.Length > 0)
			{
				Type genericDataSource2DType = typeof(GenericDataSource2D<>).MakeGenericType(types);
				var result = Activator.CreateInstance(genericDataSource2DType, data);
				dataSource = (PointDataSourceBase)result;
				return true;
			}

			return false;
		}
	}
}
