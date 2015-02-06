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
    internal sealed class EnumerableXDataSource<T> : EnumerableDataSource<T>
    {
        public EnumerableXDataSource(IEnumerable<T> data) : base(data) { }
    }
}
