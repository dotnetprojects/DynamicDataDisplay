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
    internal sealed class EnumerableYDataSource<T> : EnumerableDataSource<T>
    {
        public EnumerableYDataSource(IEnumerable<T> data) : base(data) { }
    }
}
