using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Windows.Threading;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	public class FastIsolineRenderer : IsolineRenderer
	{
		private List<IsolineCollection> additionalLines = new List<IsolineCollection>();
		private const int subDivisionNum = 10;

		protected override void CreateUIRepresentation()
		{
			InvalidateVisual();
		}

		protected override void OnPlotterAttached()
		{
			base.OnPlotterAttached();

			FrameworkElement parent = (FrameworkElement)Parent;
			Binding collectionBinding = new Binding("IsolineCollection") { Source = this };
			parent.SetBinding(IsolineCollectionProperty, collectionBinding);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (Plotter2D == null) return;
			if (Collection == null) return;
			if (DataSource == null) return;
			if (Collection.Lines.Count == 0)
			{
				IsolineBuilder.DataSource = DataSource;
				IsolineBuilder.MissingValue = MissingValue;
				Collection = IsolineBuilder.BuildIsoline();
			}

			IsolineCollection = Collection;

			var dc = drawingContext;
			var strokeThickness = StrokeThickness;
			var collection = Collection;

			var bounds = DataRect.Empty;
			// determining content bounds
			foreach (LevelLine line in collection)
			{
				foreach (Point point in line.AllPoints)
				{
					bounds.Union(point);
				}
			}

			Viewport2D.SetContentBounds(this, bounds);
			ViewportPanel.SetViewportBounds(this, bounds);

			if (bounds.IsEmpty) return;

			// custom transform with output set to renderSize of this control
			var transform = Plotter2D.Transform.WithRects(bounds, new Rect(RenderSize));

			// actual drawing of isolines
			RenderIsolineCollection(dc, strokeThickness, collection, transform);

			//var additionalLevels = GetAdditionalIsolines(collection);

			//var additionalIsolineCollections = additionalLevels.Select(level => IsolineBuilder.BuildIsoline(level));

			//foreach (var additionalCollection in additionalIsolineCollections)
			//{
			//    RenderIsolineCollection(dc, strokeThickness, additionalCollection, transform);
			//}

			RenderLabels(dc, collection);

			//    foreach (var additionalCollection in additionalIsolineCollections)
			//    {
			//        RenderLabels(dc, additionalCollection);
			//    }
		}

		private IEnumerable<double> GetAdditionalIsolines(IsolineCollection collection)
		{
			var dataSource = DataSource;
			var visibleMinMax = dataSource.GetMinMax(Plotter2D.Visible);
			var visibleMinMaxRatio = (collection.Max - collection.Min) / visibleMinMax.GetLength();

			var log = Math.Log10(visibleMinMaxRatio);
			if (log > 0.9)
			{
				var upperLog = Math.Ceiling(log);
				var divisionsNum = Math.Pow(10, upperLog);
				var delta = (collection.Max - collection.Min) / divisionsNum;

				var start = Math.Ceiling(visibleMinMax.Min / delta) * delta;

				var x = start;
				while (x < visibleMinMax.Max)
				{
					yield return x;
					x += delta;
				}
			}
		}

		private void RenderLabels(DrawingContext dc, IsolineCollection collection)
		{
			if (Plotter2D == null) return;
			if (collection == null) return;
			if (!DrawLabels) return;

			var viewportBounds = ViewportPanel.GetViewportBounds(this);
			if (viewportBounds.IsEmpty)
				return;

			var strokeThickness = StrokeThickness;
			var visible = Plotter2D.Visible;
			var output = Plotter2D.Viewport.Output;

			var transform = Plotter2D.Transform.WithRects(viewportBounds, new Rect(RenderSize));
			var labelStringFormat = LabelStringFormat;

			// drawing constants
			var labelRectangleFill = Brushes.White;

			var biggerViewport = viewportBounds.ZoomOutFromCenter(1.1);

			// getting and filtering annotations to draw only visible ones
			Annotater.WayBeforeText = Math.Sqrt(visible.Width * visible.Width + visible.Height * visible.Height) / 100 * WayBeforeTextMultiplier;
			var annotations = Annotater.Annotate(collection, visible)
			.Where(annotation =>
			{
				Point viewportPosition = annotation.Position.DataToViewport(transform);
				return biggerViewport.Contains(viewportPosition);
			});

			// drawing annotations
			foreach (var annotation in annotations)
			{
				FormattedText text = CreateFormattedText(annotation.Value.ToString(LabelStringFormat));
				Point position = annotation.Position.DataToScreen(transform);

				var labelTransform = CreateTransform(annotation, text, position);

				// creating rectange stroke
				double colorRatio = (annotation.Value - collection.Min) / (collection.Max - collection.Min);
				colorRatio = MathHelper.Clamp(colorRatio);
				Color rectangleStrokeColor = Palette.GetColor(colorRatio);
				SolidColorBrush rectangleStroke = new SolidColorBrush(rectangleStrokeColor);
				Pen labelRectangleStrokePen = new Pen(rectangleStroke, 2);

				dc.PushTransform(labelTransform);
				{
					var bounds = RectExtensions.FromCenterSize(position, new Size(text.Width, text.Height));
					bounds = bounds.ZoomOutFromCenter(1.3);
					dc.DrawRoundedRectangle(labelRectangleFill, labelRectangleStrokePen, bounds, 8, 8);

					DrawTextInPosition(dc, text, position);
				}
				dc.Pop();
			}
		}

		private static void DrawTextInPosition(DrawingContext dc, FormattedText text, Point position)
		{
			var textPosition = position;
			textPosition.Offset(-text.Width / 2, -text.Height / 2);
			dc.DrawText(text, textPosition);
		}

		private static Transform CreateTransform(IsolineTextLabel isolineLabel, FormattedText text, Point position)
		{
			double angle = isolineLabel.Rotation;
			if (angle < 0)
				angle += 360;
			if (90 < angle && angle < 270)
				angle -= 180;

			RotateTransform transform = new RotateTransform(angle, position.X, position.Y);
			return transform;
		}

		private static FormattedText CreateFormattedText(string text)
		{
			FormattedText result = new FormattedText(text,
				CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black);
			return result;
		}
	}
}
