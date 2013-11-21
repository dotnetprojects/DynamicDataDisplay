using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.MarkupExtensions
{
	public class TemplateBinding : Binding
	{
		public TemplateBinding()
		{
			RelativeSource = new RelativeSource { Mode = RelativeSourceMode.TemplatedParent };
		}

		public TemplateBinding(string propertyPath)
			: this()
		{
			Path = new System.Windows.PropertyPath(propertyPath);
		}
	}
}
