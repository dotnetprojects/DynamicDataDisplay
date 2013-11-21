//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using Microsoft.Research.DynamicDataDisplay.Charts;
//using System.Collections;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Functional
//{
//    public abstract class FunctionalDataSourceBase : DependencyObject, IPointDataSource
//    {
//        private Func<double, double> function = x => x;
//        protected Func<double, double> FunctionCore
//        {
//            get { return function; }
//            set
//            {
//                function = value;
//                RaiseChanged();
//            }
//        }

//        protected abstract IEnumerable<double> GetXValues();

//        #region IPointDataSource Members

//        protected void RaiseChanged()
//        {
//            Changed.Raise(this);
//        }
//        public event EventHandler Changed;

//        public IEnumerator<Point> GetEnumerator()
//        {
//            var xs = GetXValues();

//            foreach (var x in xs)
//            {
//                yield return new Point(x, function(x));
//            }
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }
//}
