using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class BindingHelper
	{
		public static Binding CreateAttachedPropertyBinding(DependencyProperty attachedProperty)
		{
			return new Binding { Path = new PropertyPath("(0)", attachedProperty) };
		}
	}
}
