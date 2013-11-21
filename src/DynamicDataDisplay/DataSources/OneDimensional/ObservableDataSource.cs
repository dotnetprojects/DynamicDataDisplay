using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	// todo I don't think that we should create data source which supports 
	// suspending its DataChanged event - it is better to create
	// collection with the same functionality - then it would be able to be used
	// as a source in many data sources.
	public class ObservableDataSource<T> : IPointDataSource
	{

		/// <summary>True if collection was changed between SuspendUpdate and ResumeUpdate
		/// or false otherwise</summary>
		private bool collectionChanged = false;

		/// <summary>True if event should be raised on each collection change
		/// or false otherwise</summary>
		private bool updatesEnabled = true;

		public ObservableDataSource()
		{
			collection.CollectionChanged += OnCollectionChanged;

			// todo this is hack
			if (typeof(T) == typeof(Point))
			{
				xyMapping = t => (Point)(object)t;
			}
		}

		public ObservableDataSource(IEnumerable<T> data)
			: this()
		{
			if (data == null)
				throw new ArgumentNullException("data");

			foreach (T item in data)
			{
				collection.Add(item);
			}
		}

		public void SuspendUpdate()
		{
			updatesEnabled = false;
		}

		public void ResumeUpdate()
		{
			updatesEnabled = true;
			if (collectionChanged)
			{
				collectionChanged = false;
				RaiseDataChanged();
			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (updatesEnabled)
			{
				RaiseDataChanged();
			}
			else
			{
				collectionChanged = true;
			}
		}

		private readonly ObservableCollection<T> collection = new ObservableCollection<T>();

		public ObservableCollection<T> Collection
		{
			get { return collection; }
		}

		public void AppendMany(IEnumerable<T> data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			updatesEnabled = false;
			foreach (var p in data)
			{
				collection.Add(p);
			}
			updatesEnabled = true;
			RaiseDataChanged();
		}

		public void AppendAsync(Dispatcher dispatcher, T item)
		{
			dispatcher.Invoke(DispatcherPriority.Normal,
				new Action(() =>
				{
					collection.Add(item);
					RaiseDataChanged();
				}));
		}

		private readonly List<Mapping<T>> mappings = new List<Mapping<T>>();
		private Func<T, double> xMapping;
		private Func<T, double> yMapping;
		private Func<T, Point> xyMapping;

		public void SetXMapping(Func<T, double> mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException("mapping");

			this.xMapping = mapping;
		}

		public void SetYMapping(Func<T, double> mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException("mapping");

			this.yMapping = mapping;
		}

		public void SetXYMapping(Func<T, Point> mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException("mapping");

			this.xyMapping = mapping;
		}

		#region IChartDataSource Members

		private class ObservableIterator : IPointEnumerator
		{
			private readonly ObservableDataSource<T> dataSource;
			private readonly IEnumerator<T> enumerator;

			public ObservableIterator(ObservableDataSource<T> dataSource)
			{
				this.dataSource = dataSource;
				enumerator = dataSource.collection.GetEnumerator();
			}

			#region IChartPointEnumerator Members

			public bool MoveNext()
			{
				return enumerator.MoveNext();
			}

			public void GetCurrent(ref Point p)
			{
				dataSource.FillPoint(enumerator.Current, ref p);
			}

			public void ApplyMappings(DependencyObject target)
			{
				dataSource.ApplyMappings(target, enumerator.Current);
			}

			public void Dispose()
			{
				enumerator.Dispose();
				GC.SuppressFinalize(this);
			}

			#endregion
		}

		private void FillPoint(T elem, ref Point point)
		{
			if (xyMapping != null)
			{
				point = xyMapping(elem);
			}
			else
			{
				if (xMapping != null)
				{
					point.X = xMapping(elem);
				}
				if (yMapping != null)
				{
					point.Y = yMapping(elem);
				}
			}
		}

		private void ApplyMappings(DependencyObject target, T elem)
		{
			if (target != null)
			{
				foreach (var mapping in mappings)
				{
					target.SetValue(mapping.Property, mapping.F(elem));
				}
			}
		}

		public IPointEnumerator GetEnumerator(DependencyObject context)
		{
			return new ObservableIterator(this);
		}

		public event EventHandler DataChanged;
		private void RaiseDataChanged()
		{
			if (DataChanged != null)
			{
				DataChanged(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
