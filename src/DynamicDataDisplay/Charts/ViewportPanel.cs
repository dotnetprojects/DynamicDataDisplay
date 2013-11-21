using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a panel on which elements are arranged in coordinates in viewport space.
	/// </summary>
	public partial class ViewportPanel : IndividualArrangePanel
	{
		static ViewportPanel()
		{
			Type thisType = typeof(ViewportPanel);
			Plotter.PlotterProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata { PropertyChangedCallback = OnPlotterChanged });
		}

		private static void OnPlotterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ViewportPanel panel = (ViewportPanel)d;
			panel.OnPlotterChanged((Plotter)e.NewValue, (Plotter)e.OldValue);
		}

		private void OnPlotterChanged(Plotter currPlotter, Plotter prevPlotter)
		{
			if (currPlotter != null)
			{
				Plotter2D plotter2d = (Plotter2D)currPlotter;
				viewport = plotter2d.Viewport;
			}
			else
			{
				this.viewport = null;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewportRectPanel"/> class.
		/// </summary>
		public ViewportPanel()
		{
		}

		#region Panel methods override

		protected internal override void OnChildAdded(FrameworkElement child)
		{
			InvalidatePosition(child);
		}

		protected virtual void InvalidatePosition(FrameworkElement child)
		{
			if (viewport == null) return;

			var transform = GetTransform(availableSize);

			Size elementSize = GetElementSize(child, AvailableSize, transform);
			child.Measure(elementSize);

			Rect bounds = GetElementScreenBounds(transform, child);
			if (!bounds.IsNaN())
			{
				child.Arrange(bounds);
			}
		}

		private Size availableSize;
		protected Size AvailableSize
		{
			get { return availableSize; }
			set { availableSize = value; }
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			this.availableSize = availableSize;

			var transform = GetTransform(availableSize);

			foreach (FrameworkElement child in InternalChildren)
			{
				if (child != null)
				{
					Size elementSize = GetElementSize(child, availableSize, transform);
					child.Measure(elementSize);
				}
			}

			if (availableSize.Width.IsInfinite())
				availableSize.Width = 0;
			if (availableSize.Height.IsInfinite())
				availableSize.Height = 0;
			return availableSize;
		}

		protected virtual Size GetElementSize(FrameworkElement child, Size availableSize, CoordinateTransform transform)
		{
			Size res = availableSize;

			DataRect ownViewportBounds = GetViewportBounds(child);
			if (!ownViewportBounds.IsEmpty)
			{
				res = ownViewportBounds.ViewportToScreen(transform).Size;
			}
			else
			{
				double viewportWidth = GetViewportWidth(child);
				double viewportHeight = GetViewportHeight(child);

				bool hasViewportWidth = viewportWidth.IsNotNaN();
				bool hasViewportHeight = viewportHeight.IsNotNaN();

				double minScreenWidth = GetMinScreenWidth(child);
				bool hasMinScreenWidth = minScreenWidth.IsNotNaN();

				double selfWidth = child.Width.IsNotNaN() ? child.Width : availableSize.Width;
				double width = hasViewportWidth ? viewportWidth : selfWidth;

				double selfHeight = child.Height.IsNotNaN() ? child.Height : availableSize.Height;
				double height = hasViewportHeight ? viewportHeight : selfHeight;

				if (width < 0) width = 0;
				if (height < 0) height = 0;

				DataRect bounds = new DataRect(new Size(width, height));
				Rect screenBounds = bounds.ViewportToScreen(transform);

				res = new Size(hasViewportWidth ? screenBounds.Width : selfWidth,
					hasViewportHeight ? screenBounds.Height : selfHeight);

				if (hasMinScreenWidth && res.Width < minScreenWidth)
				{
					res.Width = minScreenWidth;
				}
			}

			if (res.Width.IsNaN()) res.Width = 0;
			if (res.Height.IsNaN()) res.Height = 0;

			return res;
		}

		protected Rect GetElementScreenBounds(CoordinateTransform transform, UIElement child)
		{
			Rect screenBounds = GetElementScreenBoundsCore(transform, child);

			DataRect viewportBounds = screenBounds.ScreenToViewport(transform);
			DataRect prevViewportBounds = GetActualViewportBounds(child);

			SetPrevActualViewportBounds(child, prevViewportBounds);
			SetActualViewportBounds(child, viewportBounds);

			return screenBounds;
		}

		protected virtual Rect GetElementScreenBoundsCore(CoordinateTransform transform, UIElement child)
		{
			Rect bounds = new Rect(0, 0, 1, 1);

			DataRect ownViewportBounds = GetViewportBounds(child);
			if (!ownViewportBounds.IsEmpty)
			{
				bounds = ownViewportBounds.ViewportToScreen(transform);
			}
			else
			{
				double viewportX = GetX(child);
				double viewportY = GetY(child);

				if (viewportX.IsNaN() || viewportY.IsNaN())
				{
					//Debug.WriteLine("ViewportRectPanel: Position is not set!");

					return bounds;
				}

				double viewportWidth = GetViewportWidth(child);
				if (viewportWidth < 0) viewportWidth = 0;
				double viewportHeight = GetViewportHeight(child);
				if (viewportHeight < 0) viewportHeight = 0;

				bool hasViewportWidth = viewportWidth.IsNotNaN();
				bool hasViewportHeight = viewportHeight.IsNotNaN();

				DataRect r = new DataRect(new Size(hasViewportWidth ? viewportWidth : child.DesiredSize.Width,
										   hasViewportHeight ? viewportHeight : child.DesiredSize.Height));
				r = r.ViewportToScreen(transform);

				double screenWidth = hasViewportWidth ? r.Width : child.DesiredSize.Width;
				double screenHeight = hasViewportHeight ? r.Height : child.DesiredSize.Height;

				double minScreenWidth = GetMinScreenWidth(child);
				bool hasMinScreemWidth = minScreenWidth.IsNotNaN();

				if (hasViewportWidth && screenWidth < minScreenWidth)
					screenWidth = minScreenWidth;

				Point location = new Point(viewportX, viewportY).ViewportToScreen(transform);

				double screenX = location.X;
				double screenY = location.Y;

				HorizontalAlignment horizAlignment = GetViewportHorizontalAlignment(child);
				switch (horizAlignment)
				{
					case HorizontalAlignment.Stretch:
					case HorizontalAlignment.Center:
						screenX -= screenWidth / 2;
						break;
					case HorizontalAlignment.Left:
						break;
					case HorizontalAlignment.Right:
						screenX -= screenWidth;
						break;
				}

				VerticalAlignment vertAlignment = GetViewportVerticalAlignment(child);
				switch (vertAlignment)
				{
					case VerticalAlignment.Bottom:
						screenY -= screenHeight;
						break;
					case VerticalAlignment.Center:
					case VerticalAlignment.Stretch:
						screenY -= screenHeight / 2;
						break;
					case VerticalAlignment.Top:
						break;
					default:
						break;
				}

				bounds = new Rect(screenX, screenY, screenWidth, screenHeight);
			}

			// applying screen offset
			double screenOffsetX = GetScreenOffsetX(child);
			if (screenOffsetX.IsNaN()) screenOffsetX = 0;
			double screenOffsetY = GetScreenOffsetY(child);
			if (screenOffsetY.IsNaN()) screenOffsetY = 0;

			Vector screenOffset = new Vector(screenOffsetX, screenOffsetY);
			bounds.Offset(screenOffset);

			return bounds;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var transform = GetTransform(finalSize);

			foreach (UIElement child in InternalChildren)
			{
				if (child != null)
				{
					Rect bounds = GetElementScreenBounds(transform, child);
					if (!bounds.IsNaN())
					{
						child.Arrange(bounds);
					}
				}
			}

			return finalSize;
		}

		private CoordinateTransform GetTransform(Size size)
		{
			return viewport.Transform.WithRects(ViewportPanel.GetViewportBounds(this), new Rect(size));
		}

		#endregion

		private Viewport2D viewport;
	}
}
