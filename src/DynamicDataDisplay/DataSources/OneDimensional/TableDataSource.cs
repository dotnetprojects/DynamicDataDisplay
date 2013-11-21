using System.Data;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	/// <summary>Data source that extracts sequence of points and their attributes from DataTable</summary>
	public class TableDataSource : EnumerableDataSource<DataRow>
	{
		public TableDataSource(DataTable table)
			: base(table.Rows)
		{
			// Subscribe to DataTable events
			table.TableNewRow += NewRowInsertedHandler;
			table.RowChanged += RowChangedHandler;
			table.RowDeleted += RowChangedHandler;
		}

		private void RowChangedHandler(object sender, DataRowChangeEventArgs e)
		{
			RaiseDataChanged();
		}

		private void NewRowInsertedHandler(object sender, DataTableNewRowEventArgs e)
		{
			// Raise DataChanged event. ChartPlotter should redraw graph.
			// This will be done automatically when rows are added to table.
			RaiseDataChanged();
		}
	}
}
