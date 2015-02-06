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
    public static class DataSourceExtensions
    {
        public static RawDataSource AsDataSource(this IEnumerable<Point> points)
        {
            return new RawDataSource(points);
        }

        public static EnumerableDataSource<T> AsDataSource<T>(this IEnumerable<T> collection)
        {
            return new EnumerableDataSource<T>(collection);
        }

        public static EnumerableDataSource<T> AsXDataSource<T>(this IEnumerable<T> collection)
        {
            return new EnumerableXDataSource<T>(collection);
        }

        public static EnumerableDataSource<T> AsYDataSource<T>(this IEnumerable<T> collection)
        {
            return new EnumerableYDataSource<T>(collection);
        }

        public static EnumerableDataSource<double> AsXDataSource(this IEnumerable<double> collection)
        {
            EnumerableXDataSource<double> ds = new EnumerableXDataSource<double>(collection);
            ds.SetXMapping(x => x);
            return ds;
        }

        public static EnumerableDataSource<double> AsYDataSource(this IEnumerable<double> collection)
        {
            EnumerableYDataSource<double> ds = new EnumerableYDataSource<double>(collection);
            ds.SetYMapping(y => y);
            return ds;
        }

        public static CompositeDataSource Join(this IPointDataSource ds1, IPointDataSource ds2)
        {
            return new CompositeDataSource(ds1, ds2);
        }

        public static IEnumerable<Point> GetPoints(this IPointDataSource dataSource)
        {
            return DataSourceHelper.GetPoints(dataSource);
        }

        public static IEnumerable<Point> GetPoints(this IPointDataSource dataSource, DependencyObject context)
        {
            return DataSourceHelper.GetPoints(dataSource, context);
        }
    } 
}
