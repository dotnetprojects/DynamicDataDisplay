using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public abstract class RectangleSelectorModeHandler : SelectorModeHandler<RectangleSelector>
	{
		private Plotter2D plotter;
		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		private RectangleSelector selector;
		public RectangleSelector Selector
		{
			get { return selector; }
		}

		protected override void AttachCore(RectangleSelector selector, Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			this.selector = selector;
		}

		protected override void DetachCore()
		{
			this.plotter = null;
			this.selector = null;
		}
	}
}
