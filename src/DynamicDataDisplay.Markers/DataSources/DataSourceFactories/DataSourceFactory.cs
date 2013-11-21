using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public abstract class DataSourceFactory
	{
		public abstract bool TryBuild(object data, out PointDataSourceBase dataSource);
	}
}
