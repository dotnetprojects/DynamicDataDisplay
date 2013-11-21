using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Markers.MarkerGenerators.Rendering;

namespace Microsoft.Research.DynamicDataDisplay.Markers
{
	public class RenderingChartCanvas : FrameworkElement
	{
		public RenderingChartCanvas()
		{
			AddHandler(Plotter.PlotterChangedEvent, new PlotterChangedEventHandler((s, e) =>
			{
				InvalidateVisual();
			}));
		}


		private List<object> data;
		public List<object> Data
		{
			get { return data; }
			set { data = value; }
		}

		private MarkerRenderer renderer;
		public MarkerRenderer Renderer
		{
			get { return renderer; }
			set { renderer = value; }
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var dc = drawingContext;
			dc.DrawRectangle(Brushes.LightGreen, null, new Rect(RenderSize));

			Plotter2D plotter = (Plotter2D)Plotter.GetPlotter(this);
			if (plotter == null) return;
			if (data == null) return;
			if (renderer == null) return;

			CoordinateTransform transform = plotter.Transform.WithRects(new DataRect(0, 0, 1, 1), new Rect(RenderSize));

			foreach (var dataItem in data)
			{
				renderer.DataContext = dataItem;
				renderer.Render(dc, transform);
			}
		}
	}
}
