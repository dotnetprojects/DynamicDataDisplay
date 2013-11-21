using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;


namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>Provides common methods of mouse navigation around viewport</summary>
	public class MouseNavigation : NavigationBase {
		private AdornerLayer adornerLayer;
		protected AdornerLayer AdornerLayer {
			get {
				if (adornerLayer == null) {
					adornerLayer = AdornerLayer.GetAdornerLayer(this);
					if (adornerLayer != null) {
						adornerLayer.IsHitTestVisible = false;
					}
				}

				return adornerLayer;
			}
		}

		protected override void OnViewportChanged() {
			base.OnViewportChanged();

			//Mouse.AddPreviewMouseDownHandler(Parent, (MouseButtonEventHandler)OnMouseDown);
			//Mouse.AddPreviewMouseMoveHandler(Parent, (MouseEventHandler)OnMouseMove);
			//Mouse.AddPreviewMouseUpHandler(Parent, (MouseButtonEventHandler)OnMouseUp);

			Mouse.AddMouseDownHandler(Parent, OnMouseDown);
			Mouse.AddMouseMoveHandler(Parent, OnMouseMove);
			Mouse.AddMouseUpHandler(Parent, OnMouseUp);

			Mouse.AddMouseWheelHandler(Parent, OnMouseWheel);
		}

		private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
			if (!e.Handled) {
				Point mousePos = e.GetPosition(this);
				int delta = -e.Delta;
				MouseWheelZoom(mousePos, delta);
			}
		}

#if DEBUG
		public override string ToString()
		{
			if (!String.IsNullOrEmpty(Name)) {
				return Name;
			}
			return base.ToString();
		}
#endif

		bool adornerAdded;
		RectangleSelectionAdorner selectionAdorner;
		private void AddSelectionAdorner() {
			if (!adornerAdded) {
				AdornerLayer layer = AdornerLayer;
				if (layer != null) {
					selectionAdorner = new RectangleSelectionAdorner(this) { Border = zoomRect };

					layer.Add(selectionAdorner);
					adornerAdded = true;
				}
			}
		}

		private void RemoveSelectionAdorner() {
			AdornerLayer layer = AdornerLayer;
			if (layer != null) {
				layer.Remove(selectionAdorner);
				adornerAdded = false;
			}
		}

		private void UpdateSelectionAdorner() {
			selectionAdorner.Border = zoomRect;
			selectionAdorner.InvalidateVisual();
		}

		Rect? zoomRect = null;
		private const double wheelZoomSpeed = 1.2;
		private bool shouldKeepRatioWhileZooming;
		private bool isZooming;
		private bool isDragging;
		private Point dragStartPointInVisible;
		private Point zoomStartPoint;

		private static bool IsShiftOrCtrl {
			get {
				ModifierKeys currKeys = Keyboard.Modifiers;
				return (currKeys | ModifierKeys.Shift) == currKeys ||
					(currKeys | ModifierKeys.Control) == currKeys;
			}
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e) {
			// dragging
			bool shouldStartDrag = e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.None;

			if (shouldStartDrag) {
				Point dragStartPointInOutput = e.GetPosition(this);
				dragStartPointInVisible = dragStartPointInOutput.Transform(Viewport.Output, Viewport.Visible);

				isDragging = true;
				CaptureMouse();
			}

			bool shouldStartZoom = e.ChangedButton == MouseButton.Left && IsShiftOrCtrl;
			if (shouldStartZoom) {
				// zooming

				zoomStartPoint = e.GetPosition(this);
				if (Viewport.Output.Contains(zoomStartPoint)) {
					isZooming = true;
					AddSelectionAdorner();
					CaptureMouse();
					shouldKeepRatioWhileZooming = Keyboard.Modifiers == ModifierKeys.Shift;
				}
			}

			((IInputElement) Parent).Focus();
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			if (!isDragging && !isZooming) return;

			// dragging
			if (isDragging && e.LeftButton == MouseButtonState.Pressed) {
				Point endPoint = e.GetPosition(this).Transform(Viewport.Output, Viewport.Visible);

				Point loc = Viewport.Visible.Location;
				Vector shift = dragStartPointInVisible - endPoint;
				loc += shift;

				if (shift.X != 0 || shift.Y != 0) {
					Rect visible = Viewport.Visible;

					visible.Location = loc;
					Viewport.Visible = visible;
				}
			}
			// zooming
			else if (isZooming && e.LeftButton == MouseButtonState.Pressed) {
				Point zoomEndPoint = e.GetPosition(this);
				UpdateZoomRect(zoomEndPoint);
			}
		}

		private static bool IsShiftPressed() {
			return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
		}

		private void UpdateZoomRect(Point zoomEndPoint) {
			Rect output = Viewport.Output;
			Rect tmpZoomRect = new Rect(zoomStartPoint, zoomEndPoint);
			tmpZoomRect = Rect.Intersect(tmpZoomRect, output);

			shouldKeepRatioWhileZooming = IsShiftPressed();
			if (shouldKeepRatioWhileZooming) {
				double currZoomRatio = tmpZoomRect.Width / tmpZoomRect.Height;
				double zoomRatio = output.Width / output.Height;
				if (currZoomRatio < zoomRatio) {
					double oldHeight = tmpZoomRect.Height;
					double height = tmpZoomRect.Width / zoomRatio;
					tmpZoomRect.Height = height;
					if (!tmpZoomRect.Contains(zoomStartPoint)) {
						tmpZoomRect.Offset(0, oldHeight - height);
					}
				}
				else {
					double oldWidth = tmpZoomRect.Width;
					double width = tmpZoomRect.Height * zoomRatio;
					tmpZoomRect.Width = width;
					if (!tmpZoomRect.Contains(zoomStartPoint)) {
						tmpZoomRect.Offset(oldWidth - width, 0);
					}
				}
			}

			zoomRect = tmpZoomRect;
			UpdateSelectionAdorner();
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e) {
			if (isDragging && e.ChangedButton == MouseButton.Left) {
				isDragging = false;
				ReleaseMouseCapture();
				Point endPoint = e.GetPosition(this).Transform(Viewport.Output, Viewport.Visible);

				// focusing on LMB click
				if (endPoint == dragStartPointInVisible) {
					//Keyboard.Focus(Parent as IInputElement);
				}
			}
			else if (isZooming && e.ChangedButton == MouseButton.Left) {
				isZooming = false;
				if (zoomRect.HasValue) {
					Rect output = Viewport.Output;

					Point p1 = zoomRect.Value.TopLeft.Transform(output, Viewport.Visible);
					Point p2 = zoomRect.Value.BottomRight.Transform(output, Viewport.Visible);
					Rect newVisible = new Rect(p1, p2);
					Viewport.Visible = newVisible;

					zoomRect = null;
					ReleaseMouseCapture();
					RemoveSelectionAdorner();
				}
			}
		}

		protected override void OnLostFocus(RoutedEventArgs e) {
			if (isZooming) {
				RemoveSelectionAdorner();
			}
			ReleaseMouseCapture();
			base.OnLostFocus(e);
		}

		private void MouseWheelZoom(Point mousePos, int wheelRotationDelta) {
			Point zoomTo = mousePos.Transform(Viewport.Output, Viewport.Visible);

			double zoomSpeed = Math.Abs(wheelRotationDelta / Mouse.MouseWheelDeltaForOneLine);
			zoomSpeed *= wheelZoomSpeed;
			if (wheelRotationDelta < 0) {
				zoomSpeed = 1 / zoomSpeed;
			}
			Viewport.Visible = Viewport.Visible.Zoom(zoomTo, zoomSpeed);
		}
	}
}
