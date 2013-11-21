using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	public sealed class BindMarkerEventArgs
	{
		internal BindMarkerEventArgs() { }

		public FrameworkElement Marker { get; internal set; }
		public object Data { get; internal set; }
	}
}
