using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay
{
	public sealed class PenDescription : StandardDescription
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PenDescription"/> class.
		/// </summary>
		public PenDescription() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PenDescription"/> class.
		/// </summary>
		/// <param name="description">Custom description.</param>
		public PenDescription(string description) : base(description) { }

		protected override LegendItem CreateLegendItemCore()
		{
			return new LineLegendItem(this);
		}

		protected override void AttachCore(UIElement graph)
		{
			base.AttachCore(graph);
			LineGraph g = graph as LineGraph;
			if (g == null)
			{
				throw new ArgumentException("Pen description can only be attached to PointsGraph", "graph");
			}

			SetBinding(StrokeProperty, new Binding("Stroke") { Source = g });
			SetBinding(StrokeThicknessProperty, new Binding("StrokeThickness") { Source = g });
		}

		public Brush Stroke
		{
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
		  "Stroke",
		  typeof(Brush),
		  typeof(PenDescription),
		  new FrameworkPropertyMetadata(null));

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
		  "StrokeThickness",
		  typeof(double),
		  typeof(PenDescription),
		  new FrameworkPropertyMetadata(0.0));

	}
}
