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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
    public static class DataSourceHelper
    {
        public static IEnumerable<Point> GetPoints(IPointDataSource dataSource)
        {
            return GetPoints(dataSource, null);
        }

        public static IEnumerable<Point> GetPoints(IPointDataSource dataSource, DependencyObject context)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            if (context == null)
                context = Context.EmptyContext;

            using (IPointEnumerator enumerator = dataSource.GetEnumerator(context))
            {
                Point p = new Point();
                while (enumerator.MoveNext())
                {
                    enumerator.GetCurrent(ref p);
                    yield return p;
                    p = new Point();
                }
            }
        }
    }
}
