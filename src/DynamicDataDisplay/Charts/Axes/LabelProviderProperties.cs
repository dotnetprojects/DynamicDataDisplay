using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
	internal class LabelProviderProperties : DependencyObject
	{
		public static bool GetExponentialIsCommonLabel(DependencyObject obj)
		{
			return (bool)obj.GetValue(ExponentialIsCommonLabelProperty);
		}

		public static void SetExponentialIsCommonLabel(DependencyObject obj, bool value)
		{
			obj.SetValue(ExponentialIsCommonLabelProperty, value);
		}

		public static readonly DependencyProperty ExponentialIsCommonLabelProperty = DependencyProperty.RegisterAttached(
		  "ExponentialIsCommonLabel",
		  typeof(bool),
		  typeof(LabelProviderProperties),
		  new FrameworkPropertyMetadata(true));
	}
}
