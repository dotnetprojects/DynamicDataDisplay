using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	public class EnumerableDataSource<T> : EnumerableDataSourceBase<T>
	{
		public EnumerableDataSource(IEnumerable<T> data) : base(data) { }

		public EnumerableDataSource(IEnumerable data) : base(data) { }

		private readonly List<Mapping<T>> mappings = new List<Mapping<T>>();
		private Func<T, double> xMapping;
		private Func<T, double> yMapping;
		private Func<T, Point> xyMapping;

		public Func<T, double> XMapping
		{
			get { return xMapping; }
			set { SetXMapping(value); }
		}

		public Func<T, double> YMapping
		{
			get { return yMapping; }
			set { SetYMapping(value); }
		}

		public Func<T, Point> XYMapping
		{
			get { return xyMapping; }
			set { SetXYMapping(value); }
		}

		public void SetXMapping(Func<T, double> mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException("mapping");

			this.xMapping = mapping;
			RaiseDataChanged();
		}

		public void SetYMapping(Func<T, double> mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException("mapping");

			this.yMapping = mapping;
			RaiseDataChanged();
		}

		public void SetXYMapping(Func<T, Point> mapping)
		{
			if (mapping == null)
				throw new ArgumentNullException("mapping");

			this.xyMapping = mapping;
			RaiseDataChanged();
		}

		public void AddMapping(DependencyProperty property, Func<T, object> mapping)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			if (mapping == null)
				throw new ArgumentNullException("mapping");

			mappings.Add(new Mapping<T> { Property = property, F = mapping });
		}

		public override IPointEnumerator GetEnumerator(DependencyObject context)
		{
			return new EnumerablePointEnumerator<T>(this);
		}

		internal void FillPoint(T elem, ref Point point)
		{
			if (xyMapping != null)
			{
				point = xyMapping(elem);
			}
			else
			{
				if (xMapping != null)
				{
					point.X = xMapping(elem);
				}
				if (yMapping != null)
				{
					point.Y = yMapping(elem);
				}
			}
		}

		internal void ApplyMappings(DependencyObject target, T elem)
		{
			if (target != null)
			{
				foreach (var mapping in mappings)
				{
					target.SetValue(mapping.Property, mapping.F(elem));
				}
			}
		}
	}
}
