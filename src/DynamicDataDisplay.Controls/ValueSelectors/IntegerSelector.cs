using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	/// <summary>
	/// Represents a control for precise selecting <see cref="T:System.Int32"/> values.
	/// </summary>
	public class IntegerSelector : GenericValueSelector<int>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntegerSelector"/> class.
		/// </summary>
		public IntegerSelector()
		{
			var axis = new HorizontalIntegerAxis();
			Children.Add(axis);
			ValueConversion = axis;

			Range = new Range<int>(0, 10);
		}

		protected override Point CoerceMarkerPosition(PositionalViewportUIContainer marker, Point position)
		{
			var newPosition = base.CoerceMarkerPosition(marker, position);

			newPosition.X = Math.Round(newPosition.X);

			return newPosition;
		}
	}
}
