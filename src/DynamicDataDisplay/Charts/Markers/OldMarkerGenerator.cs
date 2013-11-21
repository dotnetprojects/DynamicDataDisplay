using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	public abstract class OldMarkerGenerator : DependencyObject
	{
		public FrameworkElement CreateMarker(object dataItem)
		{
			return CreateMarkerCore(dataItem);
		}

		protected abstract FrameworkElement CreateMarkerCore(object dataItem);

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
		public event EventHandler Changed;

		public virtual void ReleaseMarker(FrameworkElement element) { }
	}
}
