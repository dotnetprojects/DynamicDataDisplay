using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Markup;

namespace DynamicDataDisplay.Markers
{
	public class EllipseMarker : ShapeMarker
	{
		public EllipseMarker() { }

		protected override Shape CreateShape()
		{
			return new Ellipse();
		}
	}
}
