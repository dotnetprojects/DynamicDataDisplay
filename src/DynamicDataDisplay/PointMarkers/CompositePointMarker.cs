using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    /// <summary>Composite point markers renders a specified set of markers
    /// at every point of graph</summary>
	public sealed class CompositePointMarker : PointMarker {
		public CompositePointMarker() { }

		public CompositePointMarker(params PointMarker[] markers) {
			if (markers == null)
				throw new ArgumentNullException("markers");

            foreach (PointMarker m in markers)
                this.markers.Add(m);
		}

		public CompositePointMarker(IEnumerable<PointMarker> markers) {
			if (markers == null)
				throw new ArgumentNullException("markers");
            foreach (PointMarker m in markers)
                this.markers.Add(m);
		}


		private readonly Collection<PointMarker> markers = new Collection<PointMarker>();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Collection<PointMarker> Markers {
			get { return markers; }
		}

		public override void Render(DrawingContext dc, Point screenPoint) {
			LocalValueEnumerator enumerator = GetLocalValueEnumerator();
			foreach (var marker in markers) {
				enumerator.Reset();
				while (enumerator.MoveNext()) {
					marker.SetValue(enumerator.Current.Property, enumerator.Current.Value);
				}

				marker.Render(dc, screenPoint);
			}
		}
	}
}
