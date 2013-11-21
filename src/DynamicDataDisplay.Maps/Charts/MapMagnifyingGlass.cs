using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using System.Windows.Input;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using System.Windows.Markup;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	[ContentProperty("NetworkTileServer")]
	public class MapMagnifyingGlass : ContentGraph
	{
		public MapMagnifyingGlass()
		{
			Width = 200;
			Height = 200;

			UpdateSize();

			Visibility = Visibility.Collapsed;
		}

		protected override Panel HostPanel
		{
			get { return Plotter2D.ParallelCanvas; }
		}

		private ChartPlotter littlePlotter = new ChartPlotter();
		private Map map = new Map();
		protected override void OnPlotterAttached()
		{
			// leaving only viewport
			littlePlotter.Children.Clear();

			if (map.Plotter == null)
			{
				littlePlotter.Children.Add(map);
			}
			littlePlotter.Viewport.Visible = new Rect(-180, -90, 360, 180);
			littlePlotter.IsHitTestVisible = false;

			SetPlotterBinding(Control.WidthProperty);
			SetPlotterBinding(Control.HeightProperty);

			Content = littlePlotter;

			Plotter2D.PreviewMouseMove += Plotter2D_PreviewMouseMove;
			Plotter2D.PreviewMouseWheel += Plotter2D_PreviewMouseWheel;
			Plotter2D.MouseLeave += Plotter2D_MouseLeave;
			Plotter2D.LostFocus += Plotter2D_LostFocus;
		}

		public ChartPlotter MapPlotter
		{
			get { return littlePlotter; }
		}

		public Map Map
		{
			get { return map; }
		}

		[NotNull]
		public DataTransform DataTransform
		{
			get { return littlePlotter.DataTransform; }
			set { littlePlotter.DataTransform = value; }
		}

		[NotNull]
		public SourceTileServer NetworkTileServer
		{
			get { return map.SourceTileServer; }
			set { map.SourceTileServer = value; }
		}

		private void Plotter2D_LostFocus(object sender, RoutedEventArgs e)
		{
			Visibility = Visibility.Collapsed;
		}

		private void Plotter2D_MouseLeave(object sender, MouseEventArgs e)
		{
			Visibility = Visibility.Collapsed;
		}

		private void SetPlotterBinding(DependencyProperty dependencyProperty)
		{
			littlePlotter.SetBinding(dependencyProperty, dependencyProperty.Name);
		}

		private void Plotter2D_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if ((Keyboard.Modifiers | ModifierKeys.Control) == Keyboard.Modifiers)
			{
				ZoomCoeff += e.Delta / Mouse.MouseWheelDeltaForOneLine;
				e.Handled = true;
			}
			else if ((Keyboard.Modifiers | ModifierKeys.Shift) == Keyboard.Modifiers)
			{
				double size = 1;
				const double sizeZoom = 1.3;

				size *= e.Delta > 0 ? sizeZoom * e.Delta / Mouse.MouseWheelDeltaForOneLine : 1 / (-e.Delta / Mouse.MouseWheelDeltaForOneLine) / sizeZoom;
				Width *= size;
				Height *= size;

				UpdateSize();
				UpdateLittleVisible();
				UpdatePosition();

				e.Handled = true;
			}
		}

		private void UpdateSize()
		{
			//Clip = new EllipseGeometry(new Point(Width / 2, Height / 2), Width / 2, Height / 2);
		}

		protected override void OnPlotterDetaching()
		{
			Plotter2D.PreviewMouseMove -= Plotter2D_PreviewMouseMove;
			Plotter2D.PreviewMouseWheel -= Plotter2D_PreviewMouseWheel;
			Plotter2D.MouseLeave -= Plotter2D_MouseLeave;
			Plotter2D.LostFocus -= Plotter2D_LostFocus;
		}

		private double zoomCoeff = 4;
		public double ZoomCoeff
		{
			get { return zoomCoeff; }
			set
			{
				if (zoomCoeff != value)
				{
					zoomCoeff = value;

					if (zoomCoeff < 1.5)
						zoomCoeff = 1.5;

					UpdateLittleVisible();
				}
			}
		}

		private void Plotter2D_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			Point pos = e.MouseDevice.GetPosition(Plotter2D.ViewportPanel);

			if (Plotter2D.Viewport.Output.Contains(pos))
			{
				Visibility = Visibility.Visible;

				UpdatePosition(pos);
				UpdateLittleVisible(pos);
			}
			else
			{
				Visibility = Visibility.Collapsed;
			}
		}

		private void UpdatePosition()
		{
			if (Plotter2D != null)
			{
				UpdatePosition(Mouse.GetPosition(Plotter2D.ViewportPanel));
			}
		}

		private void UpdatePosition(Point pos)
		{
			Canvas.SetLeft(this, pos.X - ActualWidth / 2 + Plotter2D.LeftPanel.ActualWidth);
			Canvas.SetTop(this, pos.Y - ActualHeight / 2 + Plotter2D.TopPanel.ActualHeight);
		}

		protected override void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			UpdateLittleVisible();
		}

		private void UpdateLittleVisible()
		{
			if (Plotter2D != null)
			{
				UpdateLittleVisible(Mouse.GetPosition(Plotter2D.ViewportPanel));
			}
		}

		private void UpdateLittleVisible(Point pos)
		{
			Size littleSize = new Size(Plotter2D.Viewport.Visible.Width / zoomCoeff * ActualWidth / Plotter2D.Viewport.Output.Width,
									   Plotter2D.Viewport.Visible.Height / zoomCoeff * ActualHeight / Plotter2D.Viewport.Output.Height);
			Rect littleVisible = RectExtensions.FromCenterSize(pos.ScreenToViewport(Plotter2D.Viewport.Transform), littleSize);
			littlePlotter.Viewport.Visible = littleVisible;
		}
	}
}
