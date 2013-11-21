using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using d3 = Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class ViewportHostPanel : ViewportPanel, IPlotterElement
	{
		static ViewportHostPanel()
		{
			EventManager.RegisterClassHandler(typeof(ViewportHostPanel), d3.Plotter.PlotterChangedEvent, new PlotterChangedEventHandler(OnPlotterChanged));
		}

		public ViewportHostPanel()
		{
			this.RenderTransform = translateTransform;

#if false
			// for debug purposes
			Width = 100;
			Height = 100;
			Background = Brushes.PaleGreen.MakeTransparent(0.2);
#endif
		}

		private static void OnPlotterChanged(object sender, PlotterChangedEventArgs e)
		{
			ViewportHostPanel owner = (ViewportHostPanel)sender;

			if (owner.plotter != null && e.PreviousPlotter != null)
			{
				owner.viewport.PropertyChanged -= owner.Viewport_PropertyChanged;
				owner.viewport = null;
				owner.plotter = null;
			}
			if (owner.plotter == null && e.CurrentPlotter != null)
			{
				owner.plotter = (Plotter2D)e.CurrentPlotter;
				owner.viewport = owner.plotter.Viewport;
				owner.viewport.PropertyChanged += owner.Viewport_PropertyChanged;
			}
		}

		TranslateTransform translateTransform = new TranslateTransform();

		private Canvas hostingCanvas = new Canvas();
		internal Canvas HostingCanvas
		{
			get { return hostingCanvas; }
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		protected Plotter2D Plotter
		{
			get { return plotter; }
		}

		Viewport2D viewport;
		public virtual void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			viewport = this.plotter.Viewport;

			if (!IsMarkersHost)
			{
				plotter.CentralGrid.Children.Add(hostingCanvas);
			}
			if (Parent == null)
			{
				hostingCanvas.Children.Add(this);
			}
			this.plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;
		}

		public virtual void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;

			if (!IsMarkersHost)
			{
				plotter.CentralGrid.Children.Remove(hostingCanvas);
			}
			hostingCanvas.Children.Remove(this);

			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			InvalidatePosition((FrameworkElement)child);
		}

		Vector prevVisualOffset;
		bool sizeChanged = true;
		DataRect visibleWhileCreation;
		protected virtual void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				DataRect visible = (DataRect)e.NewValue;
				DataRect prevVisible = (DataRect)e.OldValue;

				if (prevVisible.Size.EqualsApproximately(visible.Size) && !sizeChanged)
				{
					var transform = viewport.Transform;

					Point prevLocation = prevVisible.Location.ViewportToScreen(transform);
					Point location = visible.Location.ViewportToScreen(transform);

					Vector offset = prevLocation - location;
					translateTransform.X += offset.X;
					translateTransform.Y += offset.Y;
				}
				else
				{
					visibleWhileCreation = visible;
					translateTransform.X = 0;
					translateTransform.Y = 0;
					InvalidateMeasure();
				}

				sizeChanged = false;
			}
			else if (e.PropertyName == "Output")
			{
				sizeChanged = true;
				translateTransform.X = 0;
				translateTransform.Y = 0;
				InvalidateMeasure();
			}

			prevVisualOffset = VisualOffset;
		}

		#endregion

		protected internal override void OnChildAdded(FrameworkElement child)
		{
			if (plotter == null) return;

			InvalidatePosition(child);
			//Dispatcher.BeginInvoke(((Action)(()=> InvalidatePosition(child))), DispatcherPriority.ApplicationIdle);
		}

		private DataRect overallViewportBounds = DataRect.Empty;
		internal DataRect OverallViewportBounds
		{
			get { return overallViewportBounds; }
			set { overallViewportBounds = value; }
		}

		private BoundsUnionMode boundsUnionMode;
		internal BoundsUnionMode BoundsUnionMode
		{
			get { return boundsUnionMode; }
			set { boundsUnionMode = value; }
		}

		protected override void InvalidatePosition(FrameworkElement child)
		{
			invalidatePositionCalls++;

			if (viewport == null) return;
			if (child.Visibility != Visibility.Visible)
				return;

			var transform = viewport.Transform.WithScreenOffset(-translateTransform.X, -translateTransform.Y);

			Size elementSize = GetElementSize(child, AvailableSize, transform);
			child.Measure(elementSize);

			Rect bounds = GetElementScreenBounds(transform, child);
			child.Arrange(bounds);

			var viewportBounds = Viewport2D.GetContentBounds(this);
			if (!viewportBounds.IsEmpty)
				overallViewportBounds = viewportBounds;

			UniteWithBounds(transform, bounds);

			if (!InBatchAdd)
			{
				Viewport2D.SetContentBounds(this, overallViewportBounds);
				ContentBoundsChanged.Raise(this);
			}
		}

		private void UniteWithBounds(CoordinateTransform transform, Rect bounds)
		{
			var childViewportBounds = bounds.ScreenToViewport(transform);
			if (boundsUnionMode == BoundsUnionMode.Bounds)
				overallViewportBounds.Union(childViewportBounds);
			else
			{
				overallViewportBounds.Union(childViewportBounds.GetCenter());
			}
		}

		int invalidatePositionCalls = 0;
		internal override void BeginBatchAdd()
		{
			base.BeginBatchAdd();
			invalidatePositionCalls = 0;
		}

		internal override void EndBatchAdd()
		{
			base.EndBatchAdd();

			if (plotter == null) return;

			if (Children.Count > 0 && invalidatePositionCalls == 0)
			{
				var transform = plotter.Viewport.Transform.WithScreenOffset(-translateTransform.X, -translateTransform.Y);
				overallViewportBounds = DataRect.Empty;
				foreach (FrameworkElement child in Children)
				{
					if (child != null)
					{
						if (child.Visibility != Visibility.Visible)
							continue;

						Rect bounds = GetElementScreenBounds(transform, child);
						UniteWithBounds(transform, bounds);
					}
				}
			}

			Viewport2D.SetContentBounds(this, overallViewportBounds);
			ContentBoundsChanged.Raise(this);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (plotter == null)
				return finalSize;

			var transform = plotter.Viewport.Transform.WithScreenOffset(-translateTransform.X, -translateTransform.Y);

			overallViewportBounds = DataRect.Empty;
			foreach (UIElement child in InternalChildren)
			{
				if (child != null)
				{
					if (child.Visibility != Visibility.Visible)
						continue;

					Rect bounds = GetElementScreenBounds(transform, child);
					child.Arrange(bounds);

					UniteWithBounds(transform, bounds);
				}
			}

			if (!InBatchAdd)
			{
				Viewport2D.SetContentBounds(this, overallViewportBounds);
				ContentBoundsChanged.Raise(this);
			}

			return finalSize;
		}

		public event EventHandler ContentBoundsChanged;

		protected override Size MeasureOverride(Size availableSize)
		{
			AvailableSize = availableSize;

			if (plotter == null)
			{
				if (availableSize.Width.IsInfinite() || availableSize.Height.IsInfinite())
					return new Size();
				return availableSize;
			}

			var transform = plotter.Viewport.Transform;

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
	}

	public enum BoundsUnionMode
	{
		Center,
		Bounds
	}
}
