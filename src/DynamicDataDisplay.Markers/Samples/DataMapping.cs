using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace DynamicDataDisplay.Markers.Samples
{
	class DataMapping<T>
	{
		public Func<T, object> XMapping;
		public Func<T, object> YMapping;
		public GeneralAxis XAxis { get; set; }
		public GeneralAxis YAxis { get; set; }
	}
}
