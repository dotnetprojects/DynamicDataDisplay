using System.Collections;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	public sealed class RawPointEnumerator : IPointEnumerator {
		private readonly IEnumerator enumerator; 

		public RawPointEnumerator(RawDataSource dataSource) {
			this.enumerator = dataSource.Data.GetEnumerator();
		}

		public bool MoveNext() {
			return enumerator.MoveNext();
		}

		public void GetCurrent(ref Point p) {
			p = (Point)enumerator.Current;
		}

		public void ApplyMappings(DependencyObject target) {
			// do nothing here - no mapping supported
		}

		public void Dispose() {
			// do nothing here
		}
	}
}
