using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace PerfCounterChart
{
	public sealed class FilteringDataSource<T> : IEnumerable<T>, INotifyCollectionChanged
	{
		private readonly IList<T> collection;

		public FilteringDataSource(IList<T> collection, IFilter<T> filter)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (filter == null)
				throw new ArgumentNullException("filter");
	

			this.collection = collection;
			
			INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
			if (observableCollection != null)
			{
				observableCollection.CollectionChanged += collection_CollectionChanged;
			}

			this.filter = filter;
		}

		void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaiseCollectionChanged();
		}

		private readonly IFilter<T> filter;

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)filter.Filter(collection)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void RaiseCollectionChanged()
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}
		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
