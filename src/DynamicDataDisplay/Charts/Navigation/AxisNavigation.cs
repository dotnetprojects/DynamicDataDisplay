using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
{
	public class AxisNavigation : DependencyObject, IPlotterElement
	{
		public AxisPlacement Placement
		{
			get { return (AxisPlacement)GetValue(PlacementProperty); }
			set { SetValue(PlacementProperty, value); }
		}

		public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
		  "Placement",
		  typeof(AxisPlacement),
		  typeof(AxisNavigation),
		  new FrameworkPropertyMetadata(AxisPlacement.Left, OnPlacementChanged));

		private static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisNavigation navigation = (AxisNavigation)d;
			navigation.OnPlacementChanged();
		}

		private Panel listeningPanel;
		protected Panel ListeningPanel
		{
			get { return listeningPanel; }
		}

		private void OnPlacementChanged()
		{
			SetListeningPanel();
		}

		private void SetListeningPanel()
		{
			if (plotter == null) return;

			AxisPlacement placement = Placement;
			switch (placement)
			{
				case AxisPlacement.Left:
					listeningPanel = plotter.LeftPanel;
					break;
				case AxisPlacement.Right:
					listeningPanel = plotter.RightPanel;
					break;
				case AxisPlacement.Top:
					listeningPanel = plotter.TopPanel;
					break;
				case AxisPlacement.Bottom:
					listeningPanel = plotter.BottomPanel;
					break;
				default:
					break;
			}
		}

		private CoordinateTransform Transform
		{
			get { return plotter.Viewport.Transform; }
		}

		private Panel hostPanel;

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			hostPanel = plotter.MainGrid;

			//hostPanel.Background = Brushes.Pink;

			SetListeningPanel();

			if (hostPanel != null)
			{
				hostPanel.MouseLeftButtonUp += OnMouseLeftButtonUp;
				hostPanel.MouseLeftButtonDown += OnMouseLeftButtonDown;
				hostPanel.MouseMove += OnMouseMove;
				hostPanel.MouseWheel += OnMouseWheel;

				hostPanel.MouseRightButtonDown += OnMouseRightButtonDown;
				hostPanel.MouseRightButtonUp += OnMouseRightButtonUp;
				hostPanel.LostFocus += OnLostFocus;
			}
		}

		private void OnLostFocus(object sender, RoutedEventArgs e)
		{
			//Debug.WriteLine("Lost Focus");
			RevertChanges();
			rmbPressed = false;
			lmbPressed = false;

			e.Handled = true;
		}

		#region Right button down

		DataRect rmbDragStartRect;
		private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			OnMouseRightButtonDown(e);
		}

		protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			rmbInitialPosition = e.GetPosition(listeningPanel);

			var foundActivePlotter = UpdateActivePlotter(e);
			if (foundActivePlotter)
			{
				rmbPressed = true;
				dragStartInViewport = rmbInitialPosition.ScreenToViewport(activePlotter.Transform);
				rmbDragStartRect = activePlotter.Visible;

				listeningPanel.Background = fillBrush;
				listeningPanel.CaptureMouse();

				//e.Handled = true;
			}
		}

		#endregion

		#region Right button up

		private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			OnMouseRightButtonUp(e);
		}

		protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			if (rmbPressed)
			{
				rmbPressed = false;
				RevertChanges();

				//e.Handled = true;
			}
		}

		#endregion

		private void RevertChanges()
		{
			listeningPanel.ClearValue(Panel.CursorProperty);
			listeningPanel.Background = Brushes.Transparent;
			listeningPanel.ReleaseMouseCapture();
		}

		private const double wheelZoomSpeed = 1.2;
		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			Point mousePos = e.GetPosition(listeningPanel);

			Rect listeningPanelBounds = new Rect(listeningPanel.RenderSize);
			if (!listeningPanelBounds.Contains(mousePos))
				return;

			var foundActivePlotter = UpdateActivePlotter(e);
			if (!foundActivePlotter) return;

			int delta = -e.Delta;

			Point zoomTo = mousePos.ScreenToViewport(activePlotter.Transform);

			double zoomSpeed = Math.Abs(delta / Mouse.MouseWheelDeltaForOneLine);
			zoomSpeed *= wheelZoomSpeed;
			if (delta < 0)
			{
				zoomSpeed = 1 / zoomSpeed;
			}

			DataRect visible = activePlotter.Viewport.Visible.Zoom(zoomTo, zoomSpeed);
			DataRect oldVisible = activePlotter.Viewport.Visible;
			if (Placement.IsBottomOrTop())
			{
				visible.YMin = oldVisible.YMin;
				visible.Height = oldVisible.Height;
			}
			else
			{
				visible.XMin = oldVisible.XMin;
				visible.Width = oldVisible.Width;
			}
			activePlotter.Viewport.Visible = visible;

			e.Handled = true;
		}

		private const int RmbZoomScale = 200;
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (lmbPressed)
			{
				// panning: 
				if (e.LeftButton == MouseButtonState.Released)
				{
					lmbPressed = false;
					RevertChanges();
					return;
				}

				Point screenMousePos = e.GetPosition(listeningPanel);
				Point dataMousePos = screenMousePos.ScreenToViewport(activePlotter.Transform);
				DataRect visible = activePlotter.Viewport.Visible;
				double delta;
				if (Placement.IsBottomOrTop())
				{
					delta = (dataMousePos - dragStartInViewport).X;
					visible.XMin -= delta;
				}
				else
				{
					delta = (dataMousePos - dragStartInViewport).Y;
					visible.YMin -= delta;
				}

				if (screenMousePos != lmbInitialPosition)
				{
					listeningPanel.Cursor = Placement.IsBottomOrTop() ? Cursors.ScrollWE : Cursors.ScrollNS;
				}

				activePlotter.Viewport.Visible = visible;

				e.Handled = true;
			}
			else if (rmbPressed)
			{
				// one direction zooming:
				if (e.RightButton == MouseButtonState.Released)
				{
					rmbPressed = false;
					RevertChanges();
					return;
				}

				Point screenMousePos = e.GetPosition(listeningPanel);
				DataRect visible = activePlotter.Viewport.Visible;
				double delta;

				bool isHorizontal = Placement.IsBottomOrTop();
				if (isHorizontal)
				{
					delta = (screenMousePos - rmbInitialPosition).X;
				}
				else
				{
					delta = (screenMousePos - rmbInitialPosition).Y;
				}

				if (delta < 0)
					delta = 1 / Math.Exp(-delta / RmbZoomScale);
				else
					delta = Math.Exp(delta / RmbZoomScale);

				Point center = dragStartInViewport;

				if (isHorizontal)
				{
					visible = rmbDragStartRect.ZoomX(center, delta);
				}
				else
				{
					visible = rmbDragStartRect.ZoomY(center, delta);
				}

				if (screenMousePos != lmbInitialPosition)
				{
					listeningPanel.Cursor = Placement.IsBottomOrTop() ? Cursors.ScrollWE : Cursors.ScrollNS;
				}


				activePlotter.Viewport.Visible = visible;

				//e.Handled = true;
			}
		}

		private Point lmbInitialPosition;
		protected Point LmbInitialPosition
		{
			get { return lmbInitialPosition; }
		}

		private Point rmbInitialPosition;
		private readonly SolidColorBrush fillBrush = new SolidColorBrush(Color.FromRgb(255, 228, 209)).MakeTransparent(0.2);
		private bool lmbPressed;
		private bool rmbPressed;
		private Point dragStartInViewport;
		private Plotter2D activePlotter;

		#region Left button down

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			OnMouseLeftButtonDown(e);
		}

		protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			lmbInitialPosition = e.GetPosition(listeningPanel);

			var foundActivePlotter = UpdateActivePlotter(e);
			if (foundActivePlotter)
			{
				lmbPressed = true;
				dragStartInViewport = lmbInitialPosition.ScreenToViewport(activePlotter.Transform);

				listeningPanel.Background = fillBrush;
				listeningPanel.CaptureMouse();

				e.Handled = true;
			}
		}

		#endregion

		private bool UpdateActivePlotter(MouseEventArgs e)
		{
			var axes = listeningPanel.Children.OfType<GeneralAxis>();

			foreach (var axis in axes)
			{
				var positionInAxis = e.GetPosition(axis);
				Rect axisBounds = new Rect(axis.RenderSize);
				if (axisBounds.Contains(positionInAxis))
				{
					activePlotter = axis.Plotter;

					return true;
				}
			}

			return false;
		}

		#region Left button up

		private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			OnMouseLeftButtonUp(e);
		}

		protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (lmbPressed)
			{
				lmbPressed = false;
				RevertChanges();

				e.Handled = true;
			}
		}

		#endregion

		public void OnPlotterDetaching(Plotter plotter)
		{
			if (plotter.MainGrid != null)
			{
				hostPanel.MouseLeftButtonUp -= OnMouseLeftButtonUp;
				hostPanel.MouseLeftButtonDown -= OnMouseLeftButtonDown;
				hostPanel.MouseMove -= OnMouseMove;
				hostPanel.MouseWheel -= OnMouseWheel;

				hostPanel.MouseRightButtonDown -= OnMouseRightButtonDown;
				hostPanel.MouseRightButtonUp -= OnMouseRightButtonUp;
				
				hostPanel.LostFocus -= OnLostFocus;
			}
			listeningPanel = null;
			this.plotter = null;
		}

		private Plotter2D plotter;
		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion

		public override string ToString()
		{
			return "AxisNavigation: " + Placement;
		}
	}
}
