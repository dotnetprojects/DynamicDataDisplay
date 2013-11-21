using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    /// <summary>Adds Circle element at every point of graph</summary>
	public class CircleElementPointMarker : ShapeElementPointMarker {
        
        public override UIElement CreateMarker()
        {
            Ellipse result = new Ellipse();
            result.Width = Size;
            result.Height = Size;
            result.Stroke = Brush;
			result.Fill = Fill;
            if (!String.IsNullOrEmpty(ToolTipText))
            {
                ToolTip tt = new ToolTip();
                tt.Content = ToolTipText;
                result.ToolTip = tt;
            }
            return result;
        }

        public override void SetMarkerProperties(UIElement marker)
        {
            Ellipse ellipse = (Ellipse)marker;

            ellipse.Width = Size;
            ellipse.Height = Size;
            ellipse.Stroke = Brush;
            ellipse.Fill = Fill;

            if (!String.IsNullOrEmpty(ToolTipText))
            {
                ToolTip tt = new ToolTip();
                tt.Content = ToolTipText;
                ellipse.ToolTip = tt;
            }
        }

        public override void SetPosition(UIElement marker, Point screenPoint)
        {
            Canvas.SetLeft(marker, screenPoint.X - Size / 2);
            Canvas.SetTop(marker, screenPoint.Y - Size / 2);
        }
	}
}
