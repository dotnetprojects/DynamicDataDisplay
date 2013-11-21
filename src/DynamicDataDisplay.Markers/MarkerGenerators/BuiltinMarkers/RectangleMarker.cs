using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace DynamicDataDisplay.Markers
{
	public class RectangleMarker : ShapeMarker
	{
		protected override Shape CreateShape()
		{
			return new Rectangle();
		}
	}
}
