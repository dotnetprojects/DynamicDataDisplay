using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers
{
	public abstract class PathMarker : ShapeMarker
	{
		protected sealed override Shape CreateShape()
		{
			Path path = new Path();
			path.Data = CreateGeometry();
			return path;
		}

		protected abstract Geometry CreateGeometry();
	}
}
