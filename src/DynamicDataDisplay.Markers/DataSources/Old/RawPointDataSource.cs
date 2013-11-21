//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections.Specialized;
//using System.Collections;
//using System.Windows;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
//{
//    public class RawPointDataSource : PointDataSourceBase, INotifyCollectionChanged
//    {
//        private readonly IEnumerable<Point> pointCollection;
//        public IEnumerable<Point> PointCollection
//        {
//            get { return pointCollection; }
//        }

//        internal RawPointDataSource(IEnumerable<Point> pointCollection)
//        {
//            if (pointCollection == null)
//                throw new ArgumentNullException("pointCollection");

//            this.pointCollection = pointCollection;

//            INotifyCollectionChanged observableCollection = pointCollection as INotifyCollectionChanged;
//            if (observableCollection != null)
//                observableCollection.CollectionChanged += observableCollection_CollectionChanged;
//        }

//        private void observableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            CollectionChanged.Raise(this, e);
//        }

//        #region IPointDataSource Members

//        public event EventHandler Changed;

//        public event NotifyCollectionChangedEventHandler CollectionChanged;

//        public IEnumerator<Point> GetEnumerator()
//        {
//            return pointCollection.GetEnumerator();
//        }

//        #endregion

//        #region IEnumerable Members

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotSupportedException();
//        }

//        #endregion
//    }
//}
