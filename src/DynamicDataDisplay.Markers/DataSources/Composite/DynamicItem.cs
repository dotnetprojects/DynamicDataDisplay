using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace DynamicDataDisplay.Markers.DataSources
{
	internal sealed class DynamicItem : CustomTypeDescriptor
	{
		public DynamicItem(PropertyDescriptorCollection descriptors)
		{
			this.descriptors = descriptors;
		}

		private readonly PropertyDescriptorCollection descriptors;
		public override PropertyDescriptorCollection GetProperties()
		{
			return descriptors;
		}
	}
}
