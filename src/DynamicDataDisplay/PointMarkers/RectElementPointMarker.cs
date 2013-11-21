using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    /// <summary>Adds Circle element at every point of graph</summary>
	public class RectElementPointMarker : ShapeElementPointMarker {
        
        public override UIElement CreateMarker()
        {
            Rectangle result = new Rectangle();
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
            Rectangle rect = (Rectangle)marker;

            rect.Width = Size;
            rect.Height = Size;
            rect.Stroke = Brush;
            rect.Fill = Fill;

            if (!String.IsNullOrEmpty(ToolTipText))
            {
                ToolTip tt = new ToolTip();
                tt.Content = ToolTipText;
                rect.ToolTip = tt;
            }
        }

        public override void SetPosition(UIElement marker, Point screenPoint)
        {
            Canvas.SetLeft(marker, screenPoint.X - Size / 2);
            Canvas.SetTop(marker, screenPoint.Y - Size / 2);
        }
	}
}
