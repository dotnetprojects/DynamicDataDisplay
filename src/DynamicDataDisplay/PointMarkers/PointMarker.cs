using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
	public delegate void MarkerRenderHandler(DrawingContext dc, Point screenPoint);

	/// <summary>Renders markers along graph</summary>
	public abstract class PointMarker : DependencyObject {

		/// <summary>Renders marker on screen</summary>
		/// <param name="dc">Drawing context to render marker on</param>
		/// <param name="dataPoint">Point from data source</param>
		/// <param name="screenPoint">Marker center coordinates on drawing context</param>
		public abstract void Render(DrawingContext dc, Point screenPoint);

		public static implicit operator PointMarker(MarkerRenderHandler renderer) {
            return FromRenderer(renderer);
		}

        public static PointMarker FromRenderer(MarkerRenderHandler renderer)
        {
            return new DelegatePointMarker(renderer);
        }
	}
}
