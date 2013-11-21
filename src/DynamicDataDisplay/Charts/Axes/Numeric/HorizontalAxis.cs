using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a horizontal axis with values of <see cref="Double"/> type.
	/// Can be placed only from bottom or top side of plotter.
	/// By default is placed from the bottom side.
	/// </summary>
	public class HorizontalAxis : NumericAxis
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalAxis"/> class, placed on bottom of <see cref="ChartPlotter"/>.
		/// </summary>
		public HorizontalAxis()
		{
			Placement = AxisPlacement.Bottom;
		}

		/// <summary>
		/// Validates the placement - e.g., vertical axis should not be placed from top or bottom, etc.
		/// If proposed placement is wrong, throws an ArgumentException.
		/// </summary>
		/// <param name="newPlacement">The new placement.</param>
		protected override void ValidatePlacement(AxisPlacement newPlacement)
		{
			if (newPlacement == AxisPlacement.Left || newPlacement == AxisPlacement.Right)
				throw new ArgumentException(Strings.Exceptions.HorizontalAxisCannotBeVertical);
		}
	}
}
