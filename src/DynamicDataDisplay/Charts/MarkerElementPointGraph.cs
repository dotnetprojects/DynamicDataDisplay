using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay
{
    public class ElementMarkerPointsGraph : PointsGraphBase
    {
        /// <summary>List with created but unused markers</summary>
        private readonly List<UIElement> unused = new List<UIElement>();

        /// <summary>Initializes a new instance of the <see cref="MarkerPointsGraph"/> class.</summary>
        public ElementMarkerPointsGraph()
        {
            ManualTranslate = true; // We'll handle translation by ourselves
        }

        /// <summary>Initializes a new instance of the <see cref="MarkerPointsGraph"/> class.</summary>
        /// <param name="dataSource">The data source.</param>
        public ElementMarkerPointsGraph(IPointDataSource dataSource)
            : this()
        {
            DataSource = dataSource;
        }

        Grid grid;
        Canvas canvas;

        protected override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);

            grid = new Grid();
            canvas = new Canvas { ClipToBounds = true };
            grid.Children.Add(canvas);

            Plotter2D.CentralGrid.Children.Add(grid);
        }

        protected override void OnPlotterDetaching(Plotter plotter)
        {
            Plotter2D.CentralGrid.Children.Remove(grid);
            grid = null;
            canvas = null;

            base.OnPlotterDetaching(plotter);
        }

        protected override void OnDataChanged()
        {
            //			if (canvas != null)
            //			{
            //                foreach(UIElement child in canvas.Children)
            //                    unused.Add(child);
            //				canvas.Children.Clear();
            //			}
            // todo почему так?
            base.OnDataChanged();
        }

        public ElementPointMarker Marker
        {
            get { return (ElementPointMarker)GetValue(MarkerProperty); }
            set { SetValue(MarkerProperty, value); }
        }

        public static readonly DependencyProperty MarkerProperty =
            DependencyProperty.Register(
              "Marker",
              typeof(ElementPointMarker),
              typeof(ElementMarkerPointsGraph),
              new FrameworkPropertyMetadata { DefaultValue = null, AffectsRender = true }
                  );

        protected override void OnRenderCore(DrawingContext dc, RenderState state)
        {
            if (Marker == null)
                return;

            if (DataSource == null) // No data is specified
            {
                if (canvas != null)
                {
                    foreach (UIElement child in canvas.Children)
                        unused.Add(child);
                    canvas.Children.Clear();
                }
            }
            else // There is some data
            {
          
                int index = 0;
                var transform = GetTransform();
                using (IPointEnumerator enumerator = DataSource.GetEnumerator(GetContext()))
                {
                    Point point = new Point();

					DataRect bounds = DataRect.Empty;

                    while (enumerator.MoveNext())
                    {
                        enumerator.GetCurrent(ref point);
                        enumerator.ApplyMappings(Marker);

                        if (index >= canvas.Children.Count)
                        {
                            UIElement newMarker;
                            if (unused.Count > 0)
                            {
                                newMarker = unused[unused.Count - 1];
                                unused.RemoveAt(unused.Count - 1);
                            }
                            else
                                newMarker = Marker.CreateMarker();
                            canvas.Children.Add(newMarker);
                        }

                        Marker.SetMarkerProperties(canvas.Children[index]);
						bounds.Union(point);
                        Point screenPoint = point.DataToScreen(transform);
                        Marker.SetPosition(canvas.Children[index], screenPoint);
                        index++;
                    }

					Viewport2D.SetContentBounds(this, bounds);

                    while (index < canvas.Children.Count)
                    {
                        unused.Add(canvas.Children[index]);
                        canvas.Children.RemoveAt(index);
                    }
                }
            }
        }
    }
}