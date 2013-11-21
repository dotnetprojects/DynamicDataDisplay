using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Collections;
using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources
{
	public sealed class PointArrayDataSource : PointDataSourceBase
	{
		public PointArrayDataSource(Point[] collection)
		{
			if (collection == null)
				throw new ArgumentNullException("data");

			this.collection = collection;
		}

		private readonly Point[] collection;
		public Point[] Collection
		{
			get { return collection; }
		} 

		protected override IEnumerable GetDataCore()
		{
			return Filters.Filter(collection, Environment);
		}

		public override IEnumerable GetData(int startingIndex)
		{
			return collection.Skip(startingIndex);
		}

		public override object GetDataType()
		{
			return typeof(Point);
		}

		protected override void OnEnvironmentChanged()
		{
			base.OnEnvironmentChanged();
		}
	}
}
