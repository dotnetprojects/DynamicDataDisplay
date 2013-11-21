using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class CompositeDataSource : PointDataSourceBase
	{
		public CompositeDataSource(DataSourcePartCollection parts)
		{
			if (parts == null)
				throw new ArgumentNullException("parts");

			this.parts = parts;
			TrySubscribeOnCollectionChanged(parts);
		}

		public void ReplacePart(string partName, DataSourcePart part)
		{
			parts.ReplacePart(partName, part);
		}

		public void ReplacePart(string partName, IEnumerable collection)
		{
			parts.ReplacePart(partName, new DataSourcePart(collection, partName));
		}

		private readonly DataSourcePartCollection parts = new DataSourcePartCollection();
		public DataSourcePartCollection Parts
		{
			get { return parts; }
		}

		protected override IEnumerable GetDataCore()
		{
			return new CompositeEnumerable(parts);
		}

		public override IEnumerable GetData(int startingIndex)
		{
			throw new NotImplementedException();
		}

		public override object GetDataType()
		{
			return new DynamicItem(CompositeEnumerator.CreatePropertyDescriptors(parts));
		}
	}
}
