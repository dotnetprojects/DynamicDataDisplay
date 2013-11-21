using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public class IEnumerablePointFactory : DataSourceFactory
	{
		public override bool TryBuild(object data, out PointDataSourceBase dataSource)
		{
			dataSource = null;

			IEnumerable<Point> collection = data as IEnumerable<Point>;
			if (collection != null)
			{
				dataSource = new EnumerablePointDataSource(collection);
				return true;
			}
			else
			{
				return false;
			}
		}

	}
}
