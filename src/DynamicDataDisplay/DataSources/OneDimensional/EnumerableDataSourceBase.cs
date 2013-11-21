using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
    /// <summary>Base class for all sources who receive data for charting 
    /// from any IEnumerable of T</summary>
    /// <typeparam name="T">Type of items in IEnumerable</typeparam>
	public abstract class EnumerableDataSourceBase<T> : IPointDataSource {
        private IEnumerable data;

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The data.</value>
        public IEnumerable Data {
			get { return data; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");

				data = value;

				var observableCollection = data as INotifyCollectionChanged;
				if (observableCollection != null) {
					observableCollection.CollectionChanged += observableCollection_CollectionChanged;
				}
			}
		}

		protected EnumerableDataSourceBase(IEnumerable<T> data) : this((IEnumerable)data) { }

		protected EnumerableDataSourceBase(IEnumerable data) {
			if (data == null)
				throw new ArgumentNullException("data");
			Data = data;
		}

		private void observableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RaiseDataChanged();
		}

		public event EventHandler DataChanged;
		public void RaiseDataChanged() {
			if (DataChanged != null) {
				DataChanged(this, EventArgs.Empty);
			}
		}

		public abstract IPointEnumerator GetEnumerator(DependencyObject context);
	}
}
