using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	public class ScaleInjectionRestriction : ViewportRestriction
	{
		private Viewport2D parentViewport;
		public ScaleInjectionRestriction(Viewport2D parentViewport)
		{
			if (parentViewport == null)
				throw new ArgumentNullException("parentViewport");

			this.parentViewport = parentViewport;
			parentViewport.PropertyChanged += parentViewport_PropertyChanged;
		}

		private void parentViewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				RaiseChanged();
			}
		}

		public void SetHorizontalTransform(double parentMin, double childMin, double parentMax, double childMax)
		{
			xScale = (childMax - childMin) / (parentMax - parentMin);
			xShift = childMin - parentMin;
		}

		public void SetVerticalTransform(double parentMin, double childMin, double parentMax, double childMax)
		{
			yScale = (childMax - childMin) / (parentMax - parentMin);
			yShift = childMin - parentMin;
		}

		private double xShift = 0;
		private double xScale = 1;
		private double yShift = 0;
		private double yScale = 1;

		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			DataRect parentVisible = parentViewport.Visible;

			double xmin = parentVisible.XMin * xScale + xShift;
			double xmax = parentVisible.XMax * xScale + xShift;
			double ymin = parentVisible.YMin * yScale + yShift;
			double ymax = parentVisible.YMax * yScale + yShift;

			return DataRect.Create(xmin, ymin, xmax, ymax);
		}
	}
}
