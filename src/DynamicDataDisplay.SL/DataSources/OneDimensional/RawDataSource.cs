using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
    public sealed class RawDataSource : EnumerableDataSourceBase<Point>
    {
        public RawDataSource(params Point[] data) : base(data) { }
        public RawDataSource(IEnumerable<Point> data) : base(data) { }

        public override IPointEnumerator GetEnumerator(DependencyObject context)
        {
            return new RawPointEnumerator(this);
        }
    }
}
