using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Diagnostics;


namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
	/// <summary>Provides common methods of mouse navigation around viewport</summary>
	public class MouseNavigation : NavigationBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MouseNavigation"/> class.
		/// </summary>
		public MouseNavigation() { }

		private AdornerLayer adornerLayer;
		protected AdornerLayer AdornerLayer
		{
			get
			{
				if (adornerLayer == null)
				{
					adornerLayer = AdornerLayer.GetAdornerLayer(this);
					if (adornerLayer != null)
					{
						adornerLayer.IsHitTestVisible = false;
					}
				}

				return adornerLayer;
			}
		}

		protected override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			Mouse.AddMouseDownHandler(Parent, OnMouseDown);
			Mouse.AddMouseMoveHandler(Parent, OnMouseMove);
			Mouse.AddMouseUpHandler(Parent, OnMouseUp);
			Mouse.AddMouseWheelHandler(Parent, OnMouseWheel);

			plotter.KeyDown += new KeyEventHandler(OnParentKeyDown);
		}

		protected override void OnPlotterDetaching(Plotter plotter)
		{
			plotter.KeyDown -= new KeyEventHandler(OnParentKeyDown);

			Mouse.RemoveMouseDownHandler(Parent, OnMouseDown);
			Mouse.RemoveMouseMoveHandler(Parent, OnMouseMove);
			Mouse.RemoveMouseUpHandler(Parent, OnMouseUp);
			Mouse.RemoveMouseWheelHandler(Parent, OnMouseWheel);

			base.OnPlotterDetaching(plotter);
		}

		private void OnParentKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape || e.Key == Key.Back)
			{
				if (isZooming)
				{
					isZooming = false;
					zoomRect = null;
					ReleaseMouseCapture();
					RemoveSelectionAdorner();

					e.Handled = true;
				}
			}
		}

		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!e.Handled)
			{
				Point mousePos = e.GetPosition(this);
				int delta = -e.Delta;
				MouseWheelZoom(mousePos, delta);

				e.Handled = true;
			}
		}

#if DEBUG
		public override string ToString()
		{
			if (!String.IsNullOrEmpty(Name))
			{
				return Name;
			}
			return base.ToString();
		}
#endif

		bool adornerAdded;
		RectangleSelectionAdorner selectionAdorner;
		private void AddSelectionAdorner()
		{
			if (!adornerAdded)
			{
				AdornerLayer layer = AdornerLayer;
				if (layer != null)
				{
					selectionAdorner = new RectangleSelectionAdorner(this) { Border = zoomRect };

					layer.Add(selectionAdorner);
					adornerAdded = true;
				}
			}
		}

		private void RemoveSelectionAdorner()
		{
			AdornerLayer layer = AdornerLayer;
			if (layer != null)
			{
				layer.Remove(selectionAdorner);
				adornerAdded = false;
			}
		}

		private void UpdateSelectionAdorner()
		{
			selectionAdorner.Border = zoomRect;
			selectionAdorner.InvalidateVisual();
		}

		Rect? zoomRect = null;
		private const double wheelZoomSpeed = 1.2;
		private bool shouldKeepRatioWhileZooming;

		private bool isZooming = false;
		protected bool IsZooming
		{
			get { return isZooming; }
		}

		private bool isPanning = false;
		protected bool IsPanning
		{
			get { return isPanning; }
		}

		private Point panningStartPointInViewport;
		protected Point PanningStartPointInViewport
		{
			get { return panningStartPointInViewport; }
		}

		private Point zoomStartPoint;

		private static bool IsShiftOrCtrl
		{
			get
			{
				ModifierKeys currKeys = Keyboard.Modifiers;
				return (currKeys | ModifierKeys.Shift) == currKeys ||
					(currKeys | ModifierKeys.Control) == currKeys;
			}
		}

		protected virtual bool ShouldStartPanning(MouseButtonEventArgs e)
		{
			return e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.None;
		}

		protected virtual bool ShouldStartZoom(MouseButtonEventArgs e)
		{
			return e.ChangedButton == MouseButton.Left && IsShiftOrCtrl;
		}

		Point panningStartPointInScreen;
		protected virtual void StartPanning(MouseButtonEventArgs e)
		{
			panningStartPointInScreen = e.GetPosition(this);
			panningStartPointInViewport = panningStartPointInScreen.ScreenToViewport(Viewport.Transform);

			Plotter2D.UndoProvider.CaptureOldValue(Viewport, Viewport2D.VisibleProperty, Viewport.Visible);

			isPanning = true;

			// not capturing mouse because this made some tools like PointSelector not
			// receive MouseUp events on markers;
			// Mouse will be captured later, in the first MouseMove handler call.
			// CaptureMouse();

			Viewport.PanningState = Viewport2DPanningState.Panning;

			//e.Handled = true;
		}

		protected virtual void StartZoom(MouseButtonEventArgs e)
		{
			zoomStartPoint = e.GetPosition(this);
			if (Viewport.Output.Contains(zoomStartPoint))
			{
				isZooming = true;
				AddSelectionAdorner();
				CaptureMouse();
				shouldKeepRatioWhileZooming = Keyboard.Modifiers == ModifierKeys.Shift;

				e.Handled = true;
			}
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			// dragging
			bool shouldStartDrag = ShouldStartPanning(e);
			if (shouldStartDrag)
				StartPanning(e);

			// zooming
			bool shouldStartZoom = ShouldStartZoom(e);
			if (shouldStartZoom)
				StartZoom(e);

			if (!Plotter.IsFocused)
			{
				//var window = Window.GetWindow(Plotter);
				//var focusWithinWindow = FocusManager.GetFocusedElement(window) != null;

				Plotter.Focus();

				//if (!focusWithinWindow)
				//{

				// this is done to prevent other tools like PointSelector from getting mouse click event when clicking on plotter
				// to activate window it's contained within
				e.Handled = true;

				//}
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (!isPanning && !isZooming) return;

			// dragging
			if (isPanning && e.LeftButton == MouseButtonState.Pressed)
			{
				if (!IsMouseCaptured)
				{
					CaptureMouse();
				}

				Point endPoint = e.GetPosition(this).ScreenToViewport(Viewport.Transform);

				Point loc = Viewport.Visible.Location;
				Vector shift = panningStartPointInViewport - endPoint;
				loc += shift;

				// preventing unnecessary changes, if actually visible hasn't change.
				if (shift.X != 0 || shift.Y != 0)
				{
					Cursor = Cursors.ScrollAll;

					DataRect visible = Viewport.Visible;

					visible.Location = loc;
					Viewport.Visible = visible;
				}

				e.Handled = true;
			}
			// zooming
			else if (isZooming && e.LeftButton == MouseButtonState.Pressed)
			{
				Point zoomEndPoint = e.GetPosition(this);
				UpdateZoomRect(zoomEndPoint);

				e.Handled = true;
			}
		}

		private static bool IsShiftPressed()
		{
			return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
		}

		private void UpdateZoomRect(Point zoomEndPoint)
		{
			Rect output = Viewport.Output;
			Rect tmpZoomRect = new Rect(zoomStartPoint, zoomEndPoint);
			tmpZoomRect = Rect.Intersect(tmpZoomRect, output);

			shouldKeepRatioWhileZooming = IsShiftPressed();
			if (shouldKeepRatioWhileZooming)
			{
				double currZoomRatio = tmpZoomRect.Width / tmpZoomRect.Height;
				double zoomRatio = output.Width / output.Height;
				if (currZoomRatio < zoomRatio)
				{
					double oldHeight = tmpZoomRect.Height;
					double height = tmpZoomRect.Width / zoomRatio;
					tmpZoomRect.Height = height;
					if (!tmpZoomRect.Contains(zoomStartPoint))
					{
						tmpZoomRect.Offset(0, oldHeight - height);
					}
				}
				else
				{
					double oldWidth = tmpZoomRect.Width;
					double width = tmpZoomRect.Height * zoomRatio;
					tmpZoomRect.Width = width;
					if (!tmpZoomRect.Contains(zoomStartPoint))
					{
						tmpZoomRect.Offset(oldWidth - width, 0);
					}
				}
			}

			zoomRect = tmpZoomRect;
			UpdateSelectionAdorner();
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			OnParentMouseUp(e);
		}

		protected virtual void OnParentMouseUp(MouseButtonEventArgs e)
		{
			if (isPanning && e.ChangedButton == MouseButton.Left)
			{
				isPanning = false;
				StopPanning(e);
			}
			else if (isZooming && e.ChangedButton == MouseButton.Left)
			{
				isZooming = false;
				StopZooming();
			}
		}

		protected virtual void StopZooming()
		{
			if (zoomRect.HasValue)
			{
				Rect output = Viewport.Output;

				Point p1 = zoomRect.Value.TopLeft.ScreenToViewport(Viewport.Transform);
				Point p2 = zoomRect.Value.BottomRight.ScreenToViewport(Viewport.Transform);
				DataRect newVisible = new DataRect(p1, p2);
				Viewport.Visible = newVisible;

				zoomRect = null;
				ReleaseMouseCapture();
				RemoveSelectionAdorner();
			}
		}

		protected virtual void StopPanning(MouseButtonEventArgs e)
		{
			Plotter2D.UndoProvider.CaptureNewValue(Plotter2D.Viewport, Viewport2D.VisibleProperty, Viewport.Visible);

			if (!Plotter.IsFocused)
			{
				Plotter2D.Focus();
			}

			Plotter2D.Viewport.PanningState = Viewport2DPanningState.NotPanning;

			ReleaseMouseCapture();
			ClearValue(CursorProperty);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (isZooming)
			{
				RemoveSelectionAdorner();
				isZooming = false;
			}
			if (isPanning)
			{
				Plotter2D.Viewport.PanningState = Viewport2DPanningState.NotPanning;
				isPanning = false;
			}
			ReleaseMouseCapture();
			base.OnLostFocus(e);
		}

		private void MouseWheelZoom(Point mousePos, double wheelRotationDelta)
		{
			Point zoomTo = mousePos.ScreenToViewport(Viewport.Transform);

			double zoomSpeed = Math.Abs(wheelRotationDelta / Mouse.MouseWheelDeltaForOneLine);
			zoomSpeed *= wheelZoomSpeed;
			if (wheelRotationDelta < 0)
			{
				zoomSpeed = 1 / zoomSpeed;
			}
			Viewport.Visible = Viewport.Visible.Zoom(zoomTo, zoomSpeed);
		}
	}
}
