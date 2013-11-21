using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Adds to ChartPlotter two lines upon axes, showing current cursor position.
	/// </summary>
	public class AxisCursorGraph : DependencyObject, IPlotterElement
	{
		static AxisCursorGraph()
		{
			Line.StrokeProperty.AddOwner(typeof(AxisCursorGraph), new FrameworkPropertyMetadata(Brushes.Red));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AxisCursorGraph"/> class.
		/// </summary>
		public AxisCursorGraph() { }

		#region ShowVerticalLine property

		/// <summary>
		/// Gets or sets a value indicating whether to show line upon vertical axis.
		/// </summary>
		/// <value><c>true</c> if line upon vertical axis is shown; otherwise, <c>false</c>.</value>
		public bool ShowVerticalLine
		{
			get { return (bool)GetValue(ShowVerticalLineProperty); }
			set { SetValue(ShowVerticalLineProperty, value); }
		}

		/// <summary>
		/// Identifies ShowVerticalLine dependency property.
		/// </summary>
		public static readonly DependencyProperty ShowVerticalLineProperty = DependencyProperty.Register(
		  "ShowVerticalLine",
		  typeof(bool),
		  typeof(AxisCursorGraph),
		  new FrameworkPropertyMetadata(true, OnShowLinePropertyChanged));

		private static void OnShowLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AxisCursorGraph graph = (AxisCursorGraph)d;
			graph.UpdateUIRepresentation();
		}

		#endregion

		#region ShowHorizontalLine property

		/// <summary>
		/// Gets or sets a value indicating whether to show line upon horizontal axis.
		/// </summary>
		/// <value><c>true</c> if lien upon horizontal axis is shown; otherwise, <c>false</c>.</value>
		public bool ShowHorizontalLine
		{
			get { return (bool)GetValue(ShowHorizontalLineProperty); }
			set { SetValue(ShowHorizontalLineProperty, value); }
		}

		public static readonly DependencyProperty ShowHorizontalLineProperty = DependencyProperty.Register(
		  "ShowHorizontalLine",
		  typeof(bool),
		  typeof(AxisCursorGraph),
		  new FrameworkPropertyMetadata(true, OnShowLinePropertyChanged));

		#endregion

		#region IPlotterElement Members

		private Line leftLine;
		private Line bottomLine;
		private Canvas leftCanvas;
		private Canvas bottomCanvas;

		private Plotter2D plotter;
		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			this.plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;

			var parent = plotter.MainGrid;
			parent.MouseMove += parent_MouseMove;
			parent.MouseEnter += parent_MouseEnter;
			parent.MouseLeave += parent_MouseLeave;

			Style lineStyle = new Style(typeof(Line));
			AddBindingSetter(lineStyle, Line.StrokeProperty);
			AddBindingSetter(lineStyle, Line.StrokeThicknessProperty);

			leftCanvas = new Canvas();
			Grid.SetRow(leftCanvas, 1);
			Grid.SetColumn(leftCanvas, 0);
			leftLine = new Line { Style = lineStyle, IsHitTestVisible = false };
			leftCanvas.Children.Add(leftLine);
			parent.Children.Add(leftCanvas);

			bottomCanvas = new Canvas();
			Grid.SetRow(bottomCanvas, 2);
			Grid.SetColumn(bottomCanvas, 1);
			bottomLine = new Line { Style = lineStyle, IsHitTestVisible = false };
			bottomCanvas.Children.Add(bottomLine);
			parent.Children.Add(bottomCanvas);
		}

		private void AddBindingSetter(Style style, DependencyProperty property)
		{
			style.Setters.Add(new Setter(property,
				new Binding
				{
					Path = new PropertyPath(property.Name),
					Source = this
				}));
		}

		void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			UpdateUIRepresentation();
		}

		private void UpdateUIRepresentation()
		{
			if (plotter == null) return;

			var transform = plotter.Viewport.Transform;
			DataRect visible = plotter.Viewport.Visible;
			Rect output = plotter.Viewport.Output;

			Point mousePos = Mouse.GetPosition(plotter.CentralGrid);

			if (ShowVerticalLine)
			{
				double y = mousePos.Y;
				if (output.Top <= y && y <= output.Bottom)
				{
					leftLine.Visibility = Visibility.Visible;
					leftLine.X1 = 0;
					leftLine.X2 = plotter.LeftPanel.ActualWidth;

					leftLine.Y1 = leftLine.Y2 = y;
				}
				else
				{
					leftLine.Visibility = Visibility.Collapsed;
				}
			}
			else
			{
				leftLine.Visibility = Visibility.Collapsed;
			}

			if (ShowHorizontalLine)
			{
				double x = mousePos.X;
				if (output.Left <= x && x <= output.Right)
				{
					bottomLine.Visibility = Visibility.Visible;
					bottomLine.Y1 = 0;
					bottomLine.Y2 = plotter.BottomPanel.ActualHeight;

					bottomLine.X1 = bottomLine.X2 = x;
				}
				else
				{
					bottomLine.Visibility = Visibility.Collapsed;
				}
			}
			else
			{
				bottomLine.Visibility = Visibility.Collapsed;
			}
		}

		void parent_MouseLeave(object sender, MouseEventArgs e)
		{
			UpdateUIRepresentation();
		}

		void parent_MouseEnter(object sender, MouseEventArgs e)
		{
			UpdateUIRepresentation();
		}

		void parent_MouseMove(object sender, MouseEventArgs e)
		{
			UpdateUIRepresentation();
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			this.plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;

			var parent = plotter.MainGrid;
			parent.MouseMove -= parent_MouseMove;
			parent.MouseEnter -= parent_MouseEnter;
			parent.MouseLeave -= parent_MouseLeave;

			parent.Children.Remove(leftCanvas);
			parent.Children.Remove(bottomCanvas);

			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
