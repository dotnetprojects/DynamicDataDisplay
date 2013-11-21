using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
	/// <summary>Provides elements that represent markers along the graph</summary>
	public abstract class ElementPointMarker : DependencyObject {

        /// <summary>Creates marker element at specified point</summary>
        /// <returns>UIElement representing marker</returns>
        public abstract UIElement CreateMarker();

        public abstract void SetMarkerProperties(UIElement marker);

        /// <summary>Moves specified marker so its center is located at specified screen point</summary>
        /// <param name="marker">UIElement created using CreateMarker</param>
        /// <param name="screenPoint">Point to center element around</param>
        public abstract void SetPosition(UIElement marker, Point screenPoint);
	}
}
