using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public class CrossMarker : PathMarker
	{
		protected override Geometry CreateGeometry()
		{
			GeometryGroup group = new GeometryGroup();
			group.Children.Add(new LineGeometry(new Point(0, 0), new Point(1, 1)));
			group.Children.Add(new LineGeometry(new Point(0, 1), new Point(1, 0)));

			return group;
		}
	}
}
