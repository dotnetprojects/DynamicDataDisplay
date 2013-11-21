using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a vertical axis with values of <see cref="TimeSpan"/> type.
	/// </summary>
	public class VerticalTimeSpanAxis : TimeSpanAxis
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VerticalTimeSpanAxis"/> class, placed (by default) on the left side of <see cref="ChartPlotter"/>.
		/// </summary>
		public VerticalTimeSpanAxis()
		{
			Placement = AxisPlacement.Left;
		}

		/// <summary>
		/// Validates the placement - e.g., vertical axis should not be placed from top or bottom, etc.
		/// If proposed placement is wrong, throws an ArgumentException.
		/// </summary>
		/// <param name="newPlacement">The new placement.</param>
		protected override void ValidatePlacement(AxisPlacement newPlacement)
		{
			if (newPlacement == AxisPlacement.Bottom || newPlacement == AxisPlacement.Top)
				throw new ArgumentException(Strings.Exceptions.VerticalAxisCannotBeHorizontal);
		}
	}
}
