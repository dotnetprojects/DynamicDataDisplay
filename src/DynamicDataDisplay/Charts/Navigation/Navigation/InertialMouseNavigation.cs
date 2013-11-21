using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>Adds some inertia to content of 2D viewport to make it more material</summary>
    /// <remarks>This is experimental code</remarks>
	public class InertialMouseNavigation : NavigationBase {
		protected override void OnLoaded(RoutedEventArgs e) {
			adornerLayer = AdornerLayer.GetAdornerLayer(this);
			adornerLayer.IsHitTestVisible = false;
		}

		protected override void OnViewportChanged() {
			base.OnViewportChanged();

			//Mouse.AddPreviewMouseDownHandler(Parent, (MouseButtonEventHandler)OnMouseDown);
			//Mouse.AddPreviewMouseMoveHandler(Parent, (MouseEventHandler)OnMouseMove);
			//Mouse.AddPreviewMouseUpHandler(Parent, (MouseButtonEventHandler)OnMouseUp);

			Mouse.AddMouseDownHandler(Parent, (MouseButtonEventHandler)OnMouseDown);
			Mouse.AddMouseMoveHandler(Parent, (MouseEventHandler)OnMouseMove);
			Mouse.AddMouseUpHandler(Parent, (MouseButtonEventHandler)OnMouseUp);

			Mouse.AddMouseWheelHandler(Parent, (MouseWheelEventHandler)OnMouseWheel);
		}

		protected Rect VisibleRect {
			get { return (Rect)GetValue(VisibleRectProperty); }
			set { SetValue(VisibleRectProperty, value); }
		}

		public static readonly DependencyProperty VisibleRectProperty =
			DependencyProperty.Register(
			  "VisibleRect",
			  typeof(Rect),
			  typeof(InertialMouseNavigation),
			  new FrameworkPropertyMetadata(new Rect(), 
				  OnVisibleRectChanged));

		private static void OnVisibleRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			InertialMouseNavigation nav = (InertialMouseNavigation)d;
			Rect newVisible = (Rect)e.NewValue;
			
			if (nav.shouldApplyAnimation) {
				nav.Viewport.Visible = newVisible;
			}
		}

		protected override void OnVisibleChanged(Rect newRect, Rect oldRect) {
			// todo was uncommented
			// VisibleRect = newRect;
		}

		private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
			if (!e.Handled) {
				Point mousePos = e.GetPosition(this);
				int delta = -e.Delta;
				MouseWheelZoom(mousePos, delta);
			}
		}

		bool adornerAdded;
		RectangleSelectionAdorner selectionAdorner;
		AdornerLayer adornerLayer;
		private void AddSelectionAdorner() {
			if (!adornerAdded) {
				selectionAdorner = new RectangleSelectionAdorner(this);
				selectionAdorner.Border = zoomRect;
				adornerLayer.Add(selectionAdorner);
			}
			adornerAdded = true;
		}

		private void RemoveSelectionAdorner() {
			adornerLayer.Remove(selectionAdorner);
			Debug.Assert(adornerAdded);
			adornerAdded = false;
		}

		private void UpdateSelectionAdorner() {
			selectionAdorner.Border = zoomRect;
			selectionAdorner.InvalidateVisual();
		}

		Rect? zoomRect = null;
		double wheelZoomSpeed = 1.2;
		private bool shouldKeepRatioWhileZooming;
		private bool isZooming = false;
		private bool isDragging = false;
		private Point dragStartPointInVisible;
		private Point zoomStartPoint;
		private Point dragStartPointInOutput;

		private static bool IsShiftOrCtrl {
			get {
				ModifierKeys currKeys = Keyboard.Modifiers;
				return (currKeys | ModifierKeys.Shift) == currKeys ||
					(currKeys | ModifierKeys.Control) == currKeys;
			}
		}

		private int dragStartTimestamp;
		private void OnMouseDown(object sender, MouseButtonEventArgs e) {
			// dragging
			bool shouldStartDrag = e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.None;

			if (shouldStartDrag) {
				dragStartPointInOutput = e.GetPosition(this);
				dragStartPointInVisible = dragStartPointInOutput.Transform(Viewport.Output, Viewport.Visible);	

				dragStartTimestamp = e.Timestamp;

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

			(Parent as IInputElement).Focus();
		}

		private void OnMouseMove(object sender, MouseEventArgs e) {
			if (!isDragging && !isZooming) return;
			// dragging
			if (isDragging && e.LeftButton == MouseButtonState.Pressed) {


				Point endPoint = e.GetPosition(this).Transform(Viewport.Output, Viewport.Visible);

				Point loc = Viewport.Visible.Location;
				Vector shift = dragStartPointInVisible - endPoint;
				loc += shift;

				//Debug.WriteLine("Shift = " + shift);

				if (shift.X != 0 || shift.Y != 0) {
					StopDragAnimation();
					Rect visible = Viewport.Visible;

					visible.Location = loc;
					Viewport.Visible = visible;
					if (Viewport.Visible != visible) { }
				}
				else {

				}
			}
			// zooming
			else if (isZooming && e.LeftButton == MouseButtonState.Pressed) {
				Point zoomEndPoint = e.GetPosition(this);
				UpdateZoomRect(zoomEndPoint);
			}
		}

		private bool shouldApplyAnimation = true;
		private void StopDragAnimation() {
			shouldApplyAnimation = false;
			if (dragAnimation != null && HasAnimatedProperties) {
				Debug.WriteLine("Stopping animation");
				//BeginAnimation(VisibleRectProperty, null, HandoffBehavior.SnapshotAndReplace);
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

		RectAnimation dragAnimation;
		private void OnMouseUp(object sender, MouseButtonEventArgs e) {
			if (isDragging && e.ChangedButton == MouseButton.Left) {
				isDragging = false;
				ReleaseMouseCapture();
				Point endPointInOutput = e.GetPosition(this);
				Point endPointInVisible = endPointInOutput.Transform(Viewport.Output, Viewport.Visible);

				Vector mouseShift = -(endPointInVisible - dragStartPointInVisible);
				Vector outputShift = endPointInOutput - dragStartPointInOutput;

				int time = e.Timestamp;
				int deltaTime = time - dragStartTimestamp;

				double seconds = deltaTime / 1000.0;

				// mouse moved short enough and for rather long distance.
				bool shouldStartAnimation = seconds < 1 && outputShift.Length > 20;
				if (shouldStartAnimation) {
					Rect moveTo = Viewport.Visible;
					moveTo.Offset(mouseShift * 2);

					//if (dragAnimation == null) {
					dragAnimation = new RectAnimation(moveTo,
						TimeSpan.FromSeconds(1));
					dragAnimation.DecelerationRatio = 0.2;
					//}
					//else {
					//    dragAnimation.To = moveTo;
					//    dragAnimation.Duration = TimeSpan.FromSeconds(1);
					//    AnimationClock clock = dragAnimation.CreateClock();
					//    clock.Controller.Begin();
					//}

					BeginAnimation(VisibleRectProperty, dragAnimation);
					shouldApplyAnimation = true;
					Debug.WriteLine("Starting animation");
				}
				else if (dragAnimation != null) {
					StopDragAnimation();
				}

				// focusing on LMB click
				if (endPointInVisible == dragStartPointInVisible) {
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

        /*
		class MyRectAnimation : RectAnimationBase {
			public MyRectAnimation() {
				Duration = new Duration(TimeSpan.FromSeconds(2));
			}

			protected override Rect GetCurrentValueCore(Rect defaultOriginValue, Rect defaultDestinationValue, AnimationClock animationClock) {
				Debug.WriteLine("In animation");
				return new Rect();
			}

			protected override Freezable CreateInstanceCore() {
				return new MyRectAnimation();
			}
		}
         */

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

