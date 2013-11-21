using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.MarkupExtensions
{
	public class SelfBinding : Binding
	{
		public SelfBinding()
		{
			RelativeSource = new RelativeSource { Mode = RelativeSourceMode.Self };
		}

		public SelfBinding(string propertyPath)
			: this()
		{
			Path = new PropertyPath(propertyPath);
		}
	}
}
