using System.Collections;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
    /// <summary>This enumerator enumerates given enumerable object as sequence of points</summary>
    /// <typeparam name="T">Type parameter of source IEnumerable</typeparam>
	public sealed class EnumerablePointEnumerator<T> : IPointEnumerator {
		private readonly EnumerableDataSource<T> dataSource;
		private readonly IEnumerator enumerator;

		public EnumerablePointEnumerator(EnumerableDataSource<T> dataSource) {
			this.dataSource = dataSource;
			enumerator = dataSource.Data.GetEnumerator();
		}

		public bool MoveNext() {
			return enumerator.MoveNext();
		}

		public void GetCurrent(ref Point p) {
			dataSource.FillPoint((T)enumerator.Current, ref p);
		}

		public void ApplyMappings(DependencyObject target) {
			dataSource.ApplyMappings(target, (T)enumerator.Current);
		}

		public void Dispose() {
			//enumerator.Reset();
		}
	}
}
