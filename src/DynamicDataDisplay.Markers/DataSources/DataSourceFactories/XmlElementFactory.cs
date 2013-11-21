using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public class XmlElementFactory : DataSourceFactory
	{
		public override bool TryBuild(object data, out PointDataSourceBase dataSource)
		{
			XmlElement xmlElement = data as XmlElement;
			dataSource = null;
			if (xmlElement != null)
			{
				dataSource = new XmlElementDataSource(xmlElement);
				return true;
			}
			return false;
		}
	}
}
