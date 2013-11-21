using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace Microsoft.Research.DynamicDataDisplay
{
    public class MarkerPointsGraph : PointsGraphBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerPointsGraph"/> class.
        /// </summary>
        public MarkerPointsGraph() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerPointsGraph"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public MarkerPointsGraph(IPointDataSource dataSource)
        {
            DataSource = dataSource;
        }

        public PointMarker Marker
        {
            get { return (PointMarker)GetValue(MarkerProperty); }
            set { SetValue(MarkerProperty, value); }
        }

        public static readonly DependencyProperty MarkerProperty =
            DependencyProperty.Register(
              "Marker",
              typeof(PointMarker),
              typeof(MarkerPointsGraph),
              new FrameworkPropertyMetadata { DefaultValue = null, AffectsRender = true }
                  );

        protected override void OnRenderCore(DrawingContext dc, RenderState state)
        {
            if (DataSource == null) return;
            if (Marker == null) return;

            Rect bounds = Rect.Empty;
            using (IPointEnumerator enumerator = DataSource.GetEnumerator(GetContext()))
            {
                Point point = new Point();
                while (enumerator.MoveNext())
                {
                    enumerator.GetCurrent(ref point);
                    enumerator.ApplyMappings(Marker);

                    Point screenPoint = point.Transform(state.Visible, state.Output);

                    bounds = Rect.Union(bounds, point);
                    Marker.Render(dc, screenPoint);
                }
            }

            ContentBounds = bounds;
        }
    }
}
