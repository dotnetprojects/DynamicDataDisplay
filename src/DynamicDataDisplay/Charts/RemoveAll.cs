using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public sealed class RemoveAll : IPlotterElement
	{
		private Type type;
		[NotNull]
		public Type Type
		{
			get { return type; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
	
				type = value;
			}
		}

		private Plotter plotter;
		public Plotter Plotter
		{
			get { return plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			if (type != null)
			{
				plotter.Children.RemoveAll(type);
			}
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;
		}
	}
}
