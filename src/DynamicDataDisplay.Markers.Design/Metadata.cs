using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using DynamicDataDisplay.Markers;

namespace Microsoft.Research.DynamicDataDisplay.Design
{
	internal sealed class Metadata : IRegisterMetadata
	{
		#region IRegisterMetadata Members

		public void Register()
		{
			AttributeTableBuilder tableBuilder = new AttributeTableBuilder();

			tableBuilder.AddCustomAttributes(typeof(PieChart), new FeatureAttribute(typeof(PieChartDesignModeValueProvider)));

			MetadataStore.AddAttributeTable(tableBuilder.CreateTable());
		}

		#endregion
	}
}
