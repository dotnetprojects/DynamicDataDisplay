using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Adds to ChartPlotter two crossed lines, bound to mouse cursor position, and two labels near axes with mouse position in its text.
	/// </summary>
	public partial class CursorCoordinateGraph : ContentGraph
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CursorCoordinateGraph"/> class.
		/// </summary>
		public CursorCoordinateGraph()
		{
			InitializeComponent();
		}

		Vector blockShift = new Vector(3, 3);

		#region Plotter

		protected override void OnPlotterAttached()
		{
			UIElement parent = (UIElement)Parent;

			parent.MouseMove += parent_MouseMove;
			parent.MouseEnter += Parent_MouseEnter;
			parent.MouseLeave += Parent_MouseLeave;

			UpdateVisibility();
			UpdateUIRepresentation();
		}

		protected override void OnPlotterDetaching()
		{
			UIElement parent = (UIElement)Parent;

			parent.MouseMove -= parent_MouseMove;
			parent.MouseEnter -= Parent_MouseEnter;
			parent.MouseLeave -= Parent_MouseLeave;
		}

		#endregion

		private bool autoHide = true;
		/// <summary>
		/// Gets or sets a value indicating whether to hide automatically cursor lines when mouse leaves plotter.
		/// </summary>
		/// <value><c>true</c> if auto hide; otherwise, <c>false</c>.</value>
		public bool AutoHide
		{
			get { return autoHide; }
			set { autoHide = value; }
		}

		private void Parent_MouseEnter(object sender, MouseEventArgs e)
		{
			if (autoHide)
			{
				UpdateVisibility();
			}
		}

		private void UpdateVisibility()
		{
			horizLine.Visibility = vertGrid.Visibility = GetHorizontalVisibility();
			vertLine.Visibility = horizGrid.Visibility = GetVerticalVisibility();
		}

		private Visibility GetHorizontalVisibility()
		{
			return showHorizontalLine ? Visibility.Visible : Visibility.Hidden;
		}

		private Visibility GetVerticalVisibility()
		{
			return showVerticalLine ? Visibility.Visible : Visibility.Hidden;
		}

		private void Parent_MouseLeave(object sender, MouseEventArgs e)
		{
			if (autoHide)
			{
				horizLine.Visibility = Visibility.Hidden;
				vertLine.Visibility = Visibility.Hidden;
				horizGrid.Visibility = Visibility.Hidden;
				vertGrid.Visibility = Visibility.Hidden;
			}
		}

		private bool followMouse = true;
		/// <summary>
		/// Gets or sets a value indicating whether lines are following mouse cursor position.
		/// </summary>
		/// <value><c>true</c> if lines are following mouse cursor position; otherwise, <c>false</c>.</value>
		public bool FollowMouse
		{
			get { return followMouse; }
			set
			{
				followMouse = value;

				if (!followMouse)
				{
					AutoHide = false;
				}

				UpdateUIRepresentation();
			}
		}

		private void parent_MouseMove(object sender, MouseEventArgs e)
		{
			if (followMouse)
			{
				UpdateUIRepresentation();
			}
		}

		protected override void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			UpdateUIRepresentation();
		}

		private string customXFormat = null;
		/// <summary>
		/// Gets or sets the custom format string of x label.
		/// </summary>
		/// <value>The custom X format.</value>
		public string CustomXFormat
		{
			get { return customXFormat; }
			set
			{
				if (customXFormat != value)
				{
					customXFormat = value;
					UpdateUIRepresentation();
				}
			}
		}

		private string customYFormat = null;
		/// <summary>
		/// Gets or sets the custom format string of y label.
		/// </summary>
		/// <value>The custom Y format.</value>
		public string CustomYFormat
		{
			get { return customYFormat; }
			set
			{
				if (customYFormat != value)
				{
					customYFormat = value;
					UpdateUIRepresentation();
				}
			}
		}

		private Func<double, string> xTextMapping = null;
		/// <summary>
		/// Gets or sets the text mapping of x label - function that builds text from x-coordinate of mouse in data.
		/// </summary>
		/// <value>The X text mapping.</value>
		public Func<double, string> XTextMapping
		{
			get { return xTextMapping; }
			set
			{
				if (xTextMapping != value)
				{
					xTextMapping = value;
					UpdateUIRepresentation();
				}
			}
		}

		private Func<double, string> yTextMapping = null;
		/// <summary>
		/// Gets or sets the text mapping of y label - function that builds text from y-coordinate of mouse in data.
		/// </summary>
		/// <value>The Y text mapping.</value>
		public Func<double, string> YTextMapping
		{
			get { return yTextMapping; }
			set
			{
				if (yTextMapping != value)
				{
					yTextMapping = value;
					UpdateUIRepresentation();
				}
			}
		}

		private bool showHorizontalLine = true;
		/// <summary>
		/// Gets or sets a value indicating whether to show horizontal line.
		/// </summary>
		/// <value><c>true</c> if horizontal line is shown; otherwise, <c>false</c>.</value>
		public bool ShowHorizontalLine
		{
			get { return showHorizontalLine; }
			set
			{
				if (showHorizontalLine != value)
				{
					showHorizontalLine = value;
					UpdateVisibility();
				}
			}
		}

		private bool showVerticalLine = true;
		/// <summary>
		/// Gets or sets a value indicating whether to show vertical line.
		/// </summary>
		/// <value><c>true</c> if vertical line is shown; otherwise, <c>false</c>.</value>
		public bool ShowVerticalLine
		{
			get { return showVerticalLine; }
			set
			{
				if (showVerticalLine != value)
				{
					showVerticalLine = value;
					UpdateVisibility();
				}
			}
		}

		private void UpdateUIRepresentation()
		{
			Point position = followMouse ? Mouse.GetPosition(this) : Position;
			UpdateUIRepresentation(position);
		}

		private void UpdateUIRepresentation(Point mousePos)
		{
			if (Plotter2D == null) return;

			var transform = Plotter2D.Viewport.Transform;
			DataRect visible = Plotter2D.Viewport.Visible;
			Rect output = Plotter2D.Viewport.Output;

			if (!output.Contains(mousePos))
			{
				if (autoHide)
				{
					horizGrid.Visibility = horizLine.Visibility = vertGrid.Visibility = vertLine.Visibility = Visibility.Hidden;
				}
				return;
			}

			if (!followMouse)
			{
				mousePos = mousePos.DataToScreen(transform);
			}

			horizLine.X1 = output.Left;
			horizLine.X2 = output.Right;
			horizLine.Y1 = mousePos.Y;
			horizLine.Y2 = mousePos.Y;

			vertLine.X1 = mousePos.X;
			vertLine.X2 = mousePos.X;
			vertLine.Y1 = output.Top;
			vertLine.Y2 = output.Bottom;

			if (UseDashOffset)
			{
				horizLine.StrokeDashOffset = (output.Right - mousePos.X) / 2;
				vertLine.StrokeDashOffset = (output.Bottom - mousePos.Y) / 2;
			}

			Point mousePosInData = mousePos.ScreenToData(transform);

			string text = null;

			if (showVerticalLine)
			{
				double xValue = mousePosInData.X;
				if (xTextMapping != null)
					text = xTextMapping(xValue);

				// doesnot have xTextMapping or it returned null
				if (text == null)
					text = GetRoundedValue(visible.XMin, visible.XMax, xValue);

				if (!String.IsNullOrEmpty(customXFormat))
					text = String.Format(customXFormat, text);
				horizTextBlock.Text = text;
			}

			double width = horizGrid.ActualWidth;
			double x = mousePos.X + blockShift.X;
			if (x + width > output.Right)
			{
				x = mousePos.X - blockShift.X - width;
			}
			Canvas.SetLeft(horizGrid, x);

			if (showHorizontalLine)
			{
				double yValue = mousePosInData.Y;
				text = null;
				if (yTextMapping != null)
					text = yTextMapping(yValue);

				if (text == null)
					text = GetRoundedValue(visible.YMin, visible.YMax, yValue);

				if (!String.IsNullOrEmpty(customYFormat))
					text = String.Format(customYFormat, text);
				vertTextBlock.Text = text;
			}

			// by default vertGrid is positioned on the top of line.
			double height = vertGrid.ActualHeight;
			double y = mousePos.Y - blockShift.Y - height;
			if (y < output.Top)
			{
				y = mousePos.Y + blockShift.Y;
			}
			Canvas.SetTop(vertGrid, y);

			if (followMouse)
				Position = mousePos;
		}

		/// <summary>
		/// Gets or sets the mouse position in screen coordinates.
		/// </summary>
		/// <value>The position.</value>
		public Point Position
		{
			get { return (Point)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}

		/// <summary>
		/// Identifies Position dependency property.
		/// </summary>
		public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
			"Position",
			typeof(Point),
			typeof(CursorCoordinateGraph),
			new UIPropertyMetadata(new Point(), OnPositionChanged));

		private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorCoordinateGraph graph = (CursorCoordinateGraph)d;
			graph.UpdateUIRepresentation((Point)e.NewValue);
		}

		private string GetRoundedValue(double min, double max, double value)
		{
			double roundedValue = value;
			var log = RoundingHelper.GetDifferenceLog(min, max);
			string format = "G3";
			double diff = Math.Abs(max - min);
			if (1E3 < diff && diff < 1E6)
			{
				format = "F0";
			}
			if (log < 0)
				format = "G" + (-log + 2).ToString();

			return roundedValue.ToString(format);
		}

		#region UseDashOffset property

		public bool UseDashOffset
		{
			get { return (bool)GetValue(UseDashOffsetProperty); }
			set { SetValue(UseDashOffsetProperty, value); }
		}

		public static readonly DependencyProperty UseDashOffsetProperty = DependencyProperty.Register(
		  "UseDashOffset",
		  typeof(bool),
		  typeof(CursorCoordinateGraph),
		  new FrameworkPropertyMetadata(true, UpdateUIRepresentation));

		private static void UpdateUIRepresentation(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CursorCoordinateGraph graph = (CursorCoordinateGraph)d;
			if ((bool)e.NewValue)
			{
				graph.UpdateUIRepresentation();
			}
			else
			{
				graph.vertLine.ClearValue(Line.StrokeDashOffsetProperty);
				graph.horizLine.ClearValue(Line.StrokeDashOffsetProperty);
			}
		}

		#endregion

		#region LineStroke property

		public Brush LineStroke
		{
			get { return (Brush)GetValue(LineStrokeProperty); }
			set { SetValue(LineStrokeProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeProperty = DependencyProperty.Register(
		  "LineStroke",
		  typeof(Brush),
		  typeof(CursorCoordinateGraph),
		  new PropertyMetadata(new SolidColorBrush(Color.FromArgb(170, 86, 86, 86))));

		#endregion

		#region LineStrokeThickness property

		public double LineStrokeThickness
		{
			get { return (double)GetValue(LineStrokeThicknessProperty); }
			set { SetValue(LineStrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeThicknessProperty = DependencyProperty.Register(
		  "LineStrokeThickness",
		  typeof(double),
		  typeof(CursorCoordinateGraph),
		  new PropertyMetadata(2.0));

		#endregion

		#region LineStrokeDashArray property

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public DoubleCollection LineStrokeDashArray
		{
			get { return (DoubleCollection)GetValue(LineStrokeDashArrayProperty); }
			set { SetValue(LineStrokeDashArrayProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeDashArrayProperty = DependencyProperty.Register(
		  "LineStrokeDashArray",
		  typeof(DoubleCollection),
		  typeof(CursorCoordinateGraph),
		  new FrameworkPropertyMetadata(DoubleCollectionHelper.Create(3, 3)));

		#endregion
	}
}
