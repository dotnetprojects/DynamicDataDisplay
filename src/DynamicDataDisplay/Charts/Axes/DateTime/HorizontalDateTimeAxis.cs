using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents an axis with ticks of <see cref="System.DateTime"/> type, which can be placed only from bottom or top of <see cref="Plotter"/>.
	/// By default is placed from bottom.
	/// </summary>
	public class HorizontalDateTimeAxis : DateTimeAxis
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalDateTimeAxis"/> class.
		/// </summary>
		public HorizontalDateTimeAxis()
		{
			Placement = AxisPlacement.Bottom;
		}

		protected override void ValidatePlacement(AxisPlacement newPlacement)
		{
			if (newPlacement == AxisPlacement.Left || newPlacement == AxisPlacement.Right)
				throw new ArgumentException(Strings.Exceptions.HorizontalAxisCannotBeVertical);
		}
	}
}
