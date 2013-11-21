using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.VisualStudio.Design
{
	// Container for any general design-time metadata to initialize.
	// Designers look for a type in the design-time assembly that 
	// implements IRegisterMetadata. If found, designers instantiate 
	// this class and call its Register() method automatically.
	internal class Metadata : IRegisterMetadata
	{
		// Called by the designer to register any design-time metadata.
		public void Register()
		{
			//AttributeTableBuilder builder = new AttributeTableBuilder();

			//builder.AddCustomAttributes(
			//    typeof(DraggablePoint),
			//    new FeatureAttribute(typeof(DraggablePointAdornerProvider)));

			//MetadataStore.AddAttributeTable(builder.CreateTable());
		}
	}
}
