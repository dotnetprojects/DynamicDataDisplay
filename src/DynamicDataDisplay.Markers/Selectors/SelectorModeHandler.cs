using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	public abstract class SelectorModeHandler<TSelector>
	{
		private bool isAttached = false;
		public bool IsAttached
		{
			get { return isAttached; }
		}

		public void Attach(TSelector selector, Plotter plotter)
		{
			if (isAttached)
				throw new InvalidOperationException("Cannot be attached before being detached.");

			AttachCore(selector, plotter);

			isAttached = true;
		}
		protected abstract void AttachCore(TSelector selector, Plotter plotter);

		public void Detach()
		{
			if (!isAttached)
				throw new InvalidOperationException("Cannot be detached before being attached.");

			DetachCore();

			isAttached = false;
		}
		protected abstract void DetachCore();
	}
}
