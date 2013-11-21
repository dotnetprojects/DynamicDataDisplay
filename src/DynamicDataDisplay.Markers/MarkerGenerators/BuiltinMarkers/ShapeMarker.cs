using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers
{
	public abstract class ShapeMarker : MarkerGenerator
	{
		protected ShapeMarker()
		{
			widthBinding = CreateBinding(MarkerWidthProperty);
			heightBinding = CreateBinding(MarkerHeightProperty);
			fillBinding = CreateBinding(MarkerFillProperty);
			strokeBinding = CreateBinding(MarkerStrokeProperty);
			strokeThicknessBinding = CreateBinding(MarkerStrokeThicknessProperty);
		}

		protected sealed override FrameworkElement CreateMarkerCore(object dataItem)
		{
			Shape shape = CreateShape();

			shape.Stretch = Stretch.Fill;

			shape.SetBinding(Shape.WidthProperty, widthBinding);
			shape.SetBinding(Shape.HeightProperty, heightBinding);
			shape.SetBinding(Shape.FillProperty, fillBinding);
			shape.SetBinding(Shape.StrokeProperty, strokeBinding);
			shape.SetBinding(Shape.StrokeThicknessProperty, strokeThicknessBinding);

			foreach (var property in changedProperties)
			{
				var value = GetValue(property);
				shape.SetValue(property, value);
			}

			shape.DataContext = dataItem;

			return shape;
		}

		protected override void OnInitialized(EventArgs e)
		{
			RaiseEvent(new RoutedEventArgs(LoadedEvent, this));
			base.OnInitialized(e);
		}

		private readonly List<DependencyProperty> changedProperties = new List<DependencyProperty>();
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			var property = e.Property;
			if (property != MarkerFillProperty &&
				property != MarkerHeightProperty &&
				property != MarkerWidthProperty &&
				property != MarkerStrokeProperty &&
				property != MarkerStrokeThicknessProperty)
			{
				changedProperties.Add(property);
			}
		}

		protected abstract Shape CreateShape();

		#region Properties

		public double MarkerWidth
		{
			get { return (double)GetValue(MarkerWidthProperty); }
			set { SetValue(MarkerWidthProperty, value); }
		}

		public static readonly DependencyProperty MarkerWidthProperty = DependencyProperty.Register(
		  "MarkerWidth",
		  typeof(double),
		  typeof(ShapeMarker),
		  new FrameworkPropertyMetadata(10.0));

		public double MarkerHeight
		{
			get { return (double)GetValue(MarkerHeightProperty); }
			set { SetValue(MarkerHeightProperty, value); }
		}

		public static readonly DependencyProperty MarkerHeightProperty = DependencyProperty.Register(
		  "MarkerHeight",
		  typeof(double),
		  typeof(ShapeMarker),
		  new FrameworkPropertyMetadata(10.0));

		public Brush MarkerFill
		{
			get { return (Brush)GetValue(MarkerFillProperty); }
			set { SetValue(MarkerFillProperty, value); }
		}

		public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register(
		  "MarkerFill",
		  typeof(Brush),
		  typeof(ShapeMarker),
		  new FrameworkPropertyMetadata(Brushes.Blue));

		public Brush MarkerStroke
		{
			get { return (Brush)GetValue(MarkerStrokeProperty); }
			set { SetValue(MarkerStrokeProperty, value); }
		}

		public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register(
		  "MarkerStroke",
		  typeof(Brush),
		  typeof(ShapeMarker),
		  new FrameworkPropertyMetadata(Brushes.Black));

		public double MarkerStrokeThickness
		{
			get { return (double)GetValue(MarkerStrokeThicknessProperty); }
			set { SetValue(MarkerStrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty MarkerStrokeThicknessProperty = DependencyProperty.Register(
		  "MarkerStrokeThickness",
		  typeof(double),
		  typeof(ShapeMarker),
		  new FrameworkPropertyMetadata(1.0));

		#endregion

		private Binding widthBinding;
		private Binding heightBinding;
		private Binding fillBinding;
		private Binding strokeBinding;
		private Binding strokeThicknessBinding;

		protected Binding CreateBinding(DependencyProperty sourceProperty)
		{
			Binding binging = new Binding { Path = new PropertyPath(sourceProperty.Name), Source = this };
			return binging;
		}
	}
}
