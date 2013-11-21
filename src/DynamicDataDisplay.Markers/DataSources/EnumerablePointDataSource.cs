using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Collections.Specialized;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class EnumerablePointDataSource : PointDataSourceBase
	{
		public EnumerablePointDataSource(IEnumerable<Point> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			this.collection = collection;
			TrySubscribeOnCollectionChanged(collection);
		}

		private readonly IEnumerable<Point> collection;
		public IEnumerable<Point> Collection
		{
			get { return collection; }
		} 

		protected override IEnumerable GetDataCore()
		{
			return Filters.Filter(collection, Environment);
		}

		public override IEnumerable GetData(int startingIndex)
		{
			return collection.Skip(startingIndex);
		}

		public override object GetDataType()
		{
			return typeof(Point);
		}
	}
}
