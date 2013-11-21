using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public class NoneModeHandler : PointSelectorModeHandler
	{
		protected override void AttachCore(PointSelector selector, Plotter plotter)
		{
			// do nothing here
		}

		protected override void DetachCore()
		{
			// do nothing
		}
	}
}
