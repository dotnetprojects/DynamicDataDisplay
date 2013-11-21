using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	public sealed class LinePart
	{
		public IEnumerable<Point> Points { get; set; }
		public double Parameter { get; set; }
	}
}
