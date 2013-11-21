using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    /// <summary>Abstract class that extends ElementPointMarker and contains
    /// marker property as Pen, Brush and Size</summary>
	public abstract class ShapeElementPointMarker : ElementPointMarker {
		/// <summary>Size of marker in points</summary>
		public double Size {
			get { return (double)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}

		public static readonly DependencyProperty SizeProperty =
			DependencyProperty.Register(
			  "Size",
			  typeof(double),
			  typeof(ShapeElementPointMarker),
			  new FrameworkPropertyMetadata(5.0));

        /// <summary>Tooltip to show when cursor on over</summary>
        public string ToolTipText
        {
            get { return (string)GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register(
              "ToolTipText",
              typeof(string),
              typeof(ShapeElementPointMarker),
              new FrameworkPropertyMetadata(String.Empty));

		/// <summary>Pen to outline marker</summary>
		public Pen Pen {
			get { return (Pen)GetValue(PenProperty); }
			set { SetValue(PenProperty, value); }
		}

		public static readonly DependencyProperty PenProperty =
			DependencyProperty.Register(
			  "Pen",
			  typeof(Pen),
			  typeof(ShapeElementPointMarker),
			  new FrameworkPropertyMetadata(null));


		public Brush Brush {
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}

		public static readonly DependencyProperty BrushProperty =
			DependencyProperty.Register(
			  "Brush",
			  typeof(Brush),
			  typeof(ShapeElementPointMarker),
			  new FrameworkPropertyMetadata(Brushes.Red));

		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public static readonly DependencyProperty FillProperty =
			DependencyProperty.Register(
			  "Fill",
			  typeof(Brush),
			  typeof(ShapeElementPointMarker),
			  new FrameworkPropertyMetadata(Brushes.Red));


	}
}
