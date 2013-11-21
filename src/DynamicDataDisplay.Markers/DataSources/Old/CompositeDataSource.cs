//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Collections;
//using System.Collections.Specialized;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
//{
//    public class CompositeDataSource<TFirst, TSecond> : IPointDataSource, INotifyCollectionChanged
//    {
//        private readonly IEnumerable<TFirst> xCollection;
//        private readonly IEnumerable<TSecond> yCollection;

//        // todo create get accessors for collections
//        internal CompositeDataSource(IEnumerable<TFirst> xCollection, IEnumerable<TSecond> yCollection)
//        {
//            if (xCollection == null)
//                throw new ArgumentNullException("xCollection");
//            if (yCollection == null)
//                throw new ArgumentNullException("yCollection");

//            this.xCollection = xCollection;
//            this.yCollection = yCollection;
//        }

//        private Func<TFirst, double> xMapping;
//        public Func<TFirst, double> XMapping
//        {
//            get { return xMapping; }
//            set { SetXMapping(value); }
//        }

//        public void SetXMapping(Func<TFirst, double> xMapping)
//        {
//            if (xMapping == null)
//                throw new ArgumentNullException("xMapping");

//            this.xMapping = xMapping;
//        }

//        private Func<TSecond, double> yMapping;
//        public Func<TSecond, double> YMapping
//        {
//            get { return yMapping; }
//            set { SetYMapping(value); }
//        }

//        public void SetYMapping(Func<TSecond, double> yMapping)
//        {
//            if (yMapping == null)
//                throw new ArgumentNullException("yMapping");

//            this.yMapping = yMapping;
//        }

//        #region IPointDataSource Members

//        public event EventHandler Changed;

//        public IEnumerator<Point> GetEnumerator()
//        {
//            Verify();

//            var xEnumerator = xCollection.GetEnumerator();
//            var yEnumerator = yCollection.GetEnumerator();

//            while (xEnumerator.MoveNext() && yEnumerator.MoveNext())
//            {
//                double x = xMapping(xEnumerator.Current);
//                double y = yMapping(yEnumerator.Current);

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

//        #region INotifyCollectionChanged Members

//        // todo check if inner collections are INotifyCollectionChanged and raise this event when appropriate
//        public event NotifyCollectionChangedEventHandler CollectionChanged;

//        #endregion
//    }
//}
