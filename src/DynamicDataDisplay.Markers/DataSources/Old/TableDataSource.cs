//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Windows;
//using System.Collections;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
//{
//    public class TableDataSource : IPointDataSource
//    {
//        private readonly DataTable table;
//        internal TableDataSource(DataTable table)
//        {
//            if (table == null)
//                throw new ArgumentNullException("table");

//            this.table = table;
//            table.TableNewRow += OnTableNewRow;
//            table.RowChanged += OnRowChanged;
//            table.RowDeleted += OnRowDeleted;
//        }

//        private void OnRowDeleted(object sender, DataRowChangeEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        private void OnRowChanged(object sender, DataRowChangeEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        private void OnTableNewRow(object sender, DataTableNewRowEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        #region IPointDataSource Members

//        public event EventHandler Changed;

//        public IEnumerator<Point> GetEnumerator()
//        {
//            Verify();

//            if (table.Rows == null)
//                yield break;

//            foreach (DataRow row in table.Rows)
//            {
//                if (row.RowState == DataRowState.Deleted)
//                    continue;

//                double x = xMapping(row);
//                double y = yMapping(row);

//                yield return new Point(x, y);
//            }
//        }

//        private void Verify()
//        {
//            // todo create an exception message
//            if (xMapping == null || yMapping == null)
//                throw new InvalidOperationException();
//        }

//        #endregion

//        #region IEnumerable Members

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotSupportedException();
//        }

//        #endregion

//        private Func<DataRow, double> xMapping;
//        public Func<DataRow, double> XMapping
//        {
//            get { return xMapping; }
//            set { SetXMapping(value); }
//        }

//        public void SetXMapping(Func<DataRow, double> xMapping)
//        {
//            if (xMapping == null)
//                throw new ArgumentNullException("xMapping");

//            this.xMapping = xMapping;
//        }

//        private Func<DataRow, double> yMapping;
//        public Func<DataRow, double> YMapping
//        {
//            get { return yMapping; }
//            set { SetYMapping(value); }
//        }

//        public void SetYMapping(Func<DataRow, double> yMapping)
//        {
//            if (yMapping == null)
//                throw new ArgumentNullException("yMapping");

//            this.yMapping = yMapping;
//        }
//    }
//}
