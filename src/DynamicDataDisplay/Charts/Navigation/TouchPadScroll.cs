using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;


namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>This class allows convenient navigation around viewport using touchpad on
    /// some notebooks</summary>
	public sealed class TouchpadScroll : NavigationBase {
		public TouchpadScroll() {
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			WindowInteropHelper helper = new WindowInteropHelper(Window.GetWindow(this));
			HwndSource source = HwndSource.FromHwnd(helper.Handle);
			source.AddHook(OnMessageAppeared);
		}

		private IntPtr OnMessageAppeared(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if (msg == WindowsMessages.WM_MOUSEWHEEL) {
				Point mousePos = MessagesHelper.GetMousePosFromLParam(lParam);
				mousePos = TranslatePoint(mousePos, this);

				if (Viewport.Output.Contains(mousePos)) {
					MouseWheelZoom(MessagesHelper.GetMousePosFromLParam(lParam), MessagesHelper.GetWheelDataFromWParam(wParam));
					handled = true;
				}
			}
			return IntPtr.Zero;
		}

		double wheelZoomSpeed = 1.2;
		public double WheelZoomSpeed {
			get { return wheelZoomSpeed; }
			set { wheelZoomSpeed = value; }
		}

		private void MouseWheelZoom(Point mousePos, int wheelRotationDelta) {
			Point zoomTo = mousePos.ScreenToData(Viewport.Transform);

			double zoomSpeed = Math.Abs(wheelRotationDelta / Mouse.MouseWheelDeltaForOneLine);
			zoomSpeed *= wheelZoomSpeed;
			if (wheelRotationDelta < 0) {
				zoomSpeed = 1 / zoomSpeed;
			}
			Viewport.Visible = Viewport.Visible.Zoom(zoomTo, zoomSpeed);
		}
	}
}
