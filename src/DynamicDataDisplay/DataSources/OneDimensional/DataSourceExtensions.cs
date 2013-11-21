using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
			if (typeof(T) == typeof(double))
			{
				return ((IEnumerable<double>)collection).AsXDataSource() as EnumerableDataSource<T>;
			}
			else if (typeof(T) == typeof(float))
			{
				return ((IEnumerable<float>)collection).AsXDataSource() as EnumerableDataSource<T>;
			}

			return new EnumerableXDataSource<T>(collection);
		}

		public static EnumerableDataSource<float> AsXDataSource(this IEnumerable<float> collection)
		{
			EnumerableXDataSource<float> ds = new EnumerableXDataSource<float>(collection);
			ds.SetXMapping(f => (double)f);
			return ds;
		}

		public static EnumerableDataSource<T> AsYDataSource<T>(this IEnumerable<T> collection)
		{
			if (typeof(T) == typeof(double))
			{
				return ((IEnumerable<double>)collection).AsYDataSource() as EnumerableDataSource<T>;
			}
			else if (typeof(T) == typeof(float))
			{
				return ((IEnumerable<float>)collection).AsYDataSource() as EnumerableDataSource<T>;
			}

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

		public static EnumerableDataSource<float> AsYDataSource(this IEnumerable<float> collection)
		{
			EnumerableYDataSource<float> ds = new EnumerableYDataSource<float>(collection);
			ds.SetYMapping(f => (double)f);
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
