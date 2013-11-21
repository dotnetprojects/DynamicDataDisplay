using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Markers;
using System.Windows.Data;
using DynamicDataDisplay.Markers;
using Microsoft.Research.DynamicDataDisplay.Markers.MarkerGenerators.Rendering;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	public class RenderingChart : DevMarkerChart
	{
		private List<object> data = new List<object>();
		private RenderingChartCanvas renderingCanvas = new RenderingChartCanvas();

		public RenderingChart()
		{
			//renderingCanvas.SetBinding(Viewport2D.ContentBoundsProperty, new Binding("(Viewport2D.ContentBounds)") { Source = this });
			ViewportPanel.SetViewportBounds(renderingCanvas, new DataRect(0, 0, 1, 1));
			CurrentItemsPanel.Children.Add(renderingCanvas);
		}

		protected override void OnMarkerGeneratorChanged(MarkerGenerator prevGenerator, MarkerGenerator currGenerator)
		{
			base.OnMarkerGeneratorChanged(prevGenerator, currGenerator);

			CurrentItemsPanel.Children.Add(renderingCanvas);
		}

		protected override void MarkerBuilder_OnChanged(object sender, EventArgs e)
		{
			base.MarkerBuilder_OnChanged(sender, e);

			CurrentItemsPanel.Children.Add(renderingCanvas);
			renderingCanvas.Data = data;
			renderingCanvas.Renderer = (MarkerRenderer)MarkerBuilder.CreateMarker(null);
		}

		protected override void DrawAllMarkers(bool reuseExisting, bool continueAfterDataPrepaired)
		{
			base.DrawAllMarkers(reuseExisting, continueAfterDataPrepaired);

			renderingCanvas.InvalidateVisual();
		}

		protected override void OnItemsPanelChanged()
		{
			base.OnItemsPanelChanged();

			CurrentItemsPanel.Children.Add(renderingCanvas);
		}

		protected override void CreateAndAddMarker(object dataItem, int actualChildIndex)
		{
			data.Add(dataItem);
		}
	}
}
