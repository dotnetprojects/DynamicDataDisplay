using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Draws isolines on given two-dimensional scalar data.
	/// </summary>
	public sealed class IsolineGraph : IsolineRenderer
	{
        private static Brush labelBackground = new SolidColorBrush(Color.FromArgb(130, 255, 255, 255));

		/// <summary>
		/// Initializes a new instance of the <see cref="IsolineGraph"/> class.
		/// </summary>
		public IsolineGraph()
		{
			Content = content;
			Viewport2D.SetIsContentBoundsHost(this, true);
		}

		protected override void OnPlotterAttached()
		{
			CreateUIRepresentation();
			UpdateUIRepresentation();
		}

		private readonly Canvas content = new Canvas();

		protected override void UpdateDataSource()
		{
			base.UpdateDataSource();

			CreateUIRepresentation();
			rebuildText = true;
			UpdateUIRepresentation();
		}

		protected override void OnLineThicknessChanged()
		{
			foreach (var path in linePaths)
			{
				path.StrokeThickness = StrokeThickness;
			}
		}

        private List<FrameworkElement> textBlocks = new List<FrameworkElement>();
		private List<Path> linePaths = new List<Path>();
		protected override void CreateUIRepresentation()
		{
			if (Plotter2D == null)
				return;

			content.Children.Clear();
			linePaths.Clear();

			if (Collection != null)
			{
				DataRect bounds = DataRect.Empty;

				foreach (var line in Collection.Lines)
				{
					foreach (var point in line.AllPoints)
					{
						bounds.Union(point);
					}

					Path path = new Path
					{
						Stroke = new SolidColorBrush(Palette.GetColor(line.Value01)),
						StrokeThickness = StrokeThickness,
						Data = CreateGeometry(line),
						Tag = line
					};
					content.Children.Add(path);
					linePaths.Add(path);
				}

				Viewport2D.SetContentBounds(this, bounds);

				if (DrawLabels)
				{
					var transform = Plotter2D.Viewport.Transform;
					double wayBeforeText = new Rect(new Size(2000, 2000)).ScreenToData(transform).Width;
					Annotater.WayBeforeText = wayBeforeText;
					var textLabels = Annotater.Annotate(Collection, Plotter2D.Viewport.Visible);

					foreach (var textLabel in textLabels)
					{
						var text = CreateTextLabel(textLabel);
						content.Children.Add(text);
						textBlocks.Add(text);
					}
				}
			}
		}

		private FrameworkElement CreateTextLabel(IsolineTextLabel textLabel)
		{
			var transform = Plotter2D.Viewport.Transform;
			Point screenPos = textLabel.Position.DataToScreen(transform);

			double angle = textLabel.Rotation;
			if (angle < 0)
				angle += 360;
			if (135 < angle && angle < 225)
				angle -= 180;

            string tooltip = textLabel.Value.ToString("F"); //String.Format("{0} at ({1}, {2})", textLabel.Text, textLabel.Position.X, textLabel.Position.Y);
            Grid grid = new Grid
            {
                RenderTransform = new RotateTransform(angle),
                Tag = textLabel,
                RenderTransformOrigin = new Point(0.5, 0.5),
                ToolTip = tooltip
            };

            TextBlock res = new TextBlock
            {
                Text = textLabel.Value.ToString("F"),
                Margin = new Thickness(3,1,3,1)
            };

            //res.Measure(SizeHelper.CreateInfiniteSize());

            Rectangle rect = new Rectangle
            {
                Stroke = Brushes.Gray,
                Fill = labelBackground,
                RadiusX = 8,
                RadiusY = 8
            };

            grid.Children.Add(rect);
            grid.Children.Add(res);

			grid.Measure(SizeHelper.CreateInfiniteSize());

            Size textSize = grid.DesiredSize;
			Point position = new Point(screenPos.X - textSize.Width / 2, screenPos.Y - textSize.Height / 2);

            Canvas.SetLeft(grid, position.X);
            Canvas.SetTop(grid, position.Y);
            return grid;
		}

		private Geometry CreateGeometry(LevelLine lineData)
		{
			var transform = Plotter2D.Viewport.Transform;

			StreamGeometry geometry = new StreamGeometry();
			using (var context = geometry.Open())
			{
				context.BeginFigure(lineData.StartPoint.DataToScreen(transform), false, false);
				context.PolyLineTo(lineData.OtherPoints.DataToScreenAsList(transform), true, true);
			}
			geometry.Freeze();
			return geometry;
		}

		private bool rebuildText = true;
		protected override void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible" || e.PropertyName == "Output")
			{
				bool isVisibleChanged = e.PropertyName == "Visible";
				DataRect prevRect = isVisibleChanged ? (DataRect)e.OldValue : new DataRect((Rect)e.OldValue);
				DataRect currRect = isVisibleChanged ? (DataRect)e.NewValue : new DataRect((Rect)e.NewValue);

				// completely rebuild text only if width and height have changed many
				const double smallChangePercent = 0.05;
				bool widthChangedLittle = Math.Abs(currRect.Width - prevRect.Width) / currRect.Width < smallChangePercent;
				bool heightChangedLittle = Math.Abs(currRect.Height - prevRect.Height) / currRect.Height < smallChangePercent;

				rebuildText = !(widthChangedLittle && heightChangedLittle);
			}
			UpdateUIRepresentation();
		}

		private void UpdateUIRepresentation()
		{
			if (Plotter2D == null) return;

			foreach (var path in linePaths)
			{
				LevelLine line = (LevelLine)path.Tag;
				path.Data = CreateGeometry(line);
			}

			var transform = Plotter2D.Viewport.Transform;
			Rect output = Plotter2D.Viewport.Output;
			DataRect visible = Plotter2D.Viewport.Visible;

			if (rebuildText && DrawLabels)
			{
				rebuildText = false;
				foreach (var text in textBlocks)
				{
					if (text.Visibility == Visibility.Visible)
						content.Children.Remove(text);
				}
				textBlocks.Clear();

				double wayBeforeText = new Rect(new Size(100, 100)).ScreenToData(transform).Width;
				Annotater.WayBeforeText = wayBeforeText;
				var textLabels = Annotater.Annotate(Collection, Plotter2D.Viewport.Visible);
				foreach (var textLabel in textLabels)
				{
					var text = CreateTextLabel(textLabel);
					textBlocks.Add(text);
					if (visible.Contains(textLabel.Position))
					{
						content.Children.Add(text);
					}
					else
					{
						text.Visibility = Visibility.Hidden;
					}
				}
			}
			else
			{
				foreach (var text in textBlocks)
				{
					IsolineTextLabel label = (IsolineTextLabel)text.Tag;
					Point screenPos = label.Position.DataToScreen(transform);
					Size textSize = text.DesiredSize;

					Point position = new Point(screenPos.X - textSize.Width / 2, screenPos.Y - textSize.Height / 2);

					if (output.Contains(position))
					{
						Canvas.SetLeft(text, position.X);
						Canvas.SetTop(text, position.Y);
						if (text.Visibility == Visibility.Hidden)
						{
							text.Visibility = Visibility.Visible;
							content.Children.Add(text);
						}
					}
					else if (text.Visibility == Visibility.Visible)
					{
						text.Visibility = Visibility.Hidden;
						content.Children.Remove(text);
					}
				}
			}
		}
	}
}
