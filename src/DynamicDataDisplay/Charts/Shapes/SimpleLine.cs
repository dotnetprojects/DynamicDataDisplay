using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents simple line bound to viewport coordinates.
	/// </summary>
	public abstract class SimpleLine : ViewportShape
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleLine"/> class.
		/// </summary>
		protected SimpleLine() { }

		/// <summary>
		/// Gets or sets the value of line - e.g., its horizontal or vertical coordinate.
		/// </summary>
		/// <value>The value.</value>
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Identifies Value dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register(
			  "Value",
			  typeof(double),
			  typeof(SimpleLine),
			  new PropertyMetadata(
				  0.0, OnValueChanged));

		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SimpleLine line = (SimpleLine)d;
			line.OnValueChanged();
		}

		protected virtual void OnValueChanged()
		{
			UpdateUIRepresentation();
		}

		private LineGeometry lineGeometry = new LineGeometry();
		protected LineGeometry LineGeometry
		{
			get { return lineGeometry; }
		}
		protected override Geometry DefiningGeometry
		{
			get { return lineGeometry; }
		}
	}
}
