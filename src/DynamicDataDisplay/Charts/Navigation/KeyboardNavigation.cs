using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System;
using Microsoft.Research.DynamicDataDisplay.Common.UndoSystem;
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
	/// <summary>Provides keyboard navigation around viewport of ChartPlotter.</summary>
	public class KeyboardNavigation : IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KeyboardNavigation"/> class.
		/// </summary>
		public KeyboardNavigation() { }

		private bool isReversed = true;
		/// <summary>
		/// Gets or sets a value indicating whether panning directions are reversed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if panning directions are reversed; otherwise, <c>false</c>.
		/// </value>
		public bool IsReversed
		{
			get { return isReversed; }
			set { isReversed = value; }
		}

		private readonly List<CommandBinding> addedBindings = new List<CommandBinding>();
		private void AddBinding(CommandBinding binding)
		{
			plotter2D.CommandBindings.Add(binding);
			addedBindings.Add(binding);
		}

		private void InitCommands()
		{
			if (plotter2D == null)
				return;

			var zoomOutToMouseCommandBinding = new CommandBinding(
				ChartCommands.ZoomOutToMouse,
				ZoomOutToMouseExecute,
				ZoomOutToMouseCanExecute);
			AddBinding(zoomOutToMouseCommandBinding);

			var zoomInToMouseCommandBinding = new CommandBinding(
				ChartCommands.ZoomInToMouse,
				ZoomInToMouseExecute,
				ZoomInToMouseCanExecute);
			AddBinding(zoomInToMouseCommandBinding);

			var zoomWithParamCommandBinding = new CommandBinding(
				ChartCommands.ZoomWithParameter,
				ZoomWithParamExecute,
				ZoomWithParamCanExecute);
			AddBinding(zoomWithParamCommandBinding);

			var zoomInCommandBinding = new CommandBinding(
				ChartCommands.ZoomIn,
				ZoomInExecute,
				ZoomInCanExecute);
			AddBinding(zoomInCommandBinding);

			var zoomOutCommandBinding = new CommandBinding(
				ChartCommands.ZoomOut,
				ZoomOutExecute,
				ZoomOutCanExecute);
			AddBinding(zoomOutCommandBinding);

			var fitToViewCommandBinding = new CommandBinding(
				ChartCommands.FitToView,
				FitToViewExecute,
				FitToViewCanExecute);
			AddBinding(fitToViewCommandBinding);

			var ScrollLeftCommandBinding = new CommandBinding(
					ChartCommands.ScrollLeft,
					ScrollLeftExecute,
					ScrollLeftCanExecute);
			AddBinding(ScrollLeftCommandBinding);

			var ScrollRightCommandBinding = new CommandBinding(
				ChartCommands.ScrollRight,
				ScrollRightExecute,
				ScrollRightCanExecute);
			AddBinding(ScrollRightCommandBinding);

			var ScrollUpCommandBinding = new CommandBinding(
				ChartCommands.ScrollUp,
				ScrollUpExecute,
				ScrollUpCanExecute);
			AddBinding(ScrollUpCommandBinding);

			var ScrollDownCommandBinding = new CommandBinding(
				ChartCommands.ScrollDown,
				ScrollDownExecute,
				ScrollDownCanExecute);
			AddBinding(ScrollDownCommandBinding);

			var SaveScreenshotCommandBinding = new CommandBinding(
				ChartCommands.SaveScreenshot,
				SaveScreenshotExecute,
				SaveScreenshotCanExecute);
			AddBinding(SaveScreenshotCommandBinding);

			var CopyScreenshotCommandBinding = new CommandBinding(
				ChartCommands.CopyScreenshot,
				CopyScreenshotExecute,
				CopyScreenshotCanExecute);
			AddBinding(CopyScreenshotCommandBinding);

			var ShowHelpCommandBinding = new CommandBinding(
				ChartCommands.ShowHelp,
				ShowHelpExecute,
				ShowHelpCanExecute);
			AddBinding(ShowHelpCommandBinding);

			var UndoCommandBinding = new CommandBinding(
				ApplicationCommands.Undo,
				UndoExecute,
				UndoCanExecute);
			AddBinding(UndoCommandBinding);

			var RedoCommandBinding = new CommandBinding(
				ApplicationCommands.Redo,
				RedoExecute,
				RedoCanExecute);
			AddBinding(RedoCommandBinding);
		}

		#region Zoom Out To Mouse

		private void ZoomToPoint(double coeff)
		{
			Point pt = Mouse.GetPosition(plotter2D.CentralGrid);
			Point dataPoint = Viewport.Transform.ScreenToData(pt);
			DataRect visible = Viewport.Visible;

			Viewport.Visible = visible.Zoom(dataPoint, coeff);
		}

		private void ZoomOutToMouseExecute(object target, ExecutedRoutedEventArgs e)
		{
			ZoomToPoint(zoomOutCoeff);
			e.Handled = true;
		}

		private void ZoomOutToMouseCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Zoom In To Mouse

		private void ZoomInToMouseExecute(object target, ExecutedRoutedEventArgs e)
		{
			ZoomToPoint(zoomInCoeff);
			e.Handled = true;
		}

		private void ZoomInToMouseCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Zoom With param

		private void ZoomWithParamExecute(object target, ExecutedRoutedEventArgs e)
		{
			double zoomParam = (double)e.Parameter;
			plotter2D.Viewport.Zoom(zoomParam);
			e.Handled = true;
		}

		private void ZoomWithParamCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Zoom in

		private const double zoomInCoeff = 0.9;
		private void ZoomInExecute(object target, ExecutedRoutedEventArgs e)
		{
			Viewport.Zoom(zoomInCoeff);
			e.Handled = true;
		}

		private void ZoomInCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region Zoom out

		private const double zoomOutCoeff = 1 / zoomInCoeff;
		private void ZoomOutExecute(object target, ExecutedRoutedEventArgs e)
		{
			Viewport.Zoom(zoomOutCoeff);
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
			// todo сделать нормально.
			(Viewport as Viewport2D).FitToView();
			e.Handled = true;
		}

		private void FitToViewCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			// todo add a check if viewport is already fitted to view.
			e.CanExecute = true;
		}

		#endregion

		#region Scroll

		private double scrollCoeff = 0.05;
		private void ScrollVisibleProportionally(double xShiftCoeff, double yShiftCoeff)
		{
			DataRect visible = Viewport.Visible;
			DataRect oldVisible = visible;
			double width = visible.Width;
			double height = visible.Height;

			double reverseCoeff = isReversed ? -1 : 1;
			visible.Offset(reverseCoeff * xShiftCoeff * width, reverseCoeff * yShiftCoeff * height);

			Viewport.Visible = visible;
			plotter2D.UndoProvider.AddAction(new DependencyPropertyChangedUndoAction(Viewport, Viewport2D.VisibleProperty, oldVisible, visible));
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
			dlg.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif";
			dlg.FilterIndex = 1;
			dlg.AddExtension = true;
			if (dlg.ShowDialog().GetValueOrDefault(false))
			{
				string filePath = dlg.FileName;
				plotter2D.SaveScreenshot(filePath);
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
			plotter2D.CopyScreenshotToClipboard();
			e.Handled = true;
		}

		private void CopyScreenshotCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

		#region ShowHelp

		private bool aboutWindowOpened = false;
		private void ShowHelpExecute(object target, ExecutedRoutedEventArgs e)
		{
			if (!aboutWindowOpened)
			{
				AboutWindow window = new AboutWindow();
				window.Closed += new EventHandler(aboutWindow_Closed);
				window.DataContext = plotter2D;

				window.Owner = Window.GetWindow(plotter2D);

				aboutWindowOpened = true;
				window.Show();

				e.Handled = true;
			}
		}

		void aboutWindow_Closed(object sender, EventArgs e)
		{
			Window window = (Window)sender;
			window.Closed -= aboutWindow_Closed;
			aboutWindowOpened = false;
		}

		private void ShowHelpCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !aboutWindowOpened;
		}

		#endregion

		#region Undo

		private void UndoExecute(object target, ExecutedRoutedEventArgs e)
		{
			plotter2D.UndoProvider.Undo();
			e.Handled = true;
		}

		private void UndoCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = plotter2D.UndoProvider.CanUndo;
		}

		#endregion

		#region Redo

		private void RedoExecute(object target, ExecutedRoutedEventArgs e)
		{
			plotter2D.UndoProvider.Redo();
			e.Handled = true;
		}

		private void RedoCanExecute(object target, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = plotter2D.UndoProvider.CanRedo;
		}

		#endregion

		#region IPlotterElement Members

		private Viewport2D Viewport
		{
			get { return plotter2D.Viewport; }
		}

		private Plotter2D plotter2D;
		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			plotter2D = (Plotter2D)plotter;

			InitCommands();
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			foreach (var commandBinding in addedBindings)
			{
				plotter.CommandBindings.Remove(commandBinding);
			}
			addedBindings.Clear();

			this.plotter2D = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter2D; }
		}

		#endregion
	}
}
