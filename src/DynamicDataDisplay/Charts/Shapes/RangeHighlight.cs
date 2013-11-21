using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents rectangle with corners bound to viewport coordinates.
	/// </summary>
	[TemplatePart(Name = "PART_LinesPath", Type = typeof(Path))]
	[TemplatePart(Name = "PART_RectPath", Type = typeof(Path))]
	public abstract class RangeHighlight : Control, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RangeHighlight"/> class.
		/// </summary>
		protected RangeHighlight()
		{
			Resources = new ResourceDictionary { Source = new Uri("/DynamicDataDisplay;component/Charts/Shapes/RangeHighlightStyle.xaml", UriKind.Relative) };

			Style = (Style)FindResource(typeof(RangeHighlight));
			ApplyTemplate();
		}

		bool partsLoaded = false;
		protected bool PartsLoaded
		{
			get { return partsLoaded; }
		}
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			linesPath = (Path)Template.FindName("PART_LinesPath", this);
			GeometryGroup linesGroup = new GeometryGroup();
			linesGroup.Children.Add(lineGeometry1);
			linesGroup.Children.Add(lineGeometry2);
			linesPath.Data = linesGroup;

			rectPath = (Path)Template.FindName("PART_RectPath", this);
			rectPath.Data = rectGeometry;

			partsLoaded = true;
		}

		#region Presentation DPs

		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
			"Fill",
			typeof(Brush),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.FillProperty.DefaultMetadata.DefaultValue));

		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
			"Stroke",
			typeof(Brush),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeProperty.DefaultMetadata.DefaultValue));

		public Brush Stroke
		{
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
			"StrokeThickness",
			typeof(double),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeThicknessProperty.DefaultMetadata.DefaultValue));

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register(
			"StrokeStartLineCap",
			typeof(PenLineCap),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeStartLineCapProperty.DefaultMetadata.DefaultValue));

		public PenLineCap StrokeStartLineCap
		{
			get { return (PenLineCap)GetValue(StrokeStartLineCapProperty); }
			set { SetValue(StrokeStartLineCapProperty, value); }
		}

		public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register(
			"StrokeEndLineCap",
			typeof(PenLineCap),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeEndLineCapProperty.DefaultMetadata.DefaultValue));

		public PenLineCap StrokeEndLineCap
		{
			get { return (PenLineCap)GetValue(StrokeEndLineCapProperty); }
			set { SetValue(StrokeEndLineCapProperty, value); }
		}

		public static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register(
			"StrokeDashCap",
			typeof(PenLineCap),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeDashCapProperty.DefaultMetadata.DefaultValue));

		public PenLineCap StrokeDashCap
		{
			get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
			set { SetValue(StrokeDashCapProperty, value); }
		}

		public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register(
			"StrokeLineJoin",
			typeof(PenLineJoin),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeLineJoinProperty.DefaultMetadata.DefaultValue));

		public PenLineJoin StrokeLineJoin
		{
			get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
			set { SetValue(StrokeLineJoinProperty, value); }
		}

		public static readonly DependencyProperty StrokeMiterLimitProperty = DependencyProperty.Register(
			"StrokeMiterLimit",
			typeof(double),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeMiterLimitProperty.DefaultMetadata.DefaultValue));

		public double StrokeMiterLimit
		{
			get { return (double)GetValue(StrokeMiterLimitProperty); }
			set { SetValue(StrokeMiterLimitProperty, value); }
		}

		public static readonly DependencyProperty StrokeDashOffsetProperty = DependencyProperty.Register(
			"StrokeDashOffset",
			typeof(double),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeDashOffsetProperty.DefaultMetadata.DefaultValue));

		public double StrokeDashOffset
		{
			get { return (double)GetValue(StrokeDashOffsetProperty); }
			set { SetValue(StrokeDashOffsetProperty, value); }
		}

		public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register(
			"StrokeDashArray",
			typeof(DoubleCollection),
			typeof(RangeHighlight),
			new PropertyMetadata(Shape.StrokeDashArrayProperty.DefaultMetadata.DefaultValue));

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public DoubleCollection StrokeDashArray
		{
			get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
			set { SetValue(StrokeDashArrayProperty, value); }
		}

		#endregion

		#region Values dependency properties

		/// <summary>
		/// Gets or sets the first value determining position of rectangle in viewport coordinates.
		/// </summary>
		/// <value>The value1.</value>
		public double Value1
		{
			get { return (double)GetValue(Value1Property); }
			set { SetValue(Value1Property, value); }
		}

		public static readonly DependencyProperty Value1Property =
			DependencyProperty.Register(
			  "Value1",
			  typeof(double),
			  typeof(RangeHighlight),
			  new FrameworkPropertyMetadata(0.0, OnValueChanged));

		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RangeHighlight r = (RangeHighlight)d;
			r.OnValueChanged(e);
		}

		/// <summary>
		/// Gets or sets the second value determining position of rectangle in viewport coordinates.
		/// </summary>
		/// <value>The value2.</value>
		public double Value2
		{
			get { return (double)GetValue(Value2Property); }
			set { SetValue(Value2Property, value); }
		}

		public static readonly DependencyProperty Value2Property =
			DependencyProperty.Register(
			  "Value2",
			  typeof(double),
			  typeof(RangeHighlight),
			  new FrameworkPropertyMetadata(0.0, OnValueChanged));

		private void OnValueChanged(DependencyPropertyChangedEventArgs e)
		{
			UpdateUIRepresentation();
		}

		#endregion

		#region Geometry

		private Path rectPath;
		private Path linesPath;

		private readonly RectangleGeometry rectGeometry = new RectangleGeometry();
		protected RectangleGeometry RectGeometry
		{
			get { return rectGeometry; }
		}

		private readonly LineGeometry lineGeometry1 = new LineGeometry();
		protected LineGeometry LineGeometry1
		{
			get { return lineGeometry1; }
		}

		private readonly LineGeometry lineGeometry2 = new LineGeometry();
		protected LineGeometry LineGeometry2
		{
			get { return lineGeometry2; }
		}

		#endregion

		#region IPlotterElement Members

		private Plotter2D plotter;
		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			plotter.CentralGrid.Children.Add(this);

			Plotter2D plotter2d = (Plotter2D)plotter;
			this.plotter = plotter2d;
			plotter2d.Viewport.PropertyChanged += Viewport_PropertyChanged;

			UpdateUIRepresentation();
		}

		private void UpdateUIRepresentation()
		{
			if (Plotter == null) return;

			if (partsLoaded)
			{
				UpdateUIRepresentationCore();
			}
		}
		protected virtual void UpdateUIRepresentationCore() { }

		void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			UpdateUIRepresentation();
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			Plotter2D plotter2d = (Plotter2D)plotter;
			plotter2d.Viewport.PropertyChanged -= Viewport_PropertyChanged;
			plotter.CentralGrid.Children.Remove(this);

			this.plotter = null;
		}

		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
