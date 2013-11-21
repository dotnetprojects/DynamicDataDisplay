using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace DynamicDataDisplay.Markers.Samples
{
	class Dictionary1
	{
		void Marker1()
		{
			var OnApplyMarkerTemplate = (e) =>
			{
				var data = (StockInfo)e.Data;
				var templatePart = e.Marker.GetTemplate<Ellipse>();
				templatePart.Width = 10 + data.Value;

				var templatePart2 = e.Marker.GetTemplate<Rectangle>("PART_Rect");
				temlatePart2.Fill = new SolidColorBrush(LoadColorFromValue(e.Data));
			};
		}
	}
}
