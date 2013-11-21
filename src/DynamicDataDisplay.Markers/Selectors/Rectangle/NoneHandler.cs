using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public class NoneHandler : RectangleSelectorModeHandler
	{
		protected override void AttachCore(RectangleSelector selector, Plotter plotter)
		{
		}

		protected override void DetachCore()
		{
		}
	}
}
