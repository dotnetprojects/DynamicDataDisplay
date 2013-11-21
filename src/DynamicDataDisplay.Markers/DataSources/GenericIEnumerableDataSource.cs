using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class GenericIEnumerableDataSource<T> : PointDataSourceBase
	{
		private readonly IEnumerable<T> collection;
		public IEnumerable<T> Collection
		{
			get { return collection; }
		}

		public GenericIEnumerableDataSource(IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			this.collection = collection;
			TrySubscribeOnCollectionChanged(collection);
		}

		protected override IEnumerable GetDataCore()
		{
			return collection;
		}

		public override IEnumerable GetData(int startingIndex)
		{
			throw new NotImplementedException();
		}

		public override object GetDataType()
		{
			return typeof(T);
		}
	}
}
