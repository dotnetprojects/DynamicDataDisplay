using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
	/// <summary>Provides keyboard navigation around viewport</summary>
	public class KeyboardNavigation : NavigationBase
	{

		public KeyboardNavigation()
		{
			Focusable = true;
			Loaded += KeyboardNavigation_Loaded;
		}

		private void KeyboardNavigation_Loaded(object sender, RoutedEventArgs e)
		{
			Keyboard.Focus(this);
			InitCommands();
		}

		public void InitCommands()
		{
			var zoomWithParamCommandBinding = new CommandBinding(
				ChartCommands.ZoomWithParameter,
				ZoomWithParamExecute,
				ZoomWithParamCanExecute);
			Plotter2D.CommandBindings.Add(zoomWithParamCommandBinding);

			var zoomInCommandBinding = new CommandBinding(
				ChartCommands.ZoomIn,
				ZoomInExecute,
				ZoomInCanExecute);
			Plotter2D.CommandBindings.Add(zoomInCommandBinding);

			var zoomOutCommandBinding = new CommandBinding(
				ChartCommands.ZoomOut,
				ZoomOutExecute,
				ZoomOutCanExecute);
			Plotter2D.CommandBindings.Add(zoomOutCommandBinding);

			var fitToViewCommandBinding = new CommandBinding(
				ChartCommands.FitToView,
				FitToViewExecute,
				FitToViewCanExecute);
			Plotter2D.CommandBindings.Add(fitToViewCommandBinding);

			var ScrollLeftCommandBinding = new CommandBinding(
					ChartCommands.ScrollLeft,
					ScrollLeftExecute,
					ScrollLeftCanExecute);
			Plotter2D.CommandBindings.Add(ScrollLeftCommandBinding);

			var ScrollRightCommandBinding = new CommandBinding(
				ChartCommands.ScrollRight,
				ScrollRightExecute,
				ScrollRightCanExecute);
			Plotter2D.CommandBindings.Add(ScrollRightCommandBinding);

			var ScrollUpCommandBinding = new CommandBinding(
				ChartCommands.ScrollUp,
				ScrollUpExecute,
				ScrollUpCanExecute);
			Plotter2D.CommandBindings.Add(ScrollUpCommandBinding);

			var ScrollDownCommandBinding = new CommandBinding(
					ChartCommands.ScrollDown,
					ScrollDownExecute,
					ScrollDownCanExecute);
			Plotter2D.CommandBindings.Add(ScrollDownCommandBinding);

			var SaveScreenshotCommandBinding = new CommandBinding(
				ChartCommands.SaveScreenshot,
				SaveScreenshotExecute,
				SaveScreenshotCanExecute);
			Plotter2D.CommandBindings.Add(SaveScreenshotCommandBinding);

			var CopyScreenshotCommandBinding = new CommandBinding(
				ChartCommands.CopyScreenshot,
				CopyScreenshotExecute,
				CopyScreenshotCanExecute);
			Plotter2D.CommandBindings.Add(CopyScreenshotCommandBinding);

			var ShowHelpCommandBinding = new CommandBinding(
				ChartCommands.ShowHelp,
				ShowHelpExecute,
				ShowHelpCanExecute);
			Plotter2D.CommandBindings.Add(ShowHelpCommandBinding);
		}

		#region Zoom With param

		private void ZoomWithParamExecute(object target, ExecutedRoutedEventArgs e)
		{
			// TODO: add working code
			//double zoomParam = (double)e.Parameter;
			//Plotter2d.ZoomVisible(zoomParam);
			e.Handled = true;
		}

		private void ZoomWithParamCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Zoom in

		private void ZoomInExecute(object target, ExecutedRoutedEventArgs e)
		{
			Viewport.Zoom(0.9);
			e.Handled = true;
		}

		private void ZoomInCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Zoom out

		private void ZoomOutExecute(object target, ExecutedRoutedEventArgs e)
		{
			Viewport.Zoom(1.1);
			e.Handled = true;
		}

		private void ZoomOutCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Fit to view

		private void FitToViewExecute(object target, ExecutedRoutedEventArgs e)
		{
			Viewport.FitToView();
			e.Handled = true;
		}

		private void FitToViewCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Scroll

		private double scrollCoeff = 0.05;
		private void ScrollVisibleProportionally(double xShiftCoeff, double yShiftCoeff)
		{
			Rect visible = Viewport.Visible;
			double width = visible.Width;
			double height = visible.Height;

			visible.Offset(xShiftCoeff * width, yShiftCoeff * height);

			Viewport.Visible = visible;
		}

		#region ScrollLeft

		private void ScrollLeftExecute(object target, ExecutedRoutedEventArgs e)
		{
			ScrollVisibleProportionally(scrollCoeff, 0);
			e.Handled = true;
		}

		private void ScrollLeftCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region ScrollRight

		private void ScrollRightExecute(object target, ExecutedRoutedEventArgs e)
		{
			ScrollVisibleProportionally(-scrollCoeff, 0);
			e.Handled = true;
		}

		private void ScrollRightCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region ScrollUp

		private void ScrollUpExecute(object target, ExecutedRoutedEventArgs e)
		{
			ScrollVisibleProportionally(0, -scrollCoeff);
			e.Handled = true;
		}

		private void ScrollUpCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region ScrollDown

		private void ScrollDownExecute(object target, ExecutedRoutedEventArgs e)
		{
			ScrollVisibleProportionally(0, scrollCoeff);
			e.Handled = true;
		}

		private void ScrollDownCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#endregion

		#region SaveScreenshot

		private void SaveScreenshotExecute(object target, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "JPEG(*.jpg)|*.jpg|BMP(*.bmp)|*.bmp|GIF(*.gif)|.gif|All files (*.*)|*.*";
			dlg.FilterIndex = 1;
			if (dlg.ShowDialog().GetValueOrDefault(false))
			{
				string filePath = dlg.FileName;
				Plotter2D.SaveScreenshot(filePath);
				e.Handled = true;
			}
		}

		private void SaveScreenshotCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region CopyScreenshot

		private void CopyScreenshotExecute(object target, ExecutedRoutedEventArgs e)
		{
			Plotter2D.CopyScreenshotToClipboard();
			e.Handled = true;
		}

		private void CopyScreenshotCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region ShowHelp

		private void ShowHelpExecute(object target, ExecutedRoutedEventArgs e)
		{
			string helpText =
@"Dynamic Data Display is in ""Auto Fit"" mode by default.
By using the mouse you can manually select an area to display:

Mouse wheel, '+' and '-' — Zoom in/Zoom out
Mouse drag or arrow keys — Pan the display area
Ctrl + Mouse drag — Select an area to zoom in
Shift + Mouse drag — Select an area with fixed aspect ratio
Home — Fit to view
F11 — Copy chart to clipboard";

			MessageBox.Show(helpText, "Quick Help", MessageBoxButton.OK, MessageBoxImage.Information);

			e.Handled = true;
		}

		private void ShowHelpCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		protected override void OnRenderCore(DrawingContext dc, RenderState state)
		{
			// do nothing here
		}
	}
}
