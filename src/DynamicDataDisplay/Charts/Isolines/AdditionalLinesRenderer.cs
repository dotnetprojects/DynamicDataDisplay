using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	public class AdditionalLinesRenderer : IsolineRenderer
	{
		protected override void CreateUIRepresentation()
		{
			InvalidateVisual();
		}

		protected override void OnPlotterAttached()
		{
			base.OnPlotterAttached();

			FrameworkElement parent = (FrameworkElement)Parent;
			var renderer = (FrameworkElement)parent.FindName("PART_IsolineRenderer");

			Binding contentBoundsBinding = new Binding { Path = new PropertyPath("(0)", Viewport2D.ContentBoundsProperty), Source = renderer };
			SetBinding(Viewport2D.ContentBoundsProperty, contentBoundsBinding);
			SetBinding(ViewportPanel.ViewportBoundsProperty, contentBoundsBinding);

			Plotter2D.Viewport.EndPanning += Viewport_EndPanning;
			Plotter2D.Viewport.PropertyChanged += Viewport_PropertyChanged;
		}

		void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				if (Plotter2D.Viewport.PanningState == Viewport2DPanningState.NotPanning)
					InvalidateVisual();
			}
		}

		protected override void OnPlotterDetaching()
		{
			Plotter2D.Viewport.EndPanning -= Viewport_EndPanning;
			Plotter2D.Viewport.PropertyChanged -= Viewport_PropertyChanged;

			base.OnPlotterDetaching();
		}

		private void Viewport_EndPanning(object sender, EventArgs e)
		{
			InvalidateVisual();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (Plotter2D == null) return;
			if (DataSource == null) return;

			var collection = (IsolineCollection)Parent.GetValue(IsolineCollectionProperty);
			if (collection == null) return;

			var bounds = ViewportPanel.GetViewportBounds(this);
			if (bounds.IsEmpty) return;

			var dc = drawingContext;
			var strokeThickness = StrokeThickness;

			var transform = Plotter2D.Transform.WithRects(bounds, new Rect(RenderSize));

			//dc.DrawRectangle(null, new Pen(Brushes.Green, 2), new Rect(RenderSize));

			var additionalLevels = GetAdditionalLevels(collection);
			IsolineBuilder.DataSource = DataSource;
			var additionalIsolineCollections = additionalLevels.Select(level =>
			{
				return IsolineBuilder.BuildIsoline(level);
			});

			foreach (var additionalCollection in additionalIsolineCollections)
			{
				RenderIsolineCollection(dc, strokeThickness, additionalCollection, transform);
			}
		}
	}
}
