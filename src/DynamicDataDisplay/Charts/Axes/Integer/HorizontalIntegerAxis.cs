using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
	public class HorizontalIntegerAxis : IntegerAxis
	{
		public HorizontalIntegerAxis()
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
