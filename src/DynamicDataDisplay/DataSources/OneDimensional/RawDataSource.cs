using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	public sealed class RawDataSource : EnumerableDataSourceBase<Point> {
		public RawDataSource(params Point[] data) : base(data) { }
		public RawDataSource(IEnumerable<Point> data) : base(data) { }

		public override IPointEnumerator GetEnumerator(DependencyObject context) {
			return new RawPointEnumerator(this);
		}
	}
}
