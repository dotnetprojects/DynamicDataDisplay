using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	/// <summary>
	/// Empty data source - for testing purposes, represents data source with 0 points inside.
	/// </summary>
	public class EmptyDataSource : IPointDataSource
	{
		#region IPointDataSource Members

		public IPointEnumerator GetEnumerator(DependencyObject context)
		{
			return new EmptyPointEnumerator();
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private void RaiseDataChanged()
		{
			if (DataChanged != null)
			{
				DataChanged(this, EventArgs.Empty);
			}
		}

		public event EventHandler DataChanged;

		#endregion

		private sealed class EmptyPointEnumerator : IPointEnumerator
		{
			public bool MoveNext()
			{
				return false;
			}

			public void GetCurrent(ref Point p)
			{
				// nothing to do
			}

			public void ApplyMappings(DependencyObject target)
			{
				// nothing to do
			}

			public void Dispose() { }
		}
	}
}
