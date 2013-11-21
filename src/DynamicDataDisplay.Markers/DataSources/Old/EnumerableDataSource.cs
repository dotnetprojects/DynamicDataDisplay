//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Collections;
//using System.Collections.Specialized;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
//{
//    public class EnumerableDataSource<T> : IPointDataSource, INotifyCollectionChanged
//    {
//        internal EnumerableDataSource(IEnumerable<T> collection)
//        {
//            if (collection == null)
//                throw new ArgumentNullException("collection");

//            this.collection = collection;
//            INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
//            if (observableCollection != null)
//                observableCollection.CollectionChanged += observableCollection_CollectionChanged;
//        }

//        private void observableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            CollectionChanged.Raise(this, e);
//        }

//        private readonly IEnumerable<T> collection;
//        /// <summary>
//        /// Gets the underlying collection.
//        /// </summary>
//        /// <value>The underlying collection.</value>
//        public IEnumerable<T> Collection
//        {
//            get { return collection; }
//        }

//        #region IPointDataSource Members

//        public event EventHandler Changed;

//        public event NotifyCollectionChangedEventHandler CollectionChanged;

//        public IEnumerator<Point> GetEnumerator()
//        {
//            Verify();

//            foreach (var item in collection)
//            {
//                double x = xMapping(item);
//                double y = yMapping(item);

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

//        private Func<T, double> xMapping;
//        private Func<T, double> yMapping;

//        public void SetXMapping(Func<T, double> xMapping)
//        {
//            if (xMapping == null)
//                throw new ArgumentNullException("xMapping");

//            this.xMapping = xMapping;
//        }

//        public void SetYMapping(Func<T, double> yMapping)
//        {
//            if (yMapping == null)
//                throw new ArgumentNullException("yMapping");

//            this.yMapping = yMapping;
//        }

//        #region IEnumerable Members

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotSupportedException();
//        }

//        #endregion
//    }
//}
