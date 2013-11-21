using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public class PlusMarker : PathMarker
	{
		protected override Geometry CreateGeometry()
		{
			GeometryGroup group = new GeometryGroup();

			group.Children.Add(new LineGeometry(new Point(0, 0.5), new Point(1, 0.5)));
			group.Children.Add(new LineGeometry(new Point(0.5, 0), new Point(0.5, 1)));

			return group;
		}
	}
}
